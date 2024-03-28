using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

Console.WriteLine("Select a source for receiving and completing messages from Azure Service Bus");
Console.WriteLine("1 - Queue");
Console.WriteLine("2 - Topic/subscription(with sessions enabled)");
bool validInput = true;

do
{
    string input = Console.ReadLine();

    switch (input)
    {
        case "1":
            await ReceiveFromQueue();
            break;
        case "2":
            await ReceiveFromSubscriptionWithSessions();
            break;
        default:
            validInput = false;
            System.Console.WriteLine("Invalid option, please try again");
            break;
    }
} while (!validInput);

async Task ReceiveFromQueue()
{
    Console.WriteLine("Starting receiving messages from queue...");
    const string ServiceBusConnectionString = "Endpoint=sb://sb-finproducts-ec2-dev.servicebus.windows.net/;SharedAccessKeyName=default;SharedAccessKey=BN2D1vgOTD5jgKcjyqDx6eFzEOleKkwwu+ASbErEkGM=";
    const string QueueName = "commands-marcello";
    IQueueClient queueClient;

    queueClient = new QueueClient(ServiceBusConnectionString, QueueName)
    {
        PrefetchCount = 10
    };

    Console.WriteLine("Connected to queue client successfully");

    queueClient.RegisterSessionHandler(ProcessMessagesAsync, new SessionHandlerOptions(ExceptionReceivedHandler)
    {
        AutoComplete = false,
        MaxConcurrentSessions = 10,
        MessageWaitTimeout = TimeSpan.FromMilliseconds(100)
    });

    await queueClient.CloseAsync();
}

async Task ReceiveFromSubscriptionWithSessions()
{
    const string ServiceBusConnectionString = "Endpoint=sb://sb-finproducts-ec2-dev.servicebus.windows.net/;SharedAccessKeyName=default;SharedAccessKey=2PtpbmMohETZ1KigqFfTx3ONvAaVv3WlN+ASbCdCn7c=";
    const string TopicName = "credit-scoring-notifications-marcello";
    const string SubscriptionName = "credit-risk-parameters-concession-analysis";
    ISubscriptionClient subscriptionClient;

    subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName)
    {
        PrefetchCount = 50,
    };

    Console.WriteLine("Connected to subcription client successfully.");
    Console.WriteLine("Starting receiving messages from subscription...");

    subscriptionClient.RegisterSessionHandler(ProcessMessagesAsync, new SessionHandlerOptions(ExceptionReceivedHandler)
    {
        AutoComplete = false,
        MaxConcurrentSessions = 10,
        MessageWaitTimeout = TimeSpan.FromMilliseconds(500)
    });

    Console.WriteLine("Press any key to exit.");
    Console.ReadKey();

    await subscriptionClient.CloseAsync();
}

async Task ProcessMessagesAsync(IMessageSession session, Message message, CancellationToken token)
{
    Console.WriteLine($"Received message: SessionId:{session.SessionId} SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

    await session.CompleteAsync(message.SystemProperties.LockToken);

    Console.WriteLine($"Completed message: SessionId:{session.SessionId} SequenceNumber:{message.SystemProperties.SequenceNumber}");
}

Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
{
    Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
    return Task.CompletedTask;
}
