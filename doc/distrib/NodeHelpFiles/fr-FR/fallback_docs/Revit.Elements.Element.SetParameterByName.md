## Description approfondie
`Element.SetParameterByName` définit un élément de paramètre (trouvé par nom) sur une valeur donnée. Les valeurs sont les suivantes : double, entier, booléen, ID d'élément, Element et chaîne. Dans Revit, les paramètres peuvent partager le même nom. Par conséquent, `Element.SetParameterByName` définira la valeur sur le premier paramètre trouvé, trié par ordre alphabétique selon l'ID unique.

Dans l'exemple ci-dessous, le paramètre Commentaires est défini pour tous les éléments de mobilier du modèle qui se trouvent dans une pièce. La valeur du paramètre Commentaires est le nom de pièce obtenu.
___
## Exemple de fichier

![Element.SetParameterByName](./Revit.Elements.Element.SetParameterByName_img.jpg)
