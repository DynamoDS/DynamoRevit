## 상세
`ReferencePoint.ByPoint`는 활성 Revit 패밀리 문서에서 지정된 점 위치에 참조 점 요소를 작성합니다. 참고: 패밀리 문서는 가변 구성요소 또는 매스 패밀리여야 합니다. 이 노드는 위치에 Dynamo 점을 사용하므로 "ReferencePoint.ByCoordinates"와 다릅니다. 이 노드는 최종 사용자가 직접 조작을 사용하여 Dynamo 점을 편집할 수 있고 참조 점을 업데이트할 수도 있으므로 유용합니다. 직접 조작에 대한 자세한 내용은 이 [링크](https://primer2.dynamobim.org/10_sample_workflow/10-1_getting-started-workflows/2-attractor-points#adjusting-with-direct-manipulation)를 참조하십시오.

아래 예제에서는 새 참조 점이 좌표 0,0,1에 작성됩니다.
___
## 예제 파일

![ReferencePoint.ByPoint](./Revit.Elements.ReferencePoint.ByPoint_img.jpg)
