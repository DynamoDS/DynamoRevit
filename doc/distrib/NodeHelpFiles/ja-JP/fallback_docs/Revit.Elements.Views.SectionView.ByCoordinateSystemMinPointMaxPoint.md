## 詳細
任意の CoordinateSystem で最小点と最大点を指定して断面図を作成します。

この例では、配置と方向が入力された CoordinateSystem に直接関連付けられている SectionView を生成します。ここでは、建物の右上コーナーに重点を置いた断面を定義するために、その CoordinateSystem が戦略的に配置されています。ここで重要なのは、minPoint と maxPoint の入力値の Z コンポーネントが、結果として得られる Revit の SectionView の前方クリップ オフセットのパラメータを正確に決定し、それによってモデルへの有効な視線の奥行きをコントロールしていることです。
要素表示のコントロールの詳細については、次のリンクを参照してください
https://help.autodesk.com/view/RVT/2024/JPN/?guid=GUID-48484D2E-28ED-4BC3-8703-3B0488F1DA56
___
## サンプル ファイル

![SectionView.ByCoordinateSystemMinPointMaxPoint](./Revit.Elements.Views.SectionView.ByCoordinateSystemMinPointMaxPoint_img.jpg)
