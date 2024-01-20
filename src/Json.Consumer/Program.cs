using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;
using Json.Serdes.Models;
using Newtonsoft.Json;

const string brokers = "node-1:9092,node-2:9092";
// const string schemaRegistries = "http://node-1:8081,http://node-2:8081";
const string topic = "Kafka.GetStarted.Json";

const string clientId = "Kafka.GetStarted.Json.Consumer";
const string groupId = "Kafka.GetStarted.Json.Consumer";

var cts = new CancellationTokenSource();
var ct = cts.Token;
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};

var config = new ConsumerConfig
{
    BootstrapServers = brokers,
    ClientId = clientId,
    GroupId = groupId,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    AllowAutoCreateTopics = true,
};

using var consumer = new ConsumerBuilder<string, User>(config)
    .SetValueDeserializer(new JsonDeserializer<User>().AsSyncOverAsync())
    .Build();

consumer.Subscribe(topic);

try
{
    while (!ct.IsCancellationRequested)
    {
        ct.ThrowIfCancellationRequested();
        var cr = consumer.Consume(ct);
        Console.WriteLine($"Consumed event from topic {topic}: key = {cr.Message.Key,-10} value = {cr.Message.Value.ToJsonString(),-20}");
    }
}
catch (OperationCanceledException)
{
    // Ctrl-C was pressed.
}

static class UserExtension
{
    public static string ToJsonString(this User user)
    {
        return JsonConvert.SerializeObject(user, Formatting.None);
    }
}
