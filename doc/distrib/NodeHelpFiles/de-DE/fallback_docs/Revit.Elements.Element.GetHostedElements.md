## Im Detail
Bei Angabe eines Elements, das Basisbauteile für Elemente _(z. B. Wände)_ unterstützt, gibt `Element.GetHostedElements` die Elemente zurück, die von dem angegebenen Element basisbauteilabhängig sind. Vorgabemäßig werden Familienexemplare zurückgegeben, die vom Element abhängig sind. `Element.GetHostedElements` bietet die Option, andere Typen basisbauteilabhängiger Elemente einzuschließen.

Dazu gehören:
- Öffnungen
- Elemente, die von verbundenen Basisbauteilen abhängig sind
- umschlossene Wände _(d. h. Fassaden)_
- gemeinsame genutzte eingefügte Elemente

Im folgenden Beispiel werden alle Wandelemente aus L2 gesammelt. Die Wandelemente werden dann mit `Element.GetHostedElements` auf basisbauteilabhängige Elemente überprüft. Diese Liste wird dann zum Erstellen von zwei Listen verwendet: Wände mit Türen und Wände ohne Türen.
___
## Beispieldatei

![Element.GetHostedElements](./Revit.Elements.Element.GetHostedElements_img.jpg)
