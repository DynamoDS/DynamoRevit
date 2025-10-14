## 詳細

パラメータの単位タイプを返します。

次の例では、すべての要素パラメータを抽出して入力として使用します。目標は、各パラメータの単位タイプを表示することです。
例: Element.Parameters で「Base Diameter : 1’ – 3 ¼」と表示されている場合、Parameter.Unit からの出力は「単位(名前 = フィートまたはメートル)」になります。
パラメータに単位がない場合(例: Element.Parameters = Apply Cut : No)、Parameter.Unit は null を返します。
Dynamo 自体は単位がないため、正確な計算を実行するには、取り込む単位のタイプを識別することが重要です。このノードは、Dynamo が単位データを認識し処理する際に役立ちます。

___
## サンプル ファイル

![Parameter.Unit](./Revit.Elements.Parameter.Unit_img.jpg)
