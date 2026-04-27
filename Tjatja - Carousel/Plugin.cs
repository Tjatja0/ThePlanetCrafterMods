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

namespace Carousel
{
    [BepInPlugin("Tjatja.theplanetcraftermods.Carousel", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<bool> modEnabled;
        static ConfigEntry<bool> Asset0;
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
            Asset0 = Config.Bind("General", "Carousel", true, "Add Carousel?");
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
                dict["GROUP_NAME_Carousel"] = "Carousel";
            }
            if (___localizationDictionary.TryGetValue("french", out dict))
            {
                dict["GROUP_NAME_Carousel"] = "GROUP_NAME_Carousel";
            }
            if (___localizationDictionary.TryGetValue("russian", out dict))
            {
                dict["GROUP_NAME_Carousel"] = "карусель";
            }
            if (___localizationDictionary.TryGetValue("schinese", out dict))
            {
                dict["GROUP_NAME_Carousel"] = "旋转木马";
            }
            if (___localizationDictionary.TryGetValue("tchinese", out dict))
            {
                dict["GROUP_NAME_Carousel"] = "旋轉木馬";
            }
            if (___localizationDictionary.TryGetValue("german", out dict))
            {
                dict["GROUP_NAME_Carousel"] = "Karussell";
            }
            if (___localizationDictionary.TryGetValue("portuguese", out dict))
            {
                dict["GROUP_NAME_Carousel"] = "carrossel";
            }
            if (___localizationDictionary.TryGetValue("spanish", out dict))
            {
                dict["GROUP_NAME_Carousel"] = "carrusel";
            }
            if (___localizationDictionary.TryGetValue("koreana", out dict))
            {
                dict["GROUP_NAME_Carousel"] = "회전목마";
            }
            if (___localizationDictionary.TryGetValue("japanese", out dict))
            {
                dict["GROUP_NAME_Carousel"] = "カルーセル";
            }
            if (___localizationDictionary.TryGetValue("turk", out dict))
            {
                dict["GROUP_NAME_Carousel"] = "atlıkarınca";
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(InventoryShowContent), "OnInventoryModified")]
        static void ISC_OIM(InventoryShowContent __instance, Inventory ____inventory)
        {
            if (WorldObjectsHandler.Instance.GetWorldObjectForInventory(____inventory)?.GetGroup().GetId() != "Carousel") { return; }

            var woList = ____inventory.GetInsideWorldObjects();
            //for (int i = 0; i < Math.Min(__instance.objectsContainer.Count, woList.Count); i++)
            int count = 0;
            foreach (var a in woList)
            {
                count++;
            }
            for (int i = 0; i < 4; i++)
            {
                float posy = 1.2f;
                float posz = 0.0f;
                float rot = 0;
                if (i >=count)
                {
                    Transform emptyTransform = __instance.objectsContainer[i].transform.parent.Find("Sit");
                    emptyTransform.localPosition = new Vector3(emptyTransform.localPosition.x, posy, posz);
                    continue;
                }
                switch (woList[i].GetGroup().GetId())
                {
                        case "AnimalEffigie1":
                            posy = 2.7f;
                            break;
                        case "AnimalEffigie2":
                            posy = 2.2f;
                            break;
                        case "AnimalEffigie3":
                            posy = 2.7f;
                            break;
                        case "AnimalEffigie4":
                            posy = 3.0f;
                            break;
                        case "AnimalEffigie5":
                            posy = 3.7f;
                            break;
                        case "AnimalEffigie6":
                            posy = 3.4f;
                            posz = -0.2f;
                        break;
                        case "AnimalEffigie7":
                            posy = 3.2f;
                            break;
                        case "AnimalEffigie8":
                            posy = 3.2f;
                            rot = 90f;
                            break;
                        case "AnimalEffigie9":
                            posy = 3.2f;
                            //rot = 180f;
                            posz = 0.5f;
                            break;
                        default:
                            continue;
                }
                Transform sitTransform = __instance.objectsContainer[i].transform.parent.Find("Sit");
                sitTransform.localPosition = new Vector3(sitTransform.localPosition.x, posy, posz);

                Transform containerTransform = __instance.objectsContainer[i].transform;
                Transform effigieTransform = containerTransform.GetChild(containerTransform.childCount - 1);
                effigieTransform.localRotation = Quaternion.Euler(0, rot, 0);

                Destroy(effigieTransform.Find("Socle").gameObject);
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StaticDataHandler), "LoadStaticData")]
        private static void StaticDataHandler_LoadStaticData2(List<GroupData> ___groupsData)
        {
            if (___groupsData.Select(gd => gd.id).Where(id => id == "Carousel").Any()) return;

            if (bundle == null)
            {
                string filepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Carousel";
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
            string modScriptAssemblyPath1 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "InventoryAuthorizedItems.dll");
            if (File.Exists(modScriptAssemblyPath1))
            {
                Assembly.LoadFrom(modScriptAssemblyPath1);
            }
            // networking code
            NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = false;
            Action<object, object>? SetGlobalObjectIdHash = null;
            var globalHashField = typeof(NetworkObject).GetField("GlobalObjectIdHash", BindingFlags.Instance | BindingFlags.NonPublic);
            if (globalHashField is not null) SetGlobalObjectIdHash = globalHashField.SetValue;
            //networking code
            if (Asset0.Value)
            {
                GroupDataConstructible cubeGDC = bundle.LoadAsset<GroupDataConstructible>("assets/Carousel.asset");
                MaterialsHelper.ApplyGameMaterials(cubeGDC.associatedGameObject, true);
                ___groupsData.Add(cubeGDC);
                //networking code
                uint newint = 204376328;
                SetGlobalObjectIdHash(cubeGDC.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDC.associatedGameObject);
                //networking code
            }
        }
    }
}