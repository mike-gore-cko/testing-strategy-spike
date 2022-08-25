using Amazon.SQS;
using Api.Domain.Greetings;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AWSController : ControllerBase
{
    private readonly AwsService _awsService;

    public AWSController()
    {
        _awsService = new AwsService(new AmazonSQSClient(new AmazonSQSConfig
        {
            ServiceURL = "http://host.docker.internal:4566"
        }), "http://host.docker.internal:4566/queue/test_queue");
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _awsService.GetOneMessage());
    }
    
    [HttpGet]
    [Route("PostMessage")]
    public async Task<IActionResult> Post([FromQuery]string message)
    {
        return Ok(await _awsService.PublishMessage(message));
    }
}
