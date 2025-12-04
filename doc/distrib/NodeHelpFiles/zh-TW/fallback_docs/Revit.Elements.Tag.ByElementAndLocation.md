## 深入資訊
給定視圖、元素、位置、horizontal (如果沒有，則標籤會根據元素轉向) 和 addLeader 作為輸入，此節點會為 Revit 元素加標籤。

在此範例中，選取「Studio Live Work Core B」視圖中的一扇門。擷取該扇門的位置作為 Tag.ByElementAndLocation 的原始輸入，同時提供 horizontal 和 addLeader 的布林值。使用 Tag.SetHeadLocation 節點修改原始位置，讓標籤位置不會直接疊在元素上面。

___
## 範例檔案

![Tag.ByElementAndLocation](./Revit.Elements.Tag.ByElementAndLocation_img.jpg)
