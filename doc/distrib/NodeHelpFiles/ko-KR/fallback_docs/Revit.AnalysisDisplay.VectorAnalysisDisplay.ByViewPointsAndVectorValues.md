## 상세
`VectorAnalysisDisplay.ByViewPointsAndVectorValues`는 Revit 뷰에 벡터 해석 표시를 작성합니다. 입력 뷰는 해석 표시가 나타날 위치를 정의합니다. 점은 해석 마커가 표시될 위치를 정의하고, 벡터는 표시되는 해석의 방향을 정의하며, 값은 각 점과 연관된 수치 결과를 정의합니다. 출력은 선택한 뷰에 표시되는 Revit 해석 표시 요소입니다.

아래 예제에서는 Revit 3D 뷰에서 샘플 위치를 정의하기 위해 2개의 점을 작성합니다. 그런 다음 해당 위치에서 방향 해석 데이터를 정의하기 위해 2개의 벡터를 작성합니다. 점과 벡터를 리스트로 결합한 후 뷰, 해석 이름, 설명과 함께 `VectorAnalysisDisplay.ByViewPointsAndVectorValues`의 입력으로 사용합니다. 출력은 선택한 Revit 뷰에 표시되는 벡터 해석 표시입니다.
___
## 예제 파일

![VectorAnalysisDisplay.ByViewPointsAndVectorValues](./Revit.AnalysisDisplay.VectorAnalysisDisplay.ByViewPointsAndVectorValues_img.jpg)
