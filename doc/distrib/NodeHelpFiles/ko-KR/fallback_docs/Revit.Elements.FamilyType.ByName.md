## 상세
`FamilyType.ByName`은 현재 문서에서 지정된 이름의 지정된 패밀리 유형을 검색하려고 시도합니다. 현재 문서에서 패밀리 유형을 사용할 수 없는 경우 null 값이 반환됩니다.

주: `FamilyType.ByName`은 상위 패밀리의 작성 순서대로(요소 ID 기준) 패밀리 유형 정의를 검색합니다. 여러 상위 패밀리에 이름이 같은 유형 정의가 있는 경우 첫 번째로 찾은 유형 정의가 반환됩니다. 패밀리 유형을 보다 간결하게 조회하려면 `FamilyType.ByFamilyAndName` 또는 `FamilyType.ByFamilyNameAndTypeName`을 사용하십시오.

아래 예에서는 "Door-Passage-Single-Flush" 패밀리의 문 패밀리 유형 "36" x 84"가 반환됩니다.
___
## 예제 파일

![FamilyType.ByName](./Revit.Elements.FamilyType.ByName_img.jpg)
