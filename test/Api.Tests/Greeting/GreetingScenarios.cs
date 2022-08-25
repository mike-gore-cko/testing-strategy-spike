using System.ComponentModel;
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
    [MemberData(nameof(Data))]
    public async Task User_should_get_a_greeting_on_their_birthday(int iteration) =>
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

[Collection("InProcessServer")]
public partial class GreetingScenarios
{
    private readonly InProcessServerFixture _fixture;
    private User _user;
    private string _greeting;
    private decimal _userSpend = 0;
    
    public static IEnumerable<object[]> Data => Enumerable.Range(1, 500).Select(x => new object[] { x });

    public GreetingScenarios(InProcessServerFixture fixture)
    {
        _fixture = fixture;
    }
    
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
        _fixture.SalesService.SetUserSpend(_userSpend);
        
        var uri = QueryHelpers.AddQueryString("/Greeting", new Dictionary<string, string>
        {
            { "username", _user.Username },
            { "dateOfBirth", _user.DateOfBirth.ToString("O") }
        }!);
        
        var response = await _fixture.Client.GetAsync(uri);
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

    public void SetUserSpend(decimal userSpend)
    {
        this.userSpend = userSpend;
    }
}

[CollectionDefinition("InProcessServer")]
public class InProcessServerCollection : ICollectionFixture<InProcessServerFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

public class InProcessServerFixture : IDisposable
{
    private FakeSalesService _salesService;
    private HttpClient _client;
    
    public InProcessServerFixture()
    {
        _salesService = new FakeSalesService(0);
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<ISalesService>(_salesService);
                });
            });

        _client = application.CreateClient();
    }

    public FakeSalesService SalesService => _salesService;

    public HttpClient Client => _client;
    
    public void Dispose()
    {
        _client.Dispose();
    }
}