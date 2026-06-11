## Im Detail
`ElementFaceReference.BySurface` creates a Revit face reference from a selected or generated surface. It converts a Dynamo surface that is associated with Revit geometry into an `ElementFaceReference`. This reference can then be used by other nodes that need a Revit face reference instead of just surface geometry.

In the example below, a wall is created and a surface from that wall is used as the input to `ElementFaceReference.BySurface`. The output is an `ElementFaceReference` that can be used by nodes that require a reference to a Revit element face.
___
## Beispieldatei

![ElementFaceReference.BySurface](./Revit.GeometryReferences.ElementFaceReference.BySurface_img.jpg)
