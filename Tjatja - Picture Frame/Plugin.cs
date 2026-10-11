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
using Unity;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace PictureFrame;

public class PictureHandler : Actionnable
{
    public static GameObject Content;
    public static GameObject Move;
    public static GameObject ImagePicture;
    public int curFrame = 0;
    public static Texture2D currentText;
    private static PlayerMultitool playerMultitool;
    public string hoverLocalizationId;
    private string hoverFinalId;

    public override void Start()
    {
        hoverFinalId = ((hoverLocalizationId == "" || hoverLocalizationId == null) ? "UI_Use" : hoverLocalizationId);
        init();
        ImagePicture.transform.root.GetComponent<TextProxy>().OnTextUpdate += MPOnTextUpdate;
    }
    public bool init()
    {
        if (this.transform.root.GetComponentInChildren<ConstructibleGhost>() != null) return false;
        Content = this.transform.root.gameObject;
        foreach (Transform t in this.transform.root.GetComponentsInChildren<Transform>())
        {
            switch (t.name)
            {
                case "ImagePicture":
                    //Console.WriteLine("ImagePicture found");
                    ImagePicture = t.gameObject;
                    break;

            }
            switch (t.name)
            {
                case "Move":
                    //Console.WriteLine("Move found");
                    Move = t.gameObject;
                    break;

            }
        }
        
        if (ImagePicture.transform.root.GetComponent<WorldObjectAssociated>() != null)
        {
            if (ImagePicture.transform.root.GetComponent<TextProxy>() != null)
            {
                if (ImagePicture.transform.root.GetComponent<TextProxy>().GetText() != "" ||
                    ImagePicture.transform.root.GetComponent<TextProxy>().GetText() != null)
                {
                    //Console.WriteLine("init to : "+ImagePicture.transform.root.GetComponent<TextProxy>().GetText());
                    updateMaterial(ImagePicture.transform.root.GetComponent<TextProxy>().GetText());
                }
            }
        }

        return true;
    }

    private void MPOnTextUpdate(string obj)
    {
        updateMaterial(ImagePicture.transform.root.GetComponent<TextProxy>().GetText());
    }

    public bool updateMaterial(string iconPath)
    {
        Texture2D newText = new Texture2D(2, 2);
        if (File.Exists(Path.GetDirectoryName(Plugin.customDirectory.Value) + "\\"+iconPath))
        {
            //Console.WriteLine("fileExists");
            byte[] array = File.ReadAllBytes(Path.GetDirectoryName(Plugin.customDirectory.Value) + "\\" + iconPath);
            newText.LoadImage(array);
        }
        else
        {
            iconPath = "defaultImage.png";
            if (File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + iconPath))
            {
                //Console.WriteLine("fileExists");
                byte[] array = File.ReadAllBytes(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + iconPath);
                newText.LoadImage(array);
            }
        }
        ImagePicture.GetComponent<MeshRenderer>().material.EnableKeyword("_NORMALMAP");
        ImagePicture.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        ImagePicture.GetComponent<MeshRenderer>().materials[0].SetTexture("_MainTex", newText);
        ImagePicture.GetComponent<MeshRenderer>().materials[0].SetTexture("_BumpMap", newText);
        ImagePicture.GetComponent<MeshRenderer>().materials[0].SetTexture("_BaseMap", newText);
        ImagePicture.GetComponent<MeshRenderer>().material.SetTexture("_EmissionMap", newText);
        ImagePicture.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(255,255,255));
        return true;
    }
    public override void OnAction()
    {
        if (init())
        {
            playerMultitool = ((playerMultitool != null) ? playerMultitool : Managers.GetManager<PlayersManager>().GetActivePlayerController().GetMultitool());
            if (playerMultitool.GetState() == DataConfig.MultiToolState.Deconstruct)
            {
                return;
            }
            string[] AllTextures = System.IO.Directory.GetFiles(Path.GetDirectoryName(Plugin.customDirectory.Value), "*.png");
            
            Actionnable._hudHandler.DisplayCursorText("UI_Use", 0f);
            curFrame++;
            if (curFrame > (AllTextures.Length-1)) { curFrame = 0; }
            //Console.WriteLine(AllTextures[curFrame]);
            if (ImagePicture.transform.root.GetComponent<TextProxy>() != null)
            {
                ImagePicture.transform.root.GetComponent<TextProxy>().SetText(new FileInfo(AllTextures[curFrame]).Name);
            }
            OnHoverOut();
        }
    }
    
    public override void OnHover()
    {
        if (!GamepadConfig.Instance.GetIsUsingController())
        {
            Actionnable._hudHandler.DisplayCursorText("UI_Use", 0f);
        }
        GamepadConfig.Instance.SetGamepadHintButtonVisible("Use", visible: true);
        base.OnHover();
    }

    public override void OnHoverOut()
    {
        this.OnDestroy();
        base.OnHoverOut();
    }

    private void OnDestroy()
    {
        Actionnable._hudHandler.CleanCursorTextIfCode("UI_Use");
        if (GamepadConfig.Instance != null)
        {
            GamepadConfig.Instance.SetGamepadHintButtonVisible("Use", visible: false);
        }
    }
}

[BepInPlugin("Tjatja.theplanetcraftermods.PictureFrame", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    static ConfigEntry<bool> modEnabled;
    public static ConfigEntry<Key> keyToPress;
    public static ConfigEntry<String> customDirectory;
    static ManualLogSource logger;
    static string currentLanguage;
    static string defaultDirectory = Assembly.GetExecutingAssembly().Location;
    static AssetBundle bundle;
    public static string[] files; 
    public static Dictionary<string, Texture2D> AllTextures = new Dictionary<string, Texture2D>();

    private void Awake()
    {
        
        if (ModVersionCheck.ModVersionCheck.Check(this, Logger.LogInfo, out bool hashError, out string repoURL))
        {
            ModVersionCheck.ModVersionCheck.NotifyUser(this, hashError, repoURL, Logger.LogInfo);
        }
        modEnabled = Config.Bind("General", "Enabled", true, "Additional information:\n" +
            "Small Picture Frame: WxH \n1000px, 1400px\n" +
            "Picture Frame: WxH \n2000px, 1400px\n" +
            "XL Picture Frame: WxH \n4000px, 2800px\n" +
            "\nEnable this mod?");
        keyToPress = Config.Bind("General", "hotkey", Key.PrintScreen, "Use the hotkey to capture in the correct folder\nPick the hotkey to use for pictures.");
        customDirectory = Config.Bind("General", "CustomDir", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Images\\", "Looks for Images inside of this directory.");
        logger = Logger;
        MaterialsHelper.InitMaterialsHelper(Logger);
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        if (modEnabled.Value)
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }
    }


    public void Update()
    {
        if (Keyboard.current[keyToPress.Value].isPressed && Keyboard.current[keyToPress.Value].wasPressedThisFrame)
        {
            ScreenCapture.CaptureScreenshot(Path.GetDirectoryName(customDirectory.Value) + "\\"+ DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".png");
            //Console.WriteLine("Saved : " + Path.GetDirectoryName(customDirectory.Value) +"\\"+ DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".png");
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
            dict["GROUP_NAME_PictureFrame"] = "Picture Frame";
            dict["GROUP_DESC_PictureFrame"] = "Hangs on walls or stands on floors.\nPress the button to scroll through screen captures.\nClick " + keyToPress.Value + " to capture a new screencapture.";
            dict["GROUP_NAME_XLPictureFrame"] = "XL Picture Frame";
            dict["GROUP_DESC_XLPictureFrame"] = "Hangs on walls or stands on floors.\nPress the button to scroll through screen captures.\nClick " + keyToPress.Value + " to capture a new screencapture.";
            dict["GROUP_NAME_StandingPictureFrame"] = "Small Picture Frame";
            dict["GROUP_DESC_StandingPictureFrame"] = "Hangs on walls or stands on floors.\nPress the button to scroll through screen captures.\nClick " + keyToPress.Value + " to capture a new screencapture.";
        }
        if (___localizationDictionary.TryGetValue("french", out dict))
        {
            dict["GROUP_NAME_PictureFrame"] = "Cadre photo";
            dict["GROUP_DESC_PictureFrame"] = "S'accroche au mur ou se pose au sol.\nAppuyez sur le bouton pour faire défiler les captures d'écran.\nCliquez sur " + keyToPress.Value + " pour effectuer une nouvelle capture d'écran.";
            dict["GROUP_NAME_XLPictureFrame"] = "Cadre photo XL";
            dict["GROUP_DESC_XLPictureFrame"] = "S'accroche au mur ou se pose au sol.\nAppuyez sur le bouton pour faire défiler les captures d'écran.\nCliquez sur " + keyToPress.Value + " pour effectuer une nouvelle capture d'écran.";
            dict["GROUP_NAME_StandingPictureFrame"] = "Petit cadre photo";
            dict["GROUP_DESC_StandingPictureFrame"] = "S'accroche au mur ou se pose au sol.\nAppuyez sur le bouton pour faire défiler les captures d'écran.\nCliquez sur " + keyToPress.Value + " pour effectuer une nouvelle capture d'écran.";
        }
        if (___localizationDictionary.TryGetValue("russian", out dict))
        {
            dict["GROUP_NAME_PictureFrame"] = "Фоторамка";
            dict["GROUP_DESC_PictureFrame"] = "Можно повесить на стену или поставить на пол.\nНажмите кнопку для перелистывания снимков экрана.\nНажмите " + keyToPress.Value + ", чтобы сделать новый снимок экрана.";
            dict["GROUP_NAME_XLPictureFrame"] = "Большая фоторамка";
            dict["GROUP_DESC_XLPictureFrame"] = "Можно повесить на стену или поставить на пол.\nНажмите кнопку для перелистывания снимков экрана.\nНажмите " + keyToPress.Value + ", чтобы сделать новый снимок экрана.";
            dict["GROUP_NAME_StandingPictureFrame"] = "Маленькая фоторамка";
            dict["GROUP_DESC_StandingPictureFrame"] = "Можно повесить на стену или поставить на пол.\nНажмите кнопку для перелистывания снимков экрана.\nНажмите " + keyToPress.Value + ", чтобы сделать новый снимок экрана.";
        }
        if (___localizationDictionary.TryGetValue("schinese", out dict))
        {
            dict["GROUP_NAME_PictureFrame"] = "相框";
            dict["GROUP_DESC_PictureFrame"] = "可挂在墙上或放置在地面上。\n按下按钮可循环浏览已截取的画面。\n按下 " + keyToPress.Value + " 即可截取新画面。";
            dict["GROUP_NAME_XLPictureFrame"] = "超大相框";
            dict["GROUP_DESC_XLPictureFrame"] = "可挂在墙上或放置在地面上。\n按下按钮可循环浏览已截取的画面。\n按下 " + keyToPress.Value + " 即可截取新画面。";
            dict["GROUP_NAME_StandingPictureFrame"] = "小型相框";
            dict["GROUP_DESC_StandingPictureFrame"] = "可挂在墙上或放置在地面上。\n按下按钮可循环浏览已截取的画面。\n按下 " + keyToPress.Value + " 即可截取新画面。";
        }
        if (___localizationDictionary.TryGetValue("tchinese", out dict))
        {
            dict["GROUP_NAME_PictureFrame"] = "相框";
            dict["GROUP_DESC_PictureFrame"] = "可掛在牆上或放置在地面上。\n按下按鈕可循環瀏覽已截取的畫面。\n按下 " + keyToPress.Value + " 即可截取新畫面。";
            dict["GROUP_NAME_XLPictureFrame"] = "超大相框";
            dict["GROUP_DESC_XLPictureFrame"] = "可掛在牆上或放置在地面上。\n按下按鈕可循環瀏覽已截取的畫面。\n按下 " + keyToPress.Value + " 即可截取新畫面。";
            dict["GROUP_NAME_StandingPictureFrame"] = "小型相框";
            dict["GROUP_DESC_StandingPictureFrame"] = "可掛在牆上或放置在地面上。\n按下按鈕可循環瀏覽已截取的畫面。\n按下 " + keyToPress.Value + " 即可截取新畫面。";
        }
        if (___localizationDictionary.TryGetValue("german", out dict))
        {
            dict["GROUP_NAME_PictureFrame"] = "Bilderrahmen";
            dict["GROUP_DESC_PictureFrame"] = "Kann an der Wand hängen oder auf dem Boden stehen.\nDrücke die Taste, um durch die Bildschirmaufnahmen zu blättern.\nDrücke " + keyToPress.Value + ", um eine neue Bildschirmaufnahme zu machen.";
            dict["GROUP_NAME_XLPictureFrame"] = "XL-Bilderrahmen";
            dict["GROUP_DESC_XLPictureFrame"] = "Kann an der Wand hängen oder auf dem Boden stehen.\nDrücke die Taste, um durch die Bildschirmaufnahmen zu blättern.\nDrücke " + keyToPress.Value + ", um eine neue Bildschirmaufnahme zu machen.";
            dict["GROUP_NAME_StandingPictureFrame"] = "Kleiner Bilderrahmen";
            dict["GROUP_DESC_StandingPictureFrame"] = "Kann an der Wand hängen oder auf dem Boden stehen.\nDrücke die Taste, um durch die Bildschirmaufnahmen zu blättern.\nDrücke " + keyToPress.Value + ", um eine neue Bildschirmaufnahme zu machen.";
        }
        if (___localizationDictionary.TryGetValue("portuguese", out dict))
        {
            dict["GROUP_NAME_PictureFrame"] = "Moldura de Foto";
            dict["GROUP_DESC_PictureFrame"] = "Pode ser pendurada na parede ou colocada no chão.\nPressione o botão para percorrer as capturas de tela.\nClique em " + keyToPress.Value + " para fazer uma nova captura de tela.";
            dict["GROUP_NAME_XLPictureFrame"] = "Moldura de Foto GG";
            dict["GROUP_DESC_XLPictureFrame"] = "Pode ser pendurada na parede ou colocada no chão.\nPressione o botão para percorrer as capturas de tela.\nClique em " + keyToPress.Value + " para fazer uma nova captura de tela.";
            dict["GROUP_NAME_StandingPictureFrame"] = "Moldura de Foto Pequena";
            dict["GROUP_DESC_StandingPictureFrame"] = "Pode ser pendurada na parede ou colocada no chão.\nPressione o botão para percorrer as capturas de tela.\nClique em " + keyToPress.Value + " para fazer uma nova captura de tela.";
        }
        if (___localizationDictionary.TryGetValue("spanish", out dict))
        {
            dict["GROUP_NAME_PictureFrame"] = "Marco de fotos";
            dict["GROUP_DESC_PictureFrame"] = "Se puede colgar en la pared o colocar en el suelo.\nPulsa el botón para recorrer las capturas de pantalla.\nHaz clic en " + keyToPress.Value + " para realizar una nueva captura de pantalla.";
            dict["GROUP_NAME_XLPictureFrame"] = "Marco de fotos XL";
            dict["GROUP_DESC_XLPictureFrame"] = "Se puede colgar en la pared o colocar en el suelo.\nPulsa el botón para recorrer las capturas de pantalla.\nHaz clic en " + keyToPress.Value + " para realizar una nueva captura de pantalla.";
            dict["GROUP_NAME_StandingPictureFrame"] = "Marco de fotos pequeño";
            dict["GROUP_DESC_StandingPictureFrame"] = "Se puede colgar en la pared o colocar en el suelo.\nPulsa el botón para recorrer las capturas de pantalla.\nHaz clic en " + keyToPress.Value + " para realizar una nueva captura de pantalla.";
        }
        if (___localizationDictionary.TryGetValue("koreana", out dict))
        {
            dict["GROUP_NAME_PictureFrame"] = "액자";
            dict["GROUP_DESC_PictureFrame"] = "벽에 걸거나 바닥에 세워둘 수 있습니다.\n버튼을 눌러 스크린샷을 넘겨보세요.\n" + keyToPress.Value + " 키를 눌러 새로운 스크린샷을 찍으세요.";
            dict["GROUP_NAME_XLPictureFrame"] = "대형 액자";
            dict["GROUP_DESC_XLPictureFrame"] = "벽에 걸거나 바닥에 세워둘 수 있습니다.\n버튼을 눌러 스크린샷을 넘겨보세요.\n" + keyToPress.Value + " 키를 눌러 새로운 스크린샷을 찍으세요.";
            dict["GROUP_NAME_StandingPictureFrame"] = "소형 액자";
            dict["GROUP_DESC_StandingPictureFrame"] = "벽에 걸거나 바닥에 세워둘 수 있습니다.\n버튼을 눌러 스크린샷을 넘겨보세요.\n" + keyToPress.Value + " 키를 눌러 새로운 스크린샷을 찍으세요.";
        }
        if (___localizationDictionary.TryGetValue("japanese", out dict))
        {
            dict["GROUP_NAME_PictureFrame"] = "フォトフレーム";
            dict["GROUP_DESC_PictureFrame"] = "壁に掛けたり、床に置いたりできます。\nボタンを押すと、スクリーンショットを切り替えられます。\n" + keyToPress.Value + " をクリックすると、新しいスクリーンショットを撮影できます。";
            dict["GROUP_NAME_XLPictureFrame"] = "XLフォトフレーム";
            dict["GROUP_DESC_XLPictureFrame"] = "壁に掛けたり、床に置いたりできます。\nボタンを押すと、スクリーンショットを切り替えられます。\n" + keyToPress.Value + " をクリックすると、新しいスクリーンショットを撮影できます。";
            dict["GROUP_NAME_StandingPictureFrame"] = "小型フォトフレーム";
            dict["GROUP_DESC_StandingPictureFrame"] = "壁に掛けたり、床に置いたりできます。\nボタンを押すと、スクリーンショットを切り替えられます。\n" + keyToPress.Value + " をクリックすると、新しいスクリーンショットを撮影できます。";
        }
        if (___localizationDictionary.TryGetValue("turk", out dict))
        {
            dict["GROUP_NAME_PictureFrame"] = "Resim Çerçevesi";
            dict["GROUP_DESC_PictureFrame"] = "Duvara asılabilir veya zeminde durabilir.\nEkran görüntüleri arasında gezinmek için düğmeye basın.\nYeni bir ekran görüntüsü almak için " + keyToPress.Value + " tuşuna tıklayın.";
            dict["GROUP_NAME_XLPictureFrame"] = "XL Resim Çerçevesi";
            dict["GROUP_DESC_XLPictureFrame"] = "Duvara asılabilir veya zeminde durabilir.\nEkran görüntüleri arasında gezinmek için düğmeye basın.\nYeni bir ekran görüntüsü almak için " + keyToPress.Value + " tuşuna tıklayın.";
            dict["GROUP_NAME_StandingPictureFrame"] = "Küçük Resim Çerçevesi";
            dict["GROUP_DESC_StandingPictureFrame"] = "Duvara asılabilir veya zeminde durabilir.\nEkran görüntüleri arasında gezinmek için düğmeye basın.\nYeni bir ekran görüntüsü almak için " + keyToPress.Value + " tuşuna tıklayın.";
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(StaticDataHandler), "LoadStaticData")]
    private static void StaticDataHandler_LoadStaticData2(List<GroupData> ___groupsData)
    {
        if (___groupsData.Select(gd => gd.id).Where(id => id == "PictureFrame").Any()) return;

        if (bundle == null)
        {
            string filepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/pictureframe";
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

        GroupDataConstructible cubeGDC = bundle.LoadAsset<GroupDataConstructible>("assets/PictureFrame.asset");
        if (___groupsData.Contains(cubeGDC))
        {
            return;
        }
        MaterialsHelper.ApplyGameMaterials(cubeGDC.associatedGameObject, true);
        ___groupsData.Add(cubeGDC);
        uint newint = 204376345;
        SetGlobalObjectIdHash(cubeGDC.associatedGameObject.GetComponent<NetworkObject>(), newint);
        NetworkManager.Singleton.AddNetworkPrefab(cubeGDC.associatedGameObject);

        GroupDataConstructible cubeGDC1 = bundle.LoadAsset<GroupDataConstructible>("assets/XLPictureFrame.asset");
        MaterialsHelper.ApplyGameMaterials(cubeGDC1.associatedGameObject, true);
        ___groupsData.Add(cubeGDC1);
        uint newint1 = 204376346;
        SetGlobalObjectIdHash(cubeGDC1.associatedGameObject.GetComponent<NetworkObject>(), newint1);
        NetworkManager.Singleton.AddNetworkPrefab(cubeGDC1.associatedGameObject);

        GroupDataConstructible cubeGDC2 = bundle.LoadAsset<GroupDataConstructible>("assets/StandingPictureFrame.asset");
        MaterialsHelper.ApplyGameMaterials(cubeGDC2.associatedGameObject, true);
        ___groupsData.Add(cubeGDC2);
        uint newint2 = 204376347;
        SetGlobalObjectIdHash(cubeGDC2.associatedGameObject.GetComponent<NetworkObject>(), newint2);
        NetworkManager.Singleton.AddNetworkPrefab(cubeGDC2.associatedGameObject);

        NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = true;
        // networking code
        GameObject PFGO = ___groupsData.Find(e => e.id == "PictureFrame").associatedGameObject;
        PFGO.GetComponentInChildren<ActionnableInteractive>().gameObject.AddComponent<PictureHandler>();
        GameObject PFGO1 = ___groupsData.Find(e => e.id == "XLPictureFrame").associatedGameObject;
        PFGO1.GetComponentInChildren<ActionnableInteractive>().gameObject.AddComponent<PictureHandler>();
        GameObject PFGO2 = ___groupsData.Find(e => e.id == "StandingPictureFrame").associatedGameObject;
        PFGO2.GetComponentInChildren<ActionnableInteractive>().gameObject.AddComponent<PictureHandler>();
    }
}
