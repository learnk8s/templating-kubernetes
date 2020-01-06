const { Pod, Container } = require('kubernetes-models/v1')
const k8s = require('@kubernetes/client-node')
const kc = new k8s.KubeConfig()

// Using the default credentials for kubectl
kc.loadFromDefault()
const k8sApi = kc.makeApiClient(k8s.CoreV1Api)

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

k8sApi.createNamespacedPod('default', pod).then(() => console.log('success'))