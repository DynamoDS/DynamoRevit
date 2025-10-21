## 상세
최소점과 최대점을 갖는 CoordinateSystem을 기반으로 단면 뷰를 작성합니다.

이 예제에서는 입력된 CoordinateSystem과 직접적으로 연관된 위치와 방향으로 SectionView를 생성합니다. 여기서 해당 CoordinateSystem은 건물의 오른쪽 상단 모서리에 초점을 맞춘 단면을 정의하도록 전략적으로 배치되었습니다. 특히, minPoint와 maxPoint 입력의 Z 구성요소가 생성된 Revit SectionView의 먼 쪽 자르기 간격띄우기 매개변수를 정확하게 결정하므로, 모델 내에서 단면의 실제 시야 깊이를 제어하는 데 중요한 역할을 합니다.
요소 표시 제어에 대한 자세한 내용은 아래 링크를 참조하십시오.
https://help.autodesk.com/view/RVT/2024/KOR/?guid=GUID-48484D2E-28ED-4BC3-8703-3B0488F1DA56
___
## 예제 파일

![SectionView.ByCoordinateSystemMinPointMaxPoint](./Revit.Elements.Views.SectionView.ByCoordinateSystemMinPointMaxPoint_img.jpg)
