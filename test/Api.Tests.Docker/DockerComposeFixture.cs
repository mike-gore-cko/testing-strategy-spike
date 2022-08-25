using Amazon.SQS;
using Api.Tests.Docker;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using LightBDD.XUnit2;


namespace Api.Tests.Docker;

public class DockerComposeFixture : IDisposable
{
    private ICompositeService dockerCompose;

    public DockerComposeFixture()
    {
        var composeTests = Path.Combine(Directory.GetCurrentDirectory(), "docker-compose.tests.yml");
        var composeOverride = Path.Combine(Directory.GetCurrentDirectory(), "docker-compose.override.yml");

        dockerCompose = new Builder()
            .UseContainer()
            .UseCompose()
            .FromFile(composeTests, composeOverride)
            .RemoveOrphans()
            //.WaitForHttp("AWS", "localStackURL")
            .ForceBuild()
            .Build().Start();
        
        const string sqsServiceUrl = "http://localhost:4566";
        const string queueName = $"test_queue";
        
        var sqsClient = new AmazonSQSClient(new AmazonSQSConfig
        {
            ServiceURL = sqsServiceUrl
        });

        sqsClient.CreateQueueAsync(queueName).Wait();
    }

    public void Dispose()
    {
        dockerCompose.Stop();
        dockerCompose.Remove();
    }
}