using Api.Domain.Greetings;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

[assembly: LightBddScope]

namespace Api.Domain.Tests.Greetings;

[FeatureDescription("Test feature description")]
[Label("TFPT-1234")]
public partial class GreetingScenarios : FeatureFixture
{
    [Scenario]
    public void User_should_get_a_greeting_on_their_birthday()
    {
        Runner.RunScenario(
            Given_the_user_has_their_birthday_today,
            When_creating_a_greeting_message,
            Then_the_greeting_says_happy_birthday
        );
    }

    [Scenario]
    public void User_should_get_a_welcome_message_by_default()
    {
        Runner.RunScenario(
            Given_the_user_has_no_birthday_today,
            When_creating_a_greeting_message,
            Then_the_greeting_says_hello
        );
    }

    [Scenario]
    public void User_should_get_a_big_spender_message()
    {
        Runner.RunScenario(
            _=> Given_the_user_has_no_birthday_today(),
            _ => And_the_user_has_spent_more_than(1000),
            _ => When_creating_a_greeting_message(),
            _ => Then_the_greeting_says_hello_big_spender()
            );
    }
}

public partial class GreetingScenarios
{
    private User _user;
    private string _greeting;
    private decimal _userSpend = 0;
    
    private void Given_the_user_has_their_birthday_today()
    {
        _user = new User(
            "bob@test.com",
            DateTime.Now);
    }

    private void Given_the_user_has_no_birthday_today()
    {
        _user = new User(
            "bob@test.com", 
            DateTime.Today.AddDays(-1));
    }

    private void And_the_user_has_spent_more_than(decimal spend)
    {
        _userSpend = spend;
    }

    private void When_creating_a_greeting_message()
    {
        var salesService = new FakeSalesService(_userSpend);
        _greeting = new GreetingService(salesService).GetGreeting(_user);
    }

    private void Then_the_greeting_says_happy_birthday()
    {
        Assert.Equal($"Happy birthday {_user.Username}", _greeting);
    }

    private void Then_the_greeting_says_hello()
    {
        Assert.Equal($"Hello {_user.Username}", _greeting);
    }

    private void Then_the_greeting_says_hello_big_spender()
    {
        Assert.Equal($"Hey big spender {_user.Username}", _greeting);
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