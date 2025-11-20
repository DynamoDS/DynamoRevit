## 詳細
このノードは、最小/最大角度の範囲と選択された軸でフィルタされた接合ノードのリストを返します。

この例では、W 形状の柱と角度付き HSS ブレースが選択されており、出力は「承認済み」および「却下済み」プロパティを持つディクショナリのリストで構成されています。両方の要素の X 軸を比較する場合、0 から 60 度の範囲内にある接合ノードの値が承認されます。最大値が 45 度の場合、接合ノードは却下されます。「indexFirst」および「indexSecond」パラメータは、入力データ構造に合わせて「レベルを使用」に設定されています。
___
## サンプル ファイル

![Filter.ByAngleBetweenAxes](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAngleBetweenAxes_img.jpg)
