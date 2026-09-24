## 詳細
`Subelement.GetAllParameters` は、パラメータの表示名ではなく、サブ要素の内部 Revit パラメータ ID を取得します。

次の例では、現在のビューにおけるすべての要素を選択し、サブ要素を含む要素のみが表示されるようにフィルタリングしています。次に、これらのサブ要素を`Subelement.GetAllParameters` への入力として使用します。出力は、各サブ要素に関連付けられたパラメータ ID のリストです。最後のノードでは、その出力を使用してパラメータ値を取得していることが示されています。

___
## サンプル ファイル

![Subelement.GetAllParameters](./Revit.Elements.Subelement.GetAllParameters_img.jpg)
