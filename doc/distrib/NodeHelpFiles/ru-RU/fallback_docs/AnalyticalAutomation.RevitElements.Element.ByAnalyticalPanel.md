## Подробности
This node creates a physical wall or floor element from an analytical panel, or updates an existing physical element that is already connected to the analytical panel. The physical element can inherit geometry, parameters, and openings from the analytical panel, depending on the input settings. When the analytical panel has no associated physical element, the node creates a new one based on the analytical model.

In this example, an analytical roof deck panel from the ACO Supermarket model is selected. That analytical panel is connected to this node, and the input Boolean values demonstrate the default behavior for updating geometry, parameters, and including openings. The node then creates or updates the physical roof element from the analytical panel.
___
## Файл примера

![Element.ByAnalyticalPanel](./AnalyticalAutomation.RevitElements.Element.ByAnalyticalPanel_img.jpg)
