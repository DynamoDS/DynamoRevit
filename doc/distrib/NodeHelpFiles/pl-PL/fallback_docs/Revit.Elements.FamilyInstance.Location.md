## Informacje szczegółowe
Węzeł `FamilyInstance.Location` zwraca punkt dodatku Dynamo dla danego wystąpienia rodziny. Jeśli nie ma lokalizacji, zwracana jest wartość null. Węzeł `FamilyInstance.Location` działa w przypadku elementu opartego na punkcie i nie zwraca położenia elementu opartego na krzywej w programie Revit, _np. element_ ściany lub belki_.

W poniższym przykładzie pobierane są wszystkie wystąpienia rodzin drzwi w bieżącym widoku bieżącego dokumentu. Położenia drzwi są następnie zwracane za pomocą węzła `FamilyInstance.Location`.

W przypadku tego przykładu drzwi ścian osłonowych powodują zwrócenie wartości null. Położenia paneli ścian osłonowych są dostępne za pośrednictwem węzłów paneli ścian osłonowych.
___
## Plik przykładowy

![FamilyInstance.Location](./Revit.Elements.FamilyInstance.Location_img.jpg)
