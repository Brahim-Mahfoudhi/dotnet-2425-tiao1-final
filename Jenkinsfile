pipeline {
    agent { label 'App' }
    options {
        buildDiscarder(logRotator(numToKeepStr: '10'))
    }

    environment {
        JENKINS_SERVER = 'http://172.16.128.100:8080'
        DOTNET_PROJECT_PATH = 'Rise.Server/Rise.Server.csproj'
        DOTNET_TEST_PATH = 'Rise.Domain.Tests/Rise.Domain.Tests.csproj'
        PUBLISH_OUTPUT = 'publish'
        DISCORD_WEBHOOK_URL = "https://discord.com/api/webhooks/1301160382307766292/kROxjtgZ-XVOibckTMri2fy5-nNOEjzjPLbT9jEpr_R0UH9JG0ZXb2XzUsYGE0d3yk6I"
        JENKINS_CREDENTIALS_ID = "jenkins-master-key"
        SSH_KEY_FILE = '/var/lib/jenkins/.ssh/id_rsa'
        REMOTE_HOST = 'jenkins@172.16.128.101'
        TRX_FILE_PATH = 'Rise.Domain.Tests/TestResults/test-results.trx'
        TEST_RESULT_PATH = 'Rise.Domain.Tests/TestResults'
        TRX_TO_XML_PATH = 'Rise.Domain.Tests/TestResults/test-results.xml'
        PUBLISH_DIR_PATH = '/var/lib/jenkins/artifacts/'
    }

    stages {
        stage('Clean Workspace') {
            steps {
                cleanWs()
            }
        }

        stage('Checkout Code') {
            steps {
                script {
                    git credentialsId: 'jenkins-master-key', url: 'git@github.com:HOGENT-RISE/dotnet-2425-tiao1.git', branch:'ACC'
                    echo 'Gather GitHub info!'
                    def gitInfo = sh(script: 'git show -s HEAD --pretty=format:"%an%n%ae%n%s%n%H%n%h" 2>/dev/null', returnStdout: true).trim().split("\n")
                    env.GIT_AUTHOR_NAME = gitInfo[0]
                    env.GIT_AUTHOR_EMAIL = gitInfo[1]
                    env.GIT_COMMIT_MESSAGE = gitInfo[2]
                    env.GIT_COMMIT = gitInfo[3]
                    env.GIT_BRANCH = gitInfo[4]
                }
            }
        }

 
       /*
        stage('Linting and Code Analysis') {
            steps {
                //TODO
            }
        }
        */

        stage('Restore Dependencies') {
            steps {
                echo "Restoring dependencies..."
                sh "dotnet restore ${DOTNET_PROJECT_PATH}"
                script {
                    def testPaths = [
                        Domain: 'Rise.Domain.Tests/Rise.Domain.Tests.csproj',
                        Client: 'Rise.Client.Tests/Rise.Client.Tests.csproj',
                        // Server: 'Rise.Server.Tests/Rise.Server.Tests.csproj',
                        Service: 'Rise.Services.Tests/Rise.Services.Tests.csproj'
                    ]
                    
                    testPaths.each { name, path ->
                        echo "Restoring unit tests for ${name} located at ${path}..."
                        sh "dotnet restore ${path}"
                    }
                }
            }
        }

        stage('Build Application') {
            steps {
                sh "dotnet build ${DOTNET_PROJECT_PATH}"
            }
        }
        
        stage('Run Unit Tests') {
            steps {
                script {
                    def testPaths = [
                        Domain: 'Rise.Domain.Tests/Rise.Domain.Tests.csproj',
                        Client: 'Rise.Client.Tests/Rise.Client.Tests.csproj',
                        //Server: 'Rise.Server.Tests/Rise.Server.Tests.csproj',
                        Service: 'Rise.Services.Tests/Rise.Services.Tests.csproj'
                    ]
        
                    testPaths.each { name, path ->
                        echo "Running unit tests for ${name} located at ${path}..."
        
                        sh """
                            dotnet test ${path} --collect:"XPlat Code Coverage" --logger 'trx;LogFileName=${name}.trx' \
                            /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
                        """
                    }
                }
            }
        }
        
        stage('Coverage Report') {
            steps {
                script {
                    sh "mkdir -p /var/lib/jenkins/agent/workspace/Dotnet-test-Pipeline/coverage/"
        
                    def coverageFiles = sh(script: """
                        find Rise.*/TestResults -type f -name 'coverage.cobertura.xml'
                    """, returnStdout: true).trim().split("\n")
        
                    if (coverageFiles.size() > 0) {
                        echo "Found coverage files: ${coverageFiles.join(', ')}"
        
                        coverageFiles.each { file ->
                            sh "cp ${file} /var/lib/jenkins/agent/workspace/Dotnet-test-Pipeline/coverage/"
                        }
        
                        sh """
                            /home/jenkins/.dotnet/tools/reportgenerator \
                            -reports:/var/lib/jenkins/agent/workspace/Dotnet-test-Pipeline/coverage/*.cobertura.xml \
                            -targetdir:/var/lib/jenkins/agent/workspace/Dotnet-test-Pipeline/coverage-report/ \
                            -reporttype:Html
                        """
                    } else {
                        error 'No coverage files found'
                    }
                }
        
                echo 'Publishing coverage report...'
                publishHTML([
                    allowMissing: false,
                    alwaysLinkToLastBuild: true,
                    keepAll: true,
                    reportDir: '/var/lib/jenkins/agent/workspace/Dotnet-test-Pipeline/coverage-report',
                    reportFiles: 'index.html',
                    reportName: 'Coverage Report'
                ])
            }
        }
    
        stage('Publish Application') {
            steps {
                sh "dotnet publish ${DOTNET_PROJECT_PATH} -c Release -o ${PUBLISH_OUTPUT}"
            }
        }
    
        stage('Deploy to Remote Server') {
            steps {
                withCredentials([
                    string(credentialsId: 'Authority', variable: 'AUTHORITY'),
                    string(credentialsId: 'Audience', variable: 'AUDIENCE'),
                    string(credentialsId: 'M2MClientId', variable: 'M2MCLIENTID'),
                    string(credentialsId: 'M2MClientSecret', variable: 'M2MCLIENTSECRET'),
                    string(credentialsId: 'BlazorClientId', variable: 'BLAZORCLIENTID'),
                    string(credentialsId: 'BlazorClientSecret', variable: 'BLAZORCLIENTSECRET'),
                    string(credentialsId: 'SQLConnectionString', variable: 'SQL_CONNECTION_STRING')
                ]) {
                    sshagent([JENKINS_CREDENTIALS_ID]) {
                        script {
                            def remoteScript = "/tmp/deploy_script.sh"
                            def publishDir = "/var/lib/jenkins/artifacts"  // Correct path for appsettings.json

                            // Ensure proper quoting in the shell script
                            withEnv([
                                "AUTHORITY=${AUTHORITY}",
                                "AUDIENCE=${AUDIENCE}",
                                "M2MCLIENTID=${M2MCLIENTID}",
                                "M2MCLIENTSECRET=${M2MCLIENTSECRET}",
                                "BLAZORCLIENTID=${BLAZORCLIENTID}",
                                "BLAZORCLIENTSECRET=${BLAZORCLIENTSECRET}",
                                "SQL_CONNECTION_STRING=${SQL_CONNECTION_STRING}"
                            ]) {
                                // Create the shell script content carefully
                                sh """
                                    echo '#!/bin/bash
                                    echo "Starting deployment..."
                                    export AUTHORITY="\${AUTHORITY}"
                                    export AUDIENCE="\${AUDIENCE}"
                                    export M2MCLIENTID="\${M2MCLIENTID}"
                                    export M2MCLIENTSECRET="\${M2MCLIENTSECRET}"
                                    export BLAZORCLIENTID="\${BLAZORCLIENTID}"
                                    export BLAZORCLIENTSECRET="\${BLAZORCLIENTSECRET}"
                                    export SQL_CONNECTION_STRING="\${SQL_CONNECTION_STRING}"

                                    # Print variables for debugging
                                    echo "AUTHORITY=\${AUTHORITY}"
                                    echo "AUDIENCE=\${AUDIENCE}"
                                    echo "M2MCLIENTID=\${M2MCLIENTID}"
                                    echo "M2MCLIENTSECRET=\${M2MCLIENTSECRET}"
                                    echo "BLAZORCLIENTID=\${BLAZORCLIENTID}"
                                    echo "BLAZORCLIENTSECRET=\${BLAZORCLIENTSECRET}"
                                    echo "SQL_CONNECTION_STRING=\${SQL_CONNECTION_STRING}"

                                    # Check if appsettings.json exists at the correct path
                                    if [ ! -f "\${publishDir}/appsettings.json" ]; then
                                        echo "Error: appsettings.json not found at \${publishDir}/appsettings.json"
                                        exit 1
                                    fi

                                    # Update appsettings.json with SQL connection string
                                    jq --arg sql_connection_string "\${SQL_CONNECTION_STRING}" \\
                                    '.ConnectionStrings = { "SqlServer": "Server=\${sql_connection_string};TrustServerCertificate=True;" }' \\
                                    "\${publishDir}/appsettings.json" > tmp.json && mv tmp.json "\${publishDir}/appsettings.json"

                                    # Update appsettings.json with Auth0 details
                                    jq --arg authority "\${AUTHORITY}" \\
                                    --arg audience "\${AUDIENCE}" \\
                                    --arg m2m_client_id "\${M2MCLIENTID}" \\
                                    --arg m2m_client_secret "\${M2MCLIENTSECRET}" \\
                                    --arg blazor_client_id "\${BLAZORCLIENTID}" \\
                                    --arg blazor_client_secret "\${BLAZORCLIENTSECRET}" \\
                                    '.Auth0 = { "Authority": "\${authority}", "Audience": "\${audience}", "M2MClientId": "\${m2m_client_id}", "M2MClientSecret": "\${m2m_client_secret}", "BlazorClientId": "\${blazor_client_id}", "BlazorClientSecret": "\${blazor_client_secret}" }' \\
                                    "\${publishDir}/appsettings.json" > tmp.json && mv tmp.json "\${publishDir}/appsettings.json"

                                    ' > ${remoteScript}
                                """

                                // Copy files and the script to the remote server
                                sh """
                                    scp -i ${SSH_KEY_FILE} -o StrictHostKeyChecking=no -r ${PUBLISH_OUTPUT}/* ${REMOTE_HOST}:${publishDir}
                                """
                                sh """
                                    scp -i ${SSH_KEY_FILE} -o StrictHostKeyChecking=no ${remoteScript} ${REMOTE_HOST}:${remoteScript}
                                """

                                // Execute the remote script and clean up
                                sh """
                                    ssh -i ${SSH_KEY_FILE} -o StrictHostKeyChecking=no ${REMOTE_HOST} "bash ${remoteScript} && rm ${remoteScript}"
                                """
                                sh """
                                    ssh -i ${SSH_KEY_FILE} -o StrictHostKeyChecking=no ${REMOTE_HOST} "screen -dmS rise_server dotnet /var/lib/jenkins/artifacts/Rise.Server.dll --urls 'http://0.0.0.0:5000;https://0.0.0.0:5001'"
                                """
                            }
                        }
                    }
                }
            }
        }

    }

    post {
        success {
            echo 'Build and deployment completed successfully!'
            archiveArtifacts artifacts: '**/*.dll', fingerprint: true
            //archiveArtifacts artifacts: "${TRX_FILE_PATH}", fingerprint: true
            script {
                sendDiscordNotification("Build Success")
            }
        }
        failure {
            echo 'Build or deployment has failed.'
            script {
                sendDiscordNotification("Build Failed")
            }
        }
        always {
            echo 'Build process has completed.'
            echo 'Generate Test report...'
    
            script {
                def testPaths = [
                    Domain: 'Rise.Domain.Tests/TestResults/Domain.trx',
                    Client: 'Rise.Client.Tests/TestResults/Client.trx',
                    //Server: 'Rise.Server.Tests/TestResults/Server.trx',
                    Service: 'Rise.Services.Tests/TestResults/Service.trx'
                ]
                
                testPaths.each { name, path ->
                    def outputXml = "${TEST_RESULT_PATH}"
                    sh "/home/jenkins/.dotnet/tools/trx2junit --output ${outputXml} ${path}"                    
                    junit "${TEST_RESULT_PATH}/${name}.xml"
                }
            }
        }
    }
}

def sendDiscordNotification(status) {
    script {
        discordSend(
            title: "${env.JOB_NAME} - ${status}",
            description: """
                Build #${env.BUILD_NUMBER} ${status == "Build Success" ? 'completed successfully!' : 'has failed!'}
                **Commit**: ${env.GIT_COMMIT}
                **Author**: ${env.GIT_AUTHOR_NAME} <${env.GIT_AUTHOR_EMAIL}>
                **Branch**: ${env.GIT_BRANCH}
                **Message**: ${env.GIT_COMMIT_MESSAGE}
                
                [**Build output**](${JENKINS_SERVER}/job/${env.JOB_NAME}/${env.BUILD_NUMBER}/console)
                [**Test result**](${JENKINS_SERVER}/job/${env.JOB_NAME}/lastBuild/testReport/)
                [**Coverage report**](${JENKINS_SERVER}/job/${env.JOB_NAME}/lastBuild/Coverage_20Report/)
                [**History**](${JENKINS_SERVER}/job/${env.JOB_NAME}/${env.BUILD_NUMBER}/testReport/history/)
            """,
            footer: "Build Duration: ${currentBuild.durationString.replace(' and counting', '')}",
            webhookURL: DISCORD_WEBHOOK_URL,
            result: status == "Build Success" ? 'SUCCESS' : 'FAILURE'
        )
    }
}
