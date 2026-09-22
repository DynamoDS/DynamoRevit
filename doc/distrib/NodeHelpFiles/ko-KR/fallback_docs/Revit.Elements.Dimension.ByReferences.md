## 상세
`Dimension.ByReferences`는 지정된 참조를 사용하여 선택한 뷰에서 입력 선을 따라 치수를 배치합니다. 출력은 Revit 치수 요소입니다.

아래 예제에서는 여러 점을 작성하고 선 노드로 연결하여 Revit에서 모델 곡선을 생성합니다. 그런 다음 모델 곡선을 Revit 곡선 참조로 변환합니다. 참조를 리스트로 결합한 후 `Dimension.ByReferences`의 `references` 입력으로 사용합니다. 추가 선은 치수의 배치와 방향을 정의하기 위해 작성됩니다. 활성 Revit 뷰도 `view` 입력으로 제공됩니다.
___
## 예제 파일

![Dimension.ByReferences](./Revit.Elements.Dimension.ByReferences_img.jpg)
