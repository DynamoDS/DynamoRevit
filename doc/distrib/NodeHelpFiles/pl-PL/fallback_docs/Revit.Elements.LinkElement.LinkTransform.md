## Informacje szczegółowe
Ten węzeł pobiera macierz transformacji zastosowaną do elementu połączenia programu Revit w modelu nadrzędnym.
Innymi słowy: zwraca transformację położenia, obrotu i skalowania, która odwzorowuje układ współrzędnych połączonego elementu na układ współrzędnych nadrzędnego projektu programu Revit.
Jest to przydatne, gdy trzeba wyrównać geometrię pomiędzy modelami połączonymi, przeanalizować ją lub nią manipulować.

W tym przykładzie zostają wybrane wszystkie elementy połączone programu Revit na poziomie L3 i zostają one przekazane do węzła LinkElement.LinkTransform. Wynikiem jest transformacja położenia, obrotu i skalowania połączonego elementu.
___
## Plik przykładowy

![LinkElement.LinkTransform](./Revit.Elements.LinkElement.LinkTransform_img.jpg)
