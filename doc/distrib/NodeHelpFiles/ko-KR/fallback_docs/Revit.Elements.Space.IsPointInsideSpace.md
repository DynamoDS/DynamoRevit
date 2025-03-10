## 상세
`Space.IsPointInsideSpace`는 지정된 점이 지정된 공간 안에 있는지 확인합니다. 이는 Revit 내에서 요소에 마크 값을 할당할 때 유용할 수 있습니다.

아래 예제에서는 현재 Revit 문서의 지정된 뷰에 있는 모든 공기 터미널이 수집됩니다. 그런 다음 해당 점 위치가 `Space.IsPointInsideSpace`를 사용하여 지정된 뷰의 공간과 비교됩니다. 리스트 관리를 사용하여 공간 내에 위치한 공기 터미널을 필터링하기 위한 하위 리스트가 개발됩니다. List@Level 사용에 대한 자세한 내용은 이 [문서](https://primer2.dynamobim.org/5_essential_nodes_and_concepts/5-4_designing-with-lists/3-lists-of-lists#list-level)를 참조하십시오.
___
## 예제 파일

![Space.IsPointInsideSpace](./Revit.Elements.Space.IsPointInsideSpace_img.jpg)
