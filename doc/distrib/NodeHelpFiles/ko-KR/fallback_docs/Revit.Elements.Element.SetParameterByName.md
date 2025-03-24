## 상세
`Element.SetParameterByName`은 매개변수 요소(이름으로 찾음)를 지정된 값으로 설정합니다. 값에는 double, 정수, 부울, 요소 ID, 요소 및 문자열이 포함됩니다. Revit에서 매개변수는 동일한 이름을 공유할 수 있습니다. 결과적으로 `Element.SetParameterByName`은 고유 ID를 기준으로 알파벳순으로 정렬된 매개변수 중 첫 번째로 발견된 매개변수에 값을 설정합니다.

아래 예에서는 모델 내의 룸 안에 있는 모든 가구 항목에 대해 주석 매개변수가 설정되고 있습니다. 주석 매개변수의 값은 가져온 룸 이름입니다.
___
## 예제 파일

![Element.SetParameterByName](./Revit.Elements.Element.SetParameterByName_img.jpg)
