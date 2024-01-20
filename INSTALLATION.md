NOTE: Your local environment must have Java 8+ installed.
- https://docs.confluent.io/platform/current/installation/versions-interoperability.html#java
- https://kafka.apache.org/documentation/#quickstart_startserver

NOTE: KRaft mode is production ready for new clusters as of Apache Kafka 3.3.
- https://developer.confluent.io/learn/kraft/
- https://docs.confluent.io/platform/current/kafka-metadata/kraft.html
- https://www.confluent.io/blog/kafka-without-zookeeper-a-sneak-peek/
- https://cwiki.apache.org/confluence/display/KAFKA/Kafka+Improvement+Proposals
- https://cwiki.apache.org/confluence/display/KAFKA/KIP-853%3A+KRaft+Controller+Membership+Changes
- https://www.redhat.com/en/resources/high-availability-for-apache-kafka-detail

## Optional: Setup Java Development Kit

1. Go to [Eclipse Temurin](https://adoptium.net/temurin/releases/?os=linux&arch=x64&package=jdk&version=17), and download JDK 17-LTS: 

    You can also run the following command:
    
    ```sh
    curl -LO https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.9%2B9/OpenJDK17U-jdk_x64_linux_hotspot_17.0.9_9.tar.gz
    ```

    TIP: On Windows use `certUtil -hashfile file SHA256`, on Linux use `sha256sum file`, and on macOS use `shasum -a 256 file`.

    ```txt
    7b175dbe0d6e3c9c23b6ed96449b018308d8fc94a5ecd9c0df8b8bc376c3c18a
    ```

2. Extract the tar to _/usr/local/jdk_:

    ```sh
    sudo mkdir /usr/local/jdk
    sudo tar xf OpenJDK17U-jdk_x64_linux_hotspot_17.0.9_9.tar.gz -C /usr/local/jdk  --strip-components=1
    ```

3. Set JAVA_HOME in _/etc/profile.d/java.sh_ with the following content:

    ```sh
    JAVA_HOME=/usr/local/jdk
    PATH=$JAVA_HOME/bin:$PATH
    ```

4. Load the environment variables to the current shell and verify the installation:

    ```console
    $ source /etc/profile
    $ java -version
    openjdk version "17.0.9" 2023-10-17
    OpenJDK Runtime Environment Temurin-17.0.9+9 (build 17.0.9+9)
    OpenJDK 64-Bit Server VM Temurin-17.0.9+9 (build 17.0.9+9, mixed mode, sharing)
    ```
    
    NOTE: You can also append the Java to the PATH: 

## Optional: Setup a standalone server in KRaft combined mode as a proof of concept.

1. Go to [https://dlcdn.apache.org](https://www.apache.org/dyn/closer.cgi?path=/kafka/3.6.1/kafka_2.13-3.6.1.tgz), download the latest Kafka.

    ```sh
    curl -LO https://dlcdn.apache.org/kafka/3.6.1/kafka_2.13-3.6.1.tgz
    ```
    
2. Create a `kafka` user and extract the tar to the home.

    ```console
    $ sudo useradd -m kafka # [-s /bin/bash] Specify the login shell of the new account.
    $ sudo su - kafka
    $ sudo tar xf kafka_2.13-3.6.1.tgz -C /home/kafka/ --strip-components=1
    ```
    
    NOTE: Running Kafka as root is not a recommended configuration.
    
    TIP: You might need to set the password for the kafka user, and join it to the sudo group.
    
    ```console
    $ sudo passwd kafka
    $ sudo usermod -aG sudo kafka # sudo usermod -aG wheel kafka (RHEL, or CentOS)
    ```

3. Generate a Cluster UUID
    
    ```console
    $ KAFKA_CLUSTER_ID="$(bin/kafka-storage.sh random-uuid)"
    ```
    
4. Format Log Directories

    ```console
    $ bin/kafka-storage.sh format -t $KAFKA_CLUSTER_ID -c config/kraft/server.properties
    Formatting /tmp/kraft-combined-logs with metadata.version 3.6-IV2.
    ```
    
5. Start the Kafka Server

    ```console
    $ bin/kafka-server-start.sh config/kraft/server.properties
    ...
    [2024-01-12 23:22:34,872] INFO [SocketServer listenerType=CONTROLLER, nodeId=1] Enabling request processing. (kafka.network.SocketServer)
    [2024-01-12 23:22:34,881] INFO [MetadataLoader id=1] InitializeNewPublishers: initializing ScramPublisher controller id=1 with a snapshot at offset 4 (org.apache.kafka.image.loader.MetadataLoader)
    [2024-01-12 23:22:34,911] INFO Awaiting socket connections on 0.0.0.0:9093. (kafka.network.DataPlaneAcceptor)
    ...
    [2024-01-12 23:22:36,629] INFO [SocketServer listenerType=BROKER, nodeId=1] Enabling request processing. (kafka.network.SocketServer)
    [2024-01-12 23:22:36,629] INFO Awaiting socket connections on 0.0.0.0:9092. (kafka.network.DataPlaneAcceptor)
    ...
    ```
    
    TIP: The application logs (not to be confused with the commit log, or data log) are located at `./logs` which is configured in the _log4j.properties_.

6. Once the Kafka server has successfully launched:

    * Open another terminal session and create a topic:
    
        ```console
        $ bin/kafka-topics.sh --create --topic quickstart-events --bootstrap-server localhost:9092
        Created topic quickstart-events.
        $ bin/kafka-topics.sh --describe --topic quickstart-events --bootstrap-server localhost:9092
        Topic: quickstart-events    TopicId: wx6vplZjRHaJubPnPP3_QQ    PartitionCount: 1    ReplicationFactor: 1    Configs: segment.bytes=1073741824
            Topic: quickstart-events    Partition: 0    Leader: 1    Replicas: 1    Isr: 1
        ```
    
    * Run the console producer client to write a few events into your topic:
    
        ```console
        $ bin/kafka-console-producer.sh --topic quickstart-events --bootstrap-server localhost:9092
        This is my first event
        This is my second event
        ```
        
    * Open another terminal session and run the console consumer client to read the events you just created:
    
        ```console
        $ bin/kafka-console-consumer.sh --topic quickstart-events --from-beginning --bootstrap-server localhost:9092
        This is my first event
        This is my second event
        ```
## Setup a Kafka cluster in KRaft

1. Make sure the nodes in the cluster could be reachable each other.

    TIP: You can use the hostname, DNS name, or even IP address to connect each other.
    
    TIP: You can run the `ip a s` to show the addresses assigned to all network interfaces.
    
    TIP: The following steps will be demostrated with the following two nodes (`/etc/hosts`):
    
    ```txt
    192.168.56.131 node-1
    192.168.56.132 node-2
    ```
    
    TIP: You might need to stop and disable the firewall:

    ```console
    # view the current status
    $ sudo firewall-cmd --state 
    running
    # stop the FirewallD service
    $ sudo systemctl stop firewalld.service 
    [root@node-2 ~]# iptables -L
    Chain INPUT (policy ACCEPT)
    target     prot opt source               destination         

    Chain FORWARD (policy ACCEPT)
    target     prot opt source               destination         

    Chain OUTPUT (policy ACCEPT)
    target     prot opt source               destination       
    # Disable the FirewallD service  
    $ sudo systemctl disable firewalld.service 
    Removed symlink /etc/systemd/system/multi-user.target.wants/firewalld.service.
    Removed symlink /etc/systemd/system/dbus-org.fedoraproject.FirewallD1.service.
    ```

2. Go to [https://dlcdn.apache.org](https://www.apache.org/dyn/closer.cgi?path=/kafka/3.6.1/kafka_2.13-3.6.1.tgz), download the latest Kafka.

    ```console
    $ curl -LO https://dlcdn.apache.org/kafka/3.6.1/kafka_2.13-3.6.1.tgz
    ```
    
3. Create a `kafka` user and extract the tar to the home at each node.

    ```console
    $ sudo useradd -m kafka # [-s /bin/bash] Specify the login shell of the new account.
    $ sudo su - kafka
    $ sudo tar xf kafka_2.13-3.6.1.tgz -C /home/kafka/ --strip-components=1
    ```
    
    NOTE: Running Kafka as root is not a recommended configuration.
    
    TIP: You might need to set the password for the kafka user, and join it to the sudo group.
    
    ```console
    $ sudo passwd kafka
    $ sudo usermod -aG sudo kafka # sudo usermod -aG wheel kafka (RHEL, or CentOS)
    ```    
4. Generate a Cluster UUID
    
    ```console
    $ KAFKA_CLUSTER_ID="$(bin/kafka-storage.sh random-uuid)"
    $ echo $KAFKA_CLUSTER_ID
    MkU3OEVBNTcwNTJENDM2Qk
    ```
    
    Note down the value of `KAFKA_CLUSTER_ID` and copy it to each node in _/etc/profile.d/kafka.sh_ with the following content:

    ```sh
    KAFKA_CLUSTER_ID=MkU3OEVBNTcwNTJENDM2Qk
    ```
    
    Load the environment variables to the current shell with the following command:
    
    ```sh
    source /etc/profile
    ```

5. Backup the orignal config on each node:

    ```sh
    cp -a config config.org
    ```
    
6. Create `log.dirs` with the following commands on each node:

    ```sh
    sudo mkdir -p /var/lib/kafka
    sudo chown kafka:kafka /var/lib/kafka
    ```
    
7. Update or specify the following properties in _config/kraft/controller.properties_:
    
    ```properties
    # The node id associated with this instance's roles
    # !!! on the second node, set the node.id to be 3002.
    node.id=3001
    
    # The connect string for the controller quorum
    controller.quorum.voters=3001@node-1:9093,3002@node-2:9093
    
    # Use to specify where the metadata log for clusters in KRaft mode is placed.
    log.dirs=/var/lib/kafka/controller
    ```
    
    NOTE: Each node ID (`node.id`) must be unique across all the servers in a particular cluster.
    
8. Update or specify the following properties in _config/kraft/broker.properties_:
    
    ```properties
    # The node id associated with this instance's roles
    # !!! on the second node, set the node.id to be 1002.
    node.id=1001
    
    # The connect string for the controller quorum
    controller.quorum.voters=3001@node-1:9093,3002@node-2:9093
    
    # The address the socket server listens on.
    listeners=PLAINTEXT://:9092

    # Listener name, hostname and port the broker will advertise to clients.
    # !!! on the second node, set it to be `PLAINTEXT://172.16.4.60:9092`.
    advertised.listeners=PLAINTEXT://172.16.4.45:9092

    
    # The directory in which the log data is kept.
    log.dirs=/var/lib/kafka/data
    ```
    
    NOTE: Each node ID (`node.id`) must be unique across all the servers in a particular cluster.
    
    NOTE: The `advertised.listeners` should be reachable by the clients outside the cluster. You could set it with a reachable hostname or DNS name, or an external IP address.

    TIP: You might need to disable auto creation of topic on the server:
    
    ```properties
    # Enable auto creation of topic on the server.
    auto.create.topics.enable=false
    ```
    
9. Format Log Directories

    ```sh
    bin/kafka-storage.sh format -t $KAFKA_CLUSTER_ID -c config/kraft/controller.properties
    bin/kafka-storage.sh format -t $KAFKA_CLUSTER_ID -c config/kraft/broker.properties
    ```
    
10. Start the Kafka Controller and Broker on each node.

    ```sh
    bin/kafka-server-start.sh -daemon config/kraft/controller.properties
    bin/kafka-server-start.sh -daemon config/kraft/broker.properties
    ```
        
    TIP: To enable the JMX in Kafka brokers, set the `JMX_PORT` with an used port number (e.g. 9101):
    
    ```console
    $ JMX_PORT=9101 bin/kafka-server-start.sh -daemon config/kraft/broker.properties
    ```
    
    TIP: The application logs (not to be confused with the commit log) are located at `./logs` which is configured in the _log4j.properties_.
    
11. Use the `kafka-metadata-quorum` tool to query the metadata quorum status.

    Use the `kafka-metadata-quorum` tool to query the metadata quorum status. This tool is useful when you are debugging a cluster in KRaft mode. Pass the describe command to describe the current state of the metadata quorum.

    The following code example displays a summary of the metadata quorum:

    ```console
    $ bin/kafka-metadata-quorum.sh --bootstrap-server node-1:9092 describe --status
    ClusterId:              MkU3OEVBNTcwNTJENDM2Qg
    LeaderId:               3002
    LeaderEpoch:            83
    HighWatermark:          779
    MaxFollowerLag:         0
    MaxFollowerLagTimeMs:   408
    CurrentVoters:          [3001,3002]
    CurrentObservers:       [1001,1002]
    ```
        
## Install Schema Registry

1. Download Confluent Platform using only Confluent Community components by using the `curl` command:

    ```sh
    curl -O https://packages.confluent.io/archive/7.5/confluent-community-7.5.3.tar.gz
    ```

2. Extract the contents of the archive to /home/kafka/confluent:

    ```sh
    mkdir /home/kafka/confluent
    tar xf confluent-community-7.5.3.tar.gz -C /home/kafka/confluent/ --strip-components=1
    cd /home/kafka/confluent
    cp -a etc/ etc.org # backup the orginal etc/
    ```

3. Navigate to the Schema Registry properties file (_etc/schema-registry/schema-registry.properties_) and specify or update the following properties:

    ```properties
    # Specify the address the socket server listens on, e.g. listeners = PLAINTEXT://your.host.name:9092
    listeners=http://0.0.0.0:8081
    
    # The advertised host name. Make sure to set this if running Schema Registry with multiple nodes.
    host.name=172.16.4.45
    
    # List of Kafka brokers to connect to, e.g. PLAINTEXT://hostname:9092,SSL://hostname2:9092
    kafkastore.bootstrap.servers=PLAINTEXT://172.16.4.45:9092,PLAINTEXT://172.16.4.60:9092
    ```

4. Start Schema Registry. Run this command in its own terminal:

    ```sh
    bin/schema-registry-start -daemon etc/schema-registry/schema-registry.properties
    ```

5. Show the `_schemas` information:

    ```console
    $ bin/kafka-topics.sh --describe --topic _schemas --bootstrap-server node-1:9092
    Topic: _schemas    TopicId: 9A_-36hMRYuTfUyhQwMm6Q    PartitionCount: 1    ReplicationFactor: 2    Configs: cleanup.policy=compact,segment.bytes=1073741824
        Topic: _schemas    Partition: 0    Leader: 1001    Replicas: 1001,1002    Isr: 1001,1002
    ```
    
6. Repeat the above steps on the second node.

    NOTE: Replace the hostname `172.16.4.45` with `172.16.4.60`.
    
## UI for Apache Kafka

[UI for Apache Kafka](https://github.com/provectus/kafka-ui) is a free, open-source web UI to monitor and manage Apache Kafka clusters.

To run UI for Apache Kafka, you can use either a pre-built Docker image:

```console
$ docker run -it -p 8080:8080 -e DYNAMIC_CONFIG_ENABLED=true provectuslabs/kafka-ui
```

or build it (or a jar file) yourself:

1. Create a directory at _/home/kafka/kafka-ui_, and download the latest kafka-ui jar file from the https://github.com/provectus/kafka-ui/releases:

    ```console
    $ mkdir /home/kafka/kafka-ui
    $ cd /home/kafka/kafka-ui
    $ curl -LO https://github.com/provectus/kafka-ui/releases/download/v0.7.1/kafka-ui-api-v0.7.1.jar
    ```
    
2. Create a configuration file named _application-local.yml_ with the following content:

    ```yml
    server:
      port: 8080 #- Port in which kafka-ui will run.
    
    kafka:
      clusters:
        - name: local
          bootstrapServers: 172.16.4.45:9092,172.16.4.60:9092
          schemaRegistry: http://172.16.4.45:8081,http://172.16.4.60:8081
          
    dynamic.config.enabled: true
    ```

3. Execute the jar:

    ```sh
    nohup java -Dspring.config.additional-location=application-local.yml --add-opens java.rmi/javax.rmi.ssl=ALL-UNNAMED -jar kafka-ui-api-v0.7.1.jar > kafka-ui-nohup.out 2>&1 &
    ```
4. Repeat the above steps on the second node.

## Apache Kafka .NET Client

NOTE: This guide assumes that you already have .NET (>= 6.0) installed.

Confluent develops and maintains [confluent-kafka-dotnet](https://github.com/confluentinc/confluent-kafka-dotnet), a .NET library that provides a high-level producer, consumer and AdminClient compatible with all Apache Kafka® brokers version 0.8 and later, Confluent Cloud and Confluent Platform.

TIP: Before you start to pub/sub messages, you might need to create a topic like this:

```console
$ bin/kafka-topics.sh --create --topic Kafka.GetStarted.Simple --partitions 2 --replication-factor=2 --bootstrap-server node-1:9092
WARNING: Due to limitations in metric names, topics with a period ('.') or underscore ('_') could collide. To avoid issues it is best to use either, but not both.
Created topic Kafka.GetStarted.Simple.
```

For more information about the `kafka-topics.sh` tool, see `kafka-topics.sh --help`.

You can also let the brokers to create the topics automatically, if the feature is enabled with `auto.create.topics.enable=true` which is true by default.


- https://docs.confluent.io/kafka-clients/dotnet/current/overview.html
- https://www.confluent.io/blog/designing-the-net-api-for-apache-kafka/


Install the required Nuget packages:

```sh
dotnet add package Confluent.Kafka --version 2.3.0
```
// Provides an Avro Serializer and Deserializer for use 
// with Confluent.Kafka with Confluent Schema Registry integration
```sh
dotnet add package Confluent.SchemaRegistry.Serdes.Avro --version 2.3.0
```
// Provides a JSON Serializer and Deserializer for use
// with Confluent.Kafka with Confluent Schema Registry integration
```sh
dotnet add package Confluent.SchemaRegistry.Serdes.Json --version 2.3.0
```
