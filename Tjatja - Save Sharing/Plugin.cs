using BepInEx;
using BepInEx.Configuration;
using SpaceCraft;
using HarmonyLib;
using System.Collections;
using System.IO;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using BepInEx.Logging;
using System.Net.NetworkInformation;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;
using static UnityEngine.InputForUI.InputManagerProvider;
using System.Collections.Generic;

namespace MyModNameHere
{
    [BepInPlugin("Tjatja.theplanetcraftermods.savesharing", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<bool> modEnabled;
        static ConfigEntry<String> ipAdress;
        static ManualLogSource logger;
        static Action OnContinueQuitting;

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            modEnabled = Config.Bind("General", "Enabled", true, "Is the mod enabled?");
            ipAdress = Config.Bind("General", "IPForConnection", "Localhost", "The IP address to connect to.");
            logger = Logger;
            if (modEnabled.Value)
            {
                Harmony.CreateAndPatchAll(typeof(Plugin));
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Intro), "Start")]
        static void Override_intro_start(Intro __instance)
        {
            __instance.StartCoroutine(ReadFileAsync());
        }
        static IEnumerator ReadFileAsync()
        {
            logger.LogDebug("Trying to find the file at http://" + ipAdress.Value + "/Server-1.txt");
            UnityWebRequest myWr = UnityWebRequest.Get("http://" + ipAdress.Value + "/Server-1.txt");
            yield return myWr.SendWebRequest();
            if (myWr.result == UnityWebRequest.Result.Success)
            {
                logger.LogDebug("succesfully found the file at http://" + ipAdress.Value + "/Server-1.txt");

                string textFileContents = myWr.downloadHandler.text;
                logger.LogDebug("The text loaded is " + textFileContents.Length + " Characters long.");
                if (System.IO.File.Exists(System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Server-1.json")))
                {
                    File.WriteAllText(System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Server-1.json"), textFileContents);
                    logger.LogDebug("Wrote request result to Server-1.json");
                }
                else
                {
                    File.WriteAllText(System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Server-1.json"), textFileContents);
                    logger.LogDebug("Wrote request result to Server-1.json");
                }
            }
            else
            {
                logger.LogDebug("Failed to find the file at http://" + ipAdress.Value + "/Server-1.txt");
            }
        }

        static IEnumerator PostData()
        {
            String postDataURL = "http://" + ipAdress.Value + "/upload.php?";
            string data = File.ReadAllText(System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Server-1.json"));
            WWWForm form = new WWWForm();
            form.AddField("name", "Server-1");
            form.AddField("data", data);
            UnityWebRequest www = UnityWebRequest.Post(postDataURL, form);
            yield return www.Send();
            if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Uploaded");
            }
            Debug.Log(www.downloadHandler.text);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UiWindowChat), "OnTextReceived")]
        static void Override_UiWindowChat_OnTextReceived(UiWindowChat __instance)
        {
            logger.LogDebug("Saved the game");
            Managers.GetManager<SavedDataHandler>().SaveWorldData("Server-1");

        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SavedDataHandler), "SaveWorldData")]
        static void Override_SavedDataHandler_SaveWorldData(SavedDataHandler __instance)
        {
            __instance.StartCoroutine(PostData());
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SessionController), "Awake")]
        static void Override_SessionController_SetAutoSaveDelay(SessionController __instance)
        {
            logger.LogDebug("Set auto save delay to 1.0f");
            __instance.SetAutoSaveDelay(1.0f);
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UiWindowPause), "Start")]
        static void UiWindowPause_Start(UiWindowPause __instance)
        {
            var quitBtn = __instance.transform.Find("Buttons/ButtonQuit").gameObject;

            var btn = quitBtn.GetComponent<Button>();

            var pc = btn.onClick.m_PersistentCalls;

            for (var i = pc.Count - 1; i >= 0; i--)
            {
                pc.RemoveListener(i);
            }

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(new UnityAction(() => {
                if (modEnabled.Value && (NetworkManager.Singleton?.IsServer ?? true))
                {
                    var sdh = Managers.GetManager<SavedDataHandler>();

                    OnContinueQuitting = () =>
                    {
                        sdh.OnSaved -= OnContinueQuitting;
                        __instance.OnQuit();
                    };

                    sdh.OnSaved += OnContinueQuitting;
                    __instance.OnSave();
                }
                else
                {
                    __instance.OnQuit();
                }
            }));
        }
    }
}
