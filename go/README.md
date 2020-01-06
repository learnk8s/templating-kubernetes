# Templating Kubernetes resources with Go

In this section you will learn how to use Go to:

- Create a JSON resource definition of a Kubernetes Pod
- Submit the resource definition to the cluster to create the Pod

For both tasks you will use the official Kubernetes [Go client library](https://github.com/kubernetes/client-go) (client-go).

Let's get started.

## Prerequisites

Make sure you have the `go` command installed. You can find installation instructions in the [Go documentation](https://golang.org/dl/).

## Generating a resource definition

First, create a new directory for the program that you are going to write:

```bash
mkdir generate
cd generate
```

The code for generating a Pod resource definition in JSON looks as follows:

```go
package main

import (
	"fmt"
	corev1 "k8s.io/api/core/v1"
	metav1 "k8s.io/apimachinery/pkg/apis/meta/v1"
	"k8s.io/apimachinery/pkg/util/json"
)

func main() {
	pod := createPod("dev")
	bytes, err := json.Marshal(pod)
	if err != nil {
		panic(err)
	}
	fmt.Println(string(bytes))
}

func createPod(environment string) *corev1.Pod {
	return &corev1.Pod{
		TypeMeta: metav1.TypeMeta{
			Kind:       "Pod",
			APIVersion: "v1",
		},
		ObjectMeta: metav1.ObjectMeta{
			Name: "test-pod",
		},
		Spec: corev1.PodSpec{
			Containers: []corev1.Container{
				{
					Name:  "test-container",
					Image: "nginx",
					Env: []corev1.EnvVar{
						{
							Name:  "ENV",
							Value: environment,
						},
					},
				},
			},
		},
	}
}
```

Go on and save the above code in a file named `main.go`.

> The above code uses several packages from the client-go library. You can find the documentation of every Go package by pasting its full import path into the search field on [godoc.org](https://godoc.org/). For example, [here](https://godoc.org/k8s.io/api/core/v1) is the documentation of the `k8s.io/api/core/v1` package.

You can then execute your program with:

```shell
go run main.go
```

The output is the JSON resource definition of a Pod.

Note that this JSON has no newlines and indentation, which makes it hard to read. If you want, you can pretty-print the JSON with `jq`:

```shell
$ go run main.go | jq
{
  "kind": "Pod",
  "apiVersion": "v1",
  "metadata": {
    "name": "test-pod",
    "creationTimestamp": null
  },
  "spec": {
    "containers": [
      {
        "name": "test-container",
        "image": "nginx",
        "env": [
          {
            "name": "ENV",
            "value": "dev"
          }
        ],
        "resources": {}
      }
    ]
  },
  "status": {}
}
```

You can save this JSON definition in a file:

```shell
go run main.go >pod.json
```

And then you can submit it to the cluster with kubectl as ususal:

```shell
kubectl apply -f pod.json
```

You can verify that the Pod has been created correctly with:

```shell
kubectl get pods
```

## Creating an object

Instead of saving the JSON resource definition to a file and then using kubectl to submit it to the cluster, you can submit the resource definition to the cluster directly in your code.

First of all, create a new directory for the new program that you are going to write:

```shell
cd ..
mkdir create
cd create
```

The code to generate a Pod resource definition _and_ submitting it to the cluster is as follows:

```go
package main

import (
	"fmt"
	corev1 "k8s.io/api/core/v1"
	metav1 "k8s.io/apimachinery/pkg/apis/meta/v1"
	"k8s.io/client-go/kubernetes"
	"k8s.io/client-go/tools/clientcmd"
	"k8s.io/client-go/util/homedir"
	"path/filepath"
)

func main() {
	kubeconfig, err := clientcmd.BuildConfigFromFlags("", filepath.Join(homedir.HomeDir(), ".kube", "config"))
	if err != nil {
		panic(err)
	}
	clientset, err := kubernetes.NewForConfig(kubeconfig)
	if err != nil {
		panic(err)
	}
	pod := createPod("dev")
	_, err = clientset.CoreV1().Pods("default").Create(pod)
	if err != nil {
		panic(err)
	}
	fmt.Printf("pod/%s created\n", pod.Name)
}

func createPod(environment string) *corev1.Pod {
  // ...same as in the previous program
}
```

Go on and save the above code in a file named `main.go`.

You can then run the program with:

```shell
go run main.go
```

The ouptut should be `pod/test-pod created`.

You can verify that the Pod has indeed been created with:

```shell
kubectl get pods
```

## What's next

As you can imagine, this was just a short demo. You can compose any type of Kubernetes object with the Go client library and you can create, read, updated, and delete these objects in your Kubernetes cluster.

Feel free to check out the [example programs](https://github.com/kubernetes/client-go/tree/master/examples) of the Go client library.
