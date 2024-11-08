## Informacje szczegółowe
Węzeł `Element.SetParameterByName` ustawia daną wartość dla elementu parametru (znalezionego na podstawie nazwy). Wartości obejmują liczby typu double i integer, wartości logiczne (boolean), identyfikatory elementów (ElementId), elementy oraz ciągi (string). W programie Revit parametry mogą mieć tę samą nazwę. Węzeł `Element.SetParameterByName` ustawia wartość dla pierwszego znalezionego parametru z posortowanych alfabetycznie na podstawie unikatowych identyfikatorów (UniqueId).

W poniższym przykładzie parametr komentarzy jest ustawiany dla wszystkich elementów mebli w modelu, które znajdują się w pomieszczeniu. Wartość parametru komentarzy jest pobraną nazwą pomieszczenia.
___
## Plik przykładowy

![Element.SetParameterByName](./Revit.Elements.Element.SetParameterByName_img.jpg)
