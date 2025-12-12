## 深入資訊
給定視圖、元素、偏移、horizontalAligment、verticalAlignment、horizontal (如果沒有，則標籤會根據元素轉向) 和 addLeader 作為輸入，此節點會為 Revit 元素加標籤。

在此範例中，選取「Studio Live Work Core B」視圖中的一扇門，作為 Tag.ByelementAndOffset 的輸入。擷取該扇門的位置作為向量起點。使用滑棒修改同一點來變更 x 點和 y 點作為向量終點。使用此向量作為偏移的輸入，並搭配 horizontal 和 addLeader 輸入的 true 值。由 Selection Horizontal Text Alignment 節點下拉式值 (Left、Center、Right) 和 Selection Vertical Text Alignment 節點下拉式值 (Bottom、Middle、Top) 定義 horizontalAlignment。

___
## 範例檔案

![Tag.ByElementAndOffset](./Revit.Elements.Tag.ByElementAndOffset_img.jpg)
