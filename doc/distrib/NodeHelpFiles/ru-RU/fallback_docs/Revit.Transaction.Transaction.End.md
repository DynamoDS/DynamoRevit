## Подробности
`Transaction.End` завершает текущую транзакцию Dynamo и возвращает указанный элемент. Транзакции в Revit представляют собой изменения, внесенные в документ Revit. Когда происходит изменение, активируется кнопка «Отменить». С помощью узла `Transaction.End` пользователи могут добавлять шаги в график Dynamo, создавая действие отмены для каждого шага, в котором используется `Transaction.End`.

В приведенном ниже примере в документе Revit размещается экземпляр семейства. Для завершения размещения вызывается узел `Transaction.End` до поворота экземпляра семейства с помощью узла `FamilyInstance.SetRotation`.

___
## Файл примера

![Transaction.End](./Revit.Transaction.Transaction.End_img.jpg)
