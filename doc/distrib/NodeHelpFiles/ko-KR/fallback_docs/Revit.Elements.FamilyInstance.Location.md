## 상세
`FamilyInstance.Location`은 지정된 패밀리 인스턴스의 Dynamo 점을 반환합니다. 위치가 없으면 null 값이 반환됩니다. `FamilyInstance.Location`은 점 기반 요소에서 작동하며 Revit에서 곡선 기반 요소(예: 벽 또는 보 요소)에 대한 위치를 반환하지 않습니다.

아래 예에서는 모든 문 패밀리 인스턴스가 현재 문서의 현재 뷰에서 수집됩니다. 그런 다음 `FamilyInstance.Location`을 통해 문의 위치가 반환됩니다.

이 예에서 커튼월 문은 null을 반환합니다. 커튼 패널 위치는 커튼 패널 노드를 통해 사용할 수 있습니다.
___
## 예제 파일

![FamilyInstance.Location](./Revit.Elements.FamilyInstance.Location_img.jpg)
