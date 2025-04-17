## 상세
`Element.GetHostedElements`는 요소 호스팅을 지원하는 지정된 요소(예: 벽)를 바탕으로 지정된 요소에 의존하는 요소를 반환합니다. 기본적으로 요소에 호스팅된 패밀리 인스턴스가 반환됩니다. `Element.GetHostedElements`는 다른 유형의 호스팅된 요소를 포함하는 옵션을 제공합니다.

여기에는 다음이 포함됩니다.
- 개구부
- 결합된 호스트에 호스팅된 요소
- 내장된 벽(예: 커튼월)
- 공유되는 내장된 삽입물

아래 예에서는 모든 벽 요소가 L2에서 수집됩니다. 그런 다음 `Element.GetHostedElements`를 통해 벽 요소에 호스팅된 요소가 있는지 확인됩니다. 이 리스트는 두 개의 리스트(문이 있는 벽과 문이 없는 벽)를 만드는 데 사용됩니다.
___
## 예제 파일

![Element.GetHostedElements](./Revit.Elements.Element.GetHostedElements_img.jpg)
