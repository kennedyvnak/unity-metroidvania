using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;
using Metroidvania.Serialization;

namespace MetroidvaniaEditor.Serialization
{
    [ScriptedImporter(1, "save")]
    public class GameDataImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var json = Metroidvania.Serialization.Handlers.DataHandler.EncryptDecrypt(File.ReadAllText(ctx.assetPath));
            var gameDataAsset = ScriptableObject.CreateInstance<GameDataAsset>();
            gameDataAsset.LoadFromJson(json);
            gameDataAsset.name = Path.GetFileName(ctx.assetPath);
            ctx.AddObjectToAsset("main obj", gameDataAsset);
            ctx.SetMainObject(gameDataAsset);
        }
    }
}
