NOTE: Make sure the nodes in the cluster could be reachable each other.

TIP: You can use the hostname, DNS name, or an external IP address to connect each other.
    
TIP: You can run the `ip a s` to show the addresses assigned to all network interfaces.
    
TIP: The following steps will be demostrated with the following two nodes:
    
```txt
192.168.56.131 node-1
192.168.56.132 node-2
```

1. Optional: Install Docker Engine on CentOS:

    ```sh
    sudo yum install -y yum-utils
    sudo yum-config-manager --add-repo https://download.docker.com/linux/  entos/    docker-ce.repo
    sudo yum install docker-ce docker-ce-cli containerd.io   ocker-buildx-plugin     docker-compose-plugin
    sudo systemctl enable docker.service
    sudo systemctl start docker
    ```

    You might need to configure the Docker daemon to use a different data directory (by default: _/var/lib/docker_ on Linux) and the log driver options.

    1. Create the configuration file at _/etc/docker/daemon.json_ with the following content:

        ```json
        {
          "data-root": "/mnt/docker-data",
          "log-opts": {
            "max-file": "5",
            "max-size": "10m"
          }
        }
        ```

    2. Restart Docker:

        ```sh
        sudo systemctl start docker
        ```

    3. Show Docker version:

        ```sh
        $ sudo docker --version
        Docker version 24.0.7, build afdd53b
        ```

2. Stop and disable the _firewalld.service_:

    * View the current status:
    
        ```sh
        sudo firewall-cmd --state 
        ```

    * Stop the FirewallD service:
    
        ```sh
        sudo systemctl stop firewalld.service
        ```

    * List the rules:
    
        ```console
        $ sudo iptables -L
        Chain INPUT (policy ACCEPT)
        target     prot opt source               destination         

        Chain FORWARD (policy ACCEPT)
        target     prot opt source               destination         

        Chain OUTPUT (policy ACCEPT)
        target     prot opt source               destination       
        ```

    * Disable the FirewallD service
    
        ```sh
        sudo systemctl disable firewalld.service
        ```

3. Optional: Generate a Cluster UUID:

    ```console
    $ KAFKA_CLUSTER_ID="$(docker run --rm confluentinc/cp-kafka:7.5.3 kafka-storage random-uuid)"
    $ echo $KAFKA_CLUSTER_ID
    MkU3OEVBNTcwNTJENDM2Qg
    ```

4. Copy the _docker/_ directory to all the nodes in the Kafka cluster:

5. Start the controllers:

    On node-1:

    * Update the _compose.override.yml_ in _docker/controller/compose.override.yml_:

        ```yml
        version: "2.4"
        services:
          controller:
            environment:
              KAFKA_NODE_ID: 3001
              KAFKA_CONTROLLER_QUORUM_VOTERS: '3001@node-1:9093,3002@node-2:9093'
              CLUSTER_ID: 'MkU3OEVBNTcwNTJENDM2Qg'
            extra_hosts:
              - "node-1:192.168.56.131"
              - "node-2:192.168.56.132"
        ```
      
      TIP: Update the `CLUSTER_ID` with the `KAFKA_CLUSTER_ID` that generated at step 2.

      NOTE: Each node ID (`KAFKA_NODE_ID`) must be unique across all the nodes in a particular cluster.

    * Start the Kraft controller:

        ```sh
        cd docker/controller
        docker compose up -d
        ```

    On node-2:

    * Repeat the above steps and update the `KAFKA_NODE_ID` with `3002`.


6. Start the brokers:

    On node-1:

    * Update the _compose.override.yml_ in _docker/broker/compose.override.yml_:

        ```yml
        version: "2.4"
        services:
          controller:
            environment:
              KAFKA_NODE_ID: 1001
              KAFKA_ADVERTISED_LISTENERS: 'PLAINTEXT://node-1:9092'
              KAFKA_CONTROLLER_QUORUM_VOTERS: '3001@node-1:9093,3002@node-2:9093'
              CLUSTER_ID: 'MkU3OEVBNTcwNTJENDM2Qg'
            extra_hosts:
              - "node-1:192.168.56.131"
              - "node-2:192.168.56.132"
        ```
      
      TIP: Update the `CLUSTER_ID` with the `KAFKA_CLUSTER_ID` that generated at step 2.

      NOTE: Each node ID (`KAFKA_NODE_ID`) must be unique across all the nodes in a particular cluster.

      NOTE: The `KAFKA_ADVERTISED_LISTENERS` should be reachable by the clients outside the cluster. You could set it with a reachable hostname or DNS name, or an external IP address.

    * Start the broker:

        ```sh
        cd docker/broker
        docker compose up -d
        ```

    * Use `kcat` to display the current state of the Kafka cluster and its topics, partitions, replicas and in-sync replicas (ISR).

        ```console
        $ docker run --rm --add-host node-1:192.168.56.131 confluentinc/cp-kcat:7.5.3 -b node-1:9092 -L
        Metadata for all topics (from broker -1: node-1:9092/bootstrap):
         1 brokers:
          broker 1001 at node-1:9092 (controller)
         0 topics:
        ```

    * Use the `kafka-metadata-quorum` tool to view the metadata quorum status.

        ```console
        $ docker run --rm --add-host node-1:192.168.56.131 confluentinc/cp-kafka:7.5.3 kafka-metadata-quorum --bootstrap-server node-1:9092 describe --status
        ClusterId:              MkU3OEVBNTcwNTJENDM2Qg
        LeaderId:               3002
        LeaderEpoch:            28
        HighWatermark:          47816
        MaxFollowerLag:         0
        MaxFollowerLagTimeMs:   32
        CurrentVoters:          [3001,3002]
        CurrentObservers:       [1001]
        ```

    On node-2:

    * Repeat the above steps and update the `KAFKA_NODE_ID` with `1002`, and `KAFKA_ADVERTISED_LISTENERS` with `'PLAINTEXT://node-2:9092'`.

    * Use `kcat` to display the current state of the Kafka cluster and its topics, partitions, replicas and in-sync replicas (ISR).

        ```console
        $ docker run --rm --add-host node-2:192.168.56.132 confluentinc/cp-kcat:7.5.3 -b node-2:9092 -L
        Metadata for all topics (from broker 1002: node-2:9092/1002):
         2 brokers:
          broker 1001 at node-2:9092
          broker 1002 at node-2:9092 (controller)
         0 topics:
        ```

    * Use the `kafka-metadata-quorum` tool to view the metadata quorum status.

        ```console
        $ docker run --rm --add-host node-2:192.168.56.132 confluentinc/cp-kafka:7.5.3 kafka-metadata-quorum --bootstrap-server node-2:9092 describe --status
        ClusterId:              MkU3OEVBNTcwNTJENDM2Qg
        LeaderId:               3002
        LeaderEpoch:            28
        HighWatermark:          47816
        MaxFollowerLag:         0
        MaxFollowerLagTimeMs:   32
        CurrentVoters:          [3001,3002]
        CurrentObservers:       [1001,1002]
        ```

8. Start the UI Kafka:

    On node-1:

    * Update the _compose.override.yml_ in _docker/controller/compose.override.yml_:

        ```yml
        version: "2.4"
        services:
          kafka-ui:
            environment:
              KAFKA_CLUSTERS_0_NAME: iot
              KAFKA_CLUSTERS_0_BOOTSTRAPSERVERS: node-1:9092,node-2:9092
            extra_hosts:
              - "node-1:192.168.56.131"
              - "node-2:192.168.56.132"
        ```

    * Start the kafka-ui:

        ```sh
        cd docker/kafka-ui
        docker compose up -d
        ```

    * Go to http://node-1:8080 with your browser to view the cluster status.

    On node-2:

    * Repeat the above steps to setup a replication of the kafka-ui if you need to support high available service.

    * Go to http://node-2:8080 with your browser to view the cluster status.
