## Informacje szczegółowe
Ten węzeł wyodrębnia pętle krzywych (zazwyczaj łuki i linie), które tworzą widoczny obwód chmurki rewizji. Każdy segment chmurki jest reprezentowany jako obiekt krzywej (zwykle łuk) odpowiadający „bąbelkowemu” kształtowi znacznika rewizji w widoku lub na arkuszu.

W tym przykładzie tworzony jest prostokąt przy użyciu suwaka Number Slider definiującego jego wymiary, a następnie jest on dzielony na krzywe i odwracany w celu ustalenia orientacji. Te krzywe, wraz z wybranym widokiem (Site Plan) i rewizją (Seq. 2 – Not For Construction), służą do wygenerowania chmurki rewizji za pomocą węzła RevisionCloud.ByCurve. Utworzona chmurka rewizji zostaje następnie połączona z węzłem RevisionCloud.Curves, który wyodrębnia i wyświetla krzywe definiujące tę chmurkę. Pomaga to użytkownikom zweryfikować geometrię chmurki rewizji i zapewnia elastyczną możliwość ponownego użycia lub dalszej automatyzacji.
___
## Plik przykładowy

![RevisionCloud.Curves](./Revit.Elements.RevisionCloud.Curves_img.jpg)
