## In profondità
Posizionare un elemento FamilyInstance di Revit in un progetto di Revit a partire dall'elemento FamilyType desiderato e dal relativo sistema di coordinate.

In questo esempio, vengono forniti un tipo di famiglia e un punto del sistema di coordinate specifici e viene posizionata un'istanza di famiglia.
Un caso d'uso comune prevede la creazione di un sistema di coordinate basato sul punto base del progetto di Revit e la successiva rotazione per farlo corrispondere alla rotazione della planimetria. È quindi possibile inserire questo sistema di coordinate trasformato nel nodo FamilyInstance.ByCoordinateSystem per posizionare le istanze di famiglia nelle posizioni reali previste, tenendo conto dell'orientamento della planimetria.

Per ulteriori informazioni sui sistemi di coordinate, vedere di seguito.
https://help.autodesk.com/view/RVT/2025/ITA/?guid=GUID-68611F67-ED48-4659-9C3B-59C5024CE5F2
___
## File di esempio

![FamilyInstance.ByCoordinateSystem](./Revit.Elements.FamilyInstance.ByCoordinateSystem_img.jpg)
