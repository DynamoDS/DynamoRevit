## Podrobnosti
Tento uzel označí prvky aplikace Revit s daným pohledem, prvkem, odsazením, horizontalAligment, verticalAlignment, vodorovností (pokud ne, bude popisek orientován podle prvku) a addLeader jako vstupy.

V tomto příkladu jsou dveře vybrány v pohledu „Studio Live Work: Core B“ a použijí se jako vstupy pro uzel Tag.ByelementAndOffset.  Umístění těchto dveří je extrahováno a použito jako počáteční bod vektoru.  Stejný bod je upraven pomocí posuvníku, který změní body x a y, a použije se jako koncový bod vektoru.  Tento vektor se používá jako vstup pro odsazení společně s hodnotami true ve vstupech vodorovnosti a addLeader.  Položka horizontalAlignment je definována rozevíracími hodnotami uzlu vodorovného zarovnání textu výběru (vlevo, na střed, vpravo) a rozevíracími hodnotami uzlu svislého zarovnání textu výběru (dolní, střed, horní).

___
## Vzorový soubor

![Tag.ByElementAndOffset](./Revit.Elements.Tag.ByElementAndOffset_img.jpg)
