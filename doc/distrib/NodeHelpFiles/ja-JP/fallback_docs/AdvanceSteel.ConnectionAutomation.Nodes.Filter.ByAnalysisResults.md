## 詳細
このノードは、指定されたインデックスの荷重値が定義された範囲内にあるかどうかをチェックして ConnectionNode のリストをフィルタします。荷重データは、構造解析結果または Revit 解析モデルから取得され、選択された結果タイプ(Fx、Fy、Fz、Mx、My、Mz など)でフィルタされます。

この例では、選択した解析結果と荷重ケースを使用して、Fz 荷重コンポーネントに基づいて柱要素のセットが選択され、評価されます。Fz 値が指定した荷重範囲内にある要素のみが、受け入れられた接合として返されます。
___
## サンプル ファイル

![Filter.ByAnalysisResults](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAnalysisResults_img.jpg)
