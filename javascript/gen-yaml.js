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