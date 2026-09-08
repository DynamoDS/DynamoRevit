## In profondità
`Subelement.GetAllParameters` recupera gli ID dei parametri interni di Revit degli elementi secondari, non i nomi visualizzati dei parametri.

Nell'esempio seguente, vengono selezionati tutti gli elementi della vista corrente che vengono filtrati per mostrare solo gli elementi che contengono elementi secondari. Gli elementi secondari vengono quindi utilizzati come input in`Subelement.GetAllParameters`. L'output è un elenco di ID dei parametri associati a ciascun elemento secondario. L'ultimo nodo mostra l'output utilizzato per recuperare i valori dei parametri.

___
## File di esempio

![Subelement.GetAllParameters](./Revit.Elements.Subelement.GetAllParameters_img.jpg)
