## 상세
이 노드는 숫자 길이 값과 단위 유형 식별자를 사용하여 입력 값을 활성 Revit 프로젝트의 길이 단위로 변환합니다. 출력은 변환된 결과를 나타내는 double 값입니다.

이 예에서는 숫자 슬라이더가 길이 값을 제공하고 단위(예: 미터)를 선택하여 Unit.TypeId 문자열을 가져옵니다. 둘 다 UnitsUtilities.ConvertToCurrentProjectLengthUnit 노드에 연결되어 프로젝트의 단위 설정을 기준으로 변환된 길이 값을 반환합니다.
___
## 예제 파일

![UnitsUtilities.ConvertToCurrentProjectLengthUnit](./AnalyticalAutomation.Utilities.UnitsUtilities.ConvertToCurrentProjectLengthUnit_img.jpg)
