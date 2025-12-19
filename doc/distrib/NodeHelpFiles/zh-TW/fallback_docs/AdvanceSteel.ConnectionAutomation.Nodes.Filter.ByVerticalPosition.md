## 深入資訊
此節點會根據輸入元素在模型中的垂直位置篩選輸入元素。您可以選擇要演算元素的頂部或底部 Z 座標 (高度) 進行比較或邏輯篩選。它通常用作篩選系統的一部分，通常與 ConnectionNode 搭配使用，以隔離高於或低於特定高程的元素。在涉及空間分析的工作流程 (例如依樓層或區域分隔建築元素) 中很有用。

在此範例中，我們依其「頂部」z 座標位置 (高度) 篩選所有選取的結構資料項目。這可用來進一步判定該位置是否低於某個樓層、地板或天花板。
___
## 範例檔案

![Filter.ByVerticalPosition](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByVerticalPosition_img.jpg)
