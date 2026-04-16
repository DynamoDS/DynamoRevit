## Informacje szczegółowe
Węzeł Revision.SetIssued w dodatku Dynamo umożliwia określenie, czy wybrana rewizja w programie Revit jest oznaczona jako wydana, czy jako niewydana. Pobiera on element rewizji i wejściową wartość logiczną (True/False, czyli prawda/fałsz), co zapewnia użytkownikom bezpośrednią kontrolę nad stanem rewizji bez konieczności ręcznego edytowania jej w programie Revit.
Na tym wykresie węzeł Select Revision służy do wybierania określonej rewizji (np. „Seq. 1 – Schematic Design”). Węzeł Boolean udostępnia wartość True/False (Prawda/Fałsz), która jest następnie połączona z węzłem Revision.SetIssued w celu automatycznej aktualizacji stanu wydania rewizji.

___
## Plik przykładowy

![Revision.SetIssued](./Revit.Elements.Revision.SetIssued_img.jpg)
