## Description approfondie
Ce nœud filtre une liste de nœuds de connexion en vérifiant si la valeur de force à un index spécifié se situe dans une plage définie. Les données de force proviennent des résultats de l'analyse structurelle ou du modèle analytique Revit et sont filtrées en fonction du type de résultat sélectionné (par exemple, Fx, Fy, Fz, Mx, My, Mz).

Dans cet exemple, un ensemble d'éléments de colonne est sélectionné et évalué en fonction de la composante de force Fz, à l’aide du résultat d'analyse et du cas de charge choisis. Seuls les éléments dont la valeur Fz se situe dans la plage de force spécifiée sont renvoyés en tant qu'assemblages acceptés.
___
## Exemple de fichier

![Filter.ByAnalysisResults](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAnalysisResults_img.jpg)
