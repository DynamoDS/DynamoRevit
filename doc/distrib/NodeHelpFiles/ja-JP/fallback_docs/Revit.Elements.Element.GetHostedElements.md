## 詳細
要素のホストをサポートする要素(壁など)が指定されている場合、`Element.GetHostedElements` は、指定された要素に依存している要素を返します。この要素はます。既定では、要素にホストされているファミリ インスタンスを返します。`Element.GetHostedElements` はホストされている要素のその他のタイプを含むオプションを提供します。

これらには、次が含まれます。
- 開口部
- 結合されたホストにホストされている要素
- 埋め込み壁(カーテン ウォール)
- 共有の埋め込み挿入部品

次の例では、すべての壁要素を L2 から収集します。次に `Element.GetHostedElements` を使用して、壁要素にホストされた要素があるかどうかを確認します。このリストを使用して、ドアのある壁とドアのない壁の 2 つのリストを作成します。
___
## サンプル ファイル

![Element.GetHostedElements](./Revit.Elements.Element.GetHostedElements_img.jpg)
