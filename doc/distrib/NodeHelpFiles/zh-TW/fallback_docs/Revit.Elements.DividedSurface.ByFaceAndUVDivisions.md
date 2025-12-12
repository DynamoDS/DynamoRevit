## 深入資訊
此節點會在選取的 Revit 元素面上建立新的分割表面元素，並使用指定的 U 和 V 分區數定義其配置。「分割表面」是套用到某個面的樣式網格，通常用於在塑形表面上定位帷幕板、自適應元件或嵌板化系統。

U 分區數和 V 分區數決定每個曲面方向要細分多少次，旋轉參數則相對於曲面的 U-V 座標系統調整網格方位。

在此範例中，選取一面作為 DividedSurface.ByFaceUVDivisions 節點的曲面輸入，同時提供由滑棒控制的 UDivs、VDivs。最後一個節點顯示分割表面的值。執行此範例圖表時，您必須觀察 Revit 警告並刪除建議的元素，讓網格出現在選取的曲面上。

如需其他資訊，請參閱連結。
https://help.autodesk.com/view/RVT/2025/CHT/?guid=GUID-81D9C500-F9CE-462A-AEB7-AA3AEC0C940D
___
## 範例檔案

![DividedSurface.ByFaceUVDivisionsAndRotation](./Revit.Elements.DividedSurface.ByFaceUVDivisionsAndRotation_img.jpg)
___
## 範例檔案

![DividedSurface.ByFaceAndUVDivisions](./Revit.Elements.DividedSurface.ByFaceAndUVDivisions_img.jpg)
