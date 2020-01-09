# Templating Kubernetes resources with Python

In this section you will learn how to use Python to:

- Create a JSON resource definition of a Kubernetes Pod
- Submit the resource definition to the cluster to create the Pod

For both tasks you will use the official Kubernetes [Python client library](https://github.com/kubernetes-client/python).

Let's get started.

## Prerequisites

First, make sure you have Python and the pip package manager installed. You can find installation instructions in the [Python documentation](https://www.python.org/downloads/).

Then, install the Kubernetes Python client library:

```shell
pip install kubernetes
```

## Generating a Pod resource definition

The following is the code for generating and printing out a Pod resource definition in JSON:

```python
from kubernetes import client, config
import json

def main():
    pod = create_pod("dev")
    print(json.dumps(client.ApiClient().sanitize_for_serialization(pod)))

def create_pod(environment):
    return client.V1Pod(
        api_version = "v1",
        kind = "Pod",
        metadata = client.V1ObjectMeta(
            name = "test-pod",
        ),
        spec = client.V1PodSpec(
            containers = [
                client.V1Container(
                    name = "test-container",
                    image = "nginx",
                    env = [
                        client.V1EnvVar(
                            name = "ENV",
                            value = environment,
                        )
                    ]
                )
            ]
        )
    )

if __name__ == '__main__':
    main()
```

Save the above code in a file named `generate.py`.

> You can find the documentation of the Kubernetes Python client in the project's [GitHub repository](https://github.com/kubernetes-client/python).

You can then run your Python script with:

```shell
python generate.py
```

The output is the JSON resource definition of a Pod.

Note that this JSON has no newlines and indentation, which makes it hard to read. If you want, you can pretty-print the JSON with [`jq`](https://stedolan.github.io/jq/):

```shell
$ python generate.py | jq
{
  "apiVersion": "v1",
  "kind": "Pod",
  "metadata": {
    "name": "test-pod"
  },
  "spec": {
    "containers": [
      {
        "env": [
          {
            "name": "ENV",
            "value": "dev"
          }
        ],
        "image": "nginx",
        "name": "test-container"
      }
    ]
  }
}
```

You can save this JSON definition in a file:

```shell
python generate.py >pod.json
```

And then you can submit it to the cluster with kubectl as ususal:

```shell
kubectl apply -f pod.json
```

You can verify that the Pod has been created correctly with:

```shell
kubectl get pods
```

## Submitting a Pod resource definition to the cluster

Instead of saving the JSON resource definition to a file and then using kubectl to submit it to the cluster, you can submit the resource definition to the cluster directly in your code.

The code to generate a Pod resource definition _and_ submitting it to the cluster is as follows:

```go
from kubernetes import client, config

def main():
    config.load_kube_config()
    pod = create_pod("dev")
    client.CoreV1Api().create_namespaced_pod("default", pod)
    print("pod/%s created" % pod.metadata.name)

def create_pod(environment):
    # ...same as above

if __name__ == '__main__':
    main()
```

Go on and save the above code in a file named `create.py`.

You can then run the script with:

```shell
python create.py
```

The ouptut should be `pod/test-pod created`.

You can verify that the Pod has indeed been created with:

```shell
kubectl get pods
```

## What's next

As you can imagine, this was just a short demo. You can compose any type of Kubernetes object with the Python client library and you can create, read, update, and delete these objects in your Kubernetes cluster.

Feel free to check out the [example programs](https://github.com/kubernetes-client/python/tree/master/examples) of the Python client library.
