## Подробности
Этот узел создает вид фасада из существующего элемента ElevationMarker путем указания индекса маркера. Каждый элемент ElevationMarker в Revit может содержать до четырех отдельных видов фасадов — по одному для каждого направления (северное, южное, восточное и западное). Этот узел позволяет создать одну из таких отметок направления по ссылке на маркер и требуемый номер индекса.

В этом примере создается маркер фасада, который используется в качестве входного элемента elevationMarker для узла ElevationMarker.CreateElevationByMarkerIndex вместе с видом и индексом (0,1,2,3).

___
## Файл примера

![ElevationMarker.CreateElevationByMarkerIndex](./Revit.Elements.ElevationMarker.CreateElevationByMarkerIndex_img.jpg)
