using System;
using System.Collections.Generic;
using System.Text.Json;
using k8s;
using k8s.Models;

namespace kubectl
{
    class Program
    {
        static void Main(string[] args)
        {
            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(k8SClientConfig);

            var pod = new V1Pod()
            {
                ApiVersion = "v1",
                Kind = "Pod",
                Metadata = new V1ObjectMeta()
                {
                    Name = "test-pod"
                },
                Spec = new V1PodSpec()
                {
                    Containers = new List<V1Container>()
                    {
                        new V1Container()
                        {
                            Name = "test-container",
                            Image = "nginx",
                            Env = new List<V1EnvVar>(){
                                new V1EnvVar(){
                                    Name="ENV", Value="dev"
                                }
                            }
                        }
                    }
                }
            };

            var result = client.CreateNamespacedPod(pod, "default");
            Console.WriteLine(result);
        }
    }
}




