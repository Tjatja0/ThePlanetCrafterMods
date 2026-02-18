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

namespace DirtBlocks
{
    [BepInPlugin("Tjatja.theplanetcraftermods.DirtBlocks", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<bool> modEnabled;
        static ConfigEntry<bool> DirtBlocks;
        static ConfigEntry<bool> SpiralStairs;
        static ConfigEntry<bool> PodFarm;
        static ConfigEntry<bool> UndergroundPod;
        static ConfigEntry<bool> ElevatorPod;
        static ConfigEntry<bool> FaucetFilled; 
        static ConfigEntry<String> specialthanks;
        static ManualLogSource logger;
        static string currentLanguage;
        static AssetBundle bundle;

        private void Awake()
        {
            if (ModVersionCheck.ModVersionCheck.Check(this, Logger.LogInfo))
            {
                ModVersionCheck.ModVersionCheck.NotifyUser(this, Logger.LogInfo);
            }
            modEnabled = Config.Bind("General", "Enabled", true, "Is the mod enabled?");
            DirtBlocks = Config.Bind("General", "Dirtblocks", true, "Add dirtblocks to the game assets?");
            SpiralStairs = Config.Bind("General", "Spiralstairs", true, "Add spiral stairs to the game assets?");
            PodFarm = Config.Bind("General", "Podfarm", true, "Add pod with farm as roof?");
            UndergroundPod = Config.Bind("General", "UndergroundPod", true, "Add pod with build in ladder?");
            ElevatorPod = Config.Bind("General", "ElevatorPod", true, "Add pod with Elevator?");
            FaucetFilled = Config.Bind("General", "FaucetFilled", true, "Add Faucet with drinkable water?");
            specialthanks = Config.Bind("Special", "Thanks", "Nicki0", "");
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
                dict["GROUP_NAME_FoundationDirtBlock"] = "Dirtblock";
                dict["GROUP_NAME_FoundationDirtBlockAngle"] = "Dirtblock Angle";
                dict["GROUP_NAME_FoundationDirtBlockSlope"] = "Dirtblock Slope";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlope"] = "Dirtblock Angled Slope";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlopeConcave"] = "DirtBlock Angled Slope Concave";
                dict["GROUP_NAME_ElevatorPod"] = "Elevator Pod";
                dict["GROUP_NAME_SpiralStairCase"] = "Spiral Stairs";
                dict["GROUP_NAME_RoofTopFarm"] = "Living Compartment Roof Top Farm";
                dict["GROUP_NAME_SubTerrainianPod"] = "Living Compartment to start building underground";
                dict["GROUP_NAME_FaucetFilled"] = "Faucet (Drinkable)";
            }
            if (___localizationDictionary.TryGetValue("french", out dict))
            {
                dict["GROUP_NAME_FoundationDirtBlock"] = "bloc de terre";
                dict["GROUP_NAME_FoundationDirtBlockAngle"] = "angle du bloc de terre";
                dict["GROUP_NAME_FoundationDirtBlockSlope"] = "pente de blocs de terre";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlope"] = "talus en pente de blocs de terre";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlopeConcave"] = "Pente inclinée concave en blocs de terre";
                dict["GROUP_NAME_ElevatorPod"] = "Nacelle d'ascenseur";
                dict["GROUP_NAME_SpiralStairCase"] = "Escaliers en colimaçon";
                dict["GROUP_NAME_RoofTopFarm"] = "Ferme sur le toit d'un compartiment habitable";
                dict["GROUP_NAME_SubTerrainianPod"] = "Compartiment d'habitation souterrain";
                dict["GROUP_NAME_FaucetFilled"] = "Robinet (potable)";
            }
            if (___localizationDictionary.TryGetValue("russian", out dict))
            {
                dict["GROUP_NAME_FoundationDirtBlock"] = "Грязьблок";
                dict["GROUP_NAME_FoundationDirtBlockAngle"] = "Угол блока грязи";
                dict["GROUP_NAME_FoundationDirtBlockSlope"] = "Склон из земляных блоков";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlope"] = "Угловой склон из земляных блоков";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlopeConcave"] = "Грунтовый блок, наклонный склон, вогнутая поверхность";
                dict["GROUP_NAME_ElevatorPod"] = "Лифтовая капсула";
                dict["GROUP_NAME_SpiralStairCase"] = "Винтовая лестница";
                dict["GROUP_NAME_RoofTopFarm"] = "Ферма на крыше с жилыми отсеками";
                dict["GROUP_NAME_SubTerrainianPod"] = "Строительство жилого отсека начнётся под землёй.";
                dict["GROUP_NAME_FaucetFilled"] = "Кран (питьевая вода)";
            }
            if (___localizationDictionary.TryGetValue("schinese", out dict))
            {
                dict["GROUP_NAME_FoundationDirtBlock"] = "污垢块";
                dict["GROUP_NAME_FoundationDirtBlockAngle"] = "污垢块角度";
                dict["GROUP_NAME_FoundationDirtBlockSlope"] = "土块坡度";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlope"] = "土块倾斜斜坡";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlopeConcave"] = "土块倾斜斜坡凹面";
                dict["GROUP_NAME_ElevatorPod"] = "电梯舱";
                dict["GROUP_NAME_SpiralStairCase"] = "螺旋楼梯";
                dict["GROUP_NAME_RoofTopFarm"] = "屋顶农场生活隔间";
                dict["GROUP_NAME_SubTerrainianPod"] = "开始建造地下生活舱";
                dict["GROUP_NAME_FaucetFilled"] = "水龙头（可饮用）";
            }
            if (___localizationDictionary.TryGetValue("tchinese", out dict))
            {
                dict["GROUP_NAME_FoundationDirtBlock"] = "污垢塊";
                dict["GROUP_NAME_FoundationDirtBlockAngle"] = "污垢塊角度";
                dict["GROUP_NAME_FoundationDirtBlockSlope"] = "土塊坡度";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlope"] = "土塊傾斜斜坡";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlopeConcave"] = "土塊傾斜斜坡凹面";
                dict["GROUP_NAME_ElevatorPod"] = "電梯艙";
                dict["GROUP_NAME_SpiralStairCase"] = "螺旋樓梯";
                dict["GROUP_NAME_RoofTopFarm"] = "屋頂農場生活隔間";
                dict["GROUP_NAME_SubTerrainianPod"] = "開始建造地下生活艙";
                dict["GROUP_NAME_FaucetFilled"] = "水龍頭（可飲用）";
            }
            if (___localizationDictionary.TryGetValue("german", out dict))
            {
                dict["GROUP_NAME_FoundationDirtBlock"] = "Erdblock";
                dict["GROUP_NAME_FoundationDirtBlockAngle"] = "Erdblockwinkel";
                dict["GROUP_NAME_FoundationDirtBlockSlope"] = "Erdblockhang";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlope"] = "Erdblock-Schräghang";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlopeConcave"] = "Erdblock, schräge Böschung, konkav";
                dict["GROUP_NAME_ElevatorPod"] = "Aufzugsmodul";
                dict["GROUP_NAME_SpiralStairCase"] = "Wendeltreppe";
                dict["GROUP_NAME_RoofTopFarm"] = "Wohnmodul mit Feld auf dem Dach";
                dict["GROUP_NAME_SubTerrainianPod"] = "Startmodul einer unterirdischen Basis";
                dict["GROUP_NAME_FaucetFilled"] = "Wasserhahn (Trinkwasser)";
            }
            if (___localizationDictionary.TryGetValue("portuguese", out dict))
            {
                dict["GROUP_NAME_FoundationDirtBlock"] = "bloco de terra";
                dict["GROUP_NAME_FoundationDirtBlockAngle"] = "ângulo do bloco de terra";
                dict["GROUP_NAME_FoundationDirtBlockSlope"] = "inclinação do bloco de terra";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlope"] = "declive inclinado de blocos de terra";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlopeConcave"] = "Bloco de terra com inclinação côncava";
                dict["GROUP_NAME_ElevatorPod"] = "Elevador";
                dict["GROUP_NAME_SpiralStairCase"] = "Escada em espiral";
                dict["GROUP_NAME_RoofTopFarm"] = "Fazenda no telhado com compartimento habitável";
                dict["GROUP_NAME_SubTerrainianPod"] = "Compartimento habitacional para iniciar a construção subterrânea";
                dict["GROUP_NAME_FaucetFilled"] = "Torneira (água potável)";
            }
            if (___localizationDictionary.TryGetValue("spanish", out dict))
            {
                dict["GROUP_NAME_FoundationDirtBlock"] = "bloque de tierra";
                dict["GROUP_NAME_FoundationDirtBlockAngle"] = "ángulo del bloque de tierra";
                dict["GROUP_NAME_FoundationDirtBlockSlope"] = "pendiente de bloque de tierra";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlope"] = "pendiente en ángulo de bloques de tierra";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlopeConcave"] = "Bloque de tierra con pendiente en ángulo cóncavo";
                dict["GROUP_NAME_ElevatorPod"] = "Cápsula de ascensor";
                dict["GROUP_NAME_SpiralStairCase"] = "Escaleras de caracol";
                dict["GROUP_NAME_RoofTopFarm"] = "Compartimento habitable en la azotea de la granja";
                dict["GROUP_NAME_SubTerrainianPod"] = "El compartimento habitable comenzará a construirse bajo tierra";
                dict["GROUP_NAME_FaucetFilled"] = "Grifo (potable)";
            }
            if (___localizationDictionary.TryGetValue("koreana", out dict))
            {
                dict["GROUP_NAME_FoundationDirtBlock"] = "흙 블록";
                dict["GROUP_NAME_FoundationDirtBlockAngle"] = "흙 블록 각도";
                dict["GROUP_NAME_FoundationDirtBlockSlope"] = "흙 블록 경사";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlope"] = "흙 블록 경사면";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlopeConcave"] = "흙 블록 경사면 오목부";
                dict["GROUP_NAME_ElevatorPod"] = "엘리베이터 포드";
                dict["GROUP_NAME_SpiralStairCase"] = "나선형 계단";
                dict["GROUP_NAME_RoofTopFarm"] = "주거 공간 옥상 농장";
                dict["GROUP_NAME_SubTerrainianPod"] = "지하에 주거 공간 건설 시작";
                dict["GROUP_NAME_FaucetFilled"] = "수돗물 (음용 가능)";
            }
            if (___localizationDictionary.TryGetValue("japanese", out dict))
            {
                dict["GROUP_NAME_FoundationDirtBlock"] = "ダートブロック";
                dict["GROUP_NAME_FoundationDirtBlockAngle"] = "ダートブロック角度";
                dict["GROUP_NAME_FoundationDirtBlockSlope"] = "土ブロックの斜面";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlope"] = "土ブロックの斜め斜面";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlopeConcave"] = "土ブロック 傾斜斜面 凹面";
                dict["GROUP_NAME_ElevatorPod"] = "エレベーターポッド";
                dict["GROUP_NAME_SpiralStairCase"] = "螺旋階段";
                dict["GROUP_NAME_RoofTopFarm"] = "リビングコンパートメント屋上ファーム";
                dict["GROUP_NAME_SubTerrainianPod"] = "地下に居住区画を建設開始";
                dict["GROUP_NAME_FaucetFilled"] = "蛇口（飲用可能）";
            }
            if (___localizationDictionary.TryGetValue("turk", out dict))
            {
                dict["GROUP_NAME_FoundationDirtBlock"] = "Toprak Bloğu";
                dict["GROUP_NAME_FoundationDirtBlockAngle"] = "Toprak Blok Açısı";
                dict["GROUP_NAME_FoundationDirtBlockSlope"] = "Toprak Blok Eğim";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlope"] = "toprak blok eğimli yamaç";
                dict["GROUP_NAME_FoundationDirtBlockAngleSlopeConcave"] = "Toprak Blok Açılı Eğim İçbükey";
                dict["GROUP_NAME_ElevatorPod"] = "Asansör Bölmesi";
                dict["GROUP_NAME_SpiralStairCase"] = "Sarmal Merdivenler";
                dict["GROUP_NAME_RoofTopFarm"] = "Yaşam Alanı Çatı Çiftliği";
                dict["GROUP_NAME_SubTerrainianPod"] = "Yaşam alanının yer altında inşaatına başlanacak.";
                dict["GROUP_NAME_FaucetFilled"] = "Musluktan (İçilebilir)";
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StaticDataHandler), "LoadStaticData")]
        private static void StaticDataHandler_LoadStaticData2(List<GroupData> ___groupsData)
        {
            if (___groupsData.Select(gd => gd.id).Where(id => id == "dirtblock").Any()) return;

            if (bundle == null)
            {
                string filepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/dirtblock";
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
            if (DirtBlocks.Value)
            {
                GroupDataConstructible cubeGDC = bundle.LoadAsset<GroupDataConstructible>("assets/FoundationDirtBlock.asset");
                if (___groupsData.Contains(cubeGDC))
                {
                    return;
                }
                MaterialsHelper.ApplyGameMaterials(cubeGDC.associatedGameObject);
                ___groupsData.Add(cubeGDC);
                GroupDataConstructible cubeGDC1 = bundle.LoadAsset<GroupDataConstructible>("assets/FoundationDirtBlockAngle.asset");
                MaterialsHelper.ApplyGameMaterials(cubeGDC1.associatedGameObject);
                ___groupsData.Add(cubeGDC1);
                GroupDataConstructible cubeGDC2 = bundle.LoadAsset<GroupDataConstructible>("assets/FoundationDirtBlockSlope.asset");
                MaterialsHelper.ApplyGameMaterials(cubeGDC2.associatedGameObject);
                ___groupsData.Add(cubeGDC2);
                GroupDataConstructible cubeGDC3 = bundle.LoadAsset<GroupDataConstructible>("assets/FoundationDirtBlockAngleSlope.asset");
                MaterialsHelper.ApplyGameMaterials(cubeGDC3.associatedGameObject);
                ___groupsData.Add(cubeGDC3);
                GroupDataConstructible cubeGDC5 = bundle.LoadAsset<GroupDataConstructible>("assets/FoundationDirtBlockAngleSlopeConcave.asset");
                                MaterialsHelper.ApplyGameMaterials(cubeGDC5.associatedGameObject);
                ___groupsData.Add(cubeGDC5);
            }

            if (ElevatorPod.Value)
            {
                GroupDataConstructible cubeGDC4 = bundle.LoadAsset<GroupDataConstructible>("assets/ElevatorPod.asset");
                if (___groupsData.Contains(cubeGDC4))
                {
                    return;
                }
                MaterialsHelper.ApplyGameMaterials(cubeGDC4.associatedGameObject);
                cubeGDC4.associatedGameObject.transform.Find("Elevator").gameObject.AddComponent<Set_WorldUniqueId>();
                ___groupsData.Add(cubeGDC4);
            }
            if (PodFarm.Value)
            {
                GroupDataConstructible cubeGDC6 = bundle.LoadAsset<GroupDataConstructible>("assets/RoofTopFarm.asset");
                if (___groupsData.Contains(cubeGDC6))
                {
                    return;
                }
                MaterialsHelper.ApplyGameMaterials(cubeGDC6.associatedGameObject);
                ___groupsData.Add(cubeGDC6);
            }
            if (UndergroundPod.Value)
            {
                GroupDataConstructible cubeGDC7 = bundle.LoadAsset<GroupDataConstructible>("assets/SubTerrainianPod.asset");
                if (___groupsData.Contains(cubeGDC7))
                {
                    return;
                }
                MaterialsHelper.ApplyGameMaterials(cubeGDC7.associatedGameObject);
                ___groupsData.Add(cubeGDC7);
            }
            if (SpiralStairs.Value)
            {
                GroupDataConstructible cubeGDC8 = bundle.LoadAsset<GroupDataConstructible>("assets/SpiralStairCase.asset");
                if (___groupsData.Contains(cubeGDC8))
                {
                    return;
                }
                MaterialsHelper.ApplyGameMaterials(cubeGDC8.associatedGameObject);
                ___groupsData.Add(cubeGDC8);
            }
            if (FaucetFilled.Value)
            {
                GroupDataConstructible cubeGDC9 = bundle.LoadAsset<GroupDataConstructible>("assets/FaucetFilled.asset");
                if (___groupsData.Contains(cubeGDC9))
                {
                    return;
                }
                MaterialsHelper.ApplyGameMaterials(cubeGDC9.associatedGameObject);
                ___groupsData.Add(cubeGDC9);
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Panel), "SetPanel")]
        static void DirtPanelVanish(Panel __instance)
        {
            __instance.StartCoroutine(ExecuteLater(delegate () {
                if (__instance.GetComponent<ConstructibleGhost>() == null && __instance.GetComponentInParent<ConstructibleGhost>() == null) return;
                if (!__instance.transform.root.name.Contains("Dirt", StringComparison.InvariantCultureIgnoreCase)) return;
                foreach (Renderer r in __instance.GetComponentsInChildren<Renderer>())
                {
                    r.enabled = false;
                }
            }));
        }
        static IEnumerator ExecuteLater(Action toExecute, int waitFrames = 1)
        {
            for (int i = 0; i < waitFrames; i++) yield return new WaitForEndOfFrame();
            toExecute.Invoke();
        }
    }
    class Set_WorldUniqueId : MonoBehaviour
    {
        public void Start()
        {
            this.StartCoroutine(ExecuteLater(delegate () {
                if (this.GetComponentInParent<ConstructibleGhost>() != null) return;
                // Get WO Id
                this.GetComponentInParent<WorldObjectAssociatedProxy>().GetWorldObjectDetails(delegate (WorldObject wo) {
                    WorldUniqueId wuid = this.gameObject.GetComponent<WorldUniqueId>();
                    SceneInterpolation si = this.gameObject.GetComponent<SceneInterpolation>();
                    // Set WO Id on WorldUniqueId script
                    wuid.ChangeWorldObjectIdLive(wo.GetId());
                    ref int woid_SceneInterpolation = ref AccessTools.FieldRefAccess<SceneInterpolation, int>(si, "_woId");
                    // unregister old woId of SceneInterpolation
                    SceneInterpolationHandler.Instance.UnRegisterInterpolator(woid_SceneInterpolation);
                    woid_SceneInterpolation = wo.GetId();
                    // register new woId of SceneInterpolation (see: SceneInterpolation.Start)
                    SceneInterpolationHandler.Instance.RegisterInterpolator(
                        woid_SceneInterpolation,
                        AccessTools.FieldRefAccess<SceneInterpolation, float>(si, "_normalizedSpeed"),
                        new Action<float, short>(/*this.UpdateInterpolation*/delegate (float value, short direction) {
                            AccessTools.Method(typeof(SceneInterpolation), "UpdateInterpolation").Invoke(si, [value, direction]);
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
