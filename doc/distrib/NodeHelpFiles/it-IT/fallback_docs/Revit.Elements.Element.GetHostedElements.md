## In profondità
Dato un elemento che supporta l'hosting di elementi _(ad es. muri)_, `Element.GetHostedElements` restituisce gli elementi che si basano sull'elemento specificato. Per default, vengono restituite le istanze di famiglia che sono ospitate nell'elemento. `Element.GetHostedElements` offre la possibilità di includere altri tipi di elementi ospitati.

Questi includono:
- aperture
- elementi che sono ospitati in host uniti
- muri incorporati _(es. facciate continue)_
- inserti incorporati condivisi

Nell'esempio seguente, vengono raccolti tutti gli elementi muro da L2. Gli elementi muro vengono quindi controllati per verificare la presenza di eventuali elementi ospitati con `Element.GetHostedElements`. Questo elenco viene quindi utilizzato per creare due elenchi: muri con porte e muri senza porte.
___
## File di esempio

![Element.GetHostedElements](./Revit.Elements.Element.GetHostedElements_img.jpg)
