## 상세
이 노드는 Revit 요소 세트에 대해 특정 성능 어드바이저 규칙을 실행합니다.

이 예에서 성능 어드바이저 규칙은 "뷰 자르기가 사용되지 않도록 설정"되어 있는지 여부를 확인합니다. 결과는 FailureMessage.FailingElements 노드로 전달되며, 이 노드는 이 확인에 실패한 모델의 특정 요소를 출력합니다. 이 워크플로우를 통해 사용자는 문제를 일으키는 정확한 요소를 보다 쉽게 추적하고 수정할 수 있습니다.

___
## 예제 파일

![PerformanceAdviserRule.Execute](./Revit.Elements.PerformanceAdviserRule.Execute_img.jpg)
