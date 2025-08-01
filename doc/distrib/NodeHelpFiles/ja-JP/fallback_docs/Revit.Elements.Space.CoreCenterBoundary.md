## 詳細
`Space.CoreCenterBoundary` は、指定されたスペースの躯体中心境界を表すネストされたリストを返します。返されたリストでは、最初のサブリストは最も外側の曲線を表し、後続のリストはスペース内のループを表します。躯体中心境界は、躯体として定義されている壁の中心に配置されます。壁の配置基準線の詳細については、こちらのヘルプ[記事](https://help.autodesk.com/view/RVT/2024/JPN/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89)を参照してください。

境界のないスペースまたは未配置のスペースが指定されている場合、null 値が返されます。

次の例では、現在のドキュメントと選択したビューからすべてのスペースが収集されます。その後、躯体中心境界が返されます。

___
## サンプル ファイル

![Space.CoreCenterBoundary](./Revit.Elements.Space.CoreCenterBoundary_img.jpg)
