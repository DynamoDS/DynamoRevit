## Informacje szczegółowe
Węzeł `Dimension.ValueOverride` zwraca nadpisanie wartości danego wymiaru, jeśli ma on nadpisaną wartość. W przypadku wymiarów wielosegmentowych zwracana jest zagnieżdżona lista wartości. Jeśli wymiar nie ma nadpisanej wartości, zwracana jest wartość null.

W poniższym przykładzie zbierany jest pierwszy wymiar z aktywnego widoku i jest on sprawdzany pod kątem tego, czy ma nadpisany wymiar. Jeśli wymiar jest nadpisany, jest czyszczony za pomocą węzła `Dimension.SetValueOverride`.
___
## Plik przykładowy

![Dimension.ValueOverride](./Revit.Elements.Dimension.ValueOverride_img.jpg)
