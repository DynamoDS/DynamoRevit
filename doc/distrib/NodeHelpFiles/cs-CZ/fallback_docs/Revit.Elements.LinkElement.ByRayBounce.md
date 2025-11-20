## Podrobnosti
This node casts a ray into a linked Revit model from a specified origin and direction, then traces its successive bounces off linked elements. Each bounce represents a point where the ray intersects geometry in the linked model, up to a defined maximum number of reflections.

In this example, a linked element is selected and the location of that element is used as the origin input to LinkElement.ByRayBounce along with a direction, maxBounces and a view.  The outputs are points and linked elements.
___
## Vzorový soubor

![LinkElement.ByRayBounce](./Revit.Elements.LinkElement.ByRayBounce_img.jpg)
