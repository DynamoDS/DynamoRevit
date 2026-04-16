## Informacje szczegółowe
Ten węzeł oznacza elementy programu Revit na podstawie danych wejściowych view, element, offset, horizontalAligment, verticalAlignment, horizontal (jeśli ta pozycja orientacji w poziomie ma wartość negatywną, oznaczenie zostanie zorientowane na podstawie orientacji elementu) i addLeader.

W tym przykładzie w widoku „Studio Live Work Core B” wybierane są drzwi i są one używane jako dane wejściowe węzła Tag.ByelementAndOffset. Położenie tych drzwi jest wyodrębniane i używane jako punkt początkowy wektora. Ten sam punkt jest modyfikowany przy użyciu suwaka zmieniającego punkty x i y, a następnie jest używany jako punkt końcowy wektora. Ten wektor służy jako dane wejściowe dla odsunięcia wraz z wartościami true (prawda) dla danych wejściowych horizontal i addLeader. Parametr horizontalAlignment jest definiowany przez wartości listy rozwijanej węzła Selection Horizontal Text Alignment (Left, Center, Right) i wartości listy rozwijanej węzła Selection Vertical Text Alignment (Bottom, Middle, Top).

___
## Plik przykładowy

![Tag.ByElementAndOffset](./Revit.Elements.Tag.ByElementAndOffset_img.jpg)
