## Im Detail
`Transaction.End` schließt die aktuelle Dynamo-Transaktion ab und gibt das angegebene Element zurück. In Revit stellen Transaktionen Änderungen dar, die am Revit-Dokument vorgenommen wurden. Wenn eine Änderung vorgenommen wird, ist dies anhand der aktivierten Schaltfläche Rückgängig ersichtlich. Mit `Transaction.End` können Benutzer Schritte zum Dynamo-Diagramm hinzufügen, sodass für jeden Schritt, bei dem `Transaction.End` verwendet wird, eine Rückgängig-Aktion erstellt wird.

Im folgenden Beispiel wird ein Familienexemplar in das Revit-Dokument eingefügt. `Transaction.End` wird aufgerufen, um die Platzierung abzuschließen, bevor das Familienexemplar mit `FamilyInstance.SetRotation` gedreht wird.

___
## Beispieldatei

![Transaction.End](./Revit.Transaction.Transaction.End_img.jpg)
