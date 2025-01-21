## 상세
`Space.FinishBoundary`는 지정된 공간의 마감 경계를 나타내는 내포된 리스트를 반환합니다. 반환된 리스트에서 첫 번째 하위 리스트는 가장 바깥쪽 곡선을 나타내고 후속 리스트는 공간 내의 루프를 나타냅니다. 이 노드에서 반환된 공간 경계는 Revit 공간 내부의 마감 면에 있습니다. 벽 위치 선에 대한 자세한 내용은 이 도움말 [문서](https://help.autodesk.com/view/RVT/2024/KOR/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89)를 참조하십시오.

경계가 없거나 배치되지 않은 공백이 제공되면 null 값이 반환됩니다.

아래 예제에서는 모든 공간이 현재 문서와 선택된 뷰에서 수집됩니다. 그런 다음 마감 경계가 반환됩니다.

___
## 예제 파일

![Space.FinishBoundary](./Revit.Elements.Space.FinishBoundary_img.jpg)
