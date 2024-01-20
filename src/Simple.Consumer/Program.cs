// In order to understand how to read data from Kafka, you first need to understand its
// consumers and consumer groups.

using Confluent.Kafka;

const string brokers = "node-1:9092,node-2:9092";
const string topic = "Kafka.GetStarted.Simple";

const string clientId = "Kafka.GetStarted.Simple.Consumer";
const string groupId = "Kafka.GetStarted.Simple.Consumer";

var cts = new CancellationTokenSource();
var ct = cts.Token;
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};

// By default, the .NET Consumer will commit offsets automatically. This is done
// periodically by a background thread at an interval specified by the
// AutoCommitIntervalMs config property. An offset becomes eligible to be
// committed immediately prior to being delivered to the application via the
// Consume method.
Task autoCommits = Task.Run(() =>
{
    ct.ThrowIfCancellationRequested();

    var config = new ConsumerConfig
    {
        BootstrapServers = brokers,
        ClientId = $"{clientId}.autoCommits",
        GroupId = groupId,
        AutoOffsetReset = AutoOffsetReset.Earliest,
        // AllowAutoCreateTopics = true,
    };

    using var consumer = new ConsumerBuilder<string, string>(config).Build();

    consumer.Subscribe(topic);

    try
    {
        while (!ct.IsCancellationRequested)
        {
            ct.ThrowIfCancellationRequested();
            var cr = consumer.Consume(ct);
            Console.WriteLine($"Consumed event from topic {topic}: key = {cr.Message.Key,-10} value = {cr.Message.Value,-15} partition = {cr.Partition,-10} ClientId = {config.ClientId}");
        }
    }
    catch (OperationCanceledException)
    {
        // Ctrl-C was pressed.
    }
}, ct);

// The C# client allows you to commit offsets explicitly via the Commit method.
Task synchronousCommits = Task.Run(() =>
{
    ct.ThrowIfCancellationRequested();

    var config = new ConsumerConfig
    {
        BootstrapServers = brokers,
        ClientId = $"{clientId}.synchronousCommits",
        GroupId = groupId,
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnableAutoCommit = false,
        // AllowAutoCreateTopics = true,
    };

    using var consumer = new ConsumerBuilder<string, string>(config).Build();

    consumer.Subscribe(topic);

    try
    {
        while (!ct.IsCancellationRequested)
        {
            ct.ThrowIfCancellationRequested();
            var cr = consumer.Consume(ct);
            Console.WriteLine($"Consumed event from topic {topic}: key = {cr.Message.Key,-10} value = {cr.Message.Value,-15} partition = {cr.Partition,-10} ClientId = {config.ClientId}");
            consumer.Commit(cr);
        }
    }
    catch (OperationCanceledException)
    {
        // Ctrl-C was pressed.
    }
}, ct);


await Task.WhenAll(autoCommits, synchronousCommits);