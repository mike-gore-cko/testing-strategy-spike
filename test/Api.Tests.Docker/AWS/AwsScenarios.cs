using System.Net;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace Api.Domain.Tests.Greetings;

// Tests in a class or using the same collection do not run in parallel
// The copies of the scenarios run in parallel, speeding things up

[FeatureDescription("AWS feature description")]
[Label("TFPT-1234")]
public class AwsScenarios5 : AwsScenarios
{
}

[FeatureDescription("AWS feature description")]
[Label("TFPT-1234")]
public class AwsScenarios4 : AwsScenarios
{
}

[FeatureDescription("AWS feature description")]
[Label("TFPT-1234")]
public class AwsScenarios3 : AwsScenarios
{
}

[FeatureDescription("AWS feature description")]
[Label("TFPT-1234")]
public class AwsScenarios2 : AwsScenarios
{
}

[FeatureDescription("AWS feature description")]
[Label("TFPT-1234")]
public partial class AwsScenarios : FeatureFixture
{
    [Scenario]
    [MemberData(nameof(Data))]
    public async Task AWSScenario(int iteration) =>
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

    public static IEnumerable<object[]> Data => Enumerable.Range(1, 100).Select(x => new object[] { x });

    private Task Given_a_message()
    {
        message = "Message";
        return Task.CompletedTask;
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