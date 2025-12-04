## 상세
이 노드는 표식기 색인을 지정하여 기존 ElevationMarker에서 입면도를 작성합니다. Revit의 각 ElevationMarker는 각 방향(북쪽, 남쪽, 동쪽 및 서쪽)에 하나씩 최대 4개의 개별 입면도를 호스트할 수 있습니다. 이 노드를 사용하면 표식기 및 원하는 색인 번호를 참조하여 방향 고도 중 하나를 생성할 수 있습니다.

이 예에서는 입면 표식기를 작성하여 뷰 및 색인 (0,1,2,3)과 함께 노드 ElevationMarker.CreateElevationByMarkerIndex에 대한 입력 elevationMarker로 사용합니다.

___
## 예제 파일

![ElevationMarker.CreateElevationByMarkerIndex](./Revit.Elements.ElevationMarker.CreateElevationByMarkerIndex_img.jpg)
