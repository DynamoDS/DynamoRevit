## In profondità
`Rooms By Status` restituisce tutti i locali nel documento raggruppati per stato.

Lo stato include i locali che sono:
- Posizionati e delimitati correttamente
- Non posizionati (precedentemente eliminati da una vista, ma non dal modello)
- Non racchiusi (locali non delimitati senza area calcolata)
- Locali ridondanti (locali che condividono un vano chiuso con altri locali)

Nell'esempio seguente, i locali posizionati vengono sottoposti a query per la relativa area.
___
## File di esempio

![Rooms By Status](./DSRevitNodesUI.RoomsByStatus_img.jpg)
