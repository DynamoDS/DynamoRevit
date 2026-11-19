## 詳細
`GroupType.ByTypeId` は主に、 Revit のパラメータ グループをプログラムで指定する必要がある、パラメータの作成および管理ワークフローで使用されます。入力は、Revit のパラメータ グループのタイプ ID を表す文字列です。出力は、パラメータのグループ分類を必要とする下流のノードで使用できる `GroupType` オブジェクトです。

次の例では、Revit のパラメータ グループのタイプ ID を表す複数の文字列を作成し、リスト化しています。このリストは、`GroupType.ByTypeId` の入力として使用されます。出力は、対応するパラメータ グループを表す Revit の `GroupType` オブジェクトのリストです。
___
## サンプル ファイル

![GroupType.ByTypeId](./Revit.Elements.GroupType.ByTypeId_img.jpg)
