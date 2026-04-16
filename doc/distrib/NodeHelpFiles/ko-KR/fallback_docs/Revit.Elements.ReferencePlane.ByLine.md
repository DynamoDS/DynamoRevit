## 상세
Dynamo의 ReferencePlane.ByLine 노드는 정의된 선을 기준으로 사용하여 Revit 참조 평면을 작성합니다. 이렇게 하면 특정 위치 및 방향에서 사용자 참조 평면을 생성할 수 있습니다.

이 예에서는 조정 가능한 슬라이더와 함께 Point.ByCoordinates를 사용하여 두 점을 정의합니다. 그런 다음 Line.ByStartPointEndPoint가 두 점 사이에 작성되고, 마지막으로 ReferencePlane.ByLine 노드가 해당 선을 따라 참조 평면을 생성합니다.
___
## 예제 파일

![ReferencePlane.ByLine](./Revit.Elements.ReferencePlane.ByLine_img.jpg)
