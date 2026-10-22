## Im Detail
`GroupType.ByTypeId` wird hauptsächlich in Arbeitsabläufen zum Erstellen und Verwalten von Parametern verwendet, bei denen eine Revit-Parametergruppe programmatisch angegeben werden muss. Die Eingabe ist eine Zeichenfolge, die die Typ-ID einer Revit-Parametergruppe darstellt. Die Ausgabe ist ein `GroupType`-Objekt, das später in Blöcken verwendet werden kann, die eine Parametergruppierungsklassifizierung erfordern.

Im folgenden Beispiel werden mehrere Zeichenfolgen erstellt, die die Typ-IDs von Revit-Parametergruppen darstellen, und in einer Liste zusammengefasst. Die Liste wird dann als Eingabe für `GroupType.ByTypeId` verwendet. Die Ausgabe ist eine Liste von `GroupType`-Revit-Objekten, die die entsprechenden Parametergruppen darstellen.
___
## Beispieldatei

![GroupType.ByTypeId](./Revit.Elements.GroupType.ByTypeId_img.jpg)
