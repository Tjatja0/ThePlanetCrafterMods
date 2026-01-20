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

namespace AutoNameStorage
{
    [BepInPlugin("Tjatja.theplanetcraftermods.AutoNameStorage", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<bool> modEnabled;
        static ConfigEntry<bool> setNameToFirstObjectStored;
        static ConfigEntry<bool> setDemandToFirstObjectStored;
        static ConfigEntry<String> exclusion;
        static ManualLogSource logger;
        static WorldObjectText woText = null;

        static MethodInfo mActionableHandleHoverMaterial;
        static AccessTools.FieldRef<Actionnable, bool> fActionableHovering;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            modEnabled = Config.Bind("General", "Enabled", true, "Is the mod enabled?");
            setNameToFirstObjectStored = Config.Bind("General", "SetStorageName", true, "Set the storage name on storing the first item?");
            setDemandToFirstObjectStored = Config.Bind("General", "SetDemand", true, "Set the storage demand on storing the first item?");
            exclusion = Config.Bind("General", "exclusion", "Supply", "Containers containing a name like this will be excluded.");
            logger = Logger;
            mActionableHandleHoverMaterial = AccessTools.Method(typeof(Actionnable), "HandleHoverMaterial", [typeof(bool)]);
            fActionableHovering = AccessTools.FieldRefAccess<Actionnable, bool>("_hovering");
            if (modEnabled.Value)
            {
                Harmony.CreateAndPatchAll(typeof(Plugin));
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InventoryDisplayer), "OnImageClicked")]
        static void InventoryDisplayer_OnImageClicked(EventTriggerCallbackData eventTriggerCallbackData, Inventory ____inventory)
        {
            if (eventTriggerCallbackData.pointerEventData.button == PointerEventData.InputButton.Left)
            {
                DataConfig.UiType CurrentUI = Managers.GetManager<WindowsHandler>().GetOpenedUi();

                if (CurrentUI == DataConfig.UiType.Container || CurrentUI == DataConfig.UiType.GroupSelector)
                {
                    Inventory otherInventory = ((UiWindowContainer)Managers.GetManager<WindowsHandler>().GetWindowViaUiId(CurrentUI)).GetOtherInventory(____inventory);
                    if (____inventory != null && otherInventory != null)
                    {
                        int count = 0;
                        String name = "";
                        WorldObject temp = null;
                        foreach (WorldObject worldObject in otherInventory.GetInsideWorldObjects())
                        {
                            if (count == 0)
                            {
                                name = Readable.GetGroupName(worldObject.GetGroup());
                                temp = worldObject;

                            }
                            count++;
                        }
                        if ((count == 0 || count == 1) && temp != null)
                        {

                            if (setDemandToFirstObjectStored.Value)
                            {
                                if (woText.GetText() != null && !woText.GetText().Contains(""+exclusion))
                                {
                                    otherInventory.GetLogisticEntity().ClearDemandGroups();
                                    //otherInventory.GetLogisticEntity().ClearSupplyGroups();
                                    //HashSet<Group> newgroup = otherInventory.GetLogisticEntity().GetDemandGroups();
                                    otherInventory.GetLogisticEntity().AddDemandGroup(temp.GetGroup());
                                    //otherInventory.GetLogisticEntity().SetDemandGroups(newgroup);
                                    logger.LogDebug("Set Demand to : " + name);
                                }
                                //SetDemandOfContainer

                            }
                            if (setNameToFirstObjectStored.Value)
                            {
                                if (woText.GetText() != null && !woText.GetText().Contains("" + exclusion))
                                {
                                    woText.SetText(name);
                                    logger.LogDebug("Set Text to : " + name);
                                }
                                //setNameOfContainer

                            }
                        }
                    }
                }
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActionOpenable), nameof(ActionOpenable.OnHover))]
        static void ActionOpenable_OnHover(ActionOpenable __instance, BaseHudHandler ____hudHandler)
        {
            woText = __instance.GetComponent<WorldObjectText>();
        }
    }
}
