## Подробности
Узел `ElementFaceReference.BySurface` создает ссылку на грань Revit на основе выбранной или сформированной поверхности. Поверхность Dynamo, связанная с геометрией Revit, преобразуется в объект `ElementFaceReference`. Эта ссылка затем может использоваться другими узлами, которым вместо геометрии поверхности требуется ссылка на грань Revit.

В приведенном ниже примере создается стена и ее поверхность используется в качестве входных данных для узла `ElementFaceReference.BySurface`. На выходе получается объект `ElementFaceReference`, подходящий для узлов, которым требуется ссылка на грань элемента Revit.
___
## Файл примера

![ElementFaceReference.BySurface](./Revit.GeometryReferences.ElementFaceReference.BySurface_img.jpg)
