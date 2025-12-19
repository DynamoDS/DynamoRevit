## 상세
이 노드는 지정된 색인의 힘 값이 정의된 범위 내에 있는지 확인하여 ConnectionNode 리스트를 필터링합니다. 힘 데이터는 구조 해석 결과 또는 Revit 해석 모델에서 가져오며 선택한 결과 유형(예: Fx, Fy, Fz, Mx, My, Mz)별로 필터링됩니다.

이 예에서는 선택한 해석 결과 및 하중 케이스를 사용하여 Fz 힘 구성요소를 기반으로 기둥 요소 세트를 선택하고 평가합니다. Fz 값이 지정된 힘 범위 내에 있는 요소만 허용된 연결로 반환됩니다.
___
## 예제 파일

![Filter.ByAnalysisResults](./AdvanceSteel.ConnectionAutomation.Nodes.Filter.ByAnalysisResults_img.jpg)
