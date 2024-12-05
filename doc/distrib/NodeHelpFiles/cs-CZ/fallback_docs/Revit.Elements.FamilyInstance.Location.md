## Podrobnosti
Uzel `FamilyInstance.Location` vrací bod aplikace Dynamo pro danou instanci rodiny. Pokud není k dispozici žádné umístění, vrátí se hodnota null. Uzel `FamilyInstance.Location` funguje na prvku založeném na bodu a nevrátí umístění prvku založeného na křivce v aplikaci Revit, _například prvek stěny nebo nosníku_.

V následujícím příkladu jsou shromážděny všechny instance rodiny dveří v aktuálním pohledu aktuálního dokumentu. Poté jsou vrácena umístění dveří pomocí uzlu `FamilyInstance.Location`.

V případě tohoto příkladu se u dveří obvodového pláště vrací hodnota null. Umístění panelů obvodového pláště jsou dostupná prostřednictvím uzlů panelů obvodového pláště.
___
## Vzorový soubor

![FamilyInstance.Location](./Revit.Elements.FamilyInstance.Location_img.jpg)
