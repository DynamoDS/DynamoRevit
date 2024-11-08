## 詳細
`Element.SetParameterByName` は、パラメータ要素(名前で検索)を指定した値に設定します。値には、倍精度浮動小数点数、整数、ブール値、要素 ID、要素、文字列があります。Revit では、パラメータは同じ名前を共有できます。そのため、`Element.SetParameterByName` は、一意の ID でアルファベット順に検索される最初のパラメータの値を設定します。

次の例では、部屋に配置されているモデル内のすべての家具項目に対してコメント パラメータが設定されています。コメント パラメータの値は、取得される部屋の名前です。
___
## サンプル ファイル

![Element.SetParameterByName](./Revit.Elements.Element.SetParameterByName_img.jpg)
