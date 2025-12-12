## 상세
이 노드는 호스트 모델 내의 Revit 링크 요소에 적용된 변환 매트릭스를 검색합니다.
즉, 링크된 요소의 좌표계를 호스트 Revit 프로젝트의 좌표계에 매핑하는 위치, 회전 및 축척 변환을 반환합니다.
이 옵션은 링크된 모델 간에 형상을 정렬, 분석 또는 조작해야 하는 경우에 유용합니다.

이 예에서는 레벨 L3에서 모든 Revit 링크 요소가 선택되고 LinkElement.LinkTransform에 입력됩니다. 출력은 링크된 요소의 위치, 회전 및 축척 변환입니다.
___
## 예제 파일

![LinkElement.LinkTransform](./Revit.Elements.LinkElement.LinkTransform_img.jpg)
