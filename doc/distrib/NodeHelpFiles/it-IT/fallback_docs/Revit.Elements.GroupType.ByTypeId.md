## In profondità
`GroupType.ByTypeId` viene utilizzato principalmente nei workflow di creazione e gestione dei parametri, in cui è necessario specificare a livello di programmazione un gruppo di parametri di Revit. L'input è una stringa che rappresenta un ID del tipo di gruppo di parametri di Revit. L'output è un oggetto `GroupType` che può essere utilizzato a valle nei nodi che richiedono una classificazione di raggruppamento dei parametri.

Nell'esempio seguente, vengono create diverse stringhe che rappresentano gli ID dei tipi di gruppo di parametri di Revit che vengono combinate in un elenco. L'elenco viene quindi utilizzato come input per `GroupType.ByTypeId`. L'output è un elenco di oggetti `GroupType` di Revit che rappresentano i gruppi di parametri corrispondenti.
___
## File di esempio

![GroupType.ByTypeId](./Revit.Elements.GroupType.ByTypeId_img.jpg)
