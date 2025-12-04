## 상세
이 노드는 지정된 Revit 링크 요소의 역 변환 매트릭스를 검색합니다. Revit에서 링크된 모델은 변환(이동, 회전, 축척)을 통해 배치될 수 있습니다. 이 노드를 사용하면 해당 변환의 반전을 가져와 링크된 모델의 공간에 있는 좌표를 호스트 Revit 모델의 좌표계로 효과적으로 변환할 수 있습니다.

이 예에서는 레벨 L3에서 모든 Revit 링크 요소가 선택되고 LinkElement.LinkInverseTransform에 입력됩니다. 출력은 호스트 Revit 모델의 좌표계입니다.
___
## 예제 파일

![LinkElement.LinkInverseTransform](./Revit.Elements.LinkElement.LinkInverseTransform_img.jpg)
