## 상세
이 노드는 지정된 표면을 평가하여 해석 패널을 정의하는 데 사용하기에 적합한지 확인합니다. 유효한 표면은 일반적으로 평면형이고 연속적이며 Revit의 해석 모델 환경 내에서 해석 표현으로 변환하는 데 적합합니다.

이 예에서는 프로젝트에서 슬래브 요소의 면이 수집되고 상단 면이 노드에 입력으로 제공됩니다. 노드는 선택한 표면이 해석 패널을 작성하기 위한 요구사항을 충족하는지 여부를 나타내는 부울 결과를 반환하며, 유효성 검사 중에 발생한 문제를 설명하는 메시지(선택 사항)를 반환합니다.
___
## 예제 파일

![Surface.IsValidForAnalyticalPanel](./AnalyticalAutomation.Geometry.Surface.IsValidForAnalyticalPanel_img.jpg)
