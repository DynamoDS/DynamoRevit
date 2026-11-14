## Informacje szczegółowe
Węzeł ElementFaceReference.BySurface tworzy odniesienie powierzchni programu Revit na podstawie wybranej lub wygenerowanej powierzchni. Przekształca powierzchnię dodatku Dynamo skojarzoną z geometrią programu Revit w obiekt ElementFaceReference. To odniesienie może być następnie używane przez inne węzły, które wymagają odniesienia powierzchni programu Revit zamiast samej geometrii powierzchni.

W poniższym przykładzie zostaje utworzona ściana, a powierzchnia z tej ściany jest używana jako dane wejściowe węzła ElementFaceReference.BySurface. Dane wyjściowe to obiekt ElementFaceReference, który może być używany przez węzły wymagające odniesienia do powierzchni elementu programu Revit.
___
## Plik przykładowy

![ElementFaceReference.BySurface](./Revit.GeometryReferences.ElementFaceReference.BySurface_img.jpg)
