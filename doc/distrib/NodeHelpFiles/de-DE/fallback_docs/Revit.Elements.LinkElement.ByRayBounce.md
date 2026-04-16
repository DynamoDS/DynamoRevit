## Im Detail
Dieser Block wirft einen Strahl von einem angegebenen Ursprung und in einer bestimmten Richtung auf ein verknüpftes Revit-Modell und verfolgt dann die aufeinanderfolgenden Reflexionen von verknüpften Elementen. Jede Reflexion stellt einen Punkt dar, an dem der Strahl die Geometrie im verknüpften Modell schneidet, bis zu einer definierten maximalen Anzahl von Reflexionen.

In diesem Beispiel wird ein verknüpftes Element ausgewählt und dessen Position zusammen mit einer Richtung, maxBounces und einer Ansicht als Ursprungseingabe für LinkElement.ByRayBounce verwendet. Die Ausgaben sind Punkte und verknüpfte Elemente.
___
## Beispieldatei

![LinkElement.ByRayBounce](./Revit.Elements.LinkElement.ByRayBounce_img.jpg)
