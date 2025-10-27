## 상세
지정된 ScheduleView 입력에서 정의된 모든 필터를 제거합니다.

이 예제에서는 먼저 일람표 뷰를 정의했으며 이 뷰는 ScheduleView.ScheduleFilters에 직접 라우팅되어 기존 뷰 필터를 명시적으로 표시합니다. 변환 과정을 강조하기 위해, 이 동일한 뷰가 10밀리초 정도 잠깐 일시 중지됩니다. 이 짧은 지연을 거친 뷰에 대해 ScheduleView.ClearAllFilters 노드가 진행되며 적용된 모든 필터가 제거됩니다. 마지막으로 새로 수정된 뷰는 다시 다른 ScheduleView.ScheduleFilters 노드로 전달되어 필터 리스트가 빈 리스트가 되었음을 명확히 보여주므로 원래 필터가 모두 성공적으로 제거되었음을 확인할 수 있습니다.
___
## 예제 파일

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)
