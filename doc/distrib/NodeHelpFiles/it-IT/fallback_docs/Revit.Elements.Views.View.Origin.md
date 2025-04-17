## In profondità
Ogni vista in Revit ha un'origine. `View.Origin` restituisce questo valore come punto di Dynamo. In base alla documentazione dell'API di Revit, "The origin of a plan view is not meaningful". Tenendo presente ciò, il valore offerto da ``View.Origin` dipende dall'utente finale.

Nell'esempio seguente, viene restituita l'origine della vista attiva e della vista 3D selezionata.
___
## File di esempio

![View.Origin](./Revit.Elements.Views.View.Origin_img.jpg)
