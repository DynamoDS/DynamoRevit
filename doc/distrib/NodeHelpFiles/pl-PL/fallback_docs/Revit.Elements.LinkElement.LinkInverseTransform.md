## Informacje szczegółowe
Ten węzeł pobiera macierz odwrotnej transformacji danego elementu połączenia programu Revit. W programie Revit modele połączone mogą być pozycjonowane za pomocą transformacji (przekształcenia, obrotu, skalowania). Ten węzeł umożliwia uzyskanie odwrotności tej transformacji, dzięki czemu współrzędne z przestrzeni modelu połączonego są przekształcane z powrotem do układu współrzędnych nadrzędnego modelu programu Revit.

W tym przykładzie zostają wybrane wszystkie elementy połączone programu Revit na poziomie L3 i zostają one przekazane do węzła LinkElement.LinkInverseTransform. Wynikiem jest układ współrzędnych nadrzędnego modelu programu Revit.
___
## Plik przykładowy

![LinkElement.LinkInverseTransform](./Revit.Elements.LinkElement.LinkInverseTransform_img.jpg)
