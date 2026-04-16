## Подробности
Этот узел присваивает в качестве входных данных элементам Revit параметры view, element, offset, horizontalAligment, verticalAlignment, horizontal (если нет, ориентация марки выполняется на основе элемента) и addLeader.

В этом примере в виде Studio Live Work Core B выбрана дверь, которая используется в качестве входных данных для узла Tag.ByelementAndOffset. Местоположение этой двери извлекается и используется в качестве начальной точки вектора. Та же точка изменяется с помощью ползунка, изменяющего координаты X и Y, и используется в качестве конечной точки вектора. Этот вектор используется в качестве входных данных для смещения наряду с истинными значениями во входных параметрах horizontal и addLeader. Параметр horizontalAlignment определяется значениями раскрывающегося списка узла Selection Horizontal Text Alignment (Left, Center, Right) и значениями раскрывающегося списка узла Selection Vertical Text Alignment (Bottom, Middle, Top).

___
## Файл примера

![Tag.ByElementAndOffset](./Revit.Elements.Tag.ByElementAndOffset_img.jpg)
