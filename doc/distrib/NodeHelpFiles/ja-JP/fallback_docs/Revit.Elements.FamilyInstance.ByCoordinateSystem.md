## 詳細
目的の FamilyType とその座標系を指定して、Revit FamilyInstance を Revit プロジェクトに配置します。

この例では、特定のファミリ タイプと座標系の点を指定してファミリ インスタンスを配置しています。
一般的な使用例として、Revit のプロジェクト基準点に基づいて座標系を作成し、外構の回転に合わせて回転させるというものがあります。次に、この変換された座標系を FamilyInstance.ByCoordinateSystem ノードに入力し、ファミリ インスタンスを実際の目的の位置に配置して、外構の方向を考慮できます。

座標系の詳細については、次を参照してください。
https://help.autodesk.com/view/RVT/2025/JPN/?guid=GUID-68611F67-ED48-4659-9C3B-59C5024CE5F2
___
## サンプル ファイル

![FamilyInstance.ByCoordinateSystem](./Revit.Elements.FamilyInstance.ByCoordinateSystem_img.jpg)
