## 상세
`SpecType.ByTypeId`는 Revit Spec TypeId 문자열을 `SpecType` 객체로 변환합니다. Spec Type은 Revit에서 매개변수가 나타내는 데이터 유형을 정의하며, 길이, 면적, 각도, 재료, 힘, 문자, 정수 등 다양한 유형이 있습니다. 이는 Autodesk가 기존 `ParameterType` 시스템을 Forge TypeId로 대체한 최신 버전의 Revit에서 공유 매개변수, 프로젝트 매개변수 또는 매개변수 정의를 작성할 때 주로 사용됩니다.

아래 예제에서는 Revit Spec TypeId를 나타내는 문자열을 `SpecType.ByTypeId`에 제공합니다. 출력은 새 매개변수 정의를 작성할 때 후속 단계에서 사용할 수 있는 `SpecType` 객체이며, 이를 통해 Revit은 해당 매개변수가 저장해야 하는 데이터 유형을 인식할 수 있습니다.
___
## 예제 파일

![SpecType.ByTypeId](./Revit.Elements.SpecType.ByTypeId_img.jpg)
