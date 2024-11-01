## In profondità
`ScheduleFilter.FilterType` restituisce il metodo utilizzato per il filtro di input.
I tipi di filtro possibili includono:

- Equal: il valore del campo è uguale al valore specificato.
- NotEqual: il valore del campo non è uguale al valore specificato.
- GreaterThan: il valore del campo è maggiore del valore specificato.
- GreaterThanOrEqual: il valore del campo è maggiore di o uguale al valore specificato.
- LessThan: il valore del campo è minore del valore specificato.
- LessThanOrEqual: il valore del campo è minore di o uguale al valore specificato.
- Contains: per un campo stringa, il valore del campo contiene la stringa specificata.
- NotContains: per un campo stringa, il valore del campo non contiene la stringa specificata.
- BeginsWith: per un campo stringa, il valore del campo inizia con la stringa specificata.
- NotBeginsWith: per un campo stringa, il valore del campo non inizia con la stringa specificata.
- EndsWith: per un campo stringa, il valore del campo termina con la stringa specificata.
- NotEndsWith: per un campo stringa, il valore del campo non termina con la stringa specificata.
- IsAssociatedWithGlobalParameter: il campo è associato ad un parametro globale specificato di un tipo compatibile.
- IsNotAssociatedWithGlobalParameter: il campo non è associato ad un parametro globale specificato di un tipo compatibile.

Nell'esempio seguente, viene raccolto il primo abaco del file di Revit corrente. Viene quindi controllata la vista abaco per ricercare eventuali filtri e l'unico filtro applicato è di tipo "string does not end with".
___
## File di esempio

![ScheduleFilter.FilterType](./Revit.Schedules.ScheduleFilter.FilterType_img.jpg)
