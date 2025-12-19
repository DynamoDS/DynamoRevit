## 상세
이 노드는 선택한 참조 평면의 실제 Revit 요소 참조를 추출합니다. 이 기능은 Revit 내에서 해당 평면을 형상 또는 치수에 대한 호스트 참조로 사용해야 하는 경우에 유용합니다.

예제:
이 그래프에서는 좌표를 사용하여 두 점을 정의하고 ReferencePlane.ByStartPointEndPoint를 사용하여 점 사이에 참조 평면을 작성합니다. 그런 다음 해당 참조 평면이 ReferencePlane.ElementPlaneReference에 연결됩니다. 그러면 평면의 Revit 기본 참조를 출력하여 호스팅 또는 정렬 작업에 사용할 수 있습니다.
___
## 예제 파일

![ReferencePlane.ElementPlaneReference](./Revit.Elements.ReferencePlane.ElementPlaneReference_img.jpg)
