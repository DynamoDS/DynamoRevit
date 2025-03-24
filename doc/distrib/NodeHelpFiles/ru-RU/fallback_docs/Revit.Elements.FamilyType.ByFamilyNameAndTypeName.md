## Подробности
Как и узел `Revit.Elements.FamilyType.ByFamilyAndName`, узел `Revit.Elements.FamilyType.ByFamilyNameAndTypeName` возвращает определение типоразмера семейства из текущего документа (если доступно). Эта функция аналогична узлу `Revit.Elements.FamilyType.ByFamilyAndName`. Однако вместо использования определения семейства этот узел полагается на строковый ввод для обоих значений. Если типоразмер семейства недоступен в текущем документе, возвращается значение null.

В приведенном ниже примере возвращается типоразмер семейства дверей «36" x 84"» из семейства «Дверь-Маятниковая-Одинарная-Щитовая».
___
## Файл примера

![FamilyType.ByFamilyNameAndTypeName](./Revit.Elements.FamilyType.ByFamilyNameAndTypeName_img.jpg)
