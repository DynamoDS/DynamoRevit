## Informacje szczegółowe
Umieszcza grupy programu Revit z danymi wejściowymi location (punktami) i groupType. Pozycja danych wyjściowych Object.Type dla groupType odczytuje Revit.Elements.ElementType

 W poniższym przykładzie pobierane są wszystkie grupy modelu z aktywnego dokumentu programu Revit. Typy grup są zwracane za pomocą węzła Group.GroupType, redukowane do pierwszej grupy na liście projektu i używane jako dane wejściowe dla GroupType. Pozycja danych wejściowych location jest równa 100,100.
___
## Plik przykładowy

![Group.PlaceInstance](./Revit.Elements.Group.PlaceInstance_img.jpg)
