## In profondità
`FamilyType.ByName` tenterà di recuperare il tipo di famiglia specificato con il nome dato dal documento corrente. Se il tipo di famiglia non è disponibile nel documento corrente, viene restituito un valore null.

Nota: `FamilyType.ByName` cerca le definizioni dei tipi di famiglia nell'ordine di creazione della famiglia principale (per ID elemento). Se più famiglie principali hanno una definizione del tipo con lo stesso nome, viene restituita la prima trovata. Per una ricerca più concisa dei tipi di famiglia, utilizzare `FamilyType.ByFamilyAndName` o `FamilyType.ByFamilyNameAndTypeName`.

Nell'esempio seguente, viene restituito un tipo di famiglia di porte, "36" x 84", della famiglia "Door-Passage-Single-Flush".
___
## File di esempio

![FamilyType.ByName](./Revit.Elements.FamilyType.ByName_img.jpg)
