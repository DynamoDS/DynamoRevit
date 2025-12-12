## 詳細
このノードは、入力としビュー、要素、オフセット、horizontalAligment、verticalAlignment、水平(いいえの場合、タグは要素に基づいて方向付けされます)、および addLeader を指定して Revit 要素にタグを付けます。

この例では、「Studio Live Work Core B」ビューでドアが選択され、Tag.ByelementAndOffset への入力として使用されます。このドアの位置が抽出され、ベクトルの始点として使用されます。同じ点を、x 点と y 点を変更するスライダを使用して修正し、ベクトルの終点として使用します。このベクトルが、オフセットの入力として、true 値の水平入力と addLeader 入力とともに使用されます。horizontalAlignment は、Selection Horizontal Text Alignment ノードのドロップ ダウン値([左]、[中心]、[右])と、Selection Vertical Text Alignment ノードのドロップダウン値([下]、[中央]、[上])によって定義されます。

___
## サンプル ファイル

![Tag.ByElementAndOffset](./Revit.Elements.Tag.ByElementAndOffset_img.jpg)
