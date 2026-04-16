## Im Detail
Dieser Block beschriftet Revit-Elemente anhand einer Ansicht, eines Elements, eines Versatzes, horizontalAligment, verticalAlignment, der horizontal-Angabe (wenn nein, wird die Beschriftung basierend auf dem Element ausgerichtet) und addLeader als Eingaben.

In diesem Beispiel wird eine Tür in der Ansicht “Studio Live Work Core B” ausgewählt und als Eingabe für Tag.ByelementAndOffset verwendet. Die Position dieser Tür wird extrahiert und als Startpunkt des Vektors verwendet. Derselbe Punkt wird mit einem Schieberegler für die X- und Y-Punkte geändert und als Endpunkt des Vektors verwendet. Dieser Vektor wird als Eingabe für den Versatz zusammen mit den true-Werten in den horizontal- und addLeader-Eingaben verwendet. horizontalAlignment wird durch die Dropdown-Werte für den Block Selection Horizontal Text Alignment (Links, Mitte, Rechts) und die Dropdown-Werte für den Block Selection Vertical Text Alignment (Unten, Mitte, Oben) definiert.

___
## Beispieldatei

![Tag.ByElementAndOffset](./Revit.Elements.Tag.ByElementAndOffset_img.jpg)
