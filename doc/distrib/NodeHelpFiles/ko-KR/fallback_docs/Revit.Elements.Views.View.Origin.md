## 상세
Revit의 모든 뷰에는 원점이 있습니다. `View.Origin`은 이 값을 Dynamo 점으로 반환합니다. Revit API 설명서에 따르면 "평면뷰의 원점은 의미가 없습니다". 이를 고려하면 `View.Origin`에서 제공하는 값은 최종 사용자에게 달려 있습니다.

아래 예에서는 활성 뷰와 선택한 3D 뷰의 원점이 반환됩니다.
___
## 예제 파일

![View.Origin](./Revit.Elements.Views.View.Origin_img.jpg)
