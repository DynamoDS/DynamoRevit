## 상세
`Subelement.GetAllParameters`는 매개변수 표시 이름이 아닌 하위 요소의 내부 Revit 매개변수 ID를 가져옵니다.

아래 예제에서는 현재 뷰의 모든 요소를 선택한 후 하위 요소를 포함하는 요소만 표시하도록 필터링합니다. 그런 다음 하위 요소를 `Subelement.GetAllParameters`의 입력으로 사용합니다. 출력은 각 하위 요소와 연결된 매개변수 ID 리스트입니다. 마지막 노드에서는 출력을 사용하여 매개변수 값을 가져오는 과정을 보여 줍니다.

___
## 예제 파일

![Subelement.GetAllParameters](./Revit.Elements.Subelement.GetAllParameters_img.jpg)
