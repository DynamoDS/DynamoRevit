## Informacje szczegółowe
Węzeł `Transaction.End` dokańcza bieżącą transakcję dodatku Dynamo i zwraca określony element. W programie Revit transakcje reprezentują zmiany wprowadzone w dokumencie programu Revit. Gdy nastąpi zmiana, można ją zobaczyć za pomocą włączonego przycisku cofania. Za pomocą węzła `Transaction.End` użytkownicy mogą dodawać kroki do wykresu Dynamo, tworząc działanie cofania dla każdego kroku, w którym użyto węzła `Transaction.End`.

W poniższym przykładzie w dokumencie programu Revit jest umieszczane wystąpienie rodziny. Wywoływany jest węzeł `Transaction.End` w celu ukończenia umieszczania przed obróceniem wystąpienia rodziny za pomocą węzła `FamilyInstance.SetRotation`.

___
## Plik przykładowy

![Transaction.End](./Revit.Transaction.Transaction.End_img.jpg)
