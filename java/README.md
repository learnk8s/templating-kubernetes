# Templating Kubernetes resources with Java

In this section you will learn how to use Java and Maven to:

- create JSON (and YAML) definition for a Kubernetes Pod
- submit a Pod definition to the cluster with code

Let's get started.

## Prerequisites

To your Maven application you need to add the `io.fabric8:kubernetes-client` dependency inside the `pom.xml` file

```xml
<dependency>
  <groupId>io.fabric8</groupId>
  <artifactId>kubernetes-client</artifactId>
  <version>4.6.4</version>
</dependency>
```

## Generating Pod definitions

The code is short:

```java
    public static void main(String[] args) throws Exception {
      String environment = "production";
      Pod pod = createPod(namespace);

      if (args[0].equals("yaml")) {
         System.out.println(SerializationUtils.dumpAsYaml(pod));
      } else {
         System.out.println(mapper.writeValueAsString(pod));
      }
      
    }
    
    public static Pod createPod( String environment ) {
        return new PodBuilder().withNewMetadata()
                .withName("test-pod")
                .endMetadata()
                .withNewSpec()
                .addNewContainer()
                .withName("test-container")
                .withImage("k8s.gcr.io/busybox")
                .withEnv(new EnvVarBuilder().withName("ENV").withValue(environment).build())
                .endContainer()
                .endSpec()
                .build();
    }
```

You can compile and package the Maven application with:
```shell
mvn clean package
```

You can execute the the application with:

```shell
java -jar target/k8s-client-1.0.jar --dry-run 
```

The output is a JSON object for the Pod.

```json
{
  "apiVersion" : "v1",
  "kind" : "Pod",
  "metadata" : {
    "annotations" : { },
    "labels" : { },
    "name" : "test-pod"
  },
  "spec" : {
    "containers" : [ {
      "env" : [ {
        "name" : "ENV",
        "value" : "production"
      } ],
      "image" : "k8s.gcr.io/busybox",
      "name" : "test-container"
    } ],
    "nodeSelector" : { }
  }
}
```

_But isn't Kubernetes accepting only YAML?_

YAML is a superset of JSON and any JSON file is also a valid YAML file.

You can create the Pod in the cluster with the following commands:

```shell
java -jar target/k8s-client-1.0.jar yaml --dry-run 
kubectl apply -f pod.yaml
```

## Creating a custom Kubectl

Instead of exporting the JSON and feeding it to kubectl, you can send the payload to the cluster directly.

You can use the [Fabric8 Kubernetes API](https://github.com/fabric8io/kubernetes-client) to send the Pod definition to the cluster.

Here's the code:

```java
        Config config = new ConfigBuilder().build();

        try (final KubernetesClient client = new DefaultKubernetesClient(config)) {
            if (namespace == null) {
                namespace = client.getNamespace();
            }

            boolean dryRun = false;
            for (String arg : args) {
                if (arg.equals("--dry-run")) {
                    dryRun = true;
                }
            }
            if (!dryRun) {
                client.pods().inNamespace(namespace).create(pod);
                System.out.println("Pod created!");
            }


        }

```

Assuming you are connected to a running cluster, you can execute the script with:

```shell
java -jar target/k8s-client-1.0.jar yaml
```

And you can verify that the Pod was created with:

```shell
kubectl get pods
```

## What's next

As you can imagine, this is a short demo and you can build more complex objects and use the power of Java and the Fabric8 Kubernetes API to compose large objects from smaller ones.