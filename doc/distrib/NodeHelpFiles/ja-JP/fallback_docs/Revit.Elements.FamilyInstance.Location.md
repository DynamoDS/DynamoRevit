## 詳細
`FamilyInstance.Location` は、指定されたファミリ インスタンスの Dynamo における点を返します。場所がない場合は、null 値が返されます。`FamilyInstance.Location` は点ベースの要素で動作し、Revit の曲線ベースの要素(壁や梁などの要素)の位置は返しません。

次の例では、現在のドキュメントの現在のビュー内のすべてのドア ファミリ インスタンスが収集されます。次に、ドアの位置が `FamilyInstance.Location` で返されます。

この例では、カーテン ウォールのドアが null を返しています。カーテン パネルの位置は、カーテン パネルのノードから使用できます。
___
## サンプル ファイル

![FamilyInstance.Location](./Revit.Elements.FamilyInstance.Location_img.jpg)
