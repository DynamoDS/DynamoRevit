stage ('Starting DynamoRevit build job') {
    def branchToBuild = env.BRANCH_NAME
    build job: '../DynamoRevitUtils/RC2.6.2_Revit2021.1.8', parameters: [[$class: 'StringParameterValue', name: 'BranchDynamoRevit', value: branchToBuild ]]
}
