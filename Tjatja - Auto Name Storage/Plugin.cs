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

namespace AutoNameStorage
{
    [BepInPlugin("Tjatja.theplanetcraftermods.AutoNameStorage", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<bool> modEnabled;
        static ConfigEntry<bool> setNameToFirstObjectStored;
        static ConfigEntry<bool> setDemandToFirstObjectStored;
        static ConfigEntry<bool> resetStorageNameToNull;
        static ConfigEntry<bool> resetStorageDemandToNull;
        static ConfigEntry<bool> addAsterisk;
        static ConfigEntry<String> exclusion;
        static ManualLogSource logger;
        static WorldObjectText woText = null;

        static MethodInfo mActionableHandleHoverMaterial;
        static AccessTools.FieldRef<Actionnable, bool> fActionableHovering;

        private void Awake()
        {
            if (ModVersionCheck.ModVersionCheck.Check(this, Logger.LogInfo))
            {
                ModVersionCheck.ModVersionCheck.NotifyUser(this, Logger.LogInfo);
            }
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            modEnabled = Config.Bind("General", "Enabled", true, "Is the mod enabled?");
            setNameToFirstObjectStored = Config.Bind("General", "SetStorageName", true, "Set the storage name on storing the first item?");
            setDemandToFirstObjectStored = Config.Bind("General", "SetDemand", true, "Set the storage demand on storing the first item?");
            resetStorageNameToNull = Config.Bind("General", "ResetName", true, "Set the storage name to ... when the last item is removed?");
            resetStorageDemandToNull = Config.Bind("General", "ResetDemand", true, "Set the storage demand to nothing when the last item is removed?");
            addAsterisk = Config.Bind("General", "AddAsterix", false, "adds an * in front of item names if set to true");
            exclusion = Config.Bind("General", "exclusion", "Supply", "Containers containing a name like this will be excluded.");
            logger = Logger;
            mActionableHandleHoverMaterial = AccessTools.Method(typeof(Actionnable), "HandleHoverMaterial", [typeof(bool)]);
            fActionableHovering = AccessTools.FieldRefAccess<Actionnable, bool>("_hovering");
            if (modEnabled.Value)
            {
                Harmony.CreateAndPatchAll(typeof(Plugin));
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(InventoriesHandler), "TransferAllSameGroup")]
        static void InventoriesHandler_TransferAllSameGroup_patch(Inventory fromInventory, Inventory toInventory, Group group)
        {
            DataConfig.UiType CurrentUI = Managers.GetManager<WindowsHandler>().GetOpenedUi();
            if (CurrentUI == DataConfig.UiType.Container || CurrentUI == DataConfig.UiType.GroupSelector)
            {
                if (toInventory != null && fromInventory != null)
                {
                    if (fromInventory.IsEmpty())
                    {
                        logger.LogDebug("Empty Inventory");
                        if (resetStorageNameToNull.Value)
                        {
                            if (woText.GetText() != null && !woText.GetText().Contains("" + exclusion.Value))
                            {
                                woText.SetText("...");
                                ((UiWindowContainer)Managers.GetManager<WindowsHandler>().GetWindowViaUiId(CurrentUI)).SetContainerName("...");
                                logger.LogDebug("Reset Text");
                            }

                        }
                        if (resetStorageDemandToNull.Value)
                        {
                            if (woText.GetText() != null && !woText.GetText().Contains("" + exclusion.Value))
                            {
                                fromInventory.GetLogisticEntity().ClearDemandGroups();
                                logger.LogDebug("Reset Demand groups");
                            }
                        }
                    }
                    if (toInventory.IsEmpty())
                    {

                        if (setDemandToFirstObjectStored.Value)
                        {
                            if (woText.GetText() != null && !woText.GetText().Contains("" + exclusion.Value))
                            {
                                toInventory.GetLogisticEntity().ClearDemandGroups();
                                toInventory.GetLogisticEntity().AddDemandGroup(group);
                                logger.LogDebug("Set Demand to : " + Readable.GetGroupName(group));
                            }

                        }
                        if (setNameToFirstObjectStored.Value)
                        {
                            if (woText.GetText() != null && !woText.GetText().Contains("" + exclusion.Value))
                            {
                                if (addAsterisk.Value)
                                {
                                    woText.SetText($"*{Readable.GetGroupName(group)}");
                                    ((UiWindowContainer)Managers.GetManager<WindowsHandler>().GetWindowViaUiId(CurrentUI)).SetContainerName(Readable.GetGroupName(group));
                                    logger.LogDebug("Set Text to : *" + Readable.GetGroupName(group));
                                }
                                else
                                {
                                    woText.SetText(Readable.GetGroupName(group));
                                    ((UiWindowContainer)Managers.GetManager<WindowsHandler>().GetWindowViaUiId(CurrentUI)).SetContainerName(Readable.GetGroupName(group));
                                    logger.LogDebug("Set Text to : " + Readable.GetGroupName(group));
                                }
                            }

                        }
                    }


                }
            }
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(InventoryDisplayer), "OnImageClicked")]
        [HarmonyPatch(typeof(InventoryDisplayer), "OnActionViaGamepad")]
        static void InventoryDisplayer_OnImageClicked(EventTriggerCallbackData eventTriggerCallbackData, Inventory ____inventory)
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
                    if (____inventory.IsEmpty())
                    {
                        logger.LogDebug("Empty Inventory");
                        if (resetStorageNameToNull.Value)
                        {
                            if (woText.GetText() != null && !woText.GetText().Contains("" + exclusion.Value))
                            {
                                woText.SetText("...");
                                ((UiWindowContainer)Managers.GetManager<WindowsHandler>().GetWindowViaUiId(CurrentUI)).SetContainerName("...");
                                logger.LogDebug("Reset Text");
                            }

                        }
                        if (resetStorageDemandToNull.Value)
                        {
                            if (woText.GetText() != null && !woText.GetText().Contains("" + exclusion.Value))
                            {
                                ____inventory.GetLogisticEntity().ClearDemandGroups();
                                logger.LogDebug("Reset Demand groups");
                            }
                        }
                    }
                    foreach (WorldObject worldObject in otherInventory.GetInsideWorldObjects())
                    {
                        if (count == 0)
                        {
                            name = Readable.GetGroupName(worldObject.GetGroup());
                            temp = worldObject;

                        }
                        count++;
                    }

                    if (!otherInventory.IsEmpty() && count < 2 && temp != null)
                    {

                        if (setDemandToFirstObjectStored.Value)
                        {
                            if (woText.GetText() != null && !woText.GetText().Contains("" + exclusion.Value))
                            {
                                otherInventory.GetLogisticEntity().ClearDemandGroups();
                                otherInventory.GetLogisticEntity().AddDemandGroup(temp.GetGroup());
                                logger.LogDebug("Set Demand to : " + name);
                            }
                            //SetDemandOfContainer

                        }
                        if (setNameToFirstObjectStored.Value)
                        {
                            if (woText.GetText() != null && !woText.GetText().Contains("" + exclusion.Value))
                            {
                                if (addAsterisk.Value)
                                {
                                    woText.SetText($"*{name}");
                                    ((UiWindowContainer)Managers.GetManager<WindowsHandler>().GetWindowViaUiId(CurrentUI)).SetContainerName(name);
                                    logger.LogDebug("Set Text to : *" + name);
                                }
                                else
                                {
                                    woText.SetText(name);
                                    ((UiWindowContainer)Managers.GetManager<WindowsHandler>().GetWindowViaUiId(CurrentUI)).SetContainerName(name);
                                    logger.LogDebug("Set Text to : " + name);
                                }
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
