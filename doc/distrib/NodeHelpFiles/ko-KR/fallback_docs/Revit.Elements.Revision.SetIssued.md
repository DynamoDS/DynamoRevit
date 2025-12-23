## 상세
Dynamo의 Revision.SetIssued 노드를 사용하면 Revit에서 선택한 수정기호를 발행된 것으로 표시할지 아니면 발행되지 않은 것으로 표시할지를 제어할 수 있습니다. 수정기호 요소와 부울 입력(True/False)을 사용하여 Revit에서 수정기호 상태를 수동으로 편집하지 않고도 사용자가 수정기호 상태를 직접 제어할 수 있습니다.
이 그래프에서 수정기호 선택 노드는 특정 수정기호(예: "순서 1 - 개략적 설계")를 선택하는 데 사용됩니다. 부울 노드는 True/False 값을 제공하며, 이 값은 Revision.SetIssued 노드에 연결되어 수정기호의 발행됨 상태를 자동으로 업데이트합니다.

___
## 예제 파일

![Revision.SetIssued](./Revit.Elements.Revision.SetIssued_img.jpg)
