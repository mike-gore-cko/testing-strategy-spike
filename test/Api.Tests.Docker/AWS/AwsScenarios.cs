using System.Net;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace Api.Domain.Tests.Greetings;

[FeatureDescription("AWS feature description")]
[Label("TFPT-1234")]
public partial class AwsScenarios : FeatureFixture
{
    [Scenario]
    public async Task AWSScenario() =>
        await Runner.RunScenarioAsync(
            _ => Given_a_message(),
            _ => When_publishing_message(),
            _ => Then_the_message_gets_published(),
            _ => When_getting_the_message(),
            _ => Then_message_is_the_same()
        );
}

public partial class AwsScenarios
{
    private string message;
    private static HttpClient client = new();
    private HttpResponseMessage response;
    
    private async Task Given_a_message()
    {
        message = "Message";
    }

    private async Task When_publishing_message()
    {
        response = await client.GetAsync("http://localhost:5000/AWS/PostMessage?message=" + message);
    }

    private async Task Then_the_message_gets_published()
    {
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task When_getting_the_message()
    {
        response = await client.GetAsync("http://localhost:5000/AWS");
    }

    private async Task Then_message_is_the_same()
    {
        Assert.Contains(message, await response.Content.ReadAsStringAsync());
    }
}