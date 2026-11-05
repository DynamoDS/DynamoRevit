## Description approfondie
SpecType.ByTypeId convertit une chaîne Revit Spec TypeId en objet SpecType. Les types SpecType définissent le type de données qu'un paramètre représente dans Revit, par exemple une longueur, une surface, un angle, un matériau, une force, du texte, un entier, et bien d'autres. Ce nœud est couramment utilisé lors de la création de paramètres partagés, de paramètres de projet ou de définitions de paramètres dans les versions récentes de Revit, où Autodesk a remplacé l'ancien système ParameterType par des Forge TypeIds.

Dans l'exemple ci-dessous, une chaîne représentant un Spec TypeId Revit est fournie à SpecType.ByTypeId. La sortie est un objet SpecType qui peut être utilisé en aval lors de la création d'une nouvelle définition de paramètre afin que Revit comprenne le type de données que le paramètre doit stocker.
___
## Exemple de fichier

![SpecType.ByTypeId](./Revit.Elements.SpecType.ByTypeId_img.jpg)
