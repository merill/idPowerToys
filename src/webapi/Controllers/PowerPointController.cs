using CADocGen.PowerPointGenerator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using IdPowerToys.PowerPointGenerator;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class PowerPointController : ControllerBase
{

    private readonly ILogger<PowerPointController> _logger;
    private readonly IWebHostEnvironment _hostEnvironment;

    public PowerPointController(ILogger<PowerPointController> logger, IWebHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    [HttpPost]
    public async Task<IActionResult> Post(ConfigOptions configOptions)
    {
        
        //Collect Graph data
        var graphData = new GraphData();
        if(configOptions.IsManual == true)
        {
            await graphData.ImportPolicy(configOptions);
        }
        else
        {
            StringValues accessToken;
            Request.Headers.TryGetValue("X-PowerPointGeneration-Token", out accessToken);
            await graphData.CollectData(accessToken, configOptions);
        }


        Response.Clear();
        //Generate and stream doc
        Response.ContentType = "application/octet-stream";
        Response.Headers.Add("Content-Disposition", "attachment; filename=\"Conditional Access Policies.pptx\"");

        var templateFilePath = Path.Combine(_hostEnvironment.ContentRootPath, @"wwwroot/assets/PolicyTemplate.pptx");

        var gen = new DocumentGenerator();
        var stream = new MemoryStream();
        gen.GeneratePowerPoint(graphData, templateFilePath, stream, configOptions);
        stream.Position = 0;

        return File(stream, "application/octet-stream", "cadeck.pptx");
    }
}
