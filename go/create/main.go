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
