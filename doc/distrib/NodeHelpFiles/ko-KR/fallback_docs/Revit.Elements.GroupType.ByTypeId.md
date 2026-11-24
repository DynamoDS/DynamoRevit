## 상세
`GroupType.ByTypeId`는 Revit 매개변수 그룹을 프로그래밍 방식으로 지정해야 하는 매개변수 작성 및 관리 워크플로에서 주로 사용됩니다. 입력은 Revit 매개변수 그룹 유형 ID를 나타내는 문자열입니다. 출력은 매개변수 그룹 분류가 필요한 후속 노드에서 사용할 수 있는 `GroupType` 객체입니다.

아래 예제에서는 Revit 매개변수 그룹 유형 ID를 나타내는 여러 문자열을 작성하고 리스트로 결합합니다. 그런 다음 이 리스트를 `GroupType.ByTypeId`의 입력으로 사용합니다. 출력은 해당 매개변수 그룹을 나타내는 Revit `GroupType` 객체의 리스트입니다.
___
## 예제 파일

![GroupType.ByTypeId](./Revit.Elements.GroupType.ByTypeId_img.jpg)
