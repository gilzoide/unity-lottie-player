using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Gilzoide.LottiePlayer.Editor
{
    [ScriptedImporter(0, null, overrideExts: new[] { "json" })]
    public class LottieAnimationAssetImporter : ScriptedImporter
    {
        [SerializeField] private string _resourcePath = "";

        public override void OnImportAsset(AssetImportContext ctx)
        {
            LottieAnimationAsset animation = ScriptableObject.CreateInstance<LottieAnimationAsset>();
            animation.Bytes = File.ReadAllBytes(ctx.assetPath);
            animation.CacheKey = AssetDatabase.AssetPathToGUID(ctx.assetPath);
            animation.ResourcePath = _resourcePath;
            ctx.AddObjectToAsset("main", animation);
            ctx.SetMainObject(animation);
        }
    }
}
