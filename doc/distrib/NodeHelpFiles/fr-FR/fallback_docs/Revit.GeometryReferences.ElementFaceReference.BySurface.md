## Description approfondie
ElementFaceReference.BySurface crée une référence de face Revit à partir d'une surface sélectionnée ou générée. Il convertit une surface Dynamo associée à une géométrie Revit en ElementFaceReference. Cette référence peut ensuite être utilisée par d'autres nœuds qui ont besoin d'une référence de face Revit plutôt que d'une simple géométrie de surface.

Dans l'exemple ci-dessous, un mur est créé, et une surface issue de ce mur est utilisée comme entrée pour ElementFaceReference.BySurface. La sortie est une ElementFaceReference, qui peut être utilisée par les nœuds nécessitant une référence à une face d'élément Revit.
___
## Exemple de fichier

![ElementFaceReference.BySurface](./Revit.GeometryReferences.ElementFaceReference.BySurface_img.jpg)
