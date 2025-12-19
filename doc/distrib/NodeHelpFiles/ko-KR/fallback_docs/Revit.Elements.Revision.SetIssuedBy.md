## 상세
Dynamo의 Revision.SetIssuedBy 노드는 Revit의 수정기호에 대한 "발행자" 값을 설정하거나 업데이트하는 데 사용됩니다. 수정기호 발행자를 기록하여 Revit에서 수동으로 편집하지 않고도 문서 명확성과 일관성을 유지함으로써 수정기호 제어를 자동화할 수 있습니다.

이 그래프에서 수정기호 선택 노드는 필요한 수정기호를 선택하는 데 사용되며 문자열 입력(예: ABC)은 발급자의 이름을 제공합니다. 그런 다음 Revision.SetIssuedBy 노드는 이 값을 선택한 수정기호에 적용하여 Revit 모델에서 직접 "발행자" 필드를 업데이트합니다.

___
## 예제 파일

![Revision.SetIssuedBy](./Revit.Elements.Revision.SetIssuedBy_img.jpg)
