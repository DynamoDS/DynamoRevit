## 상세
`ElementFaceReference.BySurface`는 선택하거나 생성한 표면으로부터 Revit 표면 참조를 작성합니다. 이 노드는 Revit 형상과 연결된 Dynamo 표면을 `ElementFaceReference`로 변환합니다. 이렇게 작성된 참조는 단순한 표면 형상이 아닌 Revit 면 참조가 필요한 다른 노드에서 사용할 수 있습니다.

아래 예제에서는 벽을 작성하고 해당 벽에서 가져온 표면을 `ElementFaceReference.BySurface`의 입력으로 사용합니다. 출력은 Revit 요소 면에 대한 참조가 필요한 노드에서 사용할 수 있는 `ElementFaceReference` 입니다.
___
## 예제 파일

![ElementFaceReference.BySurface](./Revit.GeometryReferences.ElementFaceReference.BySurface_img.jpg)
