# Templating Kubernetes resources with .NET Core

In this section you will learn how to use Javascript to:

- create JSON (and YAML) definition for a Kubernetes Pod
- submit a Pod definition to the cluster with code

Let's get started.

## Prerequisites

Ensure that you have installed .NET Core 3.1. See [documentation](https://dotnet.microsoft.com/download) for installation instructions.

## Generating Pod definitions

You can generate yaml by navigating to `genyaml` project and running it.

```shell
cd genyaml
dotnet build
dotnet run
```

The output is a JSON object for the Pod.

```json
{
  "apiVersion": "v1",
  "kind": "Pod",
  "metadata": {
    "name": "test-pod"
  },
  "spec": {
    "containers": [
      {
        "image": "k8s.gcr.io/busybox",
        "name": "test-container"
      }
    ]
  }
}
```

_But isn't Kubernetes accepting only YAML?_

YAML is a superset of JSON and any JSON file is also a valid YAML file.

You can create the Pod in the cluster with the following commands:

```shell
dotnet run > pod.yaml
kubectl apply -f pod.yaml
```

## Creating a custom Kubectl

Instead of exporting the JSON and feeding it to kubectl, you can send the payload to the cluster directly.

You can use the [official Kubernetes client library](https://github.com/kubernetes-client/csharp) to send the Pod definition to the cluster.

Assuming you are connected to a running cluster, you can create the pod by navigating to `kubectl` project and running it.

```shell
cd kubectl
dotnet build
dotnet run
```

And you can verify that the Pod was created with:

```shell
kubectl get pods
```

## What's next

As you can imagine, this is a short demo and you can build more complex objects and use the power of dotnet to compose large objects from smaller ones.