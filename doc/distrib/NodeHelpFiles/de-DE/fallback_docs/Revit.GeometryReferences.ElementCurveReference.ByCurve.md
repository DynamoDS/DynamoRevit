## Im Detail
`ElementCurveReference.ByCurve` creates a Revit element curve reference from a Dynamo curve. A curve reference is useful when another Revit node needs a selectable or referenceable curve, rather than just Dynamo geometry. This is commonly used for workflows that require Revit references, such as creating dimensions, alignments, constraints, or other elements that need to reference model geometry.

In the example below, a curve is selected and used as the input for `ElementCurveReference.ByCurve`. The output is an `ElementCurveReference` that is then used as a Revit curve reference for downstream nodes that require curve-based references.
___
## Beispieldatei

![ElementCurveReference.ByCurve](./Revit.GeometryReferences.ElementCurveReference.ByCurve_img.jpg)
