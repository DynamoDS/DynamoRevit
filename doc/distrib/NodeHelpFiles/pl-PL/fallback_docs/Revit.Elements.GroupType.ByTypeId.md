## Informacje szczegółowe
Węzeł GroupType.ByTypeId jest używany głównie w procesach tworzenia parametrów i zarządzania nimi, w których grupa parametrów programu Revit musi być określona programowo. Dane wejściowe to ciąg reprezentujący identyfikator typu grupy parametrów programu Revit. Dane wyjściowe to obiekt GroupType, którego można używać dalej w węzłach wymagających klasyfikacji grupowania parametrów.

W poniższym przykładzie utworzono kilka ciągów reprezentujących identyfikatory typów grup parametrów programu Revit i połączono je w listę. Lista jest następnie używana jako dane wejściowe dla węzła GroupType.ByTypeId. Wynikiem jest lista obiektów GroupType programu Revit reprezentujących odpowiednie grupy parametrów.
___
## Plik przykładowy

![GroupType.ByTypeId](./Revit.Elements.GroupType.ByTypeId_img.jpg)
