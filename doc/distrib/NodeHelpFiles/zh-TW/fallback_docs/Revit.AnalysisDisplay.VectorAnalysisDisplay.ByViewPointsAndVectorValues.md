## 深入資訊
`VectorAnalysisDisplay.ByViewPointsAndVectorValues` 會在 Revit 視圖中建立向量分析顯示。輸入視圖定義分析顯示將會顯示的位置。點定義分析標識的位置，向量定義所顯示分析的方向，值定義與每個點關聯的數值結果。輸出是所選視圖中顯示的 Revit 分析顯示元素。

以下範例建立 2 個點定義 Revit 3D 視圖中的範例位置，然後建立 2 個向量定義這些位置的方向分析資料。點和向量會合併為清單，作為 `VectorAnalysisDisplay.ByViewPointsAndVectorValues` 的輸入，同時提供視圖、分析名稱和描述。輸出是所選 Revit 視圖中顯示的向量分析顯示。
___
## 範例檔案

![VectorAnalysisDisplay.ByViewPointsAndVectorValues](./Revit.AnalysisDisplay.VectorAnalysisDisplay.ByViewPointsAndVectorValues_img.jpg)
