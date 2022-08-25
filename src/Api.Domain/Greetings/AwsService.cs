using Amazon.SQS;
using Amazon.SQS.Model;

namespace Api.Domain.Greetings;

public class AwsService
{
    private AmazonSQSClient _sqsClient;
    private string _queue;

    public AwsService(AmazonSQSClient sqsClient, string queue)
    {
        _sqsClient = sqsClient;
        _queue = queue;
    }

    public async Task<bool> PublishMessage(string message)
    {
        var response = await _sqsClient.SendMessageAsync(new SendMessageRequest
        {
            MessageBody = message,
            QueueUrl = _queue
        });

        return (int)response.HttpStatusCode >= 200 && (int)response.HttpStatusCode < 300;
    }
    
    public async Task<Message?> GetOneMessage()
    {
        var response = await _sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            QueueUrl = _queue,
            MaxNumberOfMessages = 1
        });

        return response.Messages.FirstOrDefault();
    }
}