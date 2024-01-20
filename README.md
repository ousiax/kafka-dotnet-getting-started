## Apache Kafka .NET Client

NOTE: This guide assumes that you already have .NET (>= 8.0) installed.

Confluent develops and maintains [confluent-kafka-dotnet](https://github.com/confluentinc/confluent-kafka-dotnet), a .NET library that provides a high-level producer, consumer and AdminClient compatible with all Apache Kafka® brokers version 0.8 and later, Confluent Cloud and Confluent Platform.

TIP: See [INSTALLATION.md](INSTALLATION.md) to setup a Kafka cluster.

TIP: See [INSTALLATION.DOCKER.md](INSTALLATION.DOCKER.mmd) to setup a Kafka cluster using Docker.

TIP: Before starting to pub/sub messages, we might need to create a topic like this:

```console
$ bin/kafka-topics.sh --create --topic Kafka.GetStarted.Simple --partitions 2 --replication-factor=2 --bootstrap-server node-1:9092
WARNING: Due to limitations in metric names, topics with a period ('.') or underscore ('_') could collide. To avoid issues it is best to use either, but not both.
Created topic Kafka.GetStarted.Simple.
```

For more information about the `kafka-topics.sh` tool, see `kafka-topics.sh --help`.

we can also let the brokers/clients to create the topics automatically, if the feature is enabled with `auto.create.topics.enable=true` which is true by default.

## Install the required Nuget packages

```sh
dotnet add package Confluent.Kafka --version 2.3.0
```

Provides an Avro Serializer and Deserializer for use  with Confluent.Kafka with Confluent Schema Registry integration:

```sh
dotnet add package Confluent.SchemaRegistry.Serdes.Avro --version 2.3.0
```

Provides a JSON Serializer and Deserializer for use with Confluent.Kafka with Confluent Schema Registry integration:

```sh
dotnet add package Confluent.SchemaRegistry.Serdes.Json --version 2.3.0
```

## Project layout

```txt
.
├── global.json
├── INSTALLATION.md
├── kafka-dotnet-getting-started.sln
├── README.md
└── src
    ├── Avro.Consumer
    ├── Avro.Producer
    ├── Json.Consumer
    ├── Json.Producer
    ├── Json.Serdes.Schemaless
    ├── Json.Serdes.Schemaless.Consumer
    ├── Json.Serdes.Schemaless.Producer
    ├── Simple.Consumer
    └── Simple.Producer

```

This solution contains four samples about serdes (i.e. serialzation and deserialization) to pub/sub messages to/from Kafka:

- Simple serdes: Simple.Producer, Simple.Consumer

- Avro serdes: Avro.Producer, Avro.Consumer
    TIP: High performance, and schema evolvable, but a bit complexity.
 
- Json serdes: Json.Producer, Json.Consumer
    TIP: Simple, but lower perfomance.

- Json serdes without Schema Registry: Json.Serdes.Schemaless, Json.Serdes.Schemaless.Producer, Json.Serdes.Schemaless.Consumer
    TIP: NOT recommended.

```console
kafka-dotnet-getting-started/src/Simple.Producer (main)
$ dotnet run
Produced event to topic Kafka.GetStarted.Simple: key = jbernard   value = book
Produced event to topic Kafka.GetStarted.Simple: key = jbernard   value = gift card
Produced event to topic Kafka.GetStarted.Simple: key = jsmith     value = book
Produced event to topic Kafka.GetStarted.Simple: key = sgarcia    value = book
Produced event to topic Kafka.GetStarted.Simple: key = jbernard   value = t-shirts
Produced event to topic Kafka.GetStarted.Simple: key = htanaka    value = gift card
Produced event to topic Kafka.GetStarted.Simple: key = sgarcia    value = batteries
Produced event to topic Kafka.GetStarted.Simple: key = eabara     value = alarm clock
Produced event to topic Kafka.GetStarted.Simple: key = awalther   value = book
Produced event to topic Kafka.GetStarted.Simple: key = jsmith     value = alarm clock
Produced event to topic Kafka.GetStarted.Simple: key = sgarcia    value = gift card
Produced event to topic Kafka.GetStarted.Simple: key = sgarcia    value = book
Produced event to topic Kafka.GetStarted.Simple: key = jbernard   value = batteries
Produced event to topic Kafka.GetStarted.Simple: key = eabara     value = book
Produced event to topic Kafka.GetStarted.Simple: key = sgarcia    value = batteries
Produced event to topic Kafka.GetStarted.Simple: key = eabara     value = gift card
Produced event to topic Kafka.GetStarted.Simple: key = jbernard   value = book
Produced event to topic Kafka.GetStarted.Simple: key = awalther   value = book
Produced event to topic Kafka.GetStarted.Simple: key = eabara     value = t-shirts
Produced event to topic Kafka.GetStarted.Simple: key = htanaka    value = book
Produced event to topic Kafka.GetStarted.Simple: key = eabara     value = t-shirts
Produced event to topic Kafka.GetStarted.Simple: key = jsmith     value = batteries
Produced event to topic Kafka.GetStarted.Simple: key = awalther   value = book
Produced event to topic Kafka.GetStarted.Simple: key = jbernard   value = alarm clock
Produced event to topic Kafka.GetStarted.Simple: key = sgarcia    value = book
Produced event to topic Kafka.GetStarted.Simple: key = htanaka    value = batteries
Produced event to topic Kafka.GetStarted.Simple: key = sgarcia    value = alarm clock
Produced event to topic Kafka.GetStarted.Simple: key = jsmith     value = book
Produced event to topic Kafka.GetStarted.Simple: key = eabara     value = gift card
Produced event to topic Kafka.GetStarted.Simple: key = jsmith     value = book
Produced event to topic Kafka.GetStarted.Simple: key = jsmith     value = batteries
Produced event to topic Kafka.GetStarted.Simple: key = jbernard   value = book
Produced event to topic Kafka.GetStarted.Simple: key = awalther   value = alarm clock
33 messages were produced to topic Kafka.GetStarted.Simple
```

```console
kafka-dotnet-getting-started/src/Simple.Consumer (main)
$ dotnet run
Consumed event from topic Kafka.GetStarted.Simple: key = jbernard   value = alarm clock     partition = [0]        ClientId = Kafka.GetStarted.Simple.Consumer.autoCommits
Consumed event from topic Kafka.GetStarted.Simple: key = jbernard   value = book            partition = [0]        ClientId = Kafka.GetStarted.Simple.Consumer.autoCommits
Consumed event from topic Kafka.GetStarted.Simple: key = jbernard   value = gift card       partition = [0]        ClientId = Kafka.GetStarted.Simple.Consumer.autoCommits
Consumed event from topic Kafka.GetStarted.Simple: key = jbernard   value = t-shirts        partition = [0]        ClientId = Kafka.GetStarted.Simple.Consumer.autoCommits
Consumed event from topic Kafka.GetStarted.Simple: key = eabara     value = alarm clock     partition = [0]        ClientId = Kafka.GetStarted.Simple.Consumer.autoCommits
Consumed event from topic Kafka.GetStarted.Simple: key = jbernard   value = batteries       partition = [0]        ClientId = Kafka.GetStarted.Simple.Consumer.autoCommits
Consumed event from topic Kafka.GetStarted.Simple: key = eabara     value = book            partition = [0]        ClientId = Kafka.GetStarted.Simple.Consumer.autoCommits
Consumed event from topic Kafka.GetStarted.Simple: key = eabara     value = gift card       partition = [0]        ClientId = Kafka.GetStarted.Simple.Consumer.autoCommits
Consumed event from topic Kafka.GetStarted.Simple: key = jbernard   value = book            partition = [0]        ClientId = Kafka.GetStarted.Simple.Consumer.autoCommits
Consumed event from topic Kafka.GetStarted.Simple: key = eabara     value = t-shirts        partition = [0]        ClientId = Kafka.GetStarted.Simple.Consumer.autoCommits
Consumed event from topic Kafka.GetStarted.Simple: key = eabara     value = t-shirts        partition = [0]        ClientId = Kafka.GetStarted.Simple.Consumer.autoCommits
Consumed event from topic Kafka.GetStarted.Simple: key = jbernard   value = alarm clock     partition = [0]        ClientId = Kafka.GetStarted.Simple.Consumer.autoCommits
Consumed event from topic Kafka.GetStarted.Simple: key = htanaka    value = gift card       partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = jsmith     value = book            partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = sgarcia    value = book            partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = htanaka    value = gift card       partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = sgarcia    value = batteries       partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = awalther   value = book            partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = jsmith     value = alarm clock     partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = sgarcia    value = gift card       partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = sgarcia    value = book            partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = sgarcia    value = batteries       partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = awalther   value = book            partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = htanaka    value = book            partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = jsmith     value = batteries       partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = awalther   value = book            partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = sgarcia    value = book            partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = htanaka    value = batteries       partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
Consumed event from topic Kafka.GetStarted.Simple: key = sgarcia    value = alarm clock     partition = [1]        ClientId = Kafka.GetStarted.Simple.Consumer.synchronousCommits
```

## Serializing and deserializing with Apache Avro

Defining a schema:

```avsc
{
  "namespace": "Avro.Producer.Models",
  "type": "record",
  "doc": "A simple message type as used by this sample.",
  "name": "User",
  "fields": [
    {
      "name": "Name",
      "type": "string"
    },
    {
      "name": "Item",
      "type": "string"
    }
  ]
}
```

To generate the specific .NET classes from the avro schemas, we use the Apache.Avro.Tools .NET tools to generate code as follows:

```sh
dotnet tool install --global Apache.Avro.Tools --version 1.11.3
```

The `avrogen` (version: 1.11.3) requires .NET 7.0. To install missing framework, download:

```txt
https://aka.ms/dotnet-core-applaunch?framework=Microsoft.NETCore.App&framework_version=7.0.0&arch=x64&rid=win-x64&os=win10you 
```

or use the winget (e.g. on Windows):

```sh
winget install Microsoft.DotNet.Runtime.7
```

For instance, to generate a User class from the schema defined above, run

```sh
avrogen -s Schemas/User.avsc ./Models --skip-directories
```

## Serializing and deserializing using JSON without Schema Registry (NOT recommended.)

```cs
public class JsonSerializer<T> : IAsyncSerializer<T>, ISerializer<T>
{
    public Task<byte[]> SerializeAsync(T data, SerializationContext context)
    {
        return Task.FromResult(Serialize(data, context));
    }

    public byte[] Serialize(T data, SerializationContext context)
    {
        return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(data);
    }
}

public class JsonDeserializer<T> : IAsyncDeserializer<T>, IDeserializer<T>
{
    public Task<T> DeserializeAsync(ReadOnlyMemory<byte> data, bool isNull, SerializationContext context)
    {
        T result = Deserialize(data.Span, isNull, context)!;
        return Task.FromResult(result);
    }
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(data)!;
    }
}
```

## Refereces:

* [1] https://developer.confluent.io/learn/kraft/
* [2] https://docs.confluent.io/platform/current/kafka-metadata/kraft.html
* [3] https://www.redhat.com/en/resources/high-availability-for-apache-kafka-detail
* [4] https://kafka.apache.org/quickstart
* [5] https://kafka.apache.org/documentation/#monitoring
* [6] https://access.redhat.com/documentation/en-us/red_hat_amq_streams/2.5/html/using_amq_streams_on_rhel/monitoring-str
* [7] https://www.confluent.io/blog/kafka-listeners-explained/
* [8] https://docs.confluent.io/platform/current/schema-registry/index.html
* [9] https://www.conduktor.io/blog/what-is-the-schema-registry-and-why-do-you-need-to-use-it/
* [10] "20170707-EB-Confluent_Kafka_Definitive-Guide_Complete", https://www.confluent.io/resources/kafka-the-definitive-guide/
* [11] https://docs.confluent.io/platform/current/installation/installing_cp/zip-tar.html
* [12] https://docs.confluent.io/platform/current/schema-registry/installation/deployment.html
* [13] https://docs.confluent.io/platform/current/schema-registry/multidc.html
* [14] https://docs.confluent.io/platform/current/schema-registry/fundamentals/index.html
* [15] https://docs.kafka-ui.provectus.io/overview/getting-started
* [16] https://docs.confluent.io/platform/current/schema-registry/fundamentals/serdes-develop/index.html
* [17] https://www.confluent.io/blog/avro-kafka-data/
* [18] https://avro.apache.org/
* [19] https://docs.confluent.io/kafka-clients/dotnet/current/overview.html
* [20] https://www.confluent.io/blog/decoupling-systems-with-apache-kafka-schema-registry-and-avro/
