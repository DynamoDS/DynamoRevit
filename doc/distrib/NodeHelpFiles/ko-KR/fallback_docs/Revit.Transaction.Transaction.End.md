## 상세
`Transaction.End`는 현재 Dynamo 트랜잭션을 완료하고 지정된 요소를 반환합니다. Revit에서 트랜잭션은 Revit 문서에 대한 변경 사항을 나타냅니다. 변경 사항이 발생하면 활성화된 실행 취소 버튼을 통해 해당 변경 사항을 확인할 수 있습니다. 사용자는 `Transaction.End`를 사용하여 Dynamo 그래프에 단계를 추가할 수 있으며, 이때 `Transaction.End`가 사용된 각 단계에 대해 실행 취소 작업이 작성됩니다.

아래 예제에서는 패밀리 인스턴스가 Revit 문서에 배치됩니다. 패밀리 인스턴스가 `FamilyInstance.SetRotation`을 사용하여 회전되기 전에 배치를 완료하기 위해 `Transaction.End`가 호출됩니다.

___
## 예제 파일

![Transaction.End](./Revit.Transaction.Transaction.End_img.jpg)
