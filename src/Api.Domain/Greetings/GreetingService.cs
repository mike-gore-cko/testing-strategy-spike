namespace Api.Domain.Greetings;

public class GreetingService
{
    private readonly ISalesService _salesService;

    public GreetingService(ISalesService salesService)
    {
        _salesService = salesService;
    }
    
    public string GetGreeting(User user)
    {
        if (user.DateOfBirth.Date == DateTime.Now.Date)
        {
            return $"Happy birthday {user.Username}";
        }

        if (_salesService.GetUserSpend(user) >= 1000)
        {
            return $"Hey big spender {user.Username}";
        }

        return $"Hello {user.Username}";
    }
}

public record User(string Username, DateTime DateOfBirth);

public interface ISalesService
{
    decimal GetUserSpend(User user);
}