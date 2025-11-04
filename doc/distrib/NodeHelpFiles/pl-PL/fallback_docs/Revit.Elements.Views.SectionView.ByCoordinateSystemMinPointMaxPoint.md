## Informacje szczegółowe
Tworzy widoki przekroju na podstawie dowolnego układu współrzędnych (CoordinateSystem) z punktami min. (Min) i maks (Max).

W tym przykładzie generujemy widok przekroju (SectionView), którego położenie i orientacja są bezpośrednio skorelowane z wejściowym układem współrzędnych (CoordinateSystem). W tym przypadku ten układ współrzędnych został strategicznie wypozycjonowany, aby zdefiniować przekrój skoncentrowany na prawym górnym narożniku budynku. Należy koniecznie zauważyć, że komponent Z danych wejściowych minPoint i maxPoint precyzyjnie dyktuje parametr Far Clip Offset (Odsunięcie dalekiego przycięcia) wynikowego widoku przekroju programu Revit, co pozwala sterować jego efektywną głębokością wyświetlania w modelu.
Więcej informacji o sterowaniu wyświetlaniem elementów można znaleźć za pomocą poniższego linku
https://help.autodesk.com/view/RVT/2024/PLK/?guid=GUID-48484D2E-28ED-4BC3-8703-3B0488F1DA56
___
## Plik przykładowy

![SectionView.ByCoordinateSystemMinPointMaxPoint](./Revit.Elements.Views.SectionView.ByCoordinateSystemMinPointMaxPoint_img.jpg)
