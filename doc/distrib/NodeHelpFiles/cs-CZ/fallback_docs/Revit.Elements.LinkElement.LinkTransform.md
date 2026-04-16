## Podrobnosti
Tento uzel načte transformační matici použitou pro prvek připojení aplikace Revit v hostitelském modelu.
Jinými slovy, vrací transformaci umístění, otočení a měřítka, která mapuje souřadnicový systém připojeného prvku do souřadnicového systému hostitelského projektu aplikace Revit.
To je užitečné, když potřebujete zarovnat, analyzovat nebo manipulovat s geometrií mezi připojenými modely.

V tomto příkladu jsou vybrány všechny připojené prvky aplikace Revit na podlaží L3 a jsou vloženy do objektu LinkElement.LinkTransform.  Výstupem je transformace pozice, otočení a měřítka připojeného prvku.
___
## Vzorový soubor

![LinkElement.LinkTransform](./Revit.Elements.LinkElement.LinkTransform_img.jpg)
