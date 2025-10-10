## Informacje szczegółowe
Umieść obiekt FamilyInstance programu Revit w projekcie programu Revit na podstawie żądanego elementu FamilyType i jego układu współrzędnych.

W tym przykładzie podano określony typu rodziny oraz punkt układu współrzędnych i umieszczane jest wystąpienie rodziny.
Typowym przypadkiem użycia jest utworzenie układu współrzędnych na podstawie punktu bazowego projektu programu Revit, a następnie obrócenie go w celu dopasowania do obrotu terenu. Następnie można przekazać przekształcony układ współrzędnych do węzła FamilyInstance.ByCoordinateSystem, aby umieścić wystąpienia rodzin w ich wyznaczonych położeniach rzeczywistych z uwzględnieniem orientacji terenu.

Więcej informacji na temat układów współrzędnych podano poniżej.
https://help.autodesk.com/view/RVT/2025/PLK/?guid=GUID-68611F67-ED48-4659-9C3B-59C5024CE5F2
___
## Plik przykładowy

![FamilyInstance.ByCoordinateSystem](./Revit.Elements.FamilyInstance.ByCoordinateSystem_img.jpg)
