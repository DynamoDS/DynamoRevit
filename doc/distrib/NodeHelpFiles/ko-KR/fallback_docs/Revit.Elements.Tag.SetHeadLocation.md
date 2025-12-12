## 상세
이 노드는 태그를 가져와서 헤드 위치를 변경합니다. 이렇게 하면 일관된 배치 동작을 자동화하여 태그가 지정되는 요소 바로 위에 태그를 지정할 수 있습니다.

이 예에서는 문이 "Studio Live Work Core B" 뷰에서 선택되었습니다. 해당 문의 위치가 추출된 다음, 수평 및 addLeader에 대한 부울 값과 함께 Tag.ByElementAndLocation에 대한 원래 입력으로 사용됩니다. 태그 위치가 Tag.SetHeadLocation 노드를 사용하여 요소 바로 위에 중첩되지 않도록 원래 위치가 수정됩니다.

___
## 예제 파일

![Tag.SetHeadLocation](./Revit.Elements.Tag.SetHeadLocation_img.jpg)
