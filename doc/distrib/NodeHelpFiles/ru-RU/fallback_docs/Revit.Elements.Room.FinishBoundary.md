## Подробности
Узел `Room.FinishBoundary` возвращает вложенный список, представляющий итоговую границу заданного помещения. В возвращенном списке первый вспомогательный список представляет внешние кривые, а последующие списки — контуры внутри помещения. Граница помещения, возвращаемая этим узлом, расположена на чистовой поверхности внутри помещения Revit. Дополнительные сведения о линиях расположения стен см. в этой [статье](https://help.autodesk.com/view/RVT/2024/RUS/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89) справки.

Если указано неограниченное или неразмещенное помещение, возвращается значение null.

В приведенном ниже примере собираются все помещения в текущем документе и на выбранном виде. Затем возвращаются итоговые границы.
___
## Файл примера

![Room.FinishBoundary](./Revit.Elements.Room.FinishBoundary_img.jpg)