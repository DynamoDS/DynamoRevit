## 상세

매개변수의 단위 유형을 반환합니다.

아래 예제에서는 모든 요소 매개변수를 추출하여 입력으로 사용합니다. 목표는 각 매개변수의 단위 유형을 표시하는 것입니다.
예를 들어 Element.Parameters에 "베이스 지름: 1’ – 3 ¼”"가 표시되면 Parameter.Unit의 출력은 "단위(이름 = 피트 또는 미터)"가 됩니다.
단위가 없는 매개변수(예: Element.Parameters = 절단 적용: 아니요)의 경우 Parameter.Unit 은 null을 반환합니다.
Dynamo 자체는 단위가 없으므로, 정확한 계산을 위해 입력되는 단위 유형을 식별하는 것이 중요합니다. 이 노드는 Dynamo가 해당 단위 데이터를 인식하고 처리할 수 있도록 도와줍니다.

___
## 예제 파일

![Parameter.Unit](./Revit.Elements.Parameter.Unit_img.jpg)
