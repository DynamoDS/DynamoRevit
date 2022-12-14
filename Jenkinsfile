stage ('Starting DynamoRevit build job') {
    def branchToBuild = env.BRANCH_NAME
    build job: '../DynamoRevitUtils/RC2.17.0_Revit2024', parameters: [[$class: 'StringParameterValue', name: 'BranchDynamoRevit', value: branchToBuild ]]
}
