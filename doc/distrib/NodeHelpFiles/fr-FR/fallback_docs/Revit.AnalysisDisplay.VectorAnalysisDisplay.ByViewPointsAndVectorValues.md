## Description approfondie
VectorAnalysisDisplay.ByViewPointsAndVectorValues crée un affichage d'analyse vectorielle dans une vue Revit. La vue d'entrée définit l'emplacement où l'affichage d'analyse apparaît. Les points définissent l'emplacement des marqueurs d'analyse, les vecteurs définissent la direction de l'analyse affichée, et les valeurs définissent le résultat numérique associé à chaque point. La sortie est un élément d'affichage d'analyse Revit affiché dans la vue sélectionnée.

Dans l'exemple ci-dessous, deux points sont créés pour définir des emplacements d'échantillon dans une vue 3D Revit. Deux vecteurs sont ensuite créés pour définir des données d'analyse directionnelle à ces emplacements. Les points et les vecteurs sont combinés dans des listes et utilisés comme entrées pour VectorAnalysisDisplay.ByViewPointsAndVectorValues, avec une vue, un nom d'analyse et une description. La sortie est un affichage d'analyse vectorielle affiché dans la vue Revit sélectionnée.
___
## Exemple de fichier

![VectorAnalysisDisplay.ByViewPointsAndVectorValues](./Revit.AnalysisDisplay.VectorAnalysisDisplay.ByViewPointsAndVectorValues_img.jpg)
