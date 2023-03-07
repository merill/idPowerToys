using CADocGen.PowerPointGenerator;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWebHostEnvironment _hostEnvironment;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IWebHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpPost]
    public async Task<IActionResult> Post(GeneratePowerPointManualRequest policy)
    {
        //Collect Graph data
        var graphData = new GraphData();
        await graphData.ImportPolicy(policy.ConditionalAccessPolicyJson);

        Response.Clear();
        //Generate and stream doc
        Response.ContentType = "application/octet-stream";
        Response.Headers.Add("Content-Disposition", "attachment; filename=\"Conditional Access Policies.pptx\"");

        var templateFilePath = Path.Combine(_hostEnvironment.ContentRootPath, @"wwwroot\assets\PolicyTemplate.pptx");

        var gen = new DocumentGenerator();
        var stream = new MemoryStream();
        gen.GeneratePowerPoint(graphData, templateFilePath, stream);
        stream.Position = 0;

        return File(stream, "application/octet-stream", "cadeck.pptx");
    }
}
