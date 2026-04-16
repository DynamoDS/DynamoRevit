## 詳細
このノードは、入力としてビュー、要素、位置、水平(いいえの場合、タグは要素に基づいて方向付けされます)、および addLeader を指定して Revit 要素にタグを付けます。

この例では、「Studio Live Work Core B」ビューでドアが選択されています。このドアの位置が抽出され、次に、Tag.ByElementAndLocation への元の入力として、水平および addLeader のブール値とともに使用されます。Tag.SetHeadLocation ノードを使用して、タグの位置が要素の上に直接重ならないように、元の位置が変更されます。

___
## サンプル ファイル

![Tag.ByElementAndLocation](./Revit.Elements.Tag.ByElementAndLocation_img.jpg)
