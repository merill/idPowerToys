using IdPowerToys.PowerPointGenerator;
using IdPowerToys.PowerPointGenerator.Graph;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Graph.Beta.Models;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class PowerPointController : ControllerBase
{
    private readonly ILogger<PowerPointController> _logger;

    public PowerPointController(ILogger<PowerPointController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post(ConfigOptions configOptions)
    {
        try
        {
            _logger.LogInformation("PowerPointGeneration");
            var token = GetAccessTokenFromHeader();

            var graphData = token == null ? new GraphData(configOptions) : new GraphData(configOptions, token);

            await graphData.CollectData();

            Response.Clear();
            //Generate and stream doc
            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("Content-Disposition", "attachment; filename=\"Conditional Access Policies.pptx\"");

            var gen = new DocumentGenerator();
            var stream = new MemoryStream();
            _logger.LogInformation("GeneratePowerPoint");
            gen.GeneratePowerPoint(graphData, stream, configOptions);

            stream.Position = 0;
            _logger.LogInformation("ReturnFile");
            return File(stream, "application/octet-stream", "cadeck.pptx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Generation error");
            throw;
        }
    }

    private string? GetAccessTokenFromHeader()
    {
        Request.Headers.TryGetValue("X-PowerPointGeneration-Token", out StringValues accessToken);
        var token = accessToken.FirstOrDefault();
        return token;
    }
}
