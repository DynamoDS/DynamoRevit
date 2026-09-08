## 상세
`Transaction.Start`는 현재 Revit 문서에서 트랜잭션을 시작합니다. 트랜잭션은 Revit 모델에 적용되는 변경 사항을 하나의 작업 단위로 그룹화하여 함께 커밋하거나 롤백할 수 있도록 합니다. `Transaction.Start`는 일반적으로 Dynamo 워크플로에서 Revit 문서 변경 작업의 시작과 종료 시점을 명시적으로 제어해야 할 때 `Transaction.End`와 함께 사용됩니다.

아래 예제에서는 레벨을 작성하고 이를 `Transaction.Start`의 입력으로 사용합니다. 이를 통해 모델 변경 작업을 수행하기 전에 Revit 트랜잭션을 시작합니다. 필요한 Revit 작업이 완료되면 `Transaction.End`가 트랜잭션을 종료하고 변경 사항을 문서에 커밋합니다. 출력은 다른 트랜잭션 관련 노드에 전달할 수 있는 트랜잭션 객체입니다.
___
## 예제 파일

![Transaction.Start](./Revit.Transaction.Transaction.Start_img.jpg)
