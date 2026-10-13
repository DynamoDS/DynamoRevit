## Подробности
Узел `PerspectiveView.ByEyePointTargetAndElement` создает 3D-вид Revit в перспективе с помощью точки обзора, целевой точки, элемента, имени и логического значения `isolateElement`.

В приведенном ниже примере выбран элемент Revit, который используется в качестве фокуса вида в перспективе. Создается точка для положения камеры и другая точка для целевого положения. Они используются в качестве входных данных для узла `PerspectiveView.ByEyePointTargetAndElement` вместе с выбранным элементом. На выходе получается новый вид в перспективе, направленный от точки обзора к целевой точке, с границами вида на основе выбранного элемента.
___
## Файл примера

![PerspectiveView.ByEyePointTargetAndElement](./Revit.Elements.Views.PerspectiveView.ByEyePointTargetAndElement_img.jpg)
