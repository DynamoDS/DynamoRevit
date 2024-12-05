## 詳細
`FamilyType.ByName` は、現在のドキュメントから、指定された名前の指定されたファミリ タイプを取得しようと試みます。ファミリ タイプが現在のドキュメントで使用できない場合は、null 値が返されます。

注: `FamilyType.ByName` は、ファミリ タイプ定義を親ファミリの作成順(要素 ID)で検索します。複数の親ファミリに同じ名前のタイプ定義がある場合は、最初に見つかったものが返されます。ファミリ タイプをより効率的に検索するには、`FamilyType.ByFamilyAndName` または `FamilyType.ByFamilyNameAndTypeName` を使用します。

次の例では、ファミリ「ドア-通路-片開き-フラッシュ」のドア ファミリ タイプ「36" x 84」が返されます。
___
## サンプル ファイル

![FamilyType.ByName](./Revit.Elements.FamilyType.ByName_img.jpg)
