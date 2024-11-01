## Informacje szczegółowe
Węzeł `ScheduleFilter.FilterType` zwraca metodę używaną w filtrze wejściowym.
Możliwe typy filtrów:

- Equal - wartość pola jest równa podanej wartości.
- NotEqual - wartość pola nie jest równa podanej wartości.
- GreaterThan - wartość pola jest większa od podanej wartości.
- GreaterThanOrEqual - wartość pola jest większa niż podana wartość lub jej równa.
- LessThan - wartość pola jest mniejsza od podanej wartości.
- LessThanOrEqual - wartość pola jest mniejsza niż podana wartość lub jej równa.
- Contains - w przypadku pola ciągu wartość pola zawiera podany ciąg (string).
- NotContains - w przypadku pola ciągu wartość pola nie zawiera podanego ciągu (string).
- BeginsWith - w przypadku pola ciągu wartość pola rozpoczyna się od podanego ciągu (string).
- NotBeginsWith - w przypadku pola ciągu wartość pola nie rozpoczyna się od podanego ciągu (string).
- EndsWith - w przypadku pola ciągu wartość pola kończy się podanym ciągiem (string).
- NotEndsWith - w przypadku pola ciągu wartość pola nie kończy się podanym ciągiem (string).
- IsAssociatedWithGlobalParameter - pole jest skojarzone z określonym parametrem globalnym zgodnego typu
- IsNotAssociatedWithGlobalParameter - pole nie jest skojarzone z określonym parametrem globalnym zgodnego typu

W poniższym przykładzie zbierane jest pierwsze zestawienie z bieżącego pliku programu Revit. Widok zestawienia jest następnie sprawdzany pod kątem zastosowania filtrów i jedynym zastosowanym filtrem jest filtr „ciąg nie kończy się na”.
___
## Plik przykładowy

![ScheduleFilter.FilterType](./Revit.Schedules.ScheduleFilter.FilterType_img.jpg)
