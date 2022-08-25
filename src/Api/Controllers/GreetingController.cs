using Api.Domain.Greetings;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GreetingController : ControllerBase
{
    private readonly GreetingService _greetingService;

    public GreetingController(GreetingService greetingService)
    {
        _greetingService = greetingService;
    }

    [HttpGet]
    public IActionResult Get([FromQuery]string username, [FromQuery]DateTime dateOfBirth)
    {
        var user = new User(username, dateOfBirth);
        var greeting = _greetingService.GetGreeting(user);
        return Ok(greeting);
    }
}
