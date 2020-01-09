from kubernetes import client, config

def main():
    config.load_kube_config()
    pod = create_pod("dev")
    client.CoreV1Api().create_namespaced_pod("default", pod)
    print("pod/%s created" % pod.metadata.name)

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
