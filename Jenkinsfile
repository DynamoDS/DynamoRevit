node('D4R') {
  currentBuild.result = 'SUCCESS'

  def batFolder = "C:\\DeployTool"

  def branch = "master"

  try {

    stage('Checkout') {
      echo "Checkout ..."
      try {
        checkout([$class: 'GitSCM', branches: [[name: '*/RC1.3.3_master']], doGenerateSubmoduleConfigurations: false, extensions: [[$class: 'RelativeTargetDirectory', relativeTargetDir: '../Dynamo'], [$class: 'CleanBeforeCheckout'], [$class: 'GitLFSPull']], gitTool: 'default', submoduleCfg: [], userRemoteConfigs: [[credentialsId: 'aecbuild', url: 'https://github.com/DynamoDS/Dynamo.git']]])
        checkout([$class: 'GitSCM', branches: [[name: '*/master']], doGenerateSubmoduleConfigurations: false, extensions: [[$class: 'RelativeTargetDirectory', relativeTargetDir: '../DynamoRevit'] , [$class: 'CleanBeforeCheckout'], [$class: 'GitLFSPull']], gitTool: 'default', submoduleCfg: [], userRemoteConfigs: [[credentialsId: 'aecbuild', url: 'https://git.autodesk.com/Dynamo/DynamoRevit.git']]])
      } catch(err) {
        echo "Git Checkout Failed: $err"
        currentBuild.result = 'FAILURE'
        throw err
      }
    }    

    stage('Revit API Download') {
      echo "RevitAPI and RevitAPIUI download and extract ..."
      try {
        bat batFolder + '\\' + 'RevitAPInuGet.bat'
      } catch(err) {
        echo "RevitAPI or RevitAPIUI Download or Extract Error: $err"
        currentBuild.result = 'FAILURE'
        throw err
      }
    }    

    stage('Build') {
      echo "Build..."
      try {
        bat batFolder + '\\' + 'BuildSolutions.bat'
      } catch(err) {
        echo "Dynamo or DynamoRevit Build Failed: $err"
        currentBuild.result = 'FAILURE'
        throw err
      }
    }

    if (branch == 'master') {
      stage('Deployment to Artifactory') {
        try {
          dir('../DynamoRevit/') {
           bat batFolder + '\\' + 'Deploy.bat'
          }
        } catch(err) {
          echo "Deployment to Artifactory Failed: $err"
          currentBuild.result = 'FAILURE'
          throw err
        }
      }
    }

  } finally {
  }
}
