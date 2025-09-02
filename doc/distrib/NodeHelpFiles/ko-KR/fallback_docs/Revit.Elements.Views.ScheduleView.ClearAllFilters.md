## 상세
지정된 ScheduleView 입력에서 정의된 모든 필터를 제거합니다.

이 예제에서는 먼저 일람표 뷰를 정의하고 ScheduleView.ScheduleFilters에 직접 연결하여 기존 뷰 필터를 명시적으로 표시합니다. 변환 과정을 강조하기 위해, 동일한 뷰는 짧은 10밀리초 지연을 거칩니다. 이 짧은 지연을 거친 뷰는 ScheduleView.ClearAllFilters 노드를 통과하며, 적용된 모든 필터는 이 노드에 의해 제거됩니다. 마지막으로 수정된 뷰는 다시 다른 ScheduleView.ScheduleFilters 노드로 전달되어 필터 리스트가 빈 리스트가 되었음을 명확히 보여줍니다. 따라서 원래 필터가 모두 성공적으로 제거되었음을 확인할 수 있습니다.
___
## 예제 파일

![ScheduleView.ClearAllFilters](./Revit.Elements.Views.ScheduleView.ClearAllFilters_img.jpg)
