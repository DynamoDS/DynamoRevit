## 詳細
`SpecType.ByTypeId` は、Revit の Spec TypeId 文字列を `SpecType` オブジェクトに変換します。スペック タイプは、長さ、面積、角度、マテリアル、力、文字、整数など、Revit でパラメータが表すデータの種類を定義します。これは、オートデスクが従来の `ParameterType` システムから Forge TypeId に移行した新しいバージョンの Revit において、共有パラメータ、プロジェクト パラメータ、パラメータ定義を作成する場合に通常使用されます。

次の例では、Revit の Spec TypeId を表す文字列を `SpecType.ByTypeId` に渡しています。出力は`SpecType` オブジェクトで、新しいパラメータ定義を作成するときに下流で使用できます。これにより、Revit はそのパラメータがどの種類のデータを格納すべきかを認識できます。
___
## サンプル ファイル

![SpecType.ByTypeId](./Revit.Elements.SpecType.ByTypeId_img.jpg)
