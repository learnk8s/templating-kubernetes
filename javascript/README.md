# Templating Kubernetes resources with Javascript

In this section you will learn how to use Javascript to:

- create JSON (and YAML) definition for a Kubernetes Pod
- submit a Pod definition to the cluster with code

Let's get started.

## Prerequisites

Make you you install all the dependencies with:

```shell
npm install
```

## Generating Pod definitions

The code is short:

```js
const { Pod, Container } = require('kubernetes-models/v1')

function createPod(environment = 'production') {
  return new Pod({
    metadata: {
      name: 'test-pod',
    },
    spec: {
      containers: [
        new Container({
          name: 'test-container',
          image: 'k8s.gcr.io/busybox',
          env: [{ name: 'ENV', value: environment }],
        }),
      ],
    },
  })
}

const pod = createPod('dev')

// Any valid JSON is also valid YAML
const json = JSON.stringify(pod, null, 2)

console.log(json)
```

You can execute the `gen-yaml.js` script with:

```shell
node gen-yaml.js
```

The output is a JSON object for the Pod.

```json
{
  "metadata": {
    "name": "test-pod"
  },
  "spec": {
    "containers": [
      {
        "name": "test-container",
        "image": "k8s.gcr.io/busybox",
        "env": [
          {
            "name": "ENV",
            "value": "dev"
          }
        ]
      }
    ]
  },
  "apiVersion": "v1",
  "kind": "Pod"
}
```

_But isn't Kubernetes accepting only YAML?_

YAML is a superset of JSON and any JSON file is also a valid YAML file.

You can create the Pod in the cluster with the following commands:

```shell
node gen-yaml.js > pod.yaml
kubectl apply -f pod.yaml
```

## Creating a custom Kubectl

Instead of exporting the JSON and feeding it to kubectl, you can send the payload to the cluster directly.

You can use the [official Kubernetes client library](https://github.com/kubernetes-client/javascript) to send the Pod definition to the cluster.

Here's the code:

```js
const { Pod, Container } = require('kubernetes-models/v1')
const k8s = require('@kubernetes/client-node')
const kc = new k8s.KubeConfig()

// Using the default credentials for kubectl
kc.loadFromDefault()
const k8sApi = kc.makeApiClient(k8s.CoreV1Api)

function createPod(environment = 'production') {
  /* return pod definition */
}

const pod = createPod('dev')

k8sApi.createNamespacedPod('default', pod).then(() => console.log('success'))
```

Assuming you are connected to a running cluster, you can execute the script with:

```shell
node kubectl.js
```

And you can verify that the Pod was created with:

```shell
kubectl get pods
```

## What's next

As you can imagine, this is a short demo and you can build more complex objects and use the power of Javascript to compose large objects from smaller ones.