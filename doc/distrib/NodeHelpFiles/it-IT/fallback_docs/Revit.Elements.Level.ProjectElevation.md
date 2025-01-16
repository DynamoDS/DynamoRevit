## In profondità
`Level.ProjectElevation` restituisce la quota altimetrica per il livello specificato in unità di misura. `Level.ProjectElevation` riporta il valore dall'origine del progetto. Se la quota altimetrica dal livello del suolo è obbligatoria, questo valore può essere ottenuto con `Level.Elevation`.

Nell'esempio seguente, tutti i livelli vengono raccolti nel documento di Revit corrente. Viene restituito il valore di quota altimetrica del progetto dei livelli. Inoltre, i livelli vengono ordinati in base all'altezza con "List.SortByKey".
___
## File di esempio

![Level.ProjectElevation](./Revit.Elements.Level.ProjectElevation_img.jpg)
