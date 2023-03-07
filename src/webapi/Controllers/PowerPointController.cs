using CADocGen.PowerPointGenerator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using webapi.Models;

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
    public void Post(GeneratePowerPointManualRequest policy)
    {
        //Collect Graph data
        //var graphData = new GraphData();
        //await graphData.ImportPolicy(policy.ConditionalAccessPolicyJson);

        ////Generate and stream doc
        //Response.ContentType = "application/octet-stream";
        //Response.Headers.Add("Content-Disposition", "attachment; filename=\"Conditional Access Policies.pptx\"");

        //var templateFilePath = Path.Combine(_hostEnvironment.ContentRootPath, @"wwwroot\assets\PolicyTemplate.pptx");

        //var gen = new DocumentGenerator();
        //gen.GeneratePowerPoint(graphData, templateFilePath, Response.BodyWriter.AsStream());

        //return new EmptyResult();
    }
}
