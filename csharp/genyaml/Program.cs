using System;
using System.Collections.Generic;
using System.Text.Json;
using k8s.Models;

namespace genyaml
{
    class Program
    {
        static void Main(string[] args)
        {
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
                            Name =  "test-container",
                            Image = "k8s.gcr.io/busybox"
                        }
                    }
                }
            };
            
            var serialiseOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            Console.WriteLine(JsonSerializer.Serialize<V1Pod>(pod, serialiseOptions));
        }
    }
}




