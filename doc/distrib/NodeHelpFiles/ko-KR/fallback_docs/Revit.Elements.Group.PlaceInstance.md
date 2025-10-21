## 상세
위치(점) 및 groupType을 입력으로 사용하여 Revit 그룹을 배치합니다. groupType의 Object.Type 출력은 Revit.Elements.ElementType을 읽습니다.

 아래 예제에서는 활성 Revit 문서에서 모든 모델 그룹을 수집합니다. 그룹의 유형은 Group.GroupType으로 반환되며, 프로젝트 리스트에서 첫 번째 그룹으로 축소되어 GroupType의 입력으로 사용됩니다. 위치 입력은 100,100에 배치됩니다.
___
## 예제 파일

![Group.PlaceInstance](./Revit.Elements.Group.PlaceInstance_img.jpg)
