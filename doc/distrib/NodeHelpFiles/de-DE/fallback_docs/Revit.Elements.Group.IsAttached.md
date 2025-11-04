## Im Detail
Gibt zurück, ob ein bestimmtes Gruppenelement im Revit-Projekt an eine übergeordnete Gruppe angehängt ist. Einfacher ausgedrückt wird geprüft, ob eine Gruppe Teil einer größeren, verschachtelten Gruppe ist. Dieser Block ist nützlich, wenn Sie Gruppen identifizieren und verwalten möchten, die in anderen Gruppen organisiert sind. Sie können den Block beispielsweise für Folgendes verwenden:
• Gruppen filtern: Isolieren Sie Gruppen, die nicht Teil anderer Gruppen sind
• Verschachtelte Gruppen verwalten: Verstehen Sie die Struktur verschachtelter Gruppen, und verarbeiten Sie sie entsprechend.
• Steuerelemente ändern: Vermeiden Sie direkte Änderungen an Elementen innerhalb einer Gruppe, die an eine übergeordnete Gruppe angehängt ist, da dies die Struktur der übergeordneten Gruppe beeinträchtigen könnte.
• Aufgaben automatisieren: Verwalten und bearbeiten Sie Gruppen dynamisch, je nachdem, ob sie angehängt sind oder nicht.
Im Wesentlichen hilft Ihnen der Block Group.IsAttached, die Beziehung zwischen Gruppen in Ihrem Revit-Modell zu verstehen und Dynamo-Arbeitsabläufe zu erstellen, die diese Hierarchie berücksichtigen.

Im folgenden Beispiel werden alle Modellgruppen aus dem aktiven Revit-Dokument als Eingabe übernommen. Die Ausgaben lauten True oder False. Wahre Ergebnisse besagen, dass die Gruppe über Anhänge (Verschachtelungen) verfügt. Falsche Ergebnisse besagen, dass die Gruppe KEINE Anhänge (Verschachtelung) aufweist.

___
## Beispieldatei

![Group.IsAttached](./Revit.Elements.Group.IsAttached_img.jpg)
