## 상세
`ElementCurveReference.ByCurve`는 Dynamo 곡선으로부터 Revit 요소 곡선 참조를 작성합니다. 곡선 참조는 단순한 Dynamo 형상이 아닌, 다른 Revit 노드에서 선택하거나 참조할 수 있는 곡선이 필요할 때 유용합니다. 이는 치수 작성, 정렬, 구속조건 또는 모델 형상을 참조해야 하는 기타 요소 작성 등 Revit 참조가 필요한 워크플로에서 주로 사용됩니다.

아래 예제에서는 곡선을 선택하여 `ElementCurveReference.ByCurve`의 입력으로 사용합니다. 출력은 `ElementCurveReference`이며, 이후 곡선 기반 참조가 필요한 후속 노드에서 Revit 곡선 참조로 사용됩니다.
___
## 예제 파일

![ElementCurveReference.ByCurve](./Revit.GeometryReferences.ElementCurveReference.ByCurve_img.jpg)
