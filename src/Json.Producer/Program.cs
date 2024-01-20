using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Json.Serdes.Models;
using Newtonsoft.Json;

const string brokers = "node-1:9092,node-2:9092";
const string schemaRegistries = "http://node-1:8081,http://node-2:8081";
const string topic = "Kafka.GetStarted.Json";

var cts = new CancellationTokenSource();
var ct = cts.Token;
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};

string[] users = ["eabara", "jsmith", "sgarcia", "jbernard", "htanaka", "awalther"];
string[] items = ["book", "alarm clock", "t-shirts", "gift card", "batteries"];

var schemaRegistryConfig = new SchemaRegistryConfig
{
    Url = schemaRegistries,
};

var config = new ProducerConfig
{
    BootstrapServers = brokers,
    AllowAutoCreateTopics = true,
};

using var cachedSchemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
using var producer = new ProducerBuilder<string, User>(config)
    .SetValueSerializer(new JsonSerializer<User>(cachedSchemaRegistryClient).AsSyncOverAsync())
    .Build();

var rnd = new Random();
var numProduced = 0;
const int numMessages = 1_000;

try
{
    for (int i = 0; i < numMessages; ++i)
    {
        ct.ThrowIfCancellationRequested();
        await Task.Delay(TimeSpan.FromSeconds(1));

        var user = new User { Name = users[rnd.Next(users.Length)], Item = items[rnd.Next(items.Length)] };

        producer.Produce(topic, new Message<string, User> { Key = user.Name, Value = user }, (deliveryReport) =>
        {
            if (deliveryReport.Error.Code != ErrorCode.NoError)
            {
                Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
            }
            else
            {
                Console.WriteLine($"Produced event to topic {topic}: key = {user.Name,-10} value = {user.ToJsonString()}");
                numProduced += 1;
            }
        });
    }
}
catch (OperationCanceledException)
{
    // Ctrl-C was pressed.
}
finally
{
    producer.Flush(TimeSpan.FromSeconds(10));
    Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
}

static class UserExtension
{
    public static string ToJsonString(this User user)
    {
        return JsonConvert.SerializeObject(user, Formatting.None);
    }
}