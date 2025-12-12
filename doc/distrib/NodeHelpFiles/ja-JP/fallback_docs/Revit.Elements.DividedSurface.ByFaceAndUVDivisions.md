## 詳細
このノードは、Revit 要素の選択された面に新しい分割されたサーフェス要素を作成し、指定された U 分割と V 分割を使用してレイアウトを定義します。分割されたサーフェスは、面に適用されるパターン化された通芯で、通常、フォーム サーフェス全体にカーテン パネル、アダプティブ コンポーネント、またはパネル化されたシステムを配置するために使用されます。

U 分割と V 分割でサーフェスの各方向に作成するサブ区画の数を決定し、回転パラメータでサーフェスの U-V 座標系に対して通芯の方向を調整します。

この例では、面が選択され、DividedSurface.ByFaceUVDivisions ノードのサーフェスへの入力として、スライダでコントロールされる UDivs、VDivs とともに使用されます。最後のノードは、分割されたサーフェスの値を公開します。このサンプル グラフを実行する場合は、Revit の警告を確認し、提案された要素を削除して、選択したサーフェスに通芯が表示されるようにする必要があります。

詳細については、リンクを参照してください。
https://help.autodesk.com/view/RVT/2025/JPN/?guid=GUID-81D9C500-F9CE-462A-AEB7-AA3AEC0C940D
___
## サンプル ファイル

![DividedSurface.ByFaceUVDivisionsAndRotation](./Revit.Elements.DividedSurface.ByFaceUVDivisionsAndRotation_img.jpg)
___
## サンプル ファイル

![DividedSurface.ByFaceAndUVDivisions](./Revit.Elements.DividedSurface.ByFaceAndUVDivisions_img.jpg)
