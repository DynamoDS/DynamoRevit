## 상세
`PathOfTravel.LongestOfShortestExitPaths`는 여러 점으로 구성된 그룹에서 하나 이상의 출구 점까지 이동할 때, 최단 경로들 중 가장 긴 경로를 나타내는 이동 경로를 작성합니다. 각 시작점은 사용 가능한 출구와 비교하여 테스트되며, Revit은 각 시작점에 대해 가장 가까운 유효 출구 경로를 결정합니다. 그런 다음 이 노드는 이러한 최단 경로 결과 중 이동 거리가 가장 긴 경로를 반환합니다.

아래 예제에서는 먼저 이동 경로를 계산하는 데 필요한 Revit 뷰를 제공하기 위해 레벨과 평면도를 작성합니다. 그런 다음 `Point.ByCoordinates`를 사용하여 여러 점을 생성하고, 가능한 출구 위치를 나타내는 리스트로 결합합니다. `PathOfTravel.LongestOfShortestExitPaths`는 평면 뷰와 대상 점 리스트를 사용하여 이동 경로를 계산하고, 이동 거리가 가장 긴 경로를 반환합니다.
___
## 예제 파일

![PathOfTravel.LongestOfShortestExitPaths](./Revit.Elements.PathOfTravel.LongestOfShortestExitPaths_img.jpg)
