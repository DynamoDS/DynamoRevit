## Podrobnosti

Vrací typ jednotky parametru.

V následujícím příkladu extrahujeme všechny parametry prvku a použijeme je jako vstup. Cílem je zobrazit typ jednotky pro každý parametr.
Pokud například uzel Element.Parameters zobrazuje „Průměr základny: 1' – 3 1/4“, výstup z parametru Parameter.Unit bude „Jednotka (název = stopy nebo metry)“.
Pokud parametr nemá jednotku (například Element.Parameters = Použít řez : Ne), uzel Parameter.Unit vrátí hodnotu null.
Vzhledem k tomu, že aplikace Dynamo sama o sobě je bez jednotek, je pro přesné výpočty nezbytné identifikovat typy příchozích jednotek. Tento uzel aplikace Dynamo pomáhá rozpoznat a zpracovat data jednotek.

___
## Vzorový soubor

![Parameter.Unit](./Revit.Elements.Parameter.Unit_img.jpg)
