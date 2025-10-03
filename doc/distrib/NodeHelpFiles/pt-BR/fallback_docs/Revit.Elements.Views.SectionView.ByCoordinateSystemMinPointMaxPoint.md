## Em profundidade
Cria vistas de corte com base em qualquer CoordinateSystem com pontos mínimo e máximo.

Neste exemplo, geramos uma SectionView cujo posicionamento e orientação estão diretamente correlacionados com o CoordinateSystem de entrada. Aqui, esse CoordinateSystem foi estrategicamente posicionado para definir um corte focado no canto superior direito do edifício. É crucial observar que o componente Z de nossa entrada minPoint e maxPoint determina com precisão o parâmetro Deslocamento do recorte afastado do nó SectionView do Revit resultante, controlando assim sua profundidade de visualização efetiva no modelo.
Para obter mais informações sobre como controlar a exibição dos elementos, consulte o link abaixo
https://help.autodesk.com/view/RVT/2024/PTB/?guid=GUID-48484D2E-28ED-4BC3-8703-3B0488F1DA56
___
## Arquivo de exemplo

![SectionView.ByCoordinateSystemMinPointMaxPoint](./Revit.Elements.Views.SectionView.ByCoordinateSystemMinPointMaxPoint_img.jpg)
