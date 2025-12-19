## Informacje szczegółowe
Ten węzeł umieszcza komponenty adaptacyjne przez zastosowanie wartości parametrów UV do wybranej powierzchni, co definiuje położenia umieszczania dla typu rodziny adaptacyjnej.

W tym przykładzie w rodzinie brył tworzona jest powierzchnia przez wyciągnięcie krzywej (wykonywane jest to ręcznie) i ta powierzchnia jest wybierana jako dane wejściowe face. Następnie podawane są wartości UV w celu określenia pozycji umieszczenia, a rodzina Diagnostic Tripod – 1 Point.rfa jest używana jako typ. Węzeł AdaptiveComponent.ByParametersOnFace zwraca komponenty adaptacyjne umieszczone na wybranej powierzchni. Przed uruchomieniem tego wykresu należy wczytać plik „Diagnostic Tripod – 1 Point.rfa” do rodziny brył.

Aby można było uruchomić ten przykładowy plik pomocy dla węzłów, należy wczytać plik „Diagnostics Tripod-1 point.rfa” do pliku programu Revit. Rodzina jest przechowywana tutaj: C:\ProgramData\Autodesk\RVT 2027\Dynamo\Samples\Data
___
## Plik przykładowy

![AdaptiveComponent.ByParametersOnFace](./Revit.Elements.AdaptiveComponent.ByParametersOnFace_img.jpg)
