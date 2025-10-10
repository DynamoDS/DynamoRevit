## 深入資訊
給定所需的 FamilyType 及其座標系統，在 Revit 專案中放置 Revit FamilyInstance。

此範例中提供特定的族群類型和座標系統點，並放置族群實體。
常見的使用案例包括根據 Revit 專案基準點建立座標系統，然後旋轉它以符合敷地旋轉。然後，您可以將此轉換後的座標系統饋送到 FamilyInstance.ByCoordinateSystem 節點，考量敷地方位，將族群實體放在其預期的真實位置。

如需座標系統的更多資訊，請參閱下面。
https://help.autodesk.com/view/RVT/2025/CHT/?guid=GUID-68611F67-ED48-4659-9C3B-59C5024CE5F2
___
## 範例檔案

![FamilyInstance.ByCoordinateSystem](./Revit.Elements.FamilyInstance.ByCoordinateSystem_img.jpg)
