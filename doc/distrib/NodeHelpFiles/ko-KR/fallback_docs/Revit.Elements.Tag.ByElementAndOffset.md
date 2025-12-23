## 상세
이 노드는 뷰, 요소, 간격띄우기, horizontalAligment, verticalAlignment, horizontalAlignment(그렇지 않을 경우 태그가 요소를 기준으로 방향을 지정함) 및 addLeader가 입력으로 지정된 경우 Revit 요소에 태그를 지정합니다.

이 예에서는 문이 "Studio Live Work Core B" 뷰에서 선택되고 Tag.ByelementAndOffset에 대한 입력으로 사용됩니다. 해당 문의 위치가 추출되고 벡터 시작점으로 사용됩니다. 슬라이더를 사용하여 X 및 Y 점을 변경하여 동일한 점을 수정하고 벡터 끝점으로 사용합니다. 이 벡터는 수평 및 addLeader 입력의 true 값과 함께 간격띄우기에 대한 입력으로 사용됩니다. horizontalAlignment는 선택 수평 문자 정렬 노드 드롭다운 값(왼쪽, 가운데, 오른쪽) 및 선택 수직 문자 정렬 노드 드롭다운 값(맨 아래, 중간, 맨 위)으로 정의됩니다.

___
## 예제 파일

![Tag.ByElementAndOffset](./Revit.Elements.Tag.ByElementAndOffset_img.jpg)
