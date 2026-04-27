using BepInEx;
using BepInEx.Configuration;
using SpaceCraft;
using HarmonyLib;
using Unity;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;
using System.IO;
using System.Linq;
//using UnityEngine.UIElements;
using UnityEngine.TextCore.Text;
using static UnityEngine.UIElements.UIR.GradientSettingsAtlas;
using Nicki0;
using System.Collections;
using TMPro;
using System.Threading;

namespace Rocks
{
    [BepInPlugin("Tjatja.theplanetcraftermods.Rocks", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<bool> modEnabled;
        static ConfigEntry<bool> Asset0;
        static ConfigEntry<bool> Asset1;
        static ConfigEntry<bool> Asset2;
        static ConfigEntry<bool> Asset3;
        static ConfigEntry<bool> Asset4;
        static ConfigEntry<bool> Asset5;
        static ConfigEntry<bool> Asset6;
        static ConfigEntry<bool> Asset7;
        static ConfigEntry<String> specialthanks;
        static ManualLogSource logger;
        static string currentLanguage;
        static AssetBundle bundle;
        static ActionOpenable GO;

        private void Awake()
        {
            if (ModVersionCheck.ModVersionCheck.Check(this, Logger.LogInfo, out bool hashError, out string repoURL))
            {
                ModVersionCheck.ModVersionCheck.NotifyUser(this, hashError, repoURL, Logger.LogInfo);
            }
            modEnabled = Config.Bind("General", "Enabled", true, "Is the mod enabled?");
            Asset0 = Config.Bind("General", "Rock1", true, "Add Rock1?");
            Asset1 = Config.Bind("General", "Rock2", true, "Add Rock2?");
            Asset2 = Config.Bind("General", "Rock3", true, "Add Rock3?");
            Asset3 = Config.Bind("General", "Rock4", true, "Add Rock4?");
            Asset4 = Config.Bind("General", "Rock5", true, "Add Rock5?");
            Asset5 = Config.Bind("General", "Rock6", true, "Add Rock6?");
            Asset6 = Config.Bind("General", "Rock7", true, "Add Rock7?");
            Asset7 = Config.Bind("General", "Rock8", true, "Add Rock8?");
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            MaterialsHelper.InitMaterialsHelper(Logger);
            if (modEnabled.Value)
            {
                Harmony.CreateAndPatchAll(typeof(Plugin));
            }
        }
        
        

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Localization), nameof(Localization.SetLangage))]
        static void Localization_SetLanguage(string langage)
        {
            currentLanguage = langage;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Localization), "LoadLocalization")]
        static void Localization_LoadLocalization(Dictionary<string, Dictionary<string, string>> ___localizationDictionary)
        {
            if (___localizationDictionary.TryGetValue("english", out var dict))
            {
                dict["GROUP_NAME_Rock1"] = "Rock 1";
                dict["GROUP_NAME_Rock2"] = "Rock 2";
                dict["GROUP_NAME_Rock3"] = "Rock 3";
                dict["GROUP_NAME_Rock4"] = "Rock 4";
                dict["GROUP_NAME_Rock5"] = "Rock 5";
                dict["GROUP_NAME_Rock6"] = "Rock 6";
                dict["GROUP_NAME_Rock7"] = "Rock 7";
                dict["GROUP_NAME_Rock8"] = "Rock 8";
            }
            if (___localizationDictionary.TryGetValue("french", out dict))
            {
                dict["GROUP_NAME_Rock1"] = "Rocher 1";
                dict["GROUP_NAME_Rock2"] = "Rocher 2";
                dict["GROUP_NAME_Rock3"] = "Rocher 3";
                dict["GROUP_NAME_Rock4"] = "Rocher 4";
                dict["GROUP_NAME_Rock5"] = "Rocher 5";
                dict["GROUP_NAME_Rock6"] = "Rocher 6";
                dict["GROUP_NAME_Rock7"] = "Rocher 7";
                dict["GROUP_NAME_Rock8"] = "Rocher 8";
            }
            if (___localizationDictionary.TryGetValue("russian", out dict))
            {
                dict["GROUP_NAME_Rock1"] = "камень 1";
                dict["GROUP_NAME_Rock2"] = "камень 2";
                dict["GROUP_NAME_Rock3"] = "камень 3";
                dict["GROUP_NAME_Rock4"] = "камень 4";
                dict["GROUP_NAME_Rock5"] = "камень 5";
                dict["GROUP_NAME_Rock6"] = "камень 6";
                dict["GROUP_NAME_Rock7"] = "камень 7";
                dict["GROUP_NAME_Rock8"] = "камень 8";
            }
            if (___localizationDictionary.TryGetValue("schinese", out dict))
            {
                dict["GROUP_NAME_Rock1"] = "岩石 1";
                dict["GROUP_NAME_Rock2"] = "岩石 2";
                dict["GROUP_NAME_Rock3"] = "岩石 3";
                dict["GROUP_NAME_Rock4"] = "岩石 4";
                dict["GROUP_NAME_Rock5"] = "岩石 5";
                dict["GROUP_NAME_Rock6"] = "岩石 6";
                dict["GROUP_NAME_Rock7"] = "岩石 7";
                dict["GROUP_NAME_Rock8"] = "岩石 8";
            }
            if (___localizationDictionary.TryGetValue("tchinese", out dict))
            {
                dict["GROUP_NAME_Rock1"] = "岩石 1";
                dict["GROUP_NAME_Rock2"] = "岩石 2";
                dict["GROUP_NAME_Rock3"] = "岩石 3";
                dict["GROUP_NAME_Rock4"] = "岩石 4";
                dict["GROUP_NAME_Rock5"] = "岩石 5";
                dict["GROUP_NAME_Rock6"] = "岩石 6";
                dict["GROUP_NAME_Rock7"] = "岩石 7";
                dict["GROUP_NAME_Rock8"] = "岩石 8";
            }
            if (___localizationDictionary.TryGetValue("german", out dict))
            {
                dict["GROUP_NAME_Rock1"] = "Felsen 1";
                dict["GROUP_NAME_Rock2"] = "Felsen 2";
                dict["GROUP_NAME_Rock3"] = "Felsen 3";
                dict["GROUP_NAME_Rock4"] = "Felsen 4";
                dict["GROUP_NAME_Rock5"] = "Felsen 5";
                dict["GROUP_NAME_Rock6"] = "Felsen 6";
                dict["GROUP_NAME_Rock7"] = "Felsen 7";
                dict["GROUP_NAME_Rock8"] = "Felsen 8";
            }
            if (___localizationDictionary.TryGetValue("portuguese", out dict))
            {
                dict["GROUP_NAME_Rock1"] = "pedra 1";
                dict["GROUP_NAME_Rock2"] = "pedra 2";
                dict["GROUP_NAME_Rock3"] = "pedra 3";
                dict["GROUP_NAME_Rock4"] = "pedra 4";
                dict["GROUP_NAME_Rock5"] = "pedra 5";
                dict["GROUP_NAME_Rock6"] = "pedra 6";
                dict["GROUP_NAME_Rock7"] = "pedra 7";
                dict["GROUP_NAME_Rock8"] = "pedra 8";
            }
            if (___localizationDictionary.TryGetValue("spanish", out dict))
            {
                dict["GROUP_NAME_Rock1"] = "roca 1";
                dict["GROUP_NAME_Rock2"] = "roca 2";
                dict["GROUP_NAME_Rock3"] = "roca 3";
                dict["GROUP_NAME_Rock4"] = "roca 4";
                dict["GROUP_NAME_Rock5"] = "roca 5";
                dict["GROUP_NAME_Rock6"] = "roca 6";
                dict["GROUP_NAME_Rock7"] = "roca 7";
                dict["GROUP_NAME_Rock8"] = "roca 8";
            }
            if (___localizationDictionary.TryGetValue("koreana", out dict))
            {
                dict["GROUP_NAME_Rock1"] = "바위 1";
                dict["GROUP_NAME_Rock2"] = "바위 2";
                dict["GROUP_NAME_Rock3"] = "바위 3";
                dict["GROUP_NAME_Rock4"] = "바위 4";
                dict["GROUP_NAME_Rock5"] = "바위 5";
                dict["GROUP_NAME_Rock6"] = "바위 6";
                dict["GROUP_NAME_Rock7"] = "바위 7";
                dict["GROUP_NAME_Rock8"] = "바위 8";
            }
            if (___localizationDictionary.TryGetValue("japanese", out dict))
            {
                dict["GROUP_NAME_Rock1"] = "ロック 1";
                dict["GROUP_NAME_Rock2"] = "ロック 2";
                dict["GROUP_NAME_Rock3"] = "ロック 3";
                dict["GROUP_NAME_Rock4"] = "ロック 4";
                dict["GROUP_NAME_Rock5"] = "ロック 5";
                dict["GROUP_NAME_Rock6"] = "ロック 6";
                dict["GROUP_NAME_Rock7"] = "ロック 7";
                dict["GROUP_NAME_Rock8"] = "ロック 8";
            }
            if (___localizationDictionary.TryGetValue("turk", out dict))
            {
                dict["GROUP_NAME_Rock1"] = "kaynak 1";
                dict["GROUP_NAME_Rock2"] = "kaynak 2";
                dict["GROUP_NAME_Rock3"] = "kaynak 3";
                dict["GROUP_NAME_Rock4"] = "kaynak 4";
                dict["GROUP_NAME_Rock5"] = "kaynak 5";
                dict["GROUP_NAME_Rock6"] = "kaynak 6";
                dict["GROUP_NAME_Rock7"] = "kaynak 7";
                dict["GROUP_NAME_Rock8"] = "kaynak 8";
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StaticDataHandler), "LoadStaticData")]
        private static void StaticDataHandler_LoadStaticData2(List<GroupData> ___groupsData)
        {
            if (___groupsData.Select(gd => gd.id).Where(id => id == "Rock1").Any()) return;

            if (bundle == null)
            {
                string filepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/rocks";
                if (!File.Exists(filepath))
                {
                    return;
                }
                bundle ??= AssetBundle.LoadFromFile(filepath);
            }
            if (bundle == null)
            {
                return;
            }
            // networking code
            NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = false;
            Action<object, object>? SetGlobalObjectIdHash = null;
            var globalHashField = typeof(NetworkObject).GetField("GlobalObjectIdHash", BindingFlags.Instance | BindingFlags.NonPublic);
            if (globalHashField is not null) SetGlobalObjectIdHash = globalHashField.SetValue;

            //networking code
            if (Asset0.Value)
            {
                GroupDataConstructible cubeGDI1 = bundle.LoadAsset<GroupDataConstructible>("assets/Rock1.asset");
                MaterialsHelper.ApplyGameMaterials(cubeGDI1.associatedGameObject, true);
                ___groupsData.Add(cubeGDI1);
                //networking code
                uint newint = 204376303;
                SetGlobalObjectIdHash(cubeGDI1.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDI1.associatedGameObject);
                //networking code
            }
            if (Asset1.Value)
            {
                GroupDataConstructible cubeGDI2 = bundle.LoadAsset<GroupDataConstructible>("assets/Rock2.asset");
                MaterialsHelper.ApplyGameMaterials(cubeGDI2.associatedGameObject, true);
                ___groupsData.Add(cubeGDI2);
                //networking code
                uint newint = 204376304;
                SetGlobalObjectIdHash(cubeGDI2.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDI2.associatedGameObject);
                //networking code
            }
            if (Asset2.Value)
            {
                GroupDataConstructible cubeGDI3 = bundle.LoadAsset<GroupDataConstructible>("assets/Rock3.asset");
                MaterialsHelper.ApplyGameMaterials(cubeGDI3.associatedGameObject, true);
                ___groupsData.Add(cubeGDI3);
                //networking code
                uint newint = 204376305;
                SetGlobalObjectIdHash(cubeGDI3.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDI3.associatedGameObject);
                //networking code
            }
            if (Asset3.Value)
            {
                GroupDataConstructible cubeGDI4 = bundle.LoadAsset<GroupDataConstructible>("assets/Rock4.asset");
                MaterialsHelper.ApplyGameMaterials(cubeGDI4.associatedGameObject, true);
                ___groupsData.Add(cubeGDI4);
                //networking code
                uint newint = 204376306;
                SetGlobalObjectIdHash(cubeGDI4.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDI4.associatedGameObject);
                //networking code
            }
            if (Asset4.Value)
            {
                GroupDataConstructible cubeGDI5 = bundle.LoadAsset<GroupDataConstructible>("assets/Rock5.asset");
                MaterialsHelper.ApplyGameMaterials(cubeGDI5.associatedGameObject, true);
                ___groupsData.Add(cubeGDI5);
                //networking code
                uint newint = 204376307;
                SetGlobalObjectIdHash(cubeGDI5.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDI5.associatedGameObject);
                //networking code
            }
            if (Asset5.Value)
            {
                GroupDataConstructible cubeGDI6 = bundle.LoadAsset<GroupDataConstructible>("assets/Rock6.asset");
                MaterialsHelper.ApplyGameMaterials(cubeGDI6.associatedGameObject, true);
                ___groupsData.Add(cubeGDI6);
                //networking code
                uint newint = 204376308;
                SetGlobalObjectIdHash(cubeGDI6.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDI6.associatedGameObject);
                //networking code
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDI6.associatedGameObject);
            }
            if (Asset6.Value)
            {
                GroupDataConstructible cubeGDI7 = bundle.LoadAsset<GroupDataConstructible>("assets/Rock7.asset");
                MaterialsHelper.ApplyGameMaterials(cubeGDI7.associatedGameObject, true);
                ___groupsData.Add(cubeGDI7);
                //networking code
                uint newint = 204376309;
                SetGlobalObjectIdHash(cubeGDI7.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDI7.associatedGameObject);
                //networking code
            }
            if (Asset7.Value)
            {
                GroupDataConstructible cubeGDI8 = bundle.LoadAsset<GroupDataConstructible>("assets/Rock8.asset");
                MaterialsHelper.ApplyGameMaterials(cubeGDI8.associatedGameObject, true);
                ___groupsData.Add(cubeGDI8);
                //networking code
                uint newint = 204376310;
                SetGlobalObjectIdHash(cubeGDI8.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDI8.associatedGameObject);
                //networking code
            }
            NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = true;
        }
    }
}