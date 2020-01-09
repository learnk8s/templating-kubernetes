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
