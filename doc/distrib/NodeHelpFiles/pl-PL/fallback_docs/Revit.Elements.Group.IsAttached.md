## Informacje szczegółowe
Zwraca informację, czy dany element grupy w projekcie programu Revit jest dołączony do grupy nadrzędnej. Mówiąc prościej: sprawdza, czy grupa jest częścią większej grupy zagnieżdżonej. Ten węzeł jest przydatny, gdy trzeba zidentyfikować grupy zorganizowane w innych grupach i zarządzać nimi. Na przykład umożliwia:
• Filtrowanie grup: izolowanie grup, które nie są częścią innych grup
• Zarządzanie grupami zagnieżdżonymi: analizowanie struktury grup zagnieżdżonych i odpowiednie ich przetwarzanie.
• Sterowanie modyfikowaniem elementów: unikanie wprowadzania bezpośrednich zmian w elementach wewnątrz grupy dołączonej do grupy nadrzędnej, ponieważ może to zakłócić strukturę grupy nadrzędnej.
• Automatyzowanie zadań: dynamiczne zarządzanie grupami i manipulowanie nimi w zależności od tego, czy są dołączone, czy nie.
Zasadniczo węzeł Group.IsAttached pomaga analizować relacje między grupami w modelu programu Revit i tworzyć procesy robocze dodatku Dynamo, które uwzględniają tę hierarchię.

W poniższym przykładzie pobierane są wszystkie grupy modelu z aktywnego dokumentu programu Revit jako dane wejściowe. Dane wyjściowe to True (Prawda) lub False (Fałsz). Wyniki True informują, że grupa ma dołączenia (zagnieżdżenia). Wyniki False — że grupa NIE ma dołączeń (zagnieżdżeń).

___
## Plik przykładowy

![Group.IsAttached](./Revit.Elements.Group.IsAttached_img.jpg)
