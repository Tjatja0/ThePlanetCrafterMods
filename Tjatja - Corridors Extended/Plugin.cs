using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;
using Nicki0;
using SpaceCraft;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using TMPro;
using Unity;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace CorridorsExtended;
public class CornerReplacer : MonoBehaviour
{

    public void Start()
    {
        if (this.transform.root.GetComponentInChildren<ConstructibleGhost>() != null) return;
        if (this.transform.root.name.Contains("Clone")) 
        {
            foreach (SpaceCraft.Panel panel in this.transform.root.GetComponentsInChildren<SpaceCraft.Panel>())
            {
                if (panel.panelType == DataConfig.BuildPanelType.Wall) {
                    if (panel.GetSubPanelType() == (DataConfig.BuildPanelSubType)150_001)
                    {
                        patchPillarsAway(panel, 0);
                    }
                    else if (panel.GetSubPanelType() == (DataConfig.BuildPanelSubType)150_002)
                    {
                        patchPillarsAway(panel, 1);
                    }
                    else if (panel.GetSubPanelType() == (DataConfig.BuildPanelSubType)150_003)
                    {
                        patchPillarsAway(panel, 2);
                    }
                    else
                    {
                        patchPillarsAway(panel, 3);
                    }
                }
            }
        }
    }

    public void Update()
    {
        if (this.transform.root.GetComponentInChildren<ConstructibleGhost>() != null) return;
        if (this.transform.root.name.Contains("Clone"))
        {
            foreach (SpaceCraft.Panel panel in this.transform.root.GetComponentsInChildren<SpaceCraft.Panel>())
            {
                if (panel.panelType == DataConfig.BuildPanelType.Wall)
                {
                    if (panel.GetSubPanelType() == (DataConfig.BuildPanelSubType)150_001)
                    {
                        patchPillarsAway(panel, 0);
                    }
                    else if (panel.GetSubPanelType() == (DataConfig.BuildPanelSubType)150_002)
                    {
                        patchPillarsAway(panel, 1);
                    }
                    else if (panel.GetSubPanelType() == (DataConfig.BuildPanelSubType)150_003)
                    {
                        patchPillarsAway(panel, 2);
                    }
                    else
                    {
                        patchPillarsAway(panel, 3);
                    }
                }
            }
        }
    }

    static void patchPillarsAway(SpaceCraft.Panel __instance, int type)
    {
        if (__instance.transform.root.name == "InteriorStairs1(Clone)" || __instance.transform.root.name == "InteriorStairs1")
        {
            float panelRotationRightPillar = __instance.transform.eulerAngles.y;
            float panelRotationLeftPillar = (panelRotationRightPillar + 90) % 360;
            string wallName1 = "";//lowerfloor
            string wallName2 = "";//topfloor

            switch (__instance.transform.name)
            {
                case "P_Wall_Window_01": //90
                    panelRotationRightPillar = 90;
                    panelRotationLeftPillar = 180;
                    break;
                case "P_Wall_Window_02"://270
                    panelRotationRightPillar = 270;
                    panelRotationLeftPillar = 0;
                    break;
                case "P_Wall_Window_03"://180
                    panelRotationRightPillar = 180;
                    panelRotationLeftPillar = 270;
                    wallName1 = "P_Wall_Half_01 (1)";
                    wallName2 = "P_Wall_Half_01 (3)";
                    break;
                case "P_Wall_Window_04"://0
                    panelRotationRightPillar = 0;
                    panelRotationLeftPillar = 90;
                    wallName1 = "P_Wall_Half_01";
                    wallName2 = "P_Wall_Half_01 (2)";
                    break;
            }


            Transform TA = __instance.transform.parent.parent.Find("Structure");
            foreach (Transform t in TA)
            {
                if (t.name == "Wall_Angle_03")
                {
                    if (t.localEulerAngles.y == panelRotationRightPillar && (type == 1 || type == 0))
                    {
                        t.gameObject.SetActive(false);
                    }
                    if (t.localEulerAngles.y == panelRotationLeftPillar && (type == 2 || type == 0))
                    {
                        t.gameObject.SetActive(false);
                    }
                    if (t.localEulerAngles.y == panelRotationRightPillar && (type == 3))
                    {
                        t.gameObject.SetActive(true);
                    }
                    if (t.localEulerAngles.y == panelRotationLeftPillar && (type == 3))
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }
            Transform TA1 = __instance.transform.parent.parent.parent.Find("Common");
            foreach (Transform t in TA1)
            {
                if (__instance.transform.parent.parent.name == "Pod01" || __instance.transform.parent.parent.name == "Pod02")
                {
                    if (__instance.transform.name == "P_Wall_Window_04" || __instance.transform.name == "P_Wall_Window_03")
                    {
                        if (t.name == wallName1 && (type == 1 || type == 0))
                        {
                            t.gameObject.SetActive(false);
                        }
                        if (t.name == wallName1 && (type == 3))
                        {
                            t.gameObject.SetActive(true);
                        }
                    }
                }
                if (__instance.transform.parent.parent.name == "Pod03" || __instance.transform.parent.parent.name == "Pod04")
                {
                    if (__instance.transform.name == "P_Wall_Window_04" || __instance.transform.name == "P_Wall_Window_03") {
                        if (t.name == wallName2 && (type == 2 || type == 0))
                        {
                            t.gameObject.SetActive(false);
                        }
                        if (t.name == wallName2 && (type == 3))
                        {
                            t.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
        if (__instance.transform.root.name == "Pod(Clone)" || __instance.transform.root.name == "Pod" ||
            __instance.transform.root.name == "PodUnderground(Clone)" || __instance.transform.root.name == "PodUnderground" || 
            __instance.transform.root.name == "SubTerrainianPod(Clone)" || __instance.transform.root.name == "SubTerrainianPod" ||
            __instance.transform.root.name == "SpiralStairCase(Clone)" || __instance.transform.root.name == "SpiralStairCase" ||
            __instance.transform.root.name == "RoofTopFarm(Clone)" || __instance.transform.root.name == "RoofTopFarm" ||
            __instance.transform.root.name == "MiniInteriorStairs(Clone)" || __instance.transform.root.name == "MiniInteriorStairs" ||
            __instance.transform.root.name == "ElevatorPod(Clone)" || __instance.transform.root.name == "ElevatorPod")
        {

            float panelRotationRightPillar = __instance.transform.eulerAngles.y;
            float panelRotationLeftPillar = (panelRotationRightPillar + 90) % 360;

            switch (__instance.transform.name)
            {
                case "P_Wall_Window_01": //90
                    panelRotationRightPillar = 90;
                    panelRotationLeftPillar = 180;
                    break;
                case "P_Wall_Window_02"://270
                    panelRotationRightPillar = 270;
                    panelRotationLeftPillar = 0;
                    break;
                case "P_Wall_Window_03"://180
                    panelRotationRightPillar = 180;
                    panelRotationLeftPillar = 270;
                    break;
                case "P_Wall_Window_04"://0
                    panelRotationRightPillar = 0;
                    panelRotationLeftPillar = 90;
                    break;
            }


            Transform TA2 = __instance.transform.parent.parent.Find("Structure");
            foreach (Transform t in TA2)
            {
                if (t.name == "Wall_Angle_03")
                {
                    if (t.localEulerAngles.y == panelRotationRightPillar && (type == 2 || type == 0))
                    {
                        t.gameObject.SetActive(false);
                    }
                    if (t.localEulerAngles.y == panelRotationLeftPillar && (type == 1 || type == 0))
                    {
                        t.gameObject.SetActive(false);
                    }
                    if (t.localEulerAngles.y == panelRotationLeftPillar && (type == 3))
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }
        }

        if (__instance.transform.root.name == "PodAngle" || __instance.transform.root.name == "PodAngle(Clone)")
        {
            float panelRotationRightPillar = __instance.transform.localEulerAngles.y;
            float panelRotationLeftPillar = (panelRotationRightPillar + 90) % 360;

            switch (__instance.transform.name)
            {
                case "P_Wall_Window_01": //90
                    panelRotationRightPillar = 180;
                    panelRotationLeftPillar = 90;
                    break;
                case "P_Wall_Window_02"://180
                    panelRotationRightPillar = 270;
                    panelRotationLeftPillar = 180;
                    break;
            }


            Transform TA3 = __instance.transform.parent.parent.Find("Structure");
            foreach (Transform t in TA3)
            {
                if (t.name == "Wall_Angle_03")
                {
                    if (t.localEulerAngles.y == panelRotationRightPillar && (type == 1 || type == 0))
                    {
                        t.gameObject.SetActive(false);
                    }
                    if (t.localEulerAngles.y == panelRotationLeftPillar && (type == 2 || type == 0))
                    {
                        t.gameObject.SetActive(false);
                    }
                    if (t.localEulerAngles.y == panelRotationRightPillar && (type == 3))
                    {
                        t.gameObject.SetActive(true);
                    }
                    if (t.localEulerAngles.y == panelRotationLeftPillar && (type == 3))
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }

        }

        if (__instance.transform.root.name == "Pod4x" || __instance.transform.root.name == "Pod4x(Clone)" || __instance.transform.root.name == "Biolab(Clone)" || __instance.transform.root.name == "Biolab")
        {
            string wallName = "";
            switch (__instance.transform.name)
            {
                case "P_Wall_Window_01": //90
                    if (type == 0 || type == 2)
                    {
                        if (__instance.transform.parent.parent.name == "Pod (1)")
                        {
                            wallName = "P_Wall_Half_01 (3)";
                        }
                    }
                    if (type == 0 || type == 1)
                    {
                        if (__instance.transform.parent.parent.name == "Pod (3)")
                        {
                            wallName = "P_Wall_Half_01 (3)";
                        }
                    }
                    if (type == 3)
                    {
                        wallName = "P_Wall_Half_01 (3)";
                    }
                    break;
                case "P_Wall_Window_02"://270
                    if (type == 0 || type == 2)
                    {
                        if (__instance.transform.parent.parent.name == "Pod (2)")
                        {
                            wallName = "P_Wall_Half_01 (2)";
                        }
                    }
                    if (type == 0 || type == 1)
                    {
                        if (__instance.transform.parent.parent.name == "Pod")
                        {
                            wallName = "P_Wall_Half_01 (2)";
                        }
                    }
                    if (type == 3)
                    {
                        wallName = "P_Wall_Half_01 (2)";
                    }
                    break;
                case "P_Wall_Window_03"://180
                    if (type == 0 || type == 2)
                    {
                        if (__instance.transform.parent.parent.name == "Pod")
                        {
                            wallName = "P_Wall_Half_01 (1)";
                        }
                    }
                    if (type == 0 || type == 1)
                    {
                        if (__instance.transform.parent.parent.name == "Pod (1)")
                        {
                            wallName = "P_Wall_Half_01 (1)";
                        }
                    }
                    if (type == 3)
                    {
                        wallName = "P_Wall_Half_01 (1)";
                    }
                    break;
                case "P_Wall_Window_04"://0
                    if (type == 0 || type == 2)
                    {
                        if (__instance.transform.parent.parent.name == "Pod (3)")
                        {
                            wallName = "P_Wall_Half_01";
                        }
                    }
                    if (type == 0 || type == 1)
                    {
                        if (__instance.transform.parent.parent.name == "Pod (2)")
                        {
                            wallName = "P_Wall_Half_01";
                        }
                    }
                    if (type == 3)
                    {
                        wallName = "P_Wall_Half_01";
                    }
                    break;
            }

            Transform TA4 = __instance.transform.parent.parent.Find("Structure");
            foreach (Transform t in TA4)
            {
                if (t.name == "Wall_Angle_03")
                {
                    if (type == 0 || type == 1)
                    {
                        t.gameObject.SetActive(false);
                        //wallName = "P_Wall_Half_01 (3)";
                    }
                    else if (type == 3)
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }

            Transform TA5 = __instance.transform.parent.parent.parent.Find("Common");
            foreach (Transform t in TA5)
            {
                if (wallName != "")
                {
                    if (t.name == wallName)
                    {
                        if (type != 3)
                        {
                            t.gameObject.SetActive(false);
                        }
                        else
                        {
                            t.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }

        if (__instance.transform.root.name == "Pod9xC" || __instance.transform.root.name == "Pod9xC(Clone)")
        {
            string wallName1 = "";
            string wallName2 = "";

            switch (__instance.transform.name)
            {
                case "P_Wall_Window_01": //90
                    wallName1 = "P_Wall_Half_01 (3)";//right
                    wallName2 = "P_Wall_Half_01 (6)";//left
                    break;
                case "P_Wall_Window_02"://270
                    wallName1 = "P_Wall_Half_01 (7)";//right
                    wallName2 = "P_Wall_Half_01 (2)";//left
                    break;
                case "P_Wall_Window_03"://180
                    wallName1 = "P_Wall_Half_01 (5)";//right
                    wallName2 = "P_Wall_Half_01 (1)";//left
                    break;
                case "P_Wall_Window_04"://0
                    wallName1 = "P_Wall_Half_01 (4)";//right
                    wallName2 = "P_Wall_Half_01";//left
                    break;
            }

            Transform TA6 = __instance.transform.parent.parent.Find("Structure");
            foreach (Transform t in TA6)
            {
                if (t.name == "Wall_Angle_03")
                {
                    if (type == 0 || type == 1)
                    {
                        t.gameObject.SetActive(false);
                    }
                    else if (type == 3)
                    {
                        t.gameObject.SetActive(true);
                    }

                }
            }

            switch (__instance.transform.parent.parent.name)
            {
                case "Pod":
                    Transform TA7 = __instance.transform.parent.parent.parent.Find("Common");
                    foreach (Transform t in TA7)
                    {
                        switch (__instance.transform.name)
                        {
                            case "P_Wall_Window_02":
                                if (t.name == wallName1)
                                {
                                    if (type == 0 || type == 1)
                                    {
                                        t.gameObject.SetActive(false);
                                    }
                                    else if (type == 3)
                                    {
                                        t.gameObject.SetActive(true);
                                    }
                                }
                                break;
                            case "P_Wall_Window_03":
                                if (t.name == wallName2)
                                {
                                    if (type == 0 || type == 2)
                                    {
                                        t.gameObject.SetActive(false);
                                    }
                                    else if (type == 3)
                                    {
                                        t.gameObject.SetActive(true);
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case "Pod (1)":
                    Transform TA8 = __instance.transform.parent.parent.parent.Find("Common");
                    foreach (Transform t in TA8)
                    {
                        switch (__instance.transform.name)
                        {
                            case "P_Wall_Window_01":
                                if (t.name == wallName2)
                                {
                                    if (type == 0 || type == 2)
                                    {
                                        t.gameObject.SetActive(false);
                                    }
                                    else if (type == 3)
                                    {
                                        t.gameObject.SetActive(true);
                                    }
                                }
                                break;
                            case "P_Wall_Window_03":
                                if (t.name == wallName1)
                                {
                                    if (type == 0 || type == 1)
                                    {
                                        t.gameObject.SetActive(false);
                                    }
                                    else if (type == 3)
                                    {
                                        t.gameObject.SetActive(true);
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case "Pod (2)":
                    Transform TA9 = __instance.transform.parent.parent.parent.Find("Common");
                    foreach (Transform t in TA9)
                    {
                        switch (__instance.transform.name)
                        {
                            case "P_Wall_Window_02":
                                if (t.name == wallName2)
                                {
                                    if (type == 0 || type == 2)
                                    {
                                        t.gameObject.SetActive(false);
                                    }
                                    else if (type == 3)
                                    {
                                        t.gameObject.SetActive(true);
                                    }
                                }
                                break;
                            case "P_Wall_Window_04":
                                if (t.name == wallName2)
                                {
                                    if (type == 0 || type == 1)
                                    {
                                        t.gameObject.SetActive(false);
                                    }
                                    else if (type == 3)
                                    {
                                        t.gameObject.SetActive(true);
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case "Pod (3)":
                case "Pod (5)":
                case "Pod (7)":
                case "Pod (6)":
                    /*
                    Transform TA10 = __instance.transform.parent.parent.parent.Find("Common");
                    foreach (Transform t in TA10)
                    {
                        if (type == 0 || type == 1)
                        {

                            if (t.name == wallName1)
                            {
                                t.gameObject.SetActive(false);
                            }
                        }
                        if (type == 0 || type == 2)
                        {
                            if (t.name == wallName2)
                            {
                                t.gameObject.SetActive(false);
                            }
                        }
                        if (type == 3)
                        {
                            if (t.name == wallName1 || t.name == wallName2)
                            {
                                t.gameObject.SetActive(true);
                            }
                        }

                    }*/
                    break;
                case "Pod (4)":
                    Transform TA12 = __instance.transform.parent.parent.parent.Find("Common");
                    foreach (Transform t in TA12)
                    {
                        switch (__instance.transform.name)
                        {
                            case "P_Wall_Window_01":
                                if (t.name == wallName1)
                                {
                                    if (type == 0 || type == 1)
                                    {
                                        t.gameObject.SetActive(false);
                                    }
                                    else if (type == 3)
                                    {
                                        t.gameObject.SetActive(true);
                                    }
                                }
                                break;
                            case "P_Wall_Window_04":
                                if (t.name == wallName1)
                                {
                                    if (type == 0 || type == 2)
                                    {
                                        t.gameObject.SetActive(false);
                                    }
                                    else if (type == 3)
                                    {
                                        t.gameObject.SetActive(true);
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case "Pod (8)":
                    break;
            }
        }

    }
}

[BepInPlugin("Tjatja.theplanetcraftermods.CorridorsExtended", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    static ConfigEntry<bool> modEnabled;
    static ManualLogSource logger;
    static string currentLanguage;
    static AssetBundle bundle;

    private static readonly string WallCorridor05 = "Wall_Corridor_05";
    private static readonly string WallCorridor06 = "Wall_Corridor_06L";
    private static readonly string WallCorridor07 = "Wall_Corridor_07R";
    private static readonly DataConfig.BuildPanelSubType WallCorridor05SubPanelType = (DataConfig.BuildPanelSubType)150_001;
    private static readonly DataConfig.BuildPanelSubType WallCorridor06LSubPanelType = (DataConfig.BuildPanelSubType)150_002;
    private static readonly DataConfig.BuildPanelSubType WallCorridor07RSubPanelType = (DataConfig.BuildPanelSubType)150_003;

    private void Awake()
    {
        if (ModVersionCheck.ModVersionCheck.Check(this, Logger.LogInfo, out bool hashError, out string repoURL))
        {
            ModVersionCheck.ModVersionCheck.NotifyUser(this, hashError, repoURL, Logger.LogInfo);
        }
        modEnabled = Config.Bind("General", "Enabled", true, "Do note if you disable or uninstall this mod, all walls from this mod become unusable, make sure to destroy them!");
        MaterialsHelper.InitMaterialsHelper(Logger);
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
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
            dict["GROUP_NAME_Wall_Corridor_05"] = "Floating Header";
            dict["GROUP_NAME_Wall_Corridor_06L"] = "Floating Header (L)";
            dict["GROUP_NAME_Wall_Corridor_07R"] = "Floating Header (R)";
        }
        if (___localizationDictionary.TryGetValue("french", out dict))
        {
            dict["GROUP_NAME_Wall_Corridor_05"] = "En-tête flottant";
            dict["GROUP_NAME_Wall_Corridor_06L"] = "En-tête flottant (L)";
            dict["GROUP_NAME_Wall_Corridor_07R"] = "En-tête flottant (R)";
        }
        if (___localizationDictionary.TryGetValue("russian", out dict))
        {
            dict["GROUP_NAME_Wall_Corridor_05"] = "Плавающий заголовок";
            dict["GROUP_NAME_Wall_Corridor_06L"] = "Плавающий заголовок (L)";
            dict["GROUP_NAME_Wall_Corridor_07R"] = "Плавающий заголовок (R)";
        }
        if (___localizationDictionary.TryGetValue("schinese", out dict))
        {
            dict["GROUP_NAME_Wall_Corridor_05"] = "悬浮页眉";
            dict["GROUP_NAME_Wall_Corridor_06L"] = "悬浮页眉 (L)";
            dict["GROUP_NAME_Wall_Corridor_07R"] = "悬浮页眉 (R)";
        }
        if (___localizationDictionary.TryGetValue("tchinese", out dict))
        {
            dict["GROUP_NAME_Wall_Corridor_05"] = "懸浮頁眉";
            dict["GROUP_NAME_Wall_Corridor_06L"] = "悬浮页眉 (L)";
            dict["GROUP_NAME_Wall_Corridor_07R"] = "悬浮页眉 (R)";
        }
        if (___localizationDictionary.TryGetValue("german", out dict))
        {
            dict["GROUP_NAME_Wall_Corridor_05"] = "Schwebende Kopfzeile";
            dict["GROUP_NAME_Wall_Corridor_06L"] = "Schwebende Kopfzeile (L)";
            dict["GROUP_NAME_Wall_Corridor_07R"] = "Schwebende Kopfzeile (R)";
        }
        if (___localizationDictionary.TryGetValue("portuguese", out dict))
        {
            dict["GROUP_NAME_Wall_Corridor_05"] = "Cabeçalho flutuante";
            dict["GROUP_NAME_Wall_Corridor_06L"] = "Cabeçalho flutuante (L)";
            dict["GROUP_NAME_Wall_Corridor_07R"] = "Cabeçalho flutuante (R)";
        }
        if (___localizationDictionary.TryGetValue("spanish", out dict))
        {
            dict["GROUP_NAME_Wall_Corridor_05"] = "Encabezado flotante";
            dict["GROUP_NAME_Wall_Corridor_06L"] = "Encabezado flotante (L)";
            dict["GROUP_NAME_Wall_Corridor_07R"] = "Encabezado flotante (R)";
        }
        if (___localizationDictionary.TryGetValue("koreana", out dict))
        {
            dict["GROUP_NAME_Wall_Corridor_05"] = "플로팅 헤더";
            dict["GROUP_NAME_Wall_Corridor_06L"] = "플로팅 헤더 (L)";
            dict["GROUP_NAME_Wall_Corridor_07R"] = "플로팅 헤더 (R)";
        }
        if (___localizationDictionary.TryGetValue("japanese", out dict))
        {
            dict["GROUP_NAME_Wall_Corridor_05"] = "フローティングヘッダー";
            dict["GROUP_NAME_Wall_Corridor_06L"] = "フローティングヘッダー (L)";
            dict["GROUP_NAME_Wall_Corridor_07R"] = "フローティングヘッダー (R)";
        }
        if (___localizationDictionary.TryGetValue("turk", out dict))
        {
            dict["GROUP_NAME_Wall_Corridor_05"] = "Sabit Başlık";
            dict["GROUP_NAME_Wall_Corridor_06L"] = "Sabit Başlık (L)";
            dict["GROUP_NAME_Wall_Corridor_07R"] = "Sabit Başlık (R)";
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(StaticDataHandler), "LoadStaticData")]
    [HarmonyPriority(Priority.Low)]
    private static void StaticDataHandler_LoadStaticData2(List<GroupData> ___groupsData)
    {
        //if (___groupsData == null) return;
        if (!___groupsData.Select(gd => gd.id).Where(id => id == "wall_corridors").Any())
        {
            if (bundle == null)
            {
                string filepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/wall_corridors";
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

            NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = false;
            Action<object, object>? SetGlobalObjectIdHash = null;
            var globalHashField = typeof(NetworkObject).GetField("GlobalObjectIdHash", BindingFlags.Instance | BindingFlags.NonPublic);
            if (globalHashField is not null) SetGlobalObjectIdHash = globalHashField.SetValue;

            GroupDataConstructible cubeGDC = bundle.LoadAsset<GroupDataConstructible>("assets/Wall_Corridor_05.asset");
            if (___groupsData.Contains(cubeGDC))
            {
                return;
            }
            cubeGDC.associatedGameObject.GetComponentInChildren<ConstraintSamePanel>().panelSubType = WallCorridor05SubPanelType;
            MaterialsHelper.ApplyGameMaterials(cubeGDC.associatedGameObject);
            //cubeGDC.associatedGameObject.AddComponent<CornerReplacer>();
            ___groupsData.Add(cubeGDC);

            GroupDataConstructible cubeGDC1 = bundle.LoadAsset<GroupDataConstructible>("assets/Wall_Corridor_06L.asset");
            cubeGDC1.associatedGameObject.GetComponentInChildren<ConstraintSamePanel>().panelSubType = WallCorridor06LSubPanelType;
            MaterialsHelper.ApplyGameMaterials(cubeGDC1.associatedGameObject);
            //cubeGDC1.associatedGameObject.AddComponent<CornerReplacer>();
            ___groupsData.Add(cubeGDC1);

            GroupDataConstructible cubeGDC2 = bundle.LoadAsset<GroupDataConstructible>("assets/Wall_Corridor_07R.asset");
            cubeGDC2.associatedGameObject.GetComponentInChildren<ConstraintSamePanel>().panelSubType = WallCorridor07RSubPanelType;
            MaterialsHelper.ApplyGameMaterials(cubeGDC2.associatedGameObject);
            //cubeGDC2.associatedGameObject.AddComponent<CornerReplacer>();
            ___groupsData.Add(cubeGDC2);

            NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = true;
        }
        
        String[] stringArray = ([ "InteriorStairs1", "pod", "podAngle", "Pod4x", "Biolab", "Pod9xC", "PodUnderground"]);
        foreach (String str in stringArray)
        {
            ___groupsData.Find(e => e.id == str).associatedGameObject.AddComponent<CornerReplacer>();
        }
        String[] stringArrayCustoms = ([ "SubTerrainianPod", "SpiralStairCase",  "RoofTopFarm",
            "MiniInteriorStairs",  "ElevatorPod"]);
        foreach (String str in stringArrayCustoms)
        {
            if (___groupsData.Find(e => e.id == str) != null)
            {
                ___groupsData.Find(e => e.id == str).associatedGameObject.AddComponent<CornerReplacer>();
            }
        }
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PanelsResources), nameof(PanelsResources.GetPanelGameObject))]
    [HarmonyPatch(typeof(PanelsResources), nameof(PanelsResources.GetPanelGroupConstructible))]
    [HarmonyPatch(typeof(PanelsResources), nameof(PanelsResources.GetPanelUndergroundGameObject))]
    static void PR_GPGO_GPGC(PanelsResources __instance)
    {
        if (__instance.panelsSubtypes.Contains(WallCorridor05SubPanelType)) return;

        if (GroupsHandler.GetAllGroups() == null) return;

        Group WallCorridor1 = GroupsHandler.GetGroupViaId(WallCorridor05);
        Group WallCorridor2 = GroupsHandler.GetGroupViaId(WallCorridor06);
        Group WallCorridor3 = GroupsHandler.GetGroupViaId(WallCorridor07);
        GroupDataConstructible gdc1 = null;
        GroupDataConstructible gdc2 = null;
        GroupDataConstructible gdc3 = null;
        if (WallCorridor1 == null)
        {
            gdc1 = AccessTools.FieldRefAccess<StaticDataHandler, List<GroupData>>(Managers.GetManager<StaticDataHandler>(), "groupsData").Find(e => e.id == WallCorridor05) as GroupDataConstructible;
            gdc2 = AccessTools.FieldRefAccess<StaticDataHandler, List<GroupData>>(Managers.GetManager<StaticDataHandler>(), "groupsData").Find(e => e.id == WallCorridor06) as GroupDataConstructible;
            gdc3 = AccessTools.FieldRefAccess<StaticDataHandler, List<GroupData>>(Managers.GetManager<StaticDataHandler>(), "groupsData").Find(e => e.id == WallCorridor07) as GroupDataConstructible;
        }
        else
        {
            gdc1 = WallCorridor1.GetGroupData() as GroupDataConstructible;
            gdc2 = WallCorridor2.GetGroupData() as GroupDataConstructible;
            gdc3 = WallCorridor3.GetGroupData() as GroupDataConstructible;
        }
        __instance.panelsSubtypes.Add(WallCorridor05SubPanelType);
        __instance.panelsGroupItems.Add(gdc1);
        __instance.panelsGameObjects.Add(gdc1.associatedGameObject);
        __instance.panelsUndergroundGameObjects.Add(gdc1.associatedGameObject);

        __instance.panelsSubtypes.Add(WallCorridor06LSubPanelType);
        __instance.panelsGroupItems.Add(gdc2);
        __instance.panelsGameObjects.Add(gdc2.associatedGameObject);
        __instance.panelsUndergroundGameObjects.Add(gdc2.associatedGameObject);

        __instance.panelsSubtypes.Add(WallCorridor07RSubPanelType);
        __instance.panelsGroupItems.Add(gdc3);
        __instance.panelsGameObjects.Add(gdc3.associatedGameObject);
        __instance.panelsUndergroundGameObjects.Add(gdc3.associatedGameObject);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SpaceCraft.Panel), "SetPanel")]
    static void Panel_SetPanel(List<DataConfig.BuildPanelSubType> ____deconstructiblePanelsType, SpaceCraft.Panel __instance)
    {
        if (!____deconstructiblePanelsType.Contains(WallCorridor05SubPanelType))
        {
            ____deconstructiblePanelsType.Add((WallCorridor05SubPanelType));
        }
        if (!____deconstructiblePanelsType.Contains(WallCorridor06LSubPanelType))
        {
            ____deconstructiblePanelsType.Add((WallCorridor06LSubPanelType));
        }
        if (!____deconstructiblePanelsType.Contains(WallCorridor07RSubPanelType))
        {
            ____deconstructiblePanelsType.Add((WallCorridor07RSubPanelType));
        }
    }
}
