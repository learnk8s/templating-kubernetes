package io.learnk8s;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.SerializationFeature;
import io.fabric8.kubernetes.api.model.*;
import io.fabric8.kubernetes.client.*;
import io.fabric8.kubernetes.client.Config;
import io.fabric8.kubernetes.client.ConfigBuilder;
import io.fabric8.kubernetes.client.internal.SerializationUtils;


public class KubernetesClientCLI {


    private static final ObjectMapper mapper = new ObjectMapper().enable(SerializationFeature.INDENT_OUTPUT);
    private static String namespace = null;

    public static void main(String[] args) throws Exception {
        String environment = "production";
        Pod pod = createPod(environment);

        if (args.length > 0 && args[0].equals("yaml")) {
            System.out.println(SerializationUtils.dumpAsYaml(pod));
        } else {
            System.out.println(mapper.writeValueAsString(pod));
        }

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
    }

    public static Pod createPod(String environment){
        return new PodBuilder().withNewMetadata()
                .withName("test-pod")
                .endMetadata()
                .withNewSpec()
                .addNewContainer()
                .withName("test-container")
                .withImage("nginx")
                .withEnv(new EnvVarBuilder().withName("ENV").withValue(environment).build())
                .endContainer()
                .endSpec()
                .build();
    }

}
