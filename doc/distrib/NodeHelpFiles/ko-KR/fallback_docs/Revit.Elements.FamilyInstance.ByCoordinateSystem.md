## 상세
원하는 FamilyType과 해당 좌표계를 기준으로 Revit 프로젝트에 Revit FamilyInstance를 배치합니다.

이 예제에서는 특정 패밀리 유형과 좌표계 점이 제공되면 패밀리 인스턴스가 배치됩니다.
일반적인 사용 사례로는 Revit 프로젝트 기준점을 기준으로 좌표계를 작성한 후 대지 회전에 맞춰 회전시키는 방법이 있습니다. 이렇게 변환된 좌표계를 FamilyInstance.ByCoordinateSystem 노드에 입력하면, 대지 방향을 고려하여 패밀리 인스턴스를 실제 위치에 배치할 수 있습니다.

좌표계에 대한 자세한 내용은 아래를 참조하십시오.
https://help.autodesk.com/view/RVT/2025/KOR/?guid=GUID-68611F67-ED48-4659-9C3B-59C5024CE5F2
___
## 예제 파일

![FamilyInstance.ByCoordinateSystem](./Revit.Elements.FamilyInstance.ByCoordinateSystem_img.jpg)
