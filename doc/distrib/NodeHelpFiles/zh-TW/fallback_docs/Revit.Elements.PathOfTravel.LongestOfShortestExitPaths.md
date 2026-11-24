## 深入資訊
`PathOfTravel.LongestOfShortestExitPaths` 會建立一條步行路徑，表示從一組點到一個或多個出口點所有最短路線當中最長的一條。每個起點都會根據可用的出口進行測試，而 Revit 會為每個起點決定最接近的有效出口路徑。然後，節點會傳回在這些最短路徑結果中步行距離最長的一條路徑。

以下範例先建立樓層和樓板平面視圖，提供步行路徑計算所需的 Revit 視圖。使用 `Point.ByCoordinates` 產生幾個點，然後合併成一個清單，表示可能的出口位置。`PathOfTravel.LongestOfShortestExitPaths` 會使用樓板平面視圖和目的地點清單計算步行路徑，傳回步行距離最遠的路徑。
___
## 範例檔案

![PathOfTravel.LongestOfShortestExitPaths](./Revit.Elements.PathOfTravel.LongestOfShortestExitPaths_img.jpg)
