## In Depth
This node creates a Revit face reference from a selected or generated surface.  It converts a Dynamo Surface that is associated with Revit geometry into a Revit.GeometryReferences.ElementFaceReference. This reference can then be used by other nodes that need a Revit face reference instead of just surface geometry.

In this example, a wall is created and a surface from that wall is then used as the input to the ElementFaceReference.BySurface node. The output is a Revit.GeometryReferences.ElementFaceReference, which can be used by nodes that require a reference to a Revit element face.
___
## Example File

![ElementFaceReference.BySurface](./Revit.GeometryReferences.ElementFaceReference.BySurface_img.jpg)