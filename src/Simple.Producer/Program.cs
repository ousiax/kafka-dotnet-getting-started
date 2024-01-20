// There are three primary methods of sending messages:
//
// Fire-and-forget
//  We send a message to the server and don’t really care if it arrives succesfully or
//  not. Most of the time, it will arrive successfully, since Kafka is highly available
//  and the producer will retry sending messages automatically. However, some messages
//  will get lost using this method.
//
// Synchronous send
//  We send a message, the ProduceAsync() method returns a Task object, and we use Wait()
//  to wait on the future and see if the ProduceAsync() was successful or not.
//
// Asynchronous send
//  We call the Produce() method with a callback function, which gets triggered when it
//  receives a response from the Kafka broker.

using Confluent.Kafka;

const string brokers = "node-1:9092,node-2:9092";
const string topic = "Kafka.GetStarted.Simple";

var cts = new CancellationTokenSource();
var ct = cts.Token;
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};

string[] users = ["eabara", "jsmith", "sgarcia", "jbernard", "htanaka", "awalther"];
string[] items = ["book", "alarm clock", "t-shirts", "gift card", "batteries"];

var config = new ProducerConfig
{
    BootstrapServers = brokers,
};

using var producer = new ProducerBuilder<string, string>(config).Build();

var rnd = new Random();

// Fire-and-forget
_ = producer.ProduceAsync(topic, new Message<string, string> { Key = users[rnd.Next(users.Length)], Value = items[rnd.Next(items.Length)] }, cancellationToken: ct);

// Synchronous send
await producer.ProduceAsync(topic, new Message<string, string> { Key = users[rnd.Next(users.Length)], Value = items[rnd.Next(items.Length)] }, cancellationToken: ct);

// Asynchronous send
var numProduced = 0;
const int numMessages = 1_000;

try
{
    for (int i = 0; i < numMessages; ++i)
    {
        ct.ThrowIfCancellationRequested();
        await Task.Delay(TimeSpan.FromSeconds(1));

        var user = users[rnd.Next(users.Length)];
        var item = items[rnd.Next(items.Length)];

        producer.Produce(topic, new Message<string, string> { Key = user, Value = item }, (deliveryReport) =>
        {
            if (deliveryReport.Error.Code != ErrorCode.NoError)
            {
                Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
            }
            else
            {
                Console.WriteLine($"Produced event to topic {topic}: key = {user,-10} value = {item}");
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
