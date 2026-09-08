## Informacje szczegółowe
Węzeł Transaction.Start rozpoczyna transakcję w bieżącym dokumencie programu Revit. Transakcje służą do grupowania zmian wprowadzanych w modelu programu Revit, dzięki czemu można je zatwierdzać lub wycofywać razem. Węzeł Transaction.Start jest zazwyczaj używany z węzłem Transaction.End, gdy proces w dodatku Dynamo musi jawnie kontrolować moment rozpoczęcia i zakończenia zmian w dokumencie programu Revit.

W poniższym przykładzie zostaje utworzony poziom, który jest używany jako dane wejściowe węzła Transaction.Start. Powoduje to rozpoczęcie transakcji programu Revit przed wprowadzeniem zmian w modelu. Po zakończeniu wymaganych operacji programu Revit węzeł Transaction.End zamyka transakcję i zatwierdza zmiany w dokumencie. Dane wyjściowe to obiekt transakcji, który można przekazać do innych węzłów związanych z transakcjami.
___
## Plik przykładowy

![Transaction.Start](./Revit.Transaction.Transaction.Start_img.jpg)
