#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;

[ScriptedImporter(1, "ft")]
public class ForthImporter : ScriptedImporter
{
	public override void OnImportAsset(AssetImportContext ctx)
	{
		var text = File.ReadAllText(ctx.assetPath);
		TextAsset textAsset = new TextAsset(text);
		ctx.AddObjectToAsset("Main", textAsset);
		ctx.SetMainObject(textAsset);
	}
}
[ScriptedImporter(1, "tft")]
public class TDFForthImporter : ScriptedImporter
{
	public override void OnImportAsset(AssetImportContext ctx)
	{
		var text = File.ReadAllText(ctx.assetPath);
		TextAsset textAsset = new TextAsset(text);
		ctx.AddObjectToAsset("Main", textAsset);
		ctx.SetMainObject(textAsset);
	}
}
#endif