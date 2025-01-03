## 詳細
'Room.CoreCenterBoundary' は、指定された部屋の躯体中心境界を表すネストされたリストを返します。返されたリストでは、最初のサブリストは最も外側の曲線を表し、後続のリストは部屋内のループを表します。躯体の中心境界は、躯体として定義されている壁の中心に配置されます。壁の配置基準線の詳細については、こちらのヘルプ記事(https://help.autodesk.com/view/RVT/2024/JPN/?guid=GUID-0BB62832-36DD-4E06-A9D4-EE98CE0FCF89)を参照してください。

境界のない部屋、または配置されていない部屋が指定されている場合は、null 値が返されます。

次の例では、すべての部屋が現在のドキュメントと選択されたビューから収集されます。次に、躯体中心境界が返されます。
___
## サンプル ファイル

![Room.CoreCenterBoundary](./Revit.Elements.Room.CoreCenterBoundary_img.jpg)
