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

namespace DecorBundle
{
    [BepInPlugin("Tjatja.theplanetcraftermods.DecorBundle", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
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
        static ConfigEntry<bool> Asset8;
        static ConfigEntry<bool> Asset9;
        static ConfigEntry<String> specialthanks;
        static ManualLogSource logger;
        static string currentLanguage;
        static AssetBundle bundle;

        private void Awake()
        {
            if (ModVersionCheck.ModVersionCheck.Check(this, Logger.LogInfo, out bool hashError, out string repoURL))
            {
                ModVersionCheck.ModVersionCheck.NotifyUser(this, hashError, repoURL, Logger.LogInfo);
            }
            modEnabled = Config.Bind("General", "Enabled", true, "Is the mod enabled?");
            Asset0 = Config.Bind("General", "RockingChair", true, "Add Rocking Chair?");
            Asset1 = Config.Bind("General", "WallContainer", true, "Add Wall Mounted Container?");
            Asset2 = Config.Bind("General", "Toilet", true, "Add Toilet?");
            Asset3 = Config.Bind("General", "Shower", true, "Add Shower?");
            Asset4 = Config.Bind("General", "Carpet", true, "Add Carpet?");
            Asset5 = Config.Bind("General", "Curtain", true, "Add Curtain?");
            Asset6 = Config.Bind("General", "EquipmentRack", true, "Add Equipment Rack?");
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
                dict["GROUP_NAME_RockingChair"] = "Rocking Chair";
                dict["GROUP_NAME_ContainerRack"] = "Wall Mounted Container";
                dict["GROUP_NAME_Toilet"] = "Toilet";
                dict["GROUP_NAME_Shower"] = "Shower";
                dict["GROUP_NAME_Rug"] = "Carpet";
                dict["GROUP_NAME_Curtain"] = "Curtain";
                dict["GROUP_NAME_EquipmentRack"] = "Equipment Rack";
            }
            if (___localizationDictionary.TryGetValue("french", out dict))
            {
                dict["GROUP_NAME_RockingChair"] = "Fauteuil à bascule";
                dict["GROUP_NAME_ContainerRack"] = "Conteneur mural";
                dict["GROUP_NAME_Toilet"] = "Toilettes";
                dict["GROUP_NAME_Shower"] = "Douche";
                dict["GROUP_NAME_Rug"] = "Tapis";
                dict["GROUP_NAME_Curtain"] = "Rideau";
                dict["GROUP_NAME_EquipmentRack"] = "Râtelier à matériel";
            }
            if (___localizationDictionary.TryGetValue("russian", out dict))
            {
                dict["GROUP_NAME_RockingChair"] = "Кресло-качалка";
                dict["GROUP_NAME_ContainerRack"] = "Настенный контейнер";
                dict["GROUP_NAME_Toilet"] = "Туалет";
                dict["GROUP_NAME_Shower"] = "Душ";
                dict["GROUP_NAME_Rug"] = "Ковёр";
                dict["GROUP_NAME_Curtain"] = "Штора";
                dict["GROUP_NAME_EquipmentRack"] = "Стеллаж для оборудования";
            }
            if (___localizationDictionary.TryGetValue("schinese", out dict))
            {
                dict["GROUP_NAME_RockingChair"] = "摇椅";
                dict["GROUP_NAME_ContainerRack"] = "壁挂式储物架";
                dict["GROUP_NAME_Toilet"] = "马桶";
                dict["GROUP_NAME_Shower"] = "淋浴间";
                dict["GROUP_NAME_Rug"] = "地毯";
                dict["GROUP_NAME_Curtain"] = "窗帘";
                dict["GROUP_NAME_EquipmentRack"] = "设备架";
            }
            if (___localizationDictionary.TryGetValue("tchinese", out dict))
            {
                dict["GROUP_NAME_RockingChair"] = "搖椅";
                dict["GROUP_NAME_ContainerRack"] = "壁掛式收納架";
                dict["GROUP_NAME_Toilet"] = "馬桶";
                dict["GROUP_NAME_Shower"] = "淋浴間";
                dict["GROUP_NAME_Rug"] = "地毯";
                dict["GROUP_NAME_Curtain"] = "窗簾";
                dict["GROUP_NAME_EquipmentRack"] = "設備架";
            }
            if (___localizationDictionary.TryGetValue("german", out dict))
            {
                dict["GROUP_NAME_RockingChair"] = "Schaukelstuhl";
                dict["GROUP_NAME_ContainerRack"] = "Wandregal";
                dict["GROUP_NAME_Toilet"] = "Toilette";
                dict["GROUP_NAME_Shower"] = "Dusche";
                dict["GROUP_NAME_Rug"] = "Teppich";
                dict["GROUP_NAME_Curtain"] = "Gardine";
                dict["GROUP_NAME_EquipmentRack"] = "Geräteregal";
            }
            if (___localizationDictionary.TryGetValue("portuguese", out dict))
            {
                dict["GROUP_NAME_RockingChair"] = "Cadeira de Balanço";
                dict["GROUP_NAME_ContainerRack"] = "Suporte de Parede para Recipientes";
                dict["GROUP_NAME_Toilet"] = "Vaso Sanitário";
                dict["GROUP_NAME_Shower"] = "Chuveiro";
                dict["GROUP_NAME_Rug"] = "Tapete";
                dict["GROUP_NAME_Curtain"] = "Cortina";
                dict["GROUP_NAME_EquipmentRack"] = "Suporte para Equipamentos";
            }
            if (___localizationDictionary.TryGetValue("spanish", out dict))
            {
                dict["NOMBRE_GRUPO_Mecedora"] = "Mecedora";
                dict["NOMBRE_GRUPO_Estante para Contenedores"] = "Contenedor de Pared";
                dict["NOMBRE_GRUPO_Inodoro"] = "Inodoro";
                dict["NOMBRE_GRUPO_Ducha"] = "Ducha";
                dict["NOMBRE_GRUPO_Alfombra"] = "Alfombra";
                dict["NOMBRE_GRUPO_Cortina"] = "Cortina";
                dict["NOMBRE_GRUPO_Estante para Equipos"] = "Estante para Equipos";
            }
            if (___localizationDictionary.TryGetValue("koreana", out dict))
            {
                dict["GROUP_NAME_RockingChair"] = "흔들의자";
                dict["GROUP_NAME_ContainerRack"] = "벽걸이형 수납함";
                dict["GROUP_NAME_Toilet"] = "화장실";
                dict["GROUP_NAME_Shower"] = "샤워기";
                dict["GROUP_NAME_Rug"] = "카펫";
                dict["GROUP_NAME_Curtain"] = "커튼";
                dict["GROUP_NAME_EquipmentRack"] = "장비 거치대";
            }
            if (___localizationDictionary.TryGetValue("japanese", out dict))
            {
                dict["GROUP_NAME_RockingChair"] = "ロッキングチェア";
                dict["GROUP_NAME_ContainerRack"] = "壁掛けコンテナ";
                dict["GROUP_NAME_Toilet"] = "トイレ";
                dict["GROUP_NAME_Shower"] = "シャワー";
                dict["GROUP_NAME_Rug"] = "カーペット";
                dict["GROUP_NAME_Curtain"] = "カーテン";
                dict["GROUP_NAME_EquipmentRack"] = "機器ラック";
            }
            if (___localizationDictionary.TryGetValue("turk", out dict))
            {
                dict["GROUP_NAME_RockingChair"] = "Sallanan Sandalye";
                dict["GROUP_NAME_ContainerRack"] = "Duvara Monte Edilen Konteyner";
                dict["GROUP_NAME_Toilet"] = "Tuvalet";
                dict["GROUP_NAME_Shower"] = "Duş";
                dict["GROUP_NAME_Rug"] = "Halı";
                dict["GROUP_NAME_Curtain"] = "Perde";
                dict["GROUP_NAME_EquipmentRack"] = "Ekipman Rafı";
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StaticDataHandler), "LoadStaticData")]
        private static void StaticDataHandler_LoadStaticData2(List<GroupData> ___groupsData)
        {
            //if (___groupsData.Select(gd => gd.id).Where(id => id == "RockingChair").Any()) return;

            if (bundle == null)
            {
                string filepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/usableitems";
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
            string modScriptAssemblyPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "setInterpolationPath.dll");
            if (File.Exists(modScriptAssemblyPath))
            {
                Assembly.LoadFrom(modScriptAssemblyPath);
            }

            // networking code
            NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = false;
            Action<object, object>? SetGlobalObjectIdHash = null;
            var globalHashField = typeof(NetworkObject).GetField("GlobalObjectIdHash", BindingFlags.Instance | BindingFlags.NonPublic);
            if (globalHashField is not null) SetGlobalObjectIdHash = globalHashField.SetValue;

            //networking code
            if (Asset0.Value)
            {
                GroupDataConstructible cubeGDC = bundle.LoadAsset<GroupDataConstructible>("assets/RockingChair.asset");
                if (___groupsData.Contains(cubeGDC))
                {
                    return;
                }
                MaterialsHelper.ApplyGameMaterials(cubeGDC.associatedGameObject, true);
                cubeGDC.associatedGameObject.transform.Find("Elevator").gameObject.AddComponent<Set_WorldUniqueId>();
                //cubeGDC.associatedGameObject.AddComponent<Set_WorldUniqueId>();
                ___groupsData.Add(cubeGDC);
                //networking code
                uint newint = 204376321;
                SetGlobalObjectIdHash(cubeGDC.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDC.associatedGameObject);
                //networking code
            }
            if (Asset1.Value)
            {
                GroupDataConstructible cubeGDC1 = bundle.LoadAsset<GroupDataConstructible>("assets/ContainerRack.asset");
                if (___groupsData.Contains(cubeGDC1))
                {
                    return;
                }
                MaterialsHelper.ApplyGameMaterials(cubeGDC1.associatedGameObject, true);
                cubeGDC1.associatedGameObject.transform.GetComponentInChildren<TextMeshProUGUI>().font = ___groupsData.Find(e => e.id == "Container1").associatedGameObject.GetComponentInChildren<TextMeshProUGUI>().font;
                //cubeGDC.associatedGameObject.AddComponent<Set_WorldUniqueId>();
                ___groupsData.Add(cubeGDC1);
                //networking code
                uint newint = 204376322;
                SetGlobalObjectIdHash(cubeGDC1.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDC1.associatedGameObject);
                //networking code
            }
            if (Asset2.Value)
            {
                GroupDataConstructible cubeGDC2 = bundle.LoadAsset<GroupDataConstructible>("assets/Toilet.asset");
                if (___groupsData.Contains(cubeGDC2))
                {
                    return;
                }
                MaterialsHelper.ApplyGameMaterials(cubeGDC2.associatedGameObject);
                //cubeGDC.associatedGameObject.AddComponent<Set_WorldUniqueId>();
                ___groupsData.Add(cubeGDC2);
                //networking code
                uint newint = 204376323;
                SetGlobalObjectIdHash(cubeGDC2.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDC2.associatedGameObject);
                //networking code
            }
            if (Asset3.Value)
            {
                GroupDataConstructible cubeGDC3 = bundle.LoadAsset<GroupDataConstructible>("assets/Shower.asset");
                if (___groupsData.Contains(cubeGDC3))
                {
                    return;
                }
                MaterialsHelper.ApplyGameMaterials(cubeGDC3.associatedGameObject);
                //cubeGDC.associatedGameObject.AddComponent<Set_WorldUniqueId>();
                ___groupsData.Add(cubeGDC3);
                //networking code
                uint newint = 204376324;
                SetGlobalObjectIdHash(cubeGDC3.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDC3.associatedGameObject);
                //networking code
            }
            if (Asset4.Value)
            {
                GroupDataConstructible cubeGDC4 = bundle.LoadAsset<GroupDataConstructible>("assets/Rug.asset");
                if (___groupsData.Contains(cubeGDC4))
                {
                    return;
                }


                MaterialsHelper.ApplyGameMaterials(cubeGDC4.associatedGameObject, true);
                //cubeGDC.associatedGameObject.AddComponent<Set_WorldUniqueId>();
                ___groupsData.Add(cubeGDC4);
                //networking code
                uint newint = 204376325;
                SetGlobalObjectIdHash(cubeGDC4.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDC4.associatedGameObject);
                //networking code
            }
            if (Asset5.Value)
            {
                GroupDataConstructible cubeGDC5 = bundle.LoadAsset<GroupDataConstructible>("assets/Curtain.asset");
                if (___groupsData.Contains(cubeGDC5))
                {
                    return;
                }


                MaterialsHelper.ApplyGameMaterials(cubeGDC5.associatedGameObject, true);
                //cubeGDC.associatedGameObject.AddComponent<Set_WorldUniqueId>();
                ___groupsData.Add(cubeGDC5);
                //networking code
                uint newint = 204376326;
                SetGlobalObjectIdHash(cubeGDC5.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDC5.associatedGameObject);
                //networking code
            }
            if (Asset6.Value)
            {
                GroupDataConstructible cubeGDC6 = bundle.LoadAsset<GroupDataConstructible>("assets/EquipmentRack.asset");
                if (___groupsData.Contains(cubeGDC6))
                {
                    return;
                }


                MaterialsHelper.ApplyGameMaterials(cubeGDC6.associatedGameObject, true);
                //cubeGDC.associatedGameObject.AddComponent<Set_WorldUniqueId>();
                ___groupsData.Add(cubeGDC6);
                //networking code
                uint newint = 204376327;
                SetGlobalObjectIdHash(cubeGDC6.associatedGameObject.GetComponent<NetworkObject>(), newint);
                NetworkManager.Singleton.AddNetworkPrefab(cubeGDC6.associatedGameObject);
                //networking code
            }
            NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = true;
        }
    }
    // rocking chair on action:
    // GetComponentInParent<OwnershipSwitcher>().ResetOwnership();

    class Set_WorldUniqueId : MonoBehaviour
    {
        public void Start()
        {
            this.StartCoroutine(ExecuteLater(delegate () {
                if (this.GetComponentInParent<ConstructibleGhost>() != null) return;
                // Get WO Id
                this.GetComponentInParent<WorldObjectAssociatedProxy>().GetWorldObjectDetails(delegate (WorldObject wo) {
                    WorldUniqueId wuid = this.gameObject.GetComponent<WorldUniqueId>();
                    setInterpolationPath si = this.gameObject.GetComponent<setInterpolationPath>();
                    // Set WO Id on WorldUniqueId script
                    wuid.ChangeWorldObjectIdLive(wo.GetId());
                    ref int woid_SceneInterpolation = ref AccessTools.FieldRefAccess<setInterpolationPath, int>(si, "_woId");
                    // unregister old woId of SceneInterpolation
                    SceneInterpolationHandler.Instance.UnRegisterInterpolator(woid_SceneInterpolation);
                    woid_SceneInterpolation = wo.GetId();
                    // register new woId of SceneInterpolation (see: SceneInterpolation.Start)
                    SceneInterpolationHandler.Instance.RegisterInterpolator(
                        woid_SceneInterpolation,
                        AccessTools.FieldRefAccess<setInterpolationPath, float>(si, "_normalizedSpeed"),
                        new Action<float, short>(/*this.UpdateInterpolation*/delegate (float value, short direction) {
                            AccessTools.Method(typeof(setInterpolationPath), "UpdateInterpolation").Invoke(si, [value, direction]);
                        }));
                });
            }));
        }
        private static IEnumerator ExecuteLater(Action toExecute, int waitFrames = 1)
        {
            for (int i = 0; i < waitFrames; i++) yield return new WaitForEndOfFrame();
            toExecute.Invoke();
        }
    }
}