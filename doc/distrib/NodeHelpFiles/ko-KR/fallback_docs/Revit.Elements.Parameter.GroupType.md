## 상세
`Parameter.GroupType`은 지정된 매개변수의 GroupType을 반환합니다.

Revit에서 비공유 매개변수에 대한 매개변수 그룹은 미리 정의되어 있지만, 공유 매개변수를 사용하면 사용자 지정 그룹을 정의할 수 있습니다. 공유 매개변수 및 그룹 관련 작업에 대한 자세한 내용은 도움말 문서 [공유 매개변수 파일, 그룹 및 매개변수 작성](https://help.autodesk.com/view/RVT/2025/KOR/?guid=GUID-94EA2B8E-2C00-4D29-8D5A-C7C6664DE9CE)을 참조하십시오.

GroupType을 찾을 수 없으면 null 값이 반환됩니다.

아래 예에서는 현재 문서의 프로젝트 정보에 대한 모든 매개변수가 반환됩니다. GroupType도 반환됩니다.
___
## 예제 파일

![Parameter.GroupType](./Revit.Elements.Parameter.GroupType_img.jpg)
