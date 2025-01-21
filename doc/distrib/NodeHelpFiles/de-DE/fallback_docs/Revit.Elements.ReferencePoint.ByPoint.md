## Im Detail
`ReferencePoint.ByPoint` erstellt ein Referenzpunktelement im aktiven Revit-Familiendokument an der angegebenen Punktposition. Anmerkung: Das Familiendokument muss eine adaptive Komponente oder eine Körperfamilie sein. Dieser Block unterscheidet sich von "ReferencePoint.ByCoordinates" dadurch, dass er einen Dynamo-Punkt für die Position verwendet. Dies ist nützlich, da der Endbenutzer den Dynamo-Punkt mithilfe der direkten Manipulation bearbeiten kann, wodurch der Referenzpunkt ebenfalls aktualisiert wird. Weitere Informationen zur direkten Manipulation finden Sie unter diesem [Link](https://primer2.dynamobim.org/10_sample_workflow/10-1_getting-started-workflows/2-attractor-points#adjusting-with-direct-manipulation).

Im folgenden Beispiel wird ein neuer Referenzpunkt an den Koordinaten 0,0,1 erstellt.
___
## Beispieldatei

![ReferencePoint.ByPoint](./Revit.Elements.ReferencePoint.ByPoint_img.jpg)
