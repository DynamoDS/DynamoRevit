## 상세
Dynamo의 Revision.Issued 노드는 Revit의 수정기호가 발행됨으로 표시되는지 여부를 확인하는 데 사용됩니다. true 또는 false 값(부울)을 반환하므로 팀이 Revit 수정기호 설정을 열지 않고도 수정기호의 상태를 신속하게 확인할 수 있습니다.

이 그래프에서 수정기호 선택 노드는 프로젝트에서 수정기호를 선택하는 데 사용됩니다. 그런 다음 Revision.Issued 노드가 선택한 수정기호가 발행되었는지 확인하고 결과가 Watch 노드에 true 또는 false로 표시됩니다. 이렇게 하면 Dynamo를 통해 직접 수정기호의 이슈 상태를 쉽게 확인할 수 있습니다.

___
## 예제 파일

![Revision.Issued](./Revit.Elements.Revision.Issued_img.jpg)
