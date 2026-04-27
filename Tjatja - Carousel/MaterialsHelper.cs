// Copyright 2025-2026 Nicolas Schäfer & Contributors
// Licensed under Apache License, Version 2.0

using BepInEx.Logging;
using HarmonyLib;
using SpaceCraft;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Nicki0
{

    internal class MaterialsHelper
    {

        static ManualLogSource log;

        class Nicki0_MaterialsHelper : MonoBehaviour
        {
            public static readonly Version VERSION = new Version(1, 4);
            public static readonly string GOName = "Nicki0_MaterialsHelperObject_v";

            public Dictionary<string, Material> materialDictionary;
            public Dictionary<string, Material> completeMaterialDictionary;
        }
        static GameObject materialsHelperObject;
        public static void InitMaterialsHelper(ManualLogSource pLog)
        {
            log = pLog;

            materialsHelperObject = GameObject.Find(Nicki0_MaterialsHelper.GOName + Nicki0_MaterialsHelper.VERSION.ToString());
            if (materialsHelperObject == null)
            {
                materialsHelperObject = new GameObject(Nicki0_MaterialsHelper.GOName + Nicki0_MaterialsHelper.VERSION.ToString());

                materialsHelperObject.AddComponent<Nicki0_MaterialsHelper>();

                GameObject.DontDestroyOnLoad(materialsHelperObject);

                log.LogInfo("This plugin creates the MaterialsHelper dictionary for version " + Nicki0_MaterialsHelper.VERSION.ToString());

                Harmony.CreateAndPatchAll(typeof(MaterialsHelper));
            }
        }


        public static bool ApplyGameMaterials(GameObject toFix, bool normalizeName = true, bool fromCompleteCollection = false, bool setSharedMaterials = false)
        {
            if (materialsHelperObject == null)
            {
                Console.WriteLine("[Fatal] MaterialsHelper not initialized");
                return false;
            }
            Dictionary<string, Material> materialDictionary = null;

            string nameOfDictionaryToUse = fromCompleteCollection ? nameof(Nicki0_MaterialsHelper.completeMaterialDictionary) : nameof(Nicki0_MaterialsHelper.materialDictionary);

            if (!TryGetMaterialDictionary(nameOfDictionaryToUse, out materialDictionary))
            {
                return false;
            }

            if (materialDictionary == null)
            {
                log.LogFatal("Materials not yet initialized");
                return false;
            }

            if (toFix == null)
            {
                log.LogError("GameObject is null");
                return false;
            }

            foreach (Renderer renderer in toFix.GetComponentsInChildren<MeshRenderer>())
            {
                if (renderer == null) { continue; }
                Material[] materials = setSharedMaterials ? renderer.GetSharedMaterialArray() : renderer.GetMaterialArray();
                if (materials == null) { continue; }

                for (int i = 0; i < materials.Length; i++)
                {
                    if (materialDictionary.TryGetValue(normalizeName ? materials[i].name.CanonicalizeString() : materials[i].name, out Material gameMaterial))
                    {
                        materials[i] = gameMaterial;
                    }
                }

                if (setSharedMaterials)
                {
                    renderer.SetSharedMaterials(materials.ToList());
                }
                else
                {
                    renderer.SetMaterialArray(materials);
                }
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        [HarmonyPatch(typeof(StaticDataHandler), "LoadStaticData")]
        private static void StaticDataHandler_LoadStaticData(List<GroupData> ___groupsData)
        {
            Nicki0_MaterialsHelper materialsHelper = materialsHelperObject.GetComponent<Nicki0_MaterialsHelper>();
            if (materialsHelper.materialDictionary != null)
            {
                return;
            }
            materialsHelper.materialDictionary = new Dictionary<string, Material>();
            materialsHelper.completeMaterialDictionary = new Dictionary<string, Material>();

            // --- Add materials from GroupDataConstructable ---
            // ~18ms
            foreach (GroupData gd in ___groupsData)
            {
                if (gd == null || gd.associatedGameObject == null) continue;

                //foreach (MeshRenderer renderer in gd.associatedGameObject.GetComponentsInChildren<MeshRenderer>()) {
                foreach (Renderer renderer in gd.associatedGameObject.GetComponentsInChildren<Renderer>())
                {
                    if (renderer == null) continue;

                    foreach (Material material in renderer.GetSharedMaterialArray())
                    {
                        if (material == null) continue;
                        materialsHelper.materialDictionary.TryAdd(material.name, material);
                    }
                }
            }

            // ~0.4ms
            if (Managers.GetManager<VisualsResourcesHandler>() != null)
            {
                foreach (MethodInfo mi in AccessTools.GetDeclaredMethods(typeof(VisualsResourcesHandler)))
                {
                    if (mi.ReturnType == typeof(Material) && mi.GetParameters().Length == 0)
                    {
                        Material returnedMaterial = mi.Invoke(Managers.GetManager<VisualsResourcesHandler>(), []) as Material;
                        if (returnedMaterial == null || string.IsNullOrEmpty(returnedMaterial.name)) continue;
                        materialsHelper.materialDictionary.TryAdd(returnedMaterial.name, returnedMaterial);
                    }
                }
            }


            foreach (GroupData gd in ___groupsData)
            {
                if (gd == null || gd.associatedGameObject == null) continue;

                foreach (MaterialList lst in gd.associatedGameObject.GetComponentsInChildren<MaterialList>())
                {
                    if (lst == null) continue;

                    foreach (Material material in lst.materials)
                    {
                        if (material == null) continue;
                        materialsHelper.materialDictionary.TryAdd(material.name, material);
                    }
                }
            }


            // --- Add materials of all Materials ---
            // ~0.6 ms
            foreach (Material m in Resources.FindObjectsOfTypeAll(typeof(Material)))
            {
                if (m == null) continue;
                materialsHelper.completeMaterialDictionary.TryAdd(m.name, m);
            }

        }

        public static Dictionary<string, Material> GetMaterials(bool completeCollection = false)
        {
            if (materialsHelperObject == null)
            {
                Console.WriteLine("[Fatal] MaterialsHelper not initialized");
                return null;
            }
            string dictToUse = completeCollection ? nameof(Nicki0_MaterialsHelper.completeMaterialDictionary) : nameof(Nicki0_MaterialsHelper.materialDictionary);
            if (TryGetMaterialDictionary(dictToUse, out Dictionary<string, Material> materialsDictionary))
            {
                return materialsDictionary;
            }
            log.LogFatal("material dictionary not found");
            return null;
        }

        private static bool TryGetMaterialDictionary(string nameOfDictionaryToUse, out Dictionary<string, Material> materialDictionary)
        {
            bool foundHaterialsHelperObject = false;
            bool foundHaterialsHelperObjectDictionary = false;
            foreach (MonoBehaviour script in materialsHelperObject.GetComponents<MonoBehaviour>())
            {
                if (script == null || string.IsNullOrEmpty(script.GetScriptClassName()) || script.GetType() == null) { continue; }

                if (script.GetScriptClassName().Contains(nameof(Nicki0_MaterialsHelper)))
                {
                    foundHaterialsHelperObject = true;
                    if (script.GetType().GetFields().Select(e => e.Name == nameOfDictionaryToUse).Any())
                    {
                        foundHaterialsHelperObjectDictionary = true;
                        try
                        {
                            materialDictionary = AccessTools.FieldRefAccess<Dictionary<string, Material>>(script.GetType(), nameOfDictionaryToUse).Invoke(script);
                            return true;
                        }
                        catch
                        {
                            log.LogError("Could not get dictionary from helper script");
                            materialDictionary = null;
                            return false;
                        }
                    }
                }
            }

            if (!foundHaterialsHelperObject)
            {
                log.LogFatal("Nicki0_MaterialsHelper script not found");
                materialDictionary = null;
                return false;
            }
            if (!foundHaterialsHelperObjectDictionary)
            {
                log.LogFatal("Nicki0_MaterialsHelper dictionary not found");
                materialDictionary = null;
                return false;
            }
            materialDictionary = null; // dictionary wasn't found.
            return false;
        }
    }
    public static class StringExtension
    {
        public static string CanonicalizeString(this string str)
        {
            return str.Replace("(Instance)", "").Replace("(Clone)", "").Trim();
        }
    }
}