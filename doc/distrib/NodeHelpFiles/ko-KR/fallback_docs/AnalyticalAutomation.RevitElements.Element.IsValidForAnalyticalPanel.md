## 상세
이 노드는 선택한 요소를 평가하여 해석 패널을 생성하는 데 적합한지 결정합니다. 선택적 checkOpenings 입력은 요소 내의 개구부가 유효성 검사에 포함되어야 하는지 여부를 지정합니다. true로 설정하면 개구부가 계산의 일부로 간주됩니다.

이 예에서 요소는 Element.ById 노드를 사용하여 요소 ID로 정의되고 Element.IsValidForAnalyticalPanel에 제공됩니다. 출력에는 요소가 유효한지 여부를 나타내는 부울과 요소 사용을 방해하는 문제를 설명하는 예외 메시지가 포함됩니다.
___
## 예제 파일

![Element.IsValidForAnalyticalPanel](./AnalyticalAutomation.RevitElements.Element.IsValidForAnalyticalPanel_img.jpg)
