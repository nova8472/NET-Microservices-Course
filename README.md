# .NET Microservices Course

The source code of the .NET microservices course https://www.youtube.com/watch?v=DgVjEo3OGBI written in .NET 7. 

## Core concepts explained and used: 
 - microservices
 - Docker
 - Kubernetes
 - Asynchronous messaging with RabbitMQ
 - Synchronous messaging with HTTP
 - Synchronous messaging with gRPC


### Docker commands: 
- docker build -t \<account\>/\<image\> .
- docker run -p \<outside port\>:\<application port\> -d \<account\>/\<image\>
- docker ps
- docker stop \<container ID\>
- docker start \<container ID\>
- docker push \<account\>/\<image\>
  
### Kubernetes commands:
- kubectl version
- kubectl apply -f myFile.yaml
- kubectl get deployments
- kubectl get pods
- kubectl delete deployment \<deployment name\>
- kubectl get services
- kubectl rollout restart deployment \<deployment name\>
- kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.7.0/deploy/static/provider/cloud/deploy.yaml
- kubectl get namespaces
- kubectl get pods --namespace=ingress-nginx
- kubectl get storageclass
- kubectl create secret generic mssql --from-literal=\<KEY\>="\<VALUE\>"
