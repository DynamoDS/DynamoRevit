## 詳細
このノードは、選択された要素を評価して、解析用パネルの生成に適しているかどうかを判断します。オプションの checkOpenings 入力で、要素内の開口部を有効性チェックに含めるかどうかを指定します。true に設定した場合、開口部は評価の一部として考慮されます。

この例では、要素は Element.ById ノードを使用してその要素 ID で定義され、Element.IsValidForAnalyticalPanel に提供されます。出力には、要素が有効かどうかを示すブール値と、その使用を妨げる問題を説明する例外メッセージが含まれます。
___
## サンプル ファイル

![Element.IsValidForAnalyticalPanel](./AnalyticalAutomation.RevitElements.Element.IsValidForAnalyticalPanel_img.jpg)
