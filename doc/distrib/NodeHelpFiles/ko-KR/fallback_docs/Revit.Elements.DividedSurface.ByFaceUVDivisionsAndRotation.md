## 상세
이 노드는 Revit 요소의 선택한 면에 새 분할된 표면 요소를 작성하고 지정된 U 및 V 분할과 선택적 회전 각도를 사용하여 배치를 정의합니다.
분할된 표면은 면에 적용되는 패턴화된 그리드로, 양식 표면을 가로질러 커튼 패널, 가변 구성요소 또는 패널화된 시스템을 배치하는 데 일반적으로 사용됩니다.

U 분할 및 V 분할은 각 표면 방향에서 발생하는 세분화 수를 결정하는 반면 회전 매개변수는 표면의 U-V 좌표계를 기준으로 그리드 방향이 조정됩니다.

이 예에서는 면을 선택하고 슬라이더로 제어되는 UDivs, VDivs 및 gridRotation과 함께 노드 DividedSurface.ByFaceUVDivisionsAndRotation에 대한 표면에 대한 입력으로 사용합니다. 마지막 노드는 분할된 표면의 값을 표시합니다. 이 예제 그래프를 실행할 때 Revit 경고를 관찰하고 그리드가 선택한 표면에 나타나도록 제안된 요소를 삭제해야 합니다.

추가 정보는 링크를 참조하십시오.
https://help.autodesk.com/view/RVT/2025/KOR/?guid=GUID-81D9C500-F9CE-462A-AEB7-AA3AEC0C940D
___
## 예제 파일

![DividedSurface.ByFaceUVDivisionsAndRotation](./Revit.Elements.DividedSurface.ByFaceUVDivisionsAndRotation_img.jpg)
