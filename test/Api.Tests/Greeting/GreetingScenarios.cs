using Api.Domain.Greetings;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.WebUtilities;

[assembly: LightBddScope]

namespace Api.Domain.Tests.Greetings;

[FeatureDescription("Test feature description")]
[Label("TFPT-1234")]
public partial class GreetingScenarios : FeatureFixture
{
    [Scenario]
    public async Task User_should_get_a_greeting_on_their_birthday() =>
        await Runner.RunScenarioAsync(
            _ => Given_the_user_has_their_birthday_today(),
            _ => When_creating_a_greeting_message(),
            _ => Then_the_greeting_says_happy_birthday()
        );

    [Scenario]
    public async Task User_should_get_a_welcome_message_by_default() =>
        await Runner.RunScenarioAsync(
            _ => Given_the_user_has_no_birthday_today(),
            _ => When_creating_a_greeting_message(),
            _ => Then_the_greeting_says_hello()
        );

    [Scenario]
    public async Task User_should_get_a_big_spender_message() =>
        await Runner.RunScenarioAsync(
            _=> Given_the_user_has_no_birthday_today(),
            _ => And_the_user_has_spent_more_than(1000),
            _ => When_creating_a_greeting_message(),
            _ => Then_the_greeting_says_hello_big_spender()
            );
}

public partial class GreetingScenarios
{
    private User _user;
    private string _greeting;
    private decimal _userSpend = 0;
    
    private Task Given_the_user_has_their_birthday_today()
    {
        _user = new User(
            "bob@test.com",
            DateTime.Now);

        return Task.CompletedTask;
    }

    private Task Given_the_user_has_no_birthday_today()
    {
        _user = new User(
            "bob@test.com", 
            DateTime.Today.AddDays(-1));

        return Task.CompletedTask;
    }

    private Task And_the_user_has_spent_more_than(decimal spend)
    {
        _userSpend = spend;
        return Task.CompletedTask;
    }

    private async Task When_creating_a_greeting_message()
    {
        var salesService = new FakeSalesService(_userSpend);
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<ISalesService>(salesService);
                });
            });

        var client = application.CreateClient();
        var uri = QueryHelpers.AddQueryString("/Greeting", new Dictionary<string, string>
        {
            { "username", _user.Username },
            { "dateOfBirth", _user.DateOfBirth.ToString("O") }
        }!);
        
        var response = await client.GetAsync(uri);
        _greeting = await response.Content.ReadAsStringAsync();
    }

    private Task Then_the_greeting_says_happy_birthday()
    {
        Assert.Equal($"Happy birthday {_user.Username}", _greeting);
        return Task.CompletedTask;
    }

    private Task Then_the_greeting_says_hello()
    {
        Assert.Equal($"Hello {_user.Username}", _greeting);
        return Task.CompletedTask;
    }

    private Task Then_the_greeting_says_hello_big_spender()
    {
        Assert.Equal($"Hey big spender {_user.Username}", _greeting);
        return Task.CompletedTask;
    }
}

public class FakeSalesService : ISalesService
{
    private decimal userSpend;
    
    public FakeSalesService(decimal userSpend)
    {
        this.userSpend = userSpend;
    }

    public decimal GetUserSpend(User user) => userSpend;
}