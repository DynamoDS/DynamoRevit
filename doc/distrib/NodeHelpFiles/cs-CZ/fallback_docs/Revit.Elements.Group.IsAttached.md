## Podrobnosti
Vrátí hodnotu, která určuje, zda je daný prvek skupiny v projektu aplikace Revit připojen k nadřazené skupině. Zjednodušeně řečeno, kontroluje, zda je skupina součástí větší vnořené skupiny. Tento uzel je užitečný, když potřebujete identifikovat a spravovat skupiny, které jsou uspořádány v jiných skupinách. Můžete jej například použít k následujícím činnostem:
• Filtrování skupin: Izolujte skupiny, které nejsou součástí jiných skupin
• Správa vnořených skupin: Seznamte se se strukturou vnořených skupin a podle toho je zpracujte.
• Řízení úprav prvků: Neprovádějte přímé změny prvků vrámci skupiny, která je připojena k nadřazené skupině, protože by to mohlo narušit strukturu nadřazené skupiny.
• Automatizace úloh: Dynamicky spravujte skupiny a manipulujte snimi na základě toho, zda jsou připojeny, či nikoli.
Uzel Group.IsAttached vám pomůže pochopit vztah mezi skupinami v modelu Revit a vytvořit pracovní postupy aplikace Dynamo, které tuto hierarchii zohlední.

V následujícím příkladu jsou jako vstup získány všechny modelové skupiny z aktivního dokumentu aplikace Revit. Výstupy jsou True nebo False. Výsledky True znamenají, že skupina má přílohy (vnoření). Výsledky False znamenají, že skupina nemá přílohy (vnoření).

___
## Vzorový soubor

![Group.IsAttached](./Revit.Elements.Group.IsAttached_img.jpg)
