using BepInEx;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace LongstandingSolitudeFix
{
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "royal";
        public const string PluginName = "LongstandingSolitudeFix";
        public const string PluginVersion = "1.0.0";
        public void Awake()
        {
            Log.Init(Logger);
            CustomItems.Init();
            Hooks.Init();
        }
    }
}
