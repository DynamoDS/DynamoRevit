stage ('Starting DynamoRevit build job') {
    def branchToBuild = env.BRANCH_NAME
    build job: '../DynamoRevitUtils/RC2.5.2_Revit2021', parameters: [[$class: 'StringParameterValue', name: 'BranchDynamoRevit', value: branchToBuild ]]
}
