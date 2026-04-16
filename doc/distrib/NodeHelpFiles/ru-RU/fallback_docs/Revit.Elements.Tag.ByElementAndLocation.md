## Подробности
Этот узел присваивает в качестве входных данных элементам Revit параметры view, element, location, horizontal (если нет, марка ориентируется относительно элемента) и addLeader.

В данном примере выбрана дверь в виде Studio Live Work Core B. Расположение этой двери извлекается и используется в качестве исходного входного значения для узла Tag.ByElementAndLocation вместе с логическими значениями для horizontal и addLeader. Исходное расположение изменяется таким образом, чтобы расположение марки не накладывалось непосредственно на элемент с помощью узла Tag.SetHeadLocation.

___
## Файл примера

![Tag.ByElementAndLocation](./Revit.Elements.Tag.ByElementAndLocation_img.jpg)
