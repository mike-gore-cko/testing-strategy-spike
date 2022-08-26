using Amazon.SQS;
using Api.Tests.Docker;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using LightBDD.XUnit2;

[assembly: LocalStackSetup]

namespace Api.Tests.Docker;

// This runs once across all tests - for Docker based setups this is preferred to an xUnit collection
// because it'll allow test classes to run in parallel
public class LocalStackSetupAttribute : LightBddScopeAttribute
{
    private ICompositeService dockerCompose;

    protected override void OnSetUp()
    {
        const string sqsServiceUrl = "http://localhost:4566";
        const string queueName = $"test_queue";
        
        var sqsClient = new AmazonSQSClient(new AmazonSQSConfig
        {
            ServiceURL = sqsServiceUrl
        });

        sqsClient.CreateQueueAsync(queueName).Wait();
    }
}