## 상세
이 노드는 뷰, 요소, 위치, 수평이 지정된 Revit 요소에 태그를 지정하고(그렇지 않을 경우 태그가 요소를 기준으로 방향을 지정함) 입력으로 addLeader를 추가합니다.

이 예에서는 문이 "Studio Live Work Core B" 뷰에서 선택되었습니다. 해당 문의 위치가 추출된 다음, 수평 및 addLeader에 대한 부울 값과 함께 Tag.ByElementAndLocation에 대한 원래 입력으로 사용됩니다. 태그 위치가 Tag.SetHeadLocation 노드를 사용하여 요소 바로 위에 중첩되지 않도록 원래 위치가 수정됩니다.

___
## 예제 파일

![Tag.ByElementAndLocation](./Revit.Elements.Tag.ByElementAndLocation_img.jpg)
