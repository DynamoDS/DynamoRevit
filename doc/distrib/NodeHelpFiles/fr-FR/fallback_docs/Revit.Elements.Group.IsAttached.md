## Description approfondie
Indique si un élément de groupe donné de votre projet Revit est associé à un groupe parent.  En termes plus simples, il vérifie si un groupe fait partie d’un groupe imbriqué plus grand. Ce nœud est utile lorsque vous avez besoin d’identifier et de gérer des groupes organisés au sein d’autres groupes. Par exemple, vous pouvez l’utiliser pour :
• Filtrer les groupes : isole les groupes qui ne font pas partie d’autres groupes
• Gérer les groupes imbriqués : comprendre la structure des groupes imbriqués et les traiter en conséquence.
• Modification de l’élément de contrôle : éviter d’apporter des modifications directes aux éléments d’un groupe rattaché à un groupe parent, car cela pourrait perturber la structure du groupe parent.
• Automatiser les tâches : gérer et manipuler dynamiquement les groupes selon qu’ils sont associés ou non.
Essentiellement, le nœud Group.IsAttached vous aide à comprendre la relation entre les groupes dans votre modèle Revit et à créer des processus Dynamo qui tiennent compte de cette hiérarchie.

Dans l’exemple ci-dessous, tous les groupes de modèles sont collectés à partir du document Revit actif en tant qu’entrée.  Les résultats sont Vrai ou Faux.  Le résultat Vrai indique que le groupe dispose d'éléments associés (imbriqués).  Le résultat Faux indique que le groupe n’a PAS d'élément associé (imbriqué).

___
## Exemple de fichier

![Group.IsAttached](./Revit.Elements.Group.IsAttached_img.jpg)
