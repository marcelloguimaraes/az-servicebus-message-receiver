# Service Bus Message Receiver

A console application written in C# in which receives and delete messages from a variety of sources(topics, queues)

## Requisites

- .NET SDK 6 or higher

## How to run

1. Restore nuget packages

```
dotnet restore
```

2. Build

```
dotnet build
```

3. Configure the intended Azure Service Bus connection string(available on Azure Portal or by `az cli`), topic names, subscription name, queue name

```
const string ServiceBusConnectionString = "YOUR-CON-STRING-HERE";
const string TopicName = "YOUR-TOPIC-NAME";
const string SubscriptionName = "YOUR-SUB-NAME";
```

4. Run

```
dotnet run
```

## Next steps

- Receive messages from a topic/sub without sessions enable
- Log completed messages to a local file
- Log errors to a file