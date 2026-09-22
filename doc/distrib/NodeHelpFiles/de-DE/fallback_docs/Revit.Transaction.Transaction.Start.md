## Im Detail
`Transaction.Start` startet eine Transaktion im aktuellen Revit-Dokument. Transaktionen werden verwendet, um am Revit-Modell vorgenommene Änderungen zu gruppieren, sodass sie zusammen übernommen oder zurückgesetzt werden können. `Transaction.Start` wird in der Regel zusammen mit `Transaction.End` verwendet, wenn ein Dynamo-Arbeitsablauf explizite Kontrolle darüber erhalten muss, wann Revit-Dokumentänderungen beginnen und enden.

Im folgenden Beispiel wird eine Ebene erstellt und als Eingabe für `Transaction.Start` verwendet. Dadurch wird eine Revit-Transaktion gestartet, bevor Änderungen am Modell vorgenommen werden. Nachdem die erforderlichen Revit-Vorgänge abgeschlossen sind, wird die Transaktion mit `Transaction.End` geschlossen, und die Änderungen werden in das Dokument übernommen. Die Ausgabe ist ein Transaktionsobjekt, das an andere transaktionsbezogene Blöcke übergeben werden kann.
___
## Beispieldatei

![Transaction.Start](./Revit.Transaction.Transaction.Start_img.jpg)
