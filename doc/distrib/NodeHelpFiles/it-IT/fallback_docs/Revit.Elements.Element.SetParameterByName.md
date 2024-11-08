## In profondità
`Element.SetParameterByName` imposta un elemento parametro (trovato per nome) su un valore specificato. I valori includono: double, integer, boolean, ElementId, Element e string. In Revit, i parametri possono condividere lo stesso nome. Di conseguenza, `Element.SetParameterByName` imposterà il valore sul primo parametro trovato, ordinato alfabeticamente in base a UniqueId.

Nell'esempio seguente, il parametro dei commenti viene impostato per tutti gli elementi arredi del modello che si trovano all'interno di un locale. Il valore del parametro dei commenti è il nome del locale ottenuto.
___
## File di esempio

![Element.SetParameterByName](./Revit.Elements.Element.SetParameterByName_img.jpg)
