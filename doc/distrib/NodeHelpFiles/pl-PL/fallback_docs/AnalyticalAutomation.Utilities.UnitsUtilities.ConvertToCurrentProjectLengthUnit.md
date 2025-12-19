## Informacje szczegółowe
Ten węzeł pobiera wartość liczbową długości i identyfikator typu jednostki, a następnie konwertuje wartość wejściową na jednostki długości aktywnego projektu programu Revit. Dane wyjściowe to wartość typu double reprezentująca przekonwertowany wynik.

W tym przykładzie suwak Number Slider zapewnia wartość długości, a na potrzeby pobrania ciągu Unit.TypeId wybierana jest jednostka (na przykład metry). Obie pozycje są połączone z węzłem UnitsUtilities.ConvertToCurrentProjectLengthUnit, który zwraca przekonwertowaną wartość długości na podstawie ustawień jednostek projektu.
___
## Plik przykładowy

![UnitsUtilities.ConvertToCurrentProjectLengthUnit](./AnalyticalAutomation.Utilities.UnitsUtilities.ConvertToCurrentProjectLengthUnit_img.jpg)
