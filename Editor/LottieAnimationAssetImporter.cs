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
            animation.Json = File.ReadAllText(ctx.assetPath);
            animation.CacheKey = AssetDatabase.AssetPathToGUID(ctx.assetPath);
            animation.ResourcePath = _resourcePath;
            using (var instancedAnimation = animation.CreateAnimation())
            {
                if (instancedAnimation == null)
                {
                    throw new InvalidDataException($"Invalid Lottie JSON data @ {ctx.assetPath}");
                }
            }
            ctx.AddObjectToAsset("main", animation);
            ctx.SetMainObject(animation);
        }
    }
}
