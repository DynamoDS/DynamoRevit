## In profondità
Questo nodo utilizza un valore numerico numerico di lunghezza e un identificatore del tipo di unità, convertendo il valore di input nelle unità di lunghezza del progetto di Revit attivo. L'output è un valore double che rappresenta il risultato convertito.

In questo esempio, un dispositivo di scorrimento numerico fornisce un valore di lunghezza e viene selezionata un'unità (ad esempio, Meters) per ottenere la relativa stringa Unit.TypeId. Entrambi sono connessi al nodo UnitsUtilities.ConvertToCurrentProjectLengthUnit, che restituisce il valore di lunghezza convertito in base alle impostazioni delle unità del progetto.
___
## File di esempio

![UnitsUtilities.ConvertToCurrentProjectLengthUnit](./AnalyticalAutomation.Utilities.UnitsUtilities.ConvertToCurrentProjectLengthUnit_img.jpg)
