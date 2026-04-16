## 상세
Dynamo의 Revision.IssuedTo 노드는 Revit의 수정기호에서 "발행 위치" 정보를 읽는 데 사용됩니다. 이를 통해 팀은 수정기호가 발행된 사용자를 추적할 수 있습니다.

이 그래프에서 수정기호 선택 노드는 프로젝트에서 특정 수정기호를 선택하는 데 사용됩니다. 그런 다음 Revision.IssuedTo 노드에서 "발행 위치" 필드를 추출하고 결과가 Watch 노드에 표시됩니다.

___
## 예제 파일

![Revision.IssuedTo](./Revit.Elements.Revision.IssuedTo_img.jpg)
