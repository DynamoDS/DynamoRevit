## Em profundidade
`Element.Solids` retorna a geometria sólida do elemento fornecido. Os sólidos são retornados como listas aninhadas, pois qualquer elemento pode ter mais de um sólido dentro dele. Se um único sólido que representa o elemento for desejado, `Solid.ByUnion` poderá ser usado nas listas.

No exemplo abaixo, são coletadas todas as paredes da vista selecionada. As paredes que foram criadas como famílias no local são removidas e os sólidos das paredes restantes são retornados.

___
## Arquivo de exemplo

![Element.Solids](./Revit.Elements.Element.Solids_img.jpg)
