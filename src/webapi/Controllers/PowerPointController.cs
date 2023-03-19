using IdPowerToys.PowerPointGenerator;
using IdPowerToys.PowerPointGenerator.Graph;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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
            //Collect Graph data
            var graphData = new GraphData(configOptions);
            if (configOptions.IsManual == true)
            {
                _logger.LogInformation("ImportPolicy");
                await graphData.ImportPolicy();
            }
            else
            {
                Request.Headers.TryGetValue("X-PowerPointGeneration-Token", out StringValues accessToken);
                var token = accessToken.FirstOrDefault();
                if (token != null)
                {
                    _logger.LogInformation("CollectData");
                    await graphData.CollectData(token);
                }
                else
                {
                    throw new Exception("Missing token in request");
                }
            }

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
}
