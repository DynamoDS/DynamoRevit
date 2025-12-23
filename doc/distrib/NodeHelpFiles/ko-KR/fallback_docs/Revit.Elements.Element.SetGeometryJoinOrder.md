## 상세
이 노드는 기하학적으로 이미 결합된 두 Revit 요소 간의 결합 순서를 변경합니다. 이 노드를 사용하면 사용자 중 어떤 요소의 형상이 "절단"되는지 또는 다른 요소보다 우선하는지를 결정할 수 있습니다.

이 예에서는 결합된 두 개의 벽을 선택하고 Element.SetGeometryJoinOrder 노드에 대한 입력(cuttingElement 및 otherElement)으로 사용합니다. 출력은 지정된 결합 순서입니다.

___
## 예제 파일

![Element.SetGeometryJoinOrder](./Revit.Elements.Element.SetGeometryJoinOrder_img.jpg)
