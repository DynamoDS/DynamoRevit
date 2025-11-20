## Description approfondie
This node retrieves the inverse transformation matrix of a given Revit Link Element.  In Revit, linked models may be positioned with transformations (translation, rotation, scaling). This node allows you to get the inverse of that transformation,  effectively converting coordinates from the linked model’s space back into the host Revit model’s coordinate system.

In this example, all Revit linked elements at level L3 are selected and input into LinkElement.LinkInverseTransform.  The output is the host Revit model’s coordinate system.
___
## Exemple de fichier

![LinkElement.LinkInverseTransform](./Revit.Elements.LinkElement.LinkInverseTransform_img.jpg)
