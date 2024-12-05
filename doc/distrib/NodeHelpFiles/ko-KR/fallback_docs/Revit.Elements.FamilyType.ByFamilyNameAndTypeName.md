## 상세
`Revit.Elements.FamilyType.ByFamilyAndName`과 마찬가지로 `Revit.Elements.FamilyType.ByFamilyNameAndTypeName`은 현재 문서에서 패밀리 유형 정의를 반환합니다(사용 가능한 경우). 이는 `Revit.Elements.FamilyType.ByFamilyAndName`과 유사합니다. 그러나 이 노드는 패밀리 정의를 사용하는 대신 두 값 모두에 대해 문자열 입력에 의존합니다. 현재 문서에서 패밀리 유형을 사용할 수 없는 경우 null 값이 반환됩니다.

아래 예에서는 "Door-Passage-Single-Flush" 패밀리의 문 패밀리 유형 "36" x 84"가 반환됩니다.
___
## 예제 파일

![FamilyType.ByFamilyNameAndTypeName](./Revit.Elements.FamilyType.ByFamilyNameAndTypeName_img.jpg)
