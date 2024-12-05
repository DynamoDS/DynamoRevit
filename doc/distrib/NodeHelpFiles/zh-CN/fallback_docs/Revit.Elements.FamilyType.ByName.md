## 详细
“FamilyType.ByName”将尝试从当前文档中检索给定名称的给定族类型。如果族类型在当前文档中不可用，则返回空值。

注意:“FamilyType.ByName”按父族的创建顺序(按图元 ID)搜索族类型定义。如果多个父族具有同名的类型定义，则返回找到的第一个。如需更简明地查找族类型，请使用“FamilyType.ByFamilyAndName”或“FamilyType.ByFamilyNameAndTypeName”

在下面的示例中，将返回“Door-Passage-Single-Flush”族中的门族类型“36” x 84”。
___
## 示例文件

![FamilyType.ByName](./Revit.Elements.FamilyType.ByName_img.jpg)
