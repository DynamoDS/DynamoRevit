stage ('Starting DynamoRevit build job') {
    def branchToBuild = env.BRANCH_NAME
    build job: '../DynamoRevitUtils/RC2.12.0_Revit2022.1', parameters: [[$class: 'StringParameterValue', name: 'BranchDynamoRevit', value: branchToBuild ]]
}
