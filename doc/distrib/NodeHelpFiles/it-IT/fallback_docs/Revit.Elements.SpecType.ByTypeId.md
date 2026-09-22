## In profondità
`SpecType.ByTypeId` converte una stringa Spec TypeId di Revit in un oggetto `SpecType`. I tipi di specifiche definiscono il tipo di dati che un parametro rappresenta in Revit, ad esempio lunghezza, area, angolo, materiale, forza, testo, numero intero e molti altri. Questa opzione viene comunemente utilizzata durante la creazione di parametri condivisi, parametri di progetto o definizioni di parametri nelle versioni più recenti di Revit, dove Autodesk ha sostituito il vecchio sistema `ParameterType`con Forge TypeIds.

Nell'esempio seguente, viene fornita una stringa che rappresenta Spec TypeId di Revit in `SpecType.ByTypeId`. L'output è un oggetto `SpecType` che può essere utilizzato a valle durante la creazione di una nuova definizione di parametro, in modo che Revit riconosca il tipo di dati che il parametro deve memorizzare.
___
## File di esempio

![SpecType.ByTypeId](./Revit.Elements.SpecType.ByTypeId_img.jpg)
