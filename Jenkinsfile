pipeline {
    agent any

    environment {
        // Azure Settings
        ACR = 'ccmsacr'
        RG = 'ccms-rg'
        AKS = 'ccms-aks'
        
        // Image Settings
        IMAGE = 'ccms-backend'
        IMAGE_NAME = "${ACR}.azurecr.io/${IMAGE}"
        TAG = "${env.BUILD_NUMBER}"
        
        // Jenkins Credentials IDs
        ACR_CREDENTIAL_ID = 'acr-credentials'
        AKS_CREDENTIAL_ID = 'aks-credentials'
        
        // Ingress Settings
        INGRESS_HOST = 'ccms.local'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build & Push Image') {
            steps {
                script {
                    docker.withRegistry("https://${ACR}.azurecr.io", ACR_CREDENTIAL_ID) {
                        def app = docker.build("${IMAGE_NAME}:${TAG}", "-f Dockerfile .")
                        app.push()
                    }
                }
            }
        }

        stage('Deploy to AKS') {
            environment {
                DB_CONNECTION = credentials('db-connection-string')
                JWT_KEY = credentials('jwt-secret-key')
            }
            steps {
                withKubeConfig([credentialsId: AKS_CREDENTIAL_ID]) {
                    sh """
                    # 1. Update image tags
                    sed -i "s|ccmsacr.azurecr.io/ccms-backend:latest|${IMAGE_NAME}:${TAG}|g" k8s/02-api.yaml
                    
                    # 2. Update Ingress Host
                    sed -i "s|#{ingressHost}#|${INGRESS_HOST}|g" k8s/02-api.yaml
                    
                    # 3. Update Secrets
                    sed -i "s|#{ConnectionStrings__DefaultConnection}#|\${DB_CONNECTION}|g" k8s/02-api.yaml
                    sed -i "s|#{Jwt__Key}#|\${JWT_KEY}|g" k8s/02-api.yaml
                    
                    # 4. Apply manifests to AKS
                    kubectl apply -f k8s/01-mysql.yaml
                    kubectl apply -f k8s/02-api.yaml
                    """
                }
            }
        }
    }
    
    post {
        always {
            cleanWs()
        }
    }
}
