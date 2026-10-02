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
using static UnityEngine.ExpressionEvaluator;
using static UnityEngine.ParticleSystem;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace AttachAPod;

public class AttachPodMerger : MonoBehaviour
{
    private GameObject rootPod0;//top
    private GameObject rootPod1;//bot
    private GameObject rootPod2;//right
    private GameObject rootPod3;//left
    private GameObject rootPod4;//roof
    private GameObject rootPod5;//floor
    private GameObject rootPod6;
    private GameObject rootPod7;
    private GameObject rootPod8;
    private GameObject rootPod9;
    private GameObject rootPod10;
    private GameObject rootPod11;
    private GameObject rootPod12;
    private GameObject rootPod13;
    private GameObject rootPod14;
    private GameObject rootPod15;
    private GameObject rootPod16;
    private GameObject rootPod17;
    private GameObject rootPod18;
    private GameObject rootPod19;
    private GameObject rootPod20;
    private GameObject rootPod21;
    private GameObject rootPod22;
    private GameObject rootPod23;
    private GameObject rootPod24;
    private GameObject rootPod25;
    private GameObject rootPod26;
    public GameObject Wall_Window_Top;
    public GameObject Wall_Window_Bot;
    public GameObject Wall_Window_Left;
    public GameObject Wall_Window_Right;
    public Panel Wall_Window_TopP;
    public Panel Wall_Window_BotP;
    public Panel Wall_Window_LeftP;
    public Panel Wall_Window_RightP;
    public Panel RoofP;
    public Panel FloorP;
    //corner pillars
    public GameObject BotRightLong;
    public GameObject BotLeftLong;
    public GameObject TopRightLong;
    public GameObject TopLeftLong;
    public bool GarageDoorTop;
    public bool GarageDoorBot;
    public bool GarageDoorRight;
    public bool GarageDoorLeft;
    public bool DisableTop;
    public bool DisableBot;
    public bool DisableRight;
    public bool DisableLeft;
    //roof elements
    public GameObject Roof;
    public GameObject Ceiling;
    //floor elements
    public GameObject Floor;
    public BoxCollider SurfaceFloor;
    //construction elements
    public GameObject LeftGirder;
    public GameObject RightGirder;
    public GameObject TopGirder;
    public GameObject BotGirder;
    public GameObject LeftGirderF;
    public GameObject RightGirderF;
    public GameObject TopGirderF;
    public GameObject BotGirderF;
    public GameObject TriggerDeconstruction;
    public bool init = false;
    public bool first = true;
    private float update;
    private static RaycastHit[] _castBuffer = new RaycastHit[1024];

    public void Start()
    {
        if (this.transform.root.GetComponentInChildren<ConstructibleGhost>() != null) return;
        InitSelf();
    }
    public void OnEnable()
    {
        if (this.transform.root.GetComponentInChildren<ConstructibleGhost>() != null) return;
        this.StopAllCoroutines();
        this.StartCoroutine(ExecuteLater());
    }
    public IEnumerator ExecuteLater()
    {
        while (this.transform.root.GetComponentInChildren<ConstructibleGhost>() != null)
        {
            yield return new WaitForSeconds(5);
        }
        yield return new WaitForSeconds(5);
        CheckSurroundingPods();
        CheckForGarageDoors();
        WaitForSeconds checkingTimer = new WaitForSeconds(UnityEngine.Random.Range(3.0f, 6.0f));

        while (true)
        {
            DoStuff();
            yield return checkingTimer;
        }
    }

    public void InitSelf()
    {
        if (this.transform.root.GetComponentInChildren<ConstructibleGhost>() != null) return;
        bool podF = false;
        if (this.transform.root.name.Contains("AttachFPod"))
        {
            podF = true;
        }
        if (this.transform.root.name.Contains("AttachPod")|| podF)
        {
            rootPod13 = this.transform.root.gameObject;
            foreach (Transform t in rootPod13.transform.root.GetComponentsInChildren<Transform>())
            {
                switch (t.name)
                {
                    case "AttachAPodCollider":

                        break;
                    case "Wall_Window_Top":
                        Wall_Window_Top = t.gameObject;
                        Wall_Window_TopP = Wall_Window_Top.GetComponentInChildren<Panel>();
                        break;
                    case "Wall_Window_Bot":
                        Wall_Window_Bot = t.gameObject;
                        Wall_Window_BotP = Wall_Window_Bot.GetComponentInChildren<Panel>();
                        break;
                    case "Wall_Window_Left":
                        Wall_Window_Left = t.gameObject;
                        Wall_Window_LeftP = Wall_Window_Left.GetComponentInChildren<Panel>();
                        break;
                    case "Wall_Window_Right":
                        Wall_Window_Right = t.gameObject;
                        Wall_Window_RightP = Wall_Window_Right.GetComponentInChildren<Panel>();
                        break;
                    //roof start
                    case "Roof":
                        Roof = t.gameObject;
                        RoofP = Roof.GetComponentInChildren<Panel>();
                        break;
                    case "Ceiling":
                        Ceiling = t.gameObject;
                        break;
                    //floor start
                    case "Floor":
                        Floor = t.gameObject;
                        FloorP = Floor.GetComponentInChildren<Panel>();
                        break;
                    case "SurfaceFloor":
                        SurfaceFloor = t.gameObject.GetComponentInChildren<BoxCollider>();
                        break;
                    //Wall
                    case "BotRightLong":
                        BotRightLong = t.gameObject;
                        break;
                    case "BotLeftLong":
                        BotLeftLong = t.gameObject;
                        break;
                    case "TopRightLong":
                        TopRightLong = t.gameObject;
                        break;
                    case "TopLeftLong":
                        TopLeftLong = t.gameObject;
                        break;
                    //Roof
                    case "LeftGirder":
                        LeftGirder = t.gameObject;
                        break;
                    case "RightGirder":
                        RightGirder = t.gameObject;
                        break;
                    case "TopGirder":
                        TopGirder = t.gameObject;
                        break;
                    case "BotGirder":
                        BotGirder = t.gameObject;
                        break;
                    //Floor
                    case "LeftGirder(f)":
                        LeftGirderF = t.gameObject;
                        break;
                    case "RightGirder(f)":
                        RightGirderF = t.gameObject;
                        break;
                    case "TopGirder(f)":
                        TopGirderF = t.gameObject;
                        break;
                    case "BotGirder(f)":
                        BotGirderF = t.gameObject;
                        break;
                    case "TriggerDeconstruction":
                        TriggerDeconstruction = t.gameObject;
                        t.gameObject.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
                        break;

                }
            }
        }
    }

    public void CheckForGarageDoors()
    {
        float raycastDistance = 6f;
        if (rootPod10 == null)
        {
            DisableTop = GetContingousGarageDoor(rootPod13, raycastDistance, 0);
        }   
        if (rootPod16 == null)
        {
            DisableBot = GetContingousGarageDoor(rootPod13, raycastDistance, 1);
        }
        if (rootPod14 == null)
        {
            DisableRight = GetContingousGarageDoor(rootPod13, raycastDistance, 2);
        }
        if (rootPod12 == null)
        {
            DisableLeft = GetContingousGarageDoor(rootPod13, raycastDistance, 3);
        }
    }

    public void CheckSurroundingPods()
    {
        float raycastDistance = 8.6f;//8.1f; 
        float raycastDistanceR = 10.5f;//10f; 
        float raycastDistanceT = 8.495f; //7.995f;
        if (rootPod10 == null)
        {
            rootPod10 = GetContingousPods(rootPod13, raycastDistanceT, 0);
            if (rootPod10 != null) rootPod10.GetComponentInChildren<AttachPodMerger>().rootPod16 = rootPod13;
        }
        if (rootPod16 == null)
        {
            rootPod16 = GetContingousPods(rootPod13, raycastDistanceT, 1);
            if (rootPod16 != null) rootPod16.GetComponentInChildren<AttachPodMerger>().rootPod10 = rootPod13;
        }
        if (rootPod14 == null)
        {
            rootPod14 = GetContingousPods(rootPod13, raycastDistance, 2);
            if (rootPod14 != null) rootPod14.GetComponentInChildren<AttachPodMerger>().rootPod12 = rootPod13;
        }
        if (rootPod12 == null)
        {
            rootPod12 = GetContingousPods(rootPod13, raycastDistance, 3);
            if (rootPod12 != null) rootPod12.GetComponentInChildren<AttachPodMerger>().rootPod14 = rootPod13;
        }
        if (rootPod22 == null)
        {
            rootPod22 = GetContingousPods(rootPod13, raycastDistanceR, 4);
            if (rootPod22 != null) rootPod22.GetComponentInChildren<AttachPodMerger>().rootPod4 = rootPod13;
        }
        if (rootPod4 == null)
        {
            rootPod4 = GetContingousPods(rootPod13, raycastDistanceR, 5);
            if (rootPod4 != null) rootPod4.GetComponentInChildren<AttachPodMerger>().rootPod22 = rootPod13;
        }

        if (rootPod12 != null)
        {
            if (rootPod9 == null)
            {
                rootPod9 = GetContingousPods(rootPod12, raycastDistance, 0);
                if (rootPod9 != null) rootPod9.GetComponentInChildren<AttachPodMerger>().rootPod16 = rootPod12;
            }
            if (rootPod15 == null)
            {
                rootPod15 = GetContingousPods(rootPod12, raycastDistance, 1);
                if (rootPod15 != null) rootPod15.GetComponentInChildren<AttachPodMerger>().rootPod10 = rootPod12;
            }
        }
        if (rootPod9 == null && rootPod10 != null)
        {
            rootPod9 = GetContingousPods(rootPod10, raycastDistance, 3);
            if (rootPod9 != null) rootPod9.GetComponentInChildren<AttachPodMerger>().rootPod14 = rootPod10;
        }
        if (rootPod15 == null && rootPod16 != null)
        {
            rootPod15 = GetContingousPods(rootPod16, raycastDistance, 3);
            if (rootPod15 != null) rootPod15.GetComponentInChildren<AttachPodMerger>().rootPod14 = rootPod16;
        }


        if (rootPod14 != null)
        {
            if (rootPod11 == null)
            {
                rootPod11 = GetContingousPods(rootPod14, raycastDistance, 0);
                if (rootPod11 != null) rootPod11.GetComponentInChildren<AttachPodMerger>().rootPod16 = rootPod14;
            }
            if (rootPod17 == null)
            {
                rootPod17 = GetContingousPods(rootPod14, raycastDistance, 1);
                if (rootPod17 != null) rootPod17.GetComponentInChildren<AttachPodMerger>().rootPod10 = rootPod14;
            }
        }
        if (rootPod11 == null && rootPod10 != null)
        {
            rootPod11 = GetContingousPods(rootPod10, raycastDistance, 2);
            if (rootPod11 != null) rootPod11.GetComponentInChildren<AttachPodMerger>().rootPod12 = rootPod10;
        }
        if (rootPod17 == null && rootPod16 != null)
        {
            rootPod17 = GetContingousPods(rootPod16, raycastDistance, 2);
            if (rootPod17 != null) rootPod17.GetComponentInChildren<AttachPodMerger>().rootPod12 = rootPod16;
        }

        if (rootPod22 != null)
        {
            if (rootPod19 == null)
            {
                rootPod19 = GetContingousPods(rootPod22, raycastDistance, 0);
                if (rootPod19 != null) rootPod19.GetComponentInChildren<AttachPodMerger>().rootPod16 = rootPod22;
            }
            if (rootPod25 == null)
            {
                rootPod25 = GetContingousPods(rootPod22, raycastDistance, 1);
                if (rootPod25 != null) rootPod25.GetComponentInChildren<AttachPodMerger>().rootPod10 = rootPod22;
            }
            if (rootPod23 == null)
            {
                rootPod23 = GetContingousPods(rootPod22, raycastDistance, 2);
                if (rootPod23 != null) rootPod23.GetComponentInChildren<AttachPodMerger>().rootPod12 = rootPod22;
            }
            if (rootPod21 == null)
            {
                rootPod21 = GetContingousPods(rootPod22, raycastDistance, 3);
                if (rootPod21 != null) rootPod21.GetComponentInChildren<AttachPodMerger>().rootPod14 = rootPod22;
            }
        }
        if (rootPod19 == null && rootPod10 != null)
        {
            rootPod19 = GetContingousPods(rootPod10, raycastDistanceR, 4);
            if (rootPod19 != null) rootPod19.GetComponentInChildren<AttachPodMerger>().rootPod4 = rootPod10;
        }
        if (rootPod25 == null && rootPod16 != null)
        {
            rootPod25 = GetContingousPods(rootPod16, raycastDistanceR, 4);
            if (rootPod25 != null) rootPod25.GetComponentInChildren<AttachPodMerger>().rootPod4 = rootPod16;
        }
        if (rootPod23 == null && rootPod14 != null)
        {
            rootPod23 = GetContingousPods(rootPod14, raycastDistanceR, 4);
            if (rootPod23 != null) rootPod23.GetComponentInChildren<AttachPodMerger>().rootPod4 = rootPod14;
        }
        if (rootPod21 == null && rootPod12 != null)
        {
            rootPod21 = GetContingousPods(rootPod12, raycastDistanceR, 4);
            if (rootPod21 != null) rootPod21.GetComponentInChildren<AttachPodMerger>().rootPod4 = rootPod12;
        }


        if (rootPod21 != null)
        {
            if (rootPod18 == null)
            {
                rootPod18 = GetContingousPods(rootPod21, raycastDistance, 0);
                if (rootPod18 != null) rootPod18.GetComponentInChildren<AttachPodMerger>().rootPod16 = rootPod21;
            }
            if (rootPod24 == null)
            {
                rootPod24 = GetContingousPods(rootPod21, raycastDistance, 1);
                if (rootPod24 != null) rootPod24.GetComponentInChildren<AttachPodMerger>().rootPod10 = rootPod21;
            }
        }
        if (rootPod18 == null && rootPod19 != null)
        {
            rootPod18 = GetContingousPods(rootPod19, raycastDistance, 3);
            if (rootPod18 != null) rootPod18.GetComponentInChildren<AttachPodMerger>().rootPod14 = rootPod19;
        }
        if (rootPod24 == null && rootPod25 != null)
        {
            rootPod24 = GetContingousPods(rootPod25, raycastDistance, 3);
            if (rootPod24 != null) rootPod24.GetComponentInChildren<AttachPodMerger>().rootPod14 = rootPod25;
        }

        if (rootPod23 != null)
        {
            if (rootPod20 == null)
            {
                rootPod20 = GetContingousPods(rootPod23, raycastDistance, 0);
                if (rootPod20 != null) rootPod20.GetComponentInChildren<AttachPodMerger>().rootPod16 = rootPod23;
            }
            if (rootPod26 == null)
            {
                rootPod26 = GetContingousPods(rootPod23, raycastDistance, 1);
                if (rootPod26 != null) rootPod26.GetComponentInChildren<AttachPodMerger>().rootPod10 = rootPod23;
            }
        }
        if (rootPod20 == null && rootPod19 != null)
        {
            rootPod20 = GetContingousPods(rootPod19, raycastDistance, 2);
            if (rootPod20 != null) rootPod20.GetComponentInChildren<AttachPodMerger>().rootPod12 = rootPod19;
        }
        if (rootPod26 == null && rootPod25 != null)
        {
            rootPod26 = GetContingousPods(rootPod25, raycastDistance, 2);
            if (rootPod26 != null) rootPod26.GetComponentInChildren<AttachPodMerger>().rootPod12 = rootPod25;
        }

        if (rootPod4 != null)
        {
            if (rootPod1 == null)
            {
                rootPod1 = GetContingousPods(rootPod4, raycastDistance, 0);
                if (rootPod1 != null) rootPod1.GetComponentInChildren<AttachPodMerger>().rootPod16 = rootPod4;
            }
            if (rootPod7 == null)
            {
                rootPod7 = GetContingousPods(rootPod4, raycastDistance, 1);
                if (rootPod7 != null) rootPod7.GetComponentInChildren<AttachPodMerger>().rootPod10 = rootPod4;
            }
            if (rootPod5 == null)
            {
                rootPod5 = GetContingousPods(rootPod4, raycastDistance, 2);
                if (rootPod5 != null) rootPod5.GetComponentInChildren<AttachPodMerger>().rootPod12 = rootPod4;
            }
            if (rootPod3 == null)
            {
                rootPod3 = GetContingousPods(rootPod4, raycastDistance, 3);
                if (rootPod3 != null) rootPod3.GetComponentInChildren<AttachPodMerger>().rootPod14 = rootPod4;
            }
        }
        if (rootPod1 == null && rootPod10 != null)
        {
            rootPod1 = GetContingousPods(rootPod10, raycastDistanceR, 5);
            if (rootPod1 != null) rootPod1.GetComponentInChildren<AttachPodMerger>().rootPod22 = rootPod10;
        }
        if (rootPod7 == null && rootPod16 != null)
        {
            rootPod7 = GetContingousPods(rootPod16, raycastDistanceR, 5);
            if (rootPod7 != null) rootPod7.GetComponentInChildren<AttachPodMerger>().rootPod22 = rootPod16;
        }
        if (rootPod5 == null && rootPod14 != null)
        {
            rootPod5 = GetContingousPods(rootPod14, raycastDistanceR, 5);
            if (rootPod5 != null) rootPod5.GetComponentInChildren<AttachPodMerger>().rootPod22 = rootPod14;
        }
        if (rootPod3 == null && rootPod12 != null)
        {
            rootPod3 = GetContingousPods(rootPod12, raycastDistanceR, 5);
            if (rootPod3 != null) rootPod3.GetComponentInChildren<AttachPodMerger>().rootPod22 = rootPod12;
        }

        if (rootPod3 != null)
        {
            if (rootPod0 == null)
            {
                rootPod0 = GetContingousPods(rootPod3, raycastDistance, 0);
                if (rootPod0 != null) rootPod0.GetComponentInChildren<AttachPodMerger>().rootPod16 = rootPod3;
            }
            if (rootPod6 == null)
            {
                rootPod6 = GetContingousPods(rootPod3, raycastDistance, 1);
                if (rootPod6 != null) rootPod6.GetComponentInChildren<AttachPodMerger>().rootPod10 = rootPod3;
            }
        }
        if (rootPod0 == null && rootPod1 != null)
        {
            rootPod0 = GetContingousPods(rootPod1, raycastDistance, 3);
            if (rootPod0 != null) rootPod0.GetComponentInChildren<AttachPodMerger>().rootPod14 = rootPod1;
        }
        if (rootPod6 == null && rootPod7 != null)
        {
            rootPod6 = GetContingousPods(rootPod7, raycastDistance, 3);
            if (rootPod6 != null) rootPod6.GetComponentInChildren<AttachPodMerger>().rootPod14 = rootPod7;
        }

        if (rootPod5 != null)
        {
            if (rootPod2 == null)
            {
                rootPod2 = GetContingousPods(rootPod5, raycastDistance, 0);
                if (rootPod2 != null) rootPod2.GetComponentInChildren<AttachPodMerger>().rootPod16 = rootPod5;
            }
            if (rootPod8 == null)
            {
                rootPod8 = GetContingousPods(rootPod5, raycastDistance, 1);
                if (rootPod8 != null) rootPod8.GetComponentInChildren<AttachPodMerger>().rootPod10 = rootPod5;
            }
        }
        if (rootPod2 == null && rootPod1 != null)
        {
            rootPod2 = GetContingousPods(rootPod1, raycastDistance, 2);
            if (rootPod2 != null) rootPod2.GetComponentInChildren<AttachPodMerger>().rootPod12 = rootPod1;
        }
        if (rootPod8 == null && rootPod7 != null)
        {
            rootPod8 = GetContingousPods(rootPod7, raycastDistance, 2);
            if (rootPod8 != null) rootPod8.GetComponentInChildren<AttachPodMerger>().rootPod12 = rootPod7;
        }
    }

    public void Destroy()
    {
        if (rootPod0 != null)
            rootPod0.GetComponentInChildren<AttachPodMerger>().rootPod26 = null;
        if (rootPod1 != null)
            rootPod1.GetComponentInChildren<AttachPodMerger>().rootPod25 = null;
        if (rootPod2 != null)
            rootPod2.GetComponentInChildren<AttachPodMerger>().rootPod24 = null;
        if (rootPod3 != null)
            rootPod3.GetComponentInChildren<AttachPodMerger>().rootPod23 = null;
        if (rootPod4 != null)
            rootPod4.GetComponentInChildren<AttachPodMerger>().rootPod22 = null;
        if (rootPod5 != null)
            rootPod5.GetComponentInChildren<AttachPodMerger>().rootPod21 = null;
        if (rootPod6 != null)
            rootPod6.GetComponentInChildren<AttachPodMerger>().rootPod20 = null;
        if (rootPod7 != null)
            rootPod7.GetComponentInChildren<AttachPodMerger>().rootPod19 = null;
        if (rootPod8 != null)
            rootPod8.GetComponentInChildren<AttachPodMerger>().rootPod18 = null;
        if (rootPod9 != null)
            rootPod9.GetComponentInChildren<AttachPodMerger>().rootPod17 = null;
        if (rootPod10 != null)
            rootPod10.GetComponentInChildren<AttachPodMerger>().rootPod16 = null;
        if (rootPod11 != null)
            rootPod11.GetComponentInChildren<AttachPodMerger>().rootPod15 = null;
        if (rootPod12 != null)
            rootPod12.GetComponentInChildren<AttachPodMerger>().rootPod14 = null;
        if (rootPod13 != null)
            rootPod13.GetComponentInChildren<AttachPodMerger>().rootPod13 = null;
        if (rootPod14 != null)
            rootPod14.GetComponentInChildren<AttachPodMerger>().rootPod12 = null;
        if (rootPod15 != null)
            rootPod15.GetComponentInChildren<AttachPodMerger>().rootPod11 = null;
        if (rootPod16 != null)
            rootPod16.GetComponentInChildren<AttachPodMerger>().rootPod10 = null;
        if (rootPod17 != null)
            rootPod17.GetComponentInChildren<AttachPodMerger>().rootPod9 = null;
        if (rootPod18 != null)
            rootPod18.GetComponentInChildren<AttachPodMerger>().rootPod8 = null;
        if (rootPod19 != null)
            rootPod19.GetComponentInChildren<AttachPodMerger>().rootPod7 = null;
        if (rootPod20 != null)
            rootPod20.GetComponentInChildren<AttachPodMerger>().rootPod6 = null;
        if (rootPod21 != null)
            rootPod21.GetComponentInChildren<AttachPodMerger>().rootPod5 = null;
        if (rootPod22 != null)
            rootPod22.GetComponentInChildren<AttachPodMerger>().rootPod4 = null;
        if (rootPod23 != null)
            rootPod23.GetComponentInChildren<AttachPodMerger>().rootPod3 = null;
        if (rootPod24 != null)
            rootPod24.GetComponentInChildren<AttachPodMerger>().rootPod2 = null;
        if (rootPod25 != null)
            rootPod25.GetComponentInChildren<AttachPodMerger>().rootPod1 = null;
        if (rootPod26 != null)
            rootPod26.GetComponentInChildren<AttachPodMerger>().rootPod0 = null;
    }

    public bool GetContingousGarageDoor(GameObject start, float distanceToDetect, int type)
    {
        if (start == null) return false;
        if (start.transform.root.GetComponentInChildren<ConstructibleGhost>() != null) return false;
        GameObject target = start;
        BoxCollider boxy = null;
        foreach (Transform t in start.transform.root.GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.GetComponentInChildren<BoxCollider>() != null)
            {
                if (t.gameObject.GetComponentInChildren<BoxCollider>().name == "TriggerDeconstruction")
                {
                    boxy = t.gameObject.GetComponentInChildren<BoxCollider>();
                }
            }
        }
        if (boxy != null)
        {
            target = boxy.gameObject;
        }
        Vector3 direction = target.transform.position;
        switch (type)
        {
            case 0:
                direction = target.transform.forward;
                break;
            case 1:
                direction = target.transform.forward * -1f;
                break;
            case 2:
                direction = target.transform.right;
                break;
            case 3:
                direction = target.transform.right * -1f;
                break;
        }

        foreach (RaycastHit h in Physics.RaycastAll(target.transform.position, direction, 20f))
        {
            //Console.WriteLine($"base hit Garage door check: {h.transform.root.name}");
            if (h.transform.root.name == "AttachGaragePod(Clone)")
            {
                //Console.WriteLine($"Succesfully hit AttachGaragePod(Clone) at {h.transform.root.position} own position at {target.transform.root.position}");
                if (target.transform.root != h.transform.root)
                {
                    float num1 = 12f; //z
                    float num2 = 12f; //x axis
                    float num3 = 5f; //y
                    switch (type)
                    {
                        case 0://top

                            if (Math.Abs(target.transform.root.position.z - h.transform.root.position.z) < num1 &&
                                Math.Abs(target.transform.root.position.x - h.transform.root.position.x) < num2 &&
                                Math.Abs(target.transform.root.position.y - h.transform.root.position.y) < num3 )
                            {
                                if (h.transform.gameObject.name.Contains("TriggerDeconstruction"))
                                {
                                    if (h.transform.root.GetComponentInChildren<ConstructibleGhost>() == null)
                                    {
                                        return true;
                                    }
                                }
                            }
                            break;
                        case 1://bot
                            if (Math.Abs(target.transform.root.position.z - h.transform.root.position.z) < num1 &&
                                Math.Abs(target.transform.root.position.x - h.transform.root.position.x) < num2 &&
                                Math.Abs(target.transform.root.position.y - h.transform.root.position.y) < num3)
                            {
                                if (h.transform.gameObject.name.Contains("TriggerDeconstruction"))
                                {
                                    if (h.transform.root.GetComponentInChildren<ConstructibleGhost>() == null)
                                    {
                                        return true;
                                    }
                                }
                            }
                            break;
                        case 2://right
                            if (Math.Abs(target.transform.root.position.z - h.transform.root.position.z) < num1 &&
                                Math.Abs(target.transform.root.position.x - h.transform.root.position.x) < num2 &&
                                Math.Abs(target.transform.root.position.y - h.transform.root.position.y) < num3)
                            {
                                if (h.transform.gameObject.name.Contains("TriggerDeconstruction"))
                                {
                                    if (h.transform.root.GetComponentInChildren<ConstructibleGhost>() == null)
                                    {
                                        return true;
                                    }
                                }
                            }
                            break;
                        case 3://left
                            if (Math.Abs(target.transform.root.position.z - h.transform.root.position.z) < num1 &&
                                Math.Abs(target.transform.root.position.x - h.transform.root.position.x) < num2 &&
                                Math.Abs(target.transform.root.position.y - h.transform.root.position.y) < num3)
                            {
                                if (h.transform.gameObject.name.Contains("TriggerDeconstruction"))
                                {
                                    if (h.transform.root.GetComponentInChildren<ConstructibleGhost>() == null)
                                    {
                                        return true;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                continue;
            }
        }
        return false;
    }

    public GameObject GetContingousPods(GameObject start, float distanceToDetect, int type)
    {
        if (start == null) return null;
        if (start.transform.root.GetComponentInChildren<ConstructibleGhost>() != null) return null;
        GameObject target = start;
        BoxCollider boxy = null;
        foreach (Transform t in start.transform.root.GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.GetComponentInChildren<BoxCollider>() != null)
            {
                if (t.gameObject.GetComponentInChildren<BoxCollider>().name == "AttachAPodCollider")
                {
                    boxy = t.gameObject.GetComponentInChildren<BoxCollider>();
                }
            }
        }
        if (boxy != null)
        {
            target = boxy.gameObject;
        }
        Vector3 direction = target.transform.position;
        switch (type)
        {
            case 0:
                direction = target.transform.forward;
                break;
            case 1:
                direction = target.transform.forward * -1f;
                break;
            case 2:
                direction = target.transform.right;
                break;
            case 3:
                direction = target.transform.right * -1f;
                break;
            case 4:
                direction = target.transform.up;
                break;
            case 5:
                direction = target.transform.up * -1f;
                break;

        }
        //RaycastHit
        foreach (RaycastHit h in Physics.RaycastAll(target.transform.position, direction, 20f))
        {
            if (h.transform.root.name == "AttachPod(Clone)" || h.transform.root.name == "AttachFPod(Clone)")
            {
                if (target.transform.root != h.transform.root)
                {
                    float num1 = distanceToDetect;
                    switch (type)
                    {
                        case 0:
                            if (Math.Abs(target.transform.root.position.z - h.transform.root.position.z) < num1 &&
                                Math.Abs(target.transform.root.position.z - h.transform.root.position.z) != 0 &&
                                Math.Abs(target.transform.root.position.x - h.transform.root.position.x) < 3.7f &&
                                Math.Abs(target.transform.root.position.y - h.transform.root.position.y) < 3.7f)
                            {

                                if (h.transform.gameObject.name == "AttachAPodCollider")
                                {
                                    if (h.transform.root.GetComponentInChildren<ConstructibleGhost>() == null)
                                    {
                                        return h.transform.root.gameObject;
                                    }
                                }
                            }
                            break;
                        case 1:
                            if (Math.Abs(target.transform.root.position.z - h.transform.root.position.z) < num1 &&
                                Math.Abs(target.transform.root.position.z - h.transform.root.position.z) != 0 &&
                                Math.Abs(target.transform.root.position.x - h.transform.root.position.x) < 3.7f &&
                                Math.Abs(target.transform.root.position.y - h.transform.root.position.y) < 3.7f)
                            {

                                if (h.transform.gameObject.name == "AttachAPodCollider")
                                {
                                    if (h.transform.root.GetComponentInChildren<ConstructibleGhost>() == null)
                                    {
                                        return h.transform.root.gameObject;
                                    }
                                }
                            }
                            break;
                        case 2:
                            if (Math.Abs(target.transform.root.position.x - h.transform.root.position.x) < num1 &&
                                Math.Abs(target.transform.root.position.x - h.transform.root.position.x) != 0 &&
                                Math.Abs(target.transform.root.position.y - h.transform.root.position.y) < 3.7f &&
                                Math.Abs(target.transform.root.position.z - h.transform.root.position.z) < 3.7f)
                            {

                                if (h.transform.gameObject.name == "AttachAPodCollider")
                                {
                                    if (h.transform.root.GetComponentInChildren<ConstructibleGhost>() == null)
                                    {
                                        return h.transform.root.gameObject;
                                    }
                                }
                            }
                            break;
                        case 3:
                            if (Math.Abs(target.transform.root.position.x - h.transform.root.position.x) < num1 &&
                                Math.Abs(target.transform.root.position.x - h.transform.root.position.x) != 0 &&
                                Math.Abs(target.transform.root.position.y - h.transform.root.position.y) < 3.7f &&
                                Math.Abs(target.transform.root.position.z - h.transform.root.position.z) < 3.7f)
                            {

                                if (h.transform.gameObject.name == "AttachAPodCollider")
                                {
                                    if (h.transform.root.GetComponentInChildren<ConstructibleGhost>() == null)
                                    {
                                        return h.transform.root.gameObject;
                                    }
                                }
                            }
                            break;
                        case 4:
                            if (Math.Abs(target.transform.root.position.y - h.transform.root.position.y) < num1 &&
                                Math.Abs(target.transform.root.position.y - h.transform.root.position.y) != 0 &&
                                Math.Abs(target.transform.root.position.x - h.transform.root.position.x) < 3.7f &&
                                Math.Abs(target.transform.root.position.z - h.transform.root.position.z) < 3.7f)
                            {
                                if (h.transform.gameObject.name == "AttachAPodCollider")
                                {
                                    if (h.transform.root.GetComponentInChildren<ConstructibleGhost>() == null)
                                    {
                                        return h.transform.root.gameObject;
                                    }
                                }
                            }
                            break;
                        case 5:
                            if (Math.Abs(target.transform.root.position.y - h.transform.root.position.y) < num1 &&
                                Math.Abs(target.transform.root.position.y - h.transform.root.position.y) != 0 &&
                                Math.Abs(target.transform.root.position.x - h.transform.root.position.x) < 3.7f &&
                                Math.Abs(target.transform.root.position.z - h.transform.root.position.z) < 3.7f)
                            {
                                if (h.transform.gameObject.name == "AttachAPodCollider")
                                {
                                    if (h.transform.root.GetComponentInChildren<ConstructibleGhost>() == null)
                                    {
                                        return h.transform.root.gameObject;
                                    }
                                }

                            }
                            break;
                    }
                }
            }
            else
            {
                continue;
            }
        }
        return null;
    }

    public bool CheckIfNotFloorPod(GameObject go)
    {
        if (go == null)
        {
            return true;
        }
        if (go.transform.root.name.Contains("AttachFPod(Clone)"))
        {
            return false;
        }

        return true;
    }

    public void DoStuff()
    {
        if (this.transform.root.GetComponentInChildren<ConstructibleGhost>() != null) return;
        CheckSurroundingPods();
        CheckForGarageDoors();

        if (rootPod22 != null)
        {
            GarageDoorTop = rootPod22.GetComponentInChildren<AttachPodMerger>().DisableTop;
            GarageDoorBot = rootPod22.GetComponentInChildren<AttachPodMerger>().DisableBot;
            GarageDoorRight = rootPod22.GetComponentInChildren<AttachPodMerger>().DisableRight;
            GarageDoorLeft = rootPod22.GetComponentInChildren<AttachPodMerger>().DisableLeft;
        }
        if (rootPod4 != null)
        {
            rootPod4.GetComponentInChildren<AttachPodMerger>().GarageDoorTop = DisableTop;
            rootPod4.GetComponentInChildren<AttachPodMerger>().GarageDoorBot = DisableBot;
            rootPod4.GetComponentInChildren<AttachPodMerger>().GarageDoorRight = DisableRight;
            rootPod4.GetComponentInChildren<AttachPodMerger>().GarageDoorLeft = DisableLeft;
        }

        if (rootPod10 != null && rootPod19 != null && rootPod22 != null)
        {
            TopGirder.SetActive(false);
            rootPod10.GetComponentInChildren<AttachPodMerger>().BotGirder.gameObject.SetActive(false);
            if (rootPod10.GetComponentInChildren<AttachPodMerger>().rootPod16 == null) {
                if (rootPod10 != null) rootPod10.GetComponentInChildren<AttachPodMerger>().rootPod16 = rootPod13;
            }
            if (CheckIfNotFloorPod(rootPod19))
                rootPod19.GetComponentInChildren<AttachPodMerger>().BotGirderF.gameObject.SetActive(false);
            if (rootPod19.GetComponentInChildren<AttachPodMerger>().rootPod7 == null)
            {
                if (rootPod19 != null) rootPod19.GetComponentInChildren<AttachPodMerger>().rootPod7 = rootPod13;
            }
            if (CheckIfNotFloorPod(rootPod22))
                rootPod22.GetComponentInChildren<AttachPodMerger>().TopGirderF.gameObject.SetActive(false);
            if (rootPod22.GetComponentInChildren<AttachPodMerger>().rootPod4 == null)
            {
                if (rootPod22 != null) rootPod22.GetComponentInChildren<AttachPodMerger>().rootPod4 = rootPod13;
            }
        }
        else
        {
            TopGirder.SetActive(!GarageDoorTop);
            if (rootPod10 != null)
                rootPod10.GetComponentInChildren<AttachPodMerger>().BotGirder.gameObject.SetActive(!rootPod10.GetComponentInChildren<AttachPodMerger>().GarageDoorBot);
            if (rootPod19 != null)
                rootPod19.GetComponentInChildren<AttachPodMerger>().BotGirderF.gameObject.SetActive(true);
            if (rootPod22 != null)
                rootPod22.GetComponentInChildren<AttachPodMerger>().TopGirderF.gameObject.SetActive(true);

        }
        if (rootPod12 != null && rootPod21 != null && rootPod22 != null)
        {
            LeftGirder.SetActive(false);
            rootPod12.GetComponentInChildren<AttachPodMerger>().RightGirder.gameObject.SetActive(false);
            if (rootPod12.GetComponentInChildren<AttachPodMerger>().rootPod14 == null)
            {
                if (rootPod12 != null) rootPod12.GetComponentInChildren<AttachPodMerger>().rootPod14 = rootPod13;
            }
            if (CheckIfNotFloorPod(rootPod21))
                rootPod21.GetComponentInChildren<AttachPodMerger>().RightGirderF.gameObject.SetActive(false);
            if (rootPod21.GetComponentInChildren<AttachPodMerger>().rootPod5 == null)
            {
                if (rootPod21 != null) rootPod21.GetComponentInChildren<AttachPodMerger>().rootPod5 = rootPod13;
            }
            if (CheckIfNotFloorPod(rootPod22))
                rootPod22.GetComponentInChildren<AttachPodMerger>().LeftGirderF.gameObject.SetActive(false);
            if (rootPod22.GetComponentInChildren<AttachPodMerger>().rootPod4 == null)
            {
                if (rootPod22 != null) rootPod22.GetComponentInChildren<AttachPodMerger>().rootPod4 = rootPod13;
            }
        }
        else
        {
            LeftGirder.SetActive(!GarageDoorLeft);
            
            if (rootPod12 != null)
                rootPod12.GetComponentInChildren<AttachPodMerger>().RightGirder.gameObject.SetActive(!rootPod12.GetComponentInChildren<AttachPodMerger>().GarageDoorRight);
            if (rootPod21 != null)
                rootPod21.GetComponentInChildren<AttachPodMerger>().RightGirderF.gameObject.SetActive(true);
            if (rootPod22 != null)
                rootPod22.GetComponentInChildren<AttachPodMerger>().LeftGirderF.gameObject.SetActive(true);

        }
        if (rootPod14 != null && rootPod23 != null && rootPod22 != null)
        {
            RightGirder.SetActive(false);
            rootPod14.GetComponentInChildren<AttachPodMerger>().LeftGirder.gameObject.SetActive(false);
            if (rootPod14.GetComponentInChildren<AttachPodMerger>().rootPod12 == null)
            {
                if (rootPod14 != null) rootPod14.GetComponentInChildren<AttachPodMerger>().rootPod12 = rootPod13;
            }
            if (CheckIfNotFloorPod(rootPod23))
                rootPod23.GetComponentInChildren<AttachPodMerger>().LeftGirderF.gameObject.SetActive(false);
            if (rootPod23.GetComponentInChildren<AttachPodMerger>().rootPod3 == null)
            {
                if (rootPod23 != null) rootPod23.GetComponentInChildren<AttachPodMerger>().rootPod3 = rootPod13;
            }
            if (CheckIfNotFloorPod(rootPod22))
                rootPod22.GetComponentInChildren<AttachPodMerger>().RightGirderF.gameObject.SetActive(false);
            if (rootPod22.GetComponentInChildren<AttachPodMerger>().rootPod4 == null)
            {
                if (rootPod22 != null) rootPod22.GetComponentInChildren<AttachPodMerger>().rootPod4 = rootPod13;
            }
        } 
        else
        {
                RightGirder.SetActive(!GarageDoorRight);
            
            if (rootPod14 != null)
                rootPod14.GetComponentInChildren<AttachPodMerger>().LeftGirder.gameObject.SetActive(!rootPod14.GetComponentInChildren<AttachPodMerger>().GarageDoorLeft);
            if (rootPod23 != null)
                rootPod23.GetComponentInChildren<AttachPodMerger>().LeftGirderF.gameObject.SetActive(true);
            if (rootPod22 != null)
                rootPod22.GetComponentInChildren<AttachPodMerger>().RightGirderF.gameObject.SetActive(true);

        }
        if (rootPod16 != null && rootPod25 != null && rootPod22 != null)
        {
            BotGirder.SetActive(false);
            rootPod16.GetComponentInChildren<AttachPodMerger>().TopGirder.gameObject.SetActive(false);
            if (rootPod16.GetComponentInChildren<AttachPodMerger>().rootPod10 == null)
            {
                if (rootPod16 != null) rootPod16.GetComponentInChildren<AttachPodMerger>().rootPod10 = rootPod13;
            }
            if (CheckIfNotFloorPod(rootPod25))
                rootPod25.GetComponentInChildren<AttachPodMerger>().TopGirderF.gameObject.SetActive(false);
            if (rootPod25.GetComponentInChildren<AttachPodMerger>().rootPod1 == null)
            {
                if (rootPod25 != null) rootPod25.GetComponentInChildren<AttachPodMerger>().rootPod1 = rootPod13;
            }
            if (CheckIfNotFloorPod(rootPod22))
                rootPod22.GetComponentInChildren<AttachPodMerger>().BotGirderF.gameObject.SetActive(false);
            if (rootPod22.GetComponentInChildren<AttachPodMerger>().rootPod4 == null)
            {
                if (rootPod22 != null) rootPod22.GetComponentInChildren<AttachPodMerger>().rootPod4 = rootPod13;
            }
        }
        else
        {
            BotGirder.SetActive(!GarageDoorBot);
            
            if (rootPod16 != null)
                rootPod16.GetComponentInChildren<AttachPodMerger>().TopGirder.gameObject.SetActive(!rootPod16.GetComponentInChildren<AttachPodMerger>().GarageDoorTop);
            if (rootPod25 != null)
                rootPod25.GetComponentInChildren<AttachPodMerger>().TopGirderF.gameObject.SetActive(true);
            if (rootPod22 != null)
                rootPod22.GetComponentInChildren<AttachPodMerger>().BotGirderF.gameObject.SetActive(true);

        }

        //floor corners
        if (rootPod10 != null && rootPod1 != null && rootPod4 != null)
        {
            if (CheckIfNotFloorPod(rootPod13))
                TopGirderF.SetActive(false);
            if (CheckIfNotFloorPod(rootPod10))
                rootPod10.GetComponentInChildren<AttachPodMerger>().BotGirderF.gameObject.SetActive(false);
            if (rootPod10.GetComponentInChildren<AttachPodMerger>().rootPod16 == null)
            {
                if (rootPod10 != null) rootPod10.GetComponentInChildren<AttachPodMerger>().rootPod16 = rootPod13;
            }
            rootPod1.GetComponentInChildren<AttachPodMerger>().BotGirder.gameObject.SetActive(false);
            if (rootPod1.GetComponentInChildren<AttachPodMerger>().rootPod25 == null)
            {
                if (rootPod1 != null) rootPod1.GetComponentInChildren<AttachPodMerger>().rootPod25 = rootPod13;
            }
            rootPod4.GetComponentInChildren<AttachPodMerger>().TopGirder.gameObject.SetActive(false);
            if (rootPod4.GetComponentInChildren<AttachPodMerger>().rootPod22 == null)
            {
                if (rootPod4 != null) rootPod4.GetComponentInChildren<AttachPodMerger>().rootPod22 = rootPod13;
            }
        } 
        else
        {
            TopGirderF.SetActive(true);
            
            if (rootPod10 != null)
                rootPod10.GetComponentInChildren<AttachPodMerger>().BotGirderF.gameObject.SetActive(true);
            if (rootPod1 != null)
                rootPod1.GetComponentInChildren<AttachPodMerger>().BotGirder.gameObject.SetActive(!rootPod1.GetComponentInChildren<AttachPodMerger>().GarageDoorBot);
            if (rootPod4 != null)
                rootPod4.GetComponentInChildren<AttachPodMerger>().TopGirder.gameObject.SetActive(!rootPod4.GetComponentInChildren<AttachPodMerger>().GarageDoorTop);

        }
        if (rootPod12 != null && rootPod3 != null && rootPod4 != null)
        {
            if (CheckIfNotFloorPod(rootPod13))
                LeftGirderF.SetActive(false);
            if (CheckIfNotFloorPod(rootPod12))
                rootPod12.GetComponentInChildren<AttachPodMerger>().RightGirderF.gameObject.SetActive(false);
            if (rootPod12.GetComponentInChildren<AttachPodMerger>().rootPod14 == null)
            {
                if (rootPod12 != null) rootPod12.GetComponentInChildren<AttachPodMerger>().rootPod14 = rootPod13;
            }
            rootPod3.GetComponentInChildren<AttachPodMerger>().RightGirder.gameObject.SetActive(false);
            if (rootPod3.GetComponentInChildren<AttachPodMerger>().rootPod23 == null)
            {
                if (rootPod3 != null) rootPod3.GetComponentInChildren<AttachPodMerger>().rootPod23 = rootPod13;
            }
            rootPod4.GetComponentInChildren<AttachPodMerger>().LeftGirder.gameObject.SetActive(false);
            if (rootPod4.GetComponentInChildren<AttachPodMerger>().rootPod22 == null)
            {
                if (rootPod4 != null) rootPod4.GetComponentInChildren<AttachPodMerger>().rootPod22 = rootPod13;
            }
        } 
        else
        {
            LeftGirderF.SetActive(true);
            
            if (rootPod12 != null)
                rootPod12.GetComponentInChildren<AttachPodMerger>().RightGirderF.gameObject.SetActive(true);
            if (rootPod3 != null)
                rootPod3.GetComponentInChildren<AttachPodMerger>().RightGirder.gameObject.SetActive(!rootPod3.GetComponentInChildren<AttachPodMerger>().GarageDoorRight);
            if (rootPod4 != null)
                rootPod4.GetComponentInChildren<AttachPodMerger>().LeftGirder.gameObject.SetActive(!rootPod4.GetComponentInChildren<AttachPodMerger>().GarageDoorLeft);

        }
        if (rootPod14 != null && rootPod5 != null && rootPod4 != null)
        {
            if (CheckIfNotFloorPod(rootPod13))
                RightGirderF.SetActive(false);
            if (CheckIfNotFloorPod(rootPod14))
                rootPod14.GetComponentInChildren<AttachPodMerger>().LeftGirderF.gameObject.SetActive(false);
            if (rootPod14.GetComponentInChildren<AttachPodMerger>().rootPod12 == null)
            {
                if (rootPod14 != null) rootPod14.GetComponentInChildren<AttachPodMerger>().rootPod12 = rootPod13;
            }
            rootPod5.GetComponentInChildren<AttachPodMerger>().LeftGirder.gameObject.SetActive(false);
            if (rootPod5.GetComponentInChildren<AttachPodMerger>().rootPod21 == null)
            {
                if (rootPod5 != null) rootPod5.GetComponentInChildren<AttachPodMerger>().rootPod21 = rootPod13;
            }
            rootPod4.GetComponentInChildren<AttachPodMerger>().RightGirder.gameObject.SetActive(false);
            if (rootPod4.GetComponentInChildren<AttachPodMerger>().rootPod22 == null)
            {
                if (rootPod4 != null) rootPod4.GetComponentInChildren<AttachPodMerger>().rootPod22 = rootPod13;
            }
        } 
        else
        {
            RightGirderF.SetActive(true);
            

            if (rootPod14 != null)
                rootPod14.GetComponentInChildren<AttachPodMerger>().LeftGirderF.gameObject.SetActive(true);
            if (rootPod5 != null)
                rootPod5.GetComponentInChildren<AttachPodMerger>().LeftGirder.gameObject.SetActive(!rootPod5.GetComponentInChildren<AttachPodMerger>().GarageDoorLeft);
            if (rootPod4 != null)
                rootPod4.GetComponentInChildren<AttachPodMerger>().RightGirder.gameObject.SetActive(!rootPod4.GetComponentInChildren<AttachPodMerger>().GarageDoorRight);

        }
        if (rootPod16 != null && rootPod7 != null && rootPod4 != null)
        {
            if (CheckIfNotFloorPod(rootPod13))
                BotGirderF.SetActive(false);
            if (CheckIfNotFloorPod(rootPod16))
                rootPod16.GetComponentInChildren<AttachPodMerger>().TopGirderF.gameObject.SetActive(false);
            if (rootPod16.GetComponentInChildren<AttachPodMerger>().rootPod10 == null)
            {
                if (rootPod16 != null) rootPod16.GetComponentInChildren<AttachPodMerger>().rootPod10 = rootPod13;
            }
            rootPod7.GetComponentInChildren<AttachPodMerger>().TopGirder.gameObject.SetActive(false);
            if (rootPod7.GetComponentInChildren<AttachPodMerger>().rootPod19 == null)
            {
                if (rootPod7 != null) rootPod7.GetComponentInChildren<AttachPodMerger>().rootPod19 = rootPod13;
            }
            rootPod4.GetComponentInChildren<AttachPodMerger>().BotGirder.gameObject.SetActive(false);
            if (rootPod4.GetComponentInChildren<AttachPodMerger>().rootPod22 == null)
            {
                if (rootPod4 != null) rootPod4.GetComponentInChildren<AttachPodMerger>().rootPod22 = rootPod13;
            }
        } 
        else
        {
            BotGirderF.SetActive(true);
            if (rootPod16 != null)
                rootPod16.GetComponentInChildren<AttachPodMerger>().TopGirderF.gameObject.SetActive(true);
            if (rootPod7 != null)
                rootPod7.GetComponentInChildren<AttachPodMerger>().TopGirder.gameObject.SetActive(!rootPod7.GetComponentInChildren<AttachPodMerger>().GarageDoorTop);
            if (rootPod4 != null)
                rootPod4.GetComponentInChildren<AttachPodMerger>().BotGirder.gameObject.SetActive(!rootPod4.GetComponentInChildren<AttachPodMerger>().GarageDoorBot);

        }
        //wall corners
        if (rootPod10 != null && rootPod14 != null && rootPod11 != null)
        {
            TopRightLong.SetActive(false);
            rootPod10.GetComponentInChildren<AttachPodMerger>().BotRightLong.gameObject.SetActive(false);
            if (rootPod10.GetComponentInChildren<AttachPodMerger>().rootPod16 == null)
            {
                if (rootPod10 != null) rootPod10.GetComponentInChildren<AttachPodMerger>().rootPod16 = rootPod13;
            }
            rootPod14.GetComponentInChildren<AttachPodMerger>().TopLeftLong.gameObject.SetActive(false);
            if (rootPod14.GetComponentInChildren<AttachPodMerger>().rootPod12 == null)
            {
                if (rootPod14 != null) rootPod14.GetComponentInChildren<AttachPodMerger>().rootPod12 = rootPod13;
            }
            rootPod11.GetComponentInChildren<AttachPodMerger>().BotLeftLong.gameObject.SetActive(false);
            if (rootPod11.GetComponentInChildren<AttachPodMerger>().rootPod15 == null)
            {
                if (rootPod11 != null) rootPod11.GetComponentInChildren<AttachPodMerger>().rootPod15 = rootPod13;
            }
        } 
        else
        {
            if(GarageDoorTop || GarageDoorRight)
            {
                if (rootPod10 != null && GarageDoorRight)
                {
                    if (rootPod10.GetComponentInChildren<AttachPodMerger>().GarageDoorRight)
                    {
                        TopRightLong.SetActive(false);
                    }
                }
                if (rootPod14 != null && GarageDoorTop)
                {
                    if (rootPod14.GetComponentInChildren<AttachPodMerger>().GarageDoorTop)
                    {
                        TopRightLong.SetActive(false);
                    }
                }

            }
            else
            {
                TopRightLong.SetActive(true);
            }

            if (rootPod10 != null)
            {
                rootPod10.GetComponentInChildren<AttachPodMerger>().BotRightLong.gameObject.SetActive(!rootPod10.GetComponentInChildren<AttachPodMerger>().GarageDoorRight && !rootPod10.GetComponentInChildren<AttachPodMerger>().GarageDoorBot);
            }
            if (rootPod14 != null)
            {
                rootPod14.GetComponentInChildren<AttachPodMerger>().TopLeftLong.gameObject.SetActive(!rootPod14.GetComponentInChildren<AttachPodMerger>().GarageDoorTop && !rootPod14.GetComponentInChildren<AttachPodMerger>().GarageDoorLeft);
            }
            if (rootPod11 != null)
            {
                rootPod11.GetComponentInChildren<AttachPodMerger>().BotLeftLong.gameObject.SetActive(!rootPod11.GetComponentInChildren<AttachPodMerger>().GarageDoorBot && !rootPod11.GetComponentInChildren<AttachPodMerger>().GarageDoorLeft);
            }

        }
        if (rootPod10 != null && rootPod12 != null && rootPod9 != null)
        {
            TopLeftLong.SetActive(false);
            rootPod10.GetComponentInChildren<AttachPodMerger>().BotLeftLong.gameObject.SetActive(false);
            if (rootPod10.GetComponentInChildren<AttachPodMerger>().rootPod16 == null)
            {
                if (rootPod10 != null) rootPod10.GetComponentInChildren<AttachPodMerger>().rootPod16 = rootPod13;
            }
            rootPod12.GetComponentInChildren<AttachPodMerger>().TopRightLong.gameObject.SetActive(false);
            if (rootPod12.GetComponentInChildren<AttachPodMerger>().rootPod14 == null)
            {
                if (rootPod12 != null) rootPod12.GetComponentInChildren<AttachPodMerger>().rootPod14 = rootPod13;
            }
            rootPod9.GetComponentInChildren<AttachPodMerger>().BotRightLong.gameObject.SetActive(false);
            if (rootPod9.GetComponentInChildren<AttachPodMerger>().rootPod17 == null)
            {
                if (rootPod9 != null) rootPod9.GetComponentInChildren<AttachPodMerger>().rootPod17 = rootPod13;
            }
        } 
        else
        {
            if (GarageDoorTop || GarageDoorLeft)
            {
                if (rootPod10 != null && GarageDoorLeft)
                {
                    if (rootPod10.GetComponentInChildren<AttachPodMerger>().GarageDoorLeft)
                    {
                        TopLeftLong.SetActive(false);
                    }
                }
                if (rootPod12 != null && GarageDoorTop)
                {
                    if (rootPod12.GetComponentInChildren<AttachPodMerger>().GarageDoorTop)
                    {
                        TopLeftLong.SetActive(false);
                    }
                }
            }
            else
            {
                TopLeftLong.SetActive(true);
            }        
            if (rootPod10 != null)
                rootPod10.GetComponentInChildren<AttachPodMerger>().BotLeftLong.gameObject.SetActive(!rootPod10.GetComponentInChildren<AttachPodMerger>().GarageDoorBot && !rootPod10.GetComponentInChildren<AttachPodMerger>().GarageDoorLeft);
            if (rootPod12 != null)
                rootPod12.GetComponentInChildren<AttachPodMerger>().TopRightLong.gameObject.SetActive(!rootPod12.GetComponentInChildren<AttachPodMerger>().GarageDoorTop && !rootPod12.GetComponentInChildren<AttachPodMerger>().GarageDoorRight);
            if (rootPod9 != null)
                rootPod9.GetComponentInChildren<AttachPodMerger>().BotRightLong.gameObject.SetActive(!rootPod9.GetComponentInChildren<AttachPodMerger>().GarageDoorBot && !rootPod9.GetComponentInChildren<AttachPodMerger>().GarageDoorRight);

        }
        if (rootPod16 != null && rootPod14 != null && rootPod17 != null)
        {
            BotRightLong.SetActive(false);

            rootPod16.GetComponentInChildren<AttachPodMerger>().TopRightLong.gameObject.SetActive(false);
            if (rootPod16.GetComponentInChildren<AttachPodMerger>().rootPod10 == null)
            {
                if (rootPod16 != null) rootPod16.GetComponentInChildren<AttachPodMerger>().rootPod10 = rootPod13;
            }
            rootPod14.GetComponentInChildren<AttachPodMerger>().BotLeftLong.gameObject.SetActive(false);
            if (rootPod14.GetComponentInChildren<AttachPodMerger>().rootPod12 == null)
            {
                if (rootPod14 != null) rootPod4.GetComponentInChildren<AttachPodMerger>().rootPod12 = rootPod13;
            }
            rootPod17.GetComponentInChildren<AttachPodMerger>().TopLeftLong.gameObject.SetActive(false);
            if (rootPod17.GetComponentInChildren<AttachPodMerger>().rootPod9 == null)
            {
                if (rootPod17 != null) rootPod17.GetComponentInChildren<AttachPodMerger>().rootPod9 = rootPod13;
            }

        } 
        else
        {
            if (GarageDoorBot || GarageDoorRight)
            {
                if (rootPod10 != null && GarageDoorRight)
                {
                    if (rootPod10.GetComponentInChildren<AttachPodMerger>().GarageDoorRight)
                    {
                        BotRightLong.SetActive(false);
                    }
                }
                if (rootPod14 != null && GarageDoorBot)
                {
                    if (rootPod14.GetComponentInChildren<AttachPodMerger>().GarageDoorBot)
                    {
                        BotRightLong.SetActive(false);
                    }
                }
            }
            else
            {
                BotRightLong.SetActive(true);
            }        
            if (rootPod16 != null)
                rootPod16.GetComponentInChildren<AttachPodMerger>().TopRightLong.gameObject.SetActive(!rootPod16.GetComponentInChildren<AttachPodMerger>().GarageDoorTop && !rootPod16.GetComponentInChildren<AttachPodMerger>().GarageDoorRight);
            if (rootPod14 != null)
                rootPod14.GetComponentInChildren<AttachPodMerger>().BotLeftLong.gameObject.SetActive(!rootPod14.GetComponentInChildren<AttachPodMerger>().GarageDoorBot && !rootPod14.GetComponentInChildren<AttachPodMerger>().GarageDoorLeft);
            if (rootPod17 != null)
                rootPod17.GetComponentInChildren<AttachPodMerger>().TopLeftLong.gameObject.SetActive(!rootPod17.GetComponentInChildren<AttachPodMerger>().GarageDoorTop && !rootPod17.GetComponentInChildren<AttachPodMerger>().GarageDoorLeft);

        }
        if (rootPod16 != null && rootPod12 != null && rootPod15 != null)
        {
            BotLeftLong.SetActive(false);

            rootPod16.GetComponentInChildren<AttachPodMerger>().TopLeftLong.gameObject.SetActive(false);
            if (rootPod16.GetComponentInChildren<AttachPodMerger>().rootPod10 == null)
            {
                if (rootPod16 != null) rootPod16.GetComponentInChildren<AttachPodMerger>().rootPod10 = rootPod13;
            }
            rootPod12.GetComponentInChildren<AttachPodMerger>().BotRightLong.gameObject.SetActive(false);
            if (rootPod12.GetComponentInChildren<AttachPodMerger>().rootPod14 == null)
            {
                if (rootPod12 != null) rootPod12.GetComponentInChildren<AttachPodMerger>().rootPod14 = rootPod13;
            }
            rootPod15.GetComponentInChildren<AttachPodMerger>().TopRightLong.gameObject.SetActive(false);
            if (rootPod15.GetComponentInChildren<AttachPodMerger>().rootPod11 == null)
            {
                if (rootPod15 != null) rootPod15.GetComponentInChildren<AttachPodMerger>().rootPod11 = rootPod13;
            }

        } 
        else
        {
            if (GarageDoorBot || GarageDoorLeft)
            {
                if (rootPod16 != null && GarageDoorLeft)
                {
                    if (rootPod16.GetComponentInChildren<AttachPodMerger>().GarageDoorLeft)
                    {
                        BotLeftLong.SetActive(false);
                    }
                }
                if (rootPod12 != null && GarageDoorBot)
                {
                    if (rootPod12.GetComponentInChildren<AttachPodMerger>().GarageDoorBot)
                    {
                        BotLeftLong.SetActive(false);
                    }
                }
            }
            else
            {
                BotLeftLong.SetActive(true);
            }
            if (rootPod16 != null)
                rootPod16.GetComponentInChildren<AttachPodMerger>().TopLeftLong.gameObject.SetActive(!rootPod16.GetComponentInChildren<AttachPodMerger>().GarageDoorTop && !rootPod16.GetComponentInChildren<AttachPodMerger>().GarageDoorLeft);
            if (rootPod12 != null)
                rootPod12.GetComponentInChildren<AttachPodMerger>().BotRightLong.gameObject.SetActive(!rootPod12.GetComponentInChildren<AttachPodMerger>().GarageDoorBot && !rootPod12.GetComponentInChildren<AttachPodMerger>().GarageDoorRight);
            if (rootPod15 != null)
                rootPod15.GetComponentInChildren<AttachPodMerger>().TopRightLong.gameObject.SetActive(!rootPod15.GetComponentInChildren<AttachPodMerger>().GarageDoorTop && !rootPod15.GetComponentInChildren<AttachPodMerger>().GarageDoorRight);

        }
        //panels
        if (rootPod10 != null)
        {
            Wall_Window_TopP.gameObject.SetActive(false);
            rootPod10.GetComponentInChildren<AttachPodMerger>().Wall_Window_BotP.gameObject.SetActive(false);
            if (rootPod10.GetComponentInChildren<AttachPodMerger>().rootPod16 == null)
            {
                if (rootPod10 != null) rootPod10.GetComponentInChildren<AttachPodMerger>().rootPod16 = rootPod13;
            }
        }
        else
        {
            Wall_Window_TopP.gameObject.SetActive(!GarageDoorTop);
        }
        if (rootPod16 != null)
        {
            Wall_Window_BotP.gameObject.SetActive(false);
            rootPod16.GetComponentInChildren<AttachPodMerger>().Wall_Window_TopP.gameObject.SetActive(false);
            if (rootPod16.GetComponentInChildren<AttachPodMerger>().rootPod10 == null)
            {
                if (rootPod16 != null) rootPod16.GetComponentInChildren<AttachPodMerger>().rootPod10 = rootPod13;
            }
        }
        else
        {
            Wall_Window_BotP.gameObject.SetActive(!GarageDoorBot);
        }
        if (rootPod14 != null)
        {
            Wall_Window_RightP.gameObject.SetActive(false);
            rootPod14.GetComponentInChildren<AttachPodMerger>().Wall_Window_LeftP.gameObject.SetActive(false);
            if (rootPod14.GetComponentInChildren<AttachPodMerger>().rootPod12 == null)
            {
                if (rootPod14 != null) rootPod14.GetComponentInChildren<AttachPodMerger>().rootPod12 = rootPod13;
            }
        }
        else
        {
            Wall_Window_RightP.gameObject.SetActive(!GarageDoorRight);
        }
        if (rootPod12 != null)
        {
            Wall_Window_LeftP.gameObject.SetActive(false);
            rootPod12.GetComponentInChildren<AttachPodMerger>().Wall_Window_RightP.gameObject.SetActive(false);
            if (rootPod12.GetComponentInChildren<AttachPodMerger>().rootPod14 == null)
            {
                if (rootPod12 != null) rootPod12.GetComponentInChildren<AttachPodMerger>().rootPod14 = rootPod13;
            }
        } 
        else
        {
            Wall_Window_Left.gameObject.SetActive(!GarageDoorLeft);
        }
        if (rootPod22 != null)
        {
            RoofP.gameObject.SetActive(false);
            if (CheckIfNotFloorPod(rootPod22))
                rootPod22.GetComponentInChildren<AttachPodMerger>().FloorP.gameObject.SetActive(false);
            if (CheckIfNotFloorPod(rootPod22))
                rootPod22.GetComponentInChildren<AttachPodMerger>().SurfaceFloor.gameObject.SetActive(false);
            
            if (rootPod22.GetComponentInChildren<AttachPodMerger>().rootPod4 == null)
            {
                if (rootPod22 != null) rootPod22.GetComponentInChildren<AttachPodMerger>().rootPod4 = rootPod13;
            }
        }
        else
        {
            RoofP.gameObject.SetActive(true);
        }
        if (rootPod4 != null)
        {
            if (CheckIfNotFloorPod(rootPod13))
            {
                FloorP.gameObject.SetActive(false);
                SurfaceFloor.gameObject.SetActive(false);
            }
            
            rootPod4.GetComponentInChildren<AttachPodMerger>().RoofP.gameObject.SetActive(false);
            if (rootPod4.GetComponentInChildren<AttachPodMerger>().rootPod22 == null)
            {
                if (rootPod4 != null) rootPod4.GetComponentInChildren<AttachPodMerger>().rootPod22 = rootPod13;
            }
        }
        else
        {
            FloorP.gameObject.SetActive(true);
            SurfaceFloor.gameObject.SetActive(true);
        }
        if (Managers.GetManager<PlayersManager>().GetActivePlayerController().GetPlayerInputDispatcher().IsPressingAccessibilityKey()){
            TriggerDeconstruction.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            TriggerDeconstruction.gameObject.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
        }

        foreach (Panel p in rootPod13.GetComponentsInChildren<Panel>())
        {
            if (p.isActiveAndEnabled)
            {
                if (p.panelType == DataConfig.BuildPanelType.Wall)
                {
                    if (p.GetSubPanelType().ToString().ToLower().Contains("floor"))
                    {
                        switch (Plugin.defaultWall.Value)
                        {
                            case 0:
                                p.ChangePanel(DataConfig.BuildPanelSubType.WallPlain, true, false);
                                break;
                            case 1:
                                p.ChangePanel(DataConfig.BuildPanelSubType.WallGlass, true, false);
                                break;
                            case 2:
                                p.ChangePanel(DataConfig.BuildPanelSubType.WallLab, true, false);
                                break;
                        }
                    }
                    if (p.GetSubPanelType().ToString().ToLower().Contains("corridor"))
                    {
                        if (p.GetContingousPanels(2f) == null)
                        {
                            switch (Plugin.defaultWall.Value)
                            {
                                case 0:
                                    p.ChangePanel(DataConfig.BuildPanelSubType.WallPlain, true, false);
                                    break;
                                case 1:
                                    p.ChangePanel(DataConfig.BuildPanelSubType.WallGlass, true, false);
                                    break;
                                case 2:
                                    p.ChangePanel(DataConfig.BuildPanelSubType.WallLab, true, false);
                                    break;
                            }
                        }
                    }
                }
            }
        }
        
    }
}

[BepInPlugin("Tjatja.theplanetcraftermods.AttachAPod", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    static ConfigEntry<bool> modEnabled;
    static ConfigEntry<bool> Asset0;
    public static ConfigEntry<int> defaultWall;
    static ManualLogSource logger;
    static string currentLanguage;
    static AssetBundle bundle;
    public static List<WorldObject> allWorldObjects;

    private void Awake()
    {
        if (ModVersionCheck.ModVersionCheck.Check(this, Logger.LogInfo, out bool hashError, out string repoURL))
        {
            ModVersionCheck.ModVersionCheck.NotifyUser(this, hashError, repoURL, Logger.LogInfo);
        }
        modEnabled = Config.Bind("General", "Enabled", true, "!Hold down Ctrl to access deconstruction!\n[Default is Enabled]");
        Asset0 = Config.Bind("General", "ExpandablePod", true, "[Add Expandable Pod]\n[Add Expandable Pod w/Floor]\n[Add Expandable Pod Stairs]");
        defaultWall = Config.Bind("General", "DefaultWall", 1, "[0 = Default Wall]\n[1 = Wall Glass]\n[2 = Biolab Wall]");
        logger = Logger;
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
            dict["GROUP_NAME_AttachPod"] = "Expandable Pod";
            dict["GROUP_DESC_AttachPod"] = "Expands into others of the same type.<br><color=\"red\">!Hold down Ctrl to access deconstruction!";
            dict["GROUP_NAME_AttachFPod"] = "Expandable Pod - Deck";
            dict["GROUP_DESC_AttachFPod"] = "Expands into others of the same type.<br>The floor of this pod does not dissolve.<br><color=\"red\">!Hold down Ctrl to access deconstruction!";
            dict["GROUP_NAME_AttachStairsPod"] = "Stairs for Expandable Pods";
            dict["GROUP_NAME_AttachGaragePod"] = "Garage door for Expandable Pods";
        }
        if (___localizationDictionary.TryGetValue("french", out dict))
        {
            dict["GROUP_NAME_AttachPod"] = "Module extensible";
            dict["GROUP_DESC_AttachPod"] = "S'étend à d'autres éléments du même type.<br><color=\"red\">!Maintenez la touche Ctrl enfoncée pour accéder à la déconstruction !";
            dict["GROUP_NAME_AttachFPod"] = "Module extensible – Terrasse";
            dict["GROUP_DESC_AttachFPod"] = "S'étend à d'autres éléments du même type.<br>Le sol de cette capsule ne se dissout pas.<br><color=\"red\">!Maintenez la touche Ctrl enfoncée pour accéder à la déconstruction !";
            dict["GROUP_NAME_AttachStairsPod"] = "Escaliers pour modules extensibles";
            dict["GROUP_NAME_AttachGaragePod"] = "Porte de garage pour modules extensibles";
        }
        if (___localizationDictionary.TryGetValue("russian", out dict))
        {
            dict["GROUP_NAME_AttachPod"] = "Раздвижной модуль";
            dict["GROUP_DESC_AttachPod"] = "Расширяется на другие объекты того же типа.<br><color=\"red\">!Удерживайте Ctrl для доступа к режиму разбора!";
            dict["GROUP_NAME_AttachFPod"] = "Раздвижной модуль — терраса";
            dict["GROUP_DESC_AttachFPod"] = "Распространяется на другие объекты того же типа.<br>Пол этой капсулы не растворяется.<br><color=\"red\">!Удерживайте Ctrl для доступа к режиму разбора!";
            dict["GROUP_NAME_AttachStairsPod"] = "Лестницы для модулей с раздвижными секциями";
            dict["GROUP_NAME_AttachGaragePod"] = "Гаражные ворота для модулей с раздвижными секциями";
        }
        if (___localizationDictionary.TryGetValue("schinese", out dict))
        {
            dict["GROUP_NAME_AttachPod"] = "可扩展舱体";
            dict["GROUP_DESC_AttachPod"] = "向同类目标扩展。<br><color=\"red\">!按住 Ctrl 键进行拆解!";
            dict["GROUP_NAME_AttachFPod"] = "可扩展舱体——甲板";
            dict["GROUP_DESC_AttachFPod"] = "向同类目标扩散。<br>该荚舱的底部不会溶解。<br><color=\"red\">!按住 Ctrl 键即可进行拆除!";
            dict["GROUP_NAME_AttachStairsPod"] = "适用于可扩展舱体的楼梯";
            dict["GROUP_NAME_AttachGaragePod"] = "适用于可扩展舱体的车库门";
        }
        if (___localizationDictionary.TryGetValue("tchinese", out dict))
        {
            dict["GROUP_NAME_AttachPod"] = "可擴展艙體";
            dict["GROUP_DESC_AttachPod"] = "向同類目標擴展。 <br><color=\"red\">!按住 Ctrl 鍵進行拆解!";
            dict["GROUP_NAME_AttachFPod"] = "可擴展機艙——甲板";
            dict["GROUP_DESC_AttachFPod"] = "向同類目標擴散。 <br>該莢艙的底部不會溶解。 <br><color=\"red\">!按住 Ctrl 鍵即可進行拆除!";
            dict["GROUP_NAME_AttachStairsPod"] = "適用於可擴展艙室的樓梯";
            dict["GROUP_NAME_AttachGaragePod"] = "適用於可擴展機艙的車庫門";
        }
        if (___localizationDictionary.TryGetValue("german", out dict))
        {
            dict["GROUP_NAME_AttachPod"] = "Erweiterbares Modul";
            dict["GROUP_DESC_AttachPod"] = "Breitet sich auf andere desselben Typs aus.<br><color=\"red\">!Halte Strg gedrückt, um auf die Zerlegung zuzugreifen!";
            dict["GROUP_NAME_AttachFPod"] = "Erweiterbares Modul – Deck";
            dict["GROUP_DESC_AttachFPod"] = "Breitet sich auf andere desselben Typs aus.<br>Der Boden dieser Kapsel löst sich nicht auf.<br><color=\"red\">!Halte Strg gedrückt, um auf die Demontage zuzugreifen!";
            dict["GROUP_NAME_AttachStairsPod"] = "Treppen für ausziehbare Module";
            dict["GROUP_NAME_AttachGaragePod"] = "Garagentor für ausziehbare Module";
        }
        if (___localizationDictionary.TryGetValue("portuguese", out dict))
        {
            dict["GROUP_NAME_AttachPod"] = "Módulo expansível";
            dict["GROUP_DESC_AttachPod"] = "Expande-se para outros do mesmo tipo.<br><color=\"red\">!Mantenha a tecla Ctrl pressionada para acessar a desconstrução!";
            dict["GROUP_NAME_AttachFPod"] = "Módulo Expansível – Deck";
            dict["GROUP_DESC_AttachFPod"] = "Expande-se para outros do mesmo tipo.<br>O piso desta cápsula não se dissolve.<br><color=\"red\">!Mantenha a tecla Ctrl pressionada para acessar a desconstrução!";
            dict["GROUP_NAME_AttachStairsPod"] = "Escadas para Módulos Expansíveis";
            dict["GROUP_NAME_AttachGaragePod"] = "Porta de garagem para módulos expansíveis";
        }
        if (___localizationDictionary.TryGetValue("spanish", out dict))
        {
            dict["GROUP_NAME_AttachPod"] = "Módulo extensible";
            dict["GROUP_DESC_AttachPod"] = "Se expande hacia otros del mismo tipo.<br><color=\"red\">!Mantén pulsada la tecla Ctrl para acceder a la deconstrucción!";
            dict["GROUP_NAME_AttachFPod"] = "Módulo extensible - Plataforma";
            dict["GROUP_DESC_AttachFPod"] = "Se expande hacia otros del mismo tipo.<br>El suelo de esta cápsula no se disuelve.<br><color=\"red\">¡Mantén pulsada la tecla Ctrl para acceder a la deconstrucción!";
            dict["GROUP_NAME_AttachStairsPod"] = "Escaleras para módulos expandibles";
            dict["GROUP_NAME_AttachGaragePod"] = "Puerta de garaje para módulos expandibles";
        }
        if (___localizationDictionary.TryGetValue("koreana", out dict))
        {
            dict["GROUP_NAME_AttachPod"] = "확장형 포드";
            dict["GROUP_DESC_AttachPod"] = "같은 유형의 다른 대상으로 확장됩니다.<br><color=\"red\">!Ctrl 키를 누른 채로 해체할 수 있습니다!";
            dict["GROUP_NAME_AttachFPod"] = "확장형 포드 - 데크";
            dict["GROUP_DESC_AttachFPod"] = "같은 유형의 다른 개체로 확장됩니다.<br>이 포드의 바닥은 녹지 않습니다.<br><color=\"red\">!Ctrl 키를 길게 눌러 해체 모드를 사용하세요!";
            dict["GROUP_NAME_AttachStairsPod"] = "확장형 포드용 계단";
            dict["GROUP_NAME_AttachGaragePod"] = "확장형 포드용 차고 문";
        }
        if (___localizationDictionary.TryGetValue("japanese", out dict))
        {
            dict["GROUP_NAME_AttachPod"] = "拡張型ポッド";
            dict["GROUP_DESC_AttachPod"] = "同種の他のものへと広がります。<br><color=\"red\">!Ctrlキーを長押しすると解体できます!";
            dict["GROUP_NAME_AttachFPod"] = "拡張型ポッド - デッキ";
            dict["GROUP_DESC_AttachFPod"] = "同種の他のものへと広がります。<br>このポッドの底面は溶けません。<br><color=\"red\">!Ctrlキーを長押しすると解体できます!";
            dict["GROUP_NAME_AttachStairsPod"] = "拡張型ポッド用階段";
            dict["GROUP_NAME_AttachGaragePod"] = "拡張型ポッド用ガレージドア";
        }
        if (___localizationDictionary.TryGetValue("turk", out dict))
        {
            dict["GROUP_NAME_AttachPod"] = "Genişletilebilir Modül";
            dict["GROUP_DESC_AttachPod"] = "Aynı türden diğerlerine doğru genişler.<br><color=\"red\">!Sökme işlemine erişmek için Ctrl tuşunu basılı tutun!";
            dict["GROUP_NAME_AttachFPod"] = "Genişletilebilir Modül - Güverte";
            dict["GROUP_DESC_AttachFPod"] = "Aynı türden diğerlerine doğru genişler.<br>Bu kapsülün tabanı çözülmez.<br><color=\"red\">!Sökme işlemine erişmek için Ctrl tuşunu basılı tutun!";
            dict["GROUP_NAME_AttachStairsPod"] = "Genişletilebilir Modüller için Merdivenler";
            dict["GROUP_NAME_AttachGaragePod"] = "Genişletilebilir Modüller için Garaj Kapısı";
        }
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObjectAssociated), nameof(WorldObjectAssociated.RefreshPanelsId))]
    public static bool WorldObjectAssociated_RefreshPanelsId(WorldObjectAssociated __instance, WorldObject ____worldObjectAssociated)
    {
        List<int> list = new List<int>();
        Panel[] componentsInChildren = __instance.GetComponentsInChildren<Panel>(true); 
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            int subPanelType = (int)componentsInChildren[i].GetSubPanelType();
            list.Add(subPanelType);
        }
        ____worldObjectAssociated.SetPanelsId(list);
        RequireEnergy component = __instance.GetComponent<RequireEnergy>();
        if (component != null)
        {
            component.RefreshComponentsLists();
            component.CheckEnergyStatus();
        }
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(StaticDataHandler), "LoadStaticData")]
    private static void StaticDataHandler_LoadStaticData2(List<GroupData> ___groupsData)
    {
        if (___groupsData.Select(gd => gd.id).Where(id => id == "AttachPod").Any()) return;

        if (bundle == null)
        {
            string filepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/attachpod";
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
            GroupDataConstructible cubeGDC = bundle.LoadAsset<GroupDataConstructible>("assets/AttachPod.asset");
            GroupDataConstructible cubeGDC1 = bundle.LoadAsset<GroupDataConstructible>("assets/AttachFPod.asset");
            GroupDataConstructible cubeGDC2 = bundle.LoadAsset<GroupDataConstructible>("assets/AttachStairsPod.asset");
            GroupDataConstructible cubeGDC3 = bundle.LoadAsset<GroupDataConstructible>("assets/AttachGaragePod.asset"); 
            if (___groupsData.Contains(cubeGDC))
            {
                return;
            }
            MaterialsHelper.ApplyGameMaterials(cubeGDC.associatedGameObject, true);
            ___groupsData.Add(cubeGDC);
            uint newint = 204376340;
            SetGlobalObjectIdHash(cubeGDC.associatedGameObject.GetComponent<NetworkObject>(), newint);
            NetworkManager.Singleton.AddNetworkPrefab(cubeGDC.associatedGameObject);

            MaterialsHelper.ApplyGameMaterials(cubeGDC1.associatedGameObject, true);
            ___groupsData.Add(cubeGDC1);
            uint newint1 = 204376341;
            SetGlobalObjectIdHash(cubeGDC1.associatedGameObject.GetComponent<NetworkObject>(), newint1);
            NetworkManager.Singleton.AddNetworkPrefab(cubeGDC1.associatedGameObject);

            MaterialsHelper.ApplyGameMaterials(cubeGDC2.associatedGameObject, true);
            ___groupsData.Add(cubeGDC2);
            uint newint2 = 204376342;
            SetGlobalObjectIdHash(cubeGDC2.associatedGameObject.GetComponent<NetworkObject>(), newint2);
            NetworkManager.Singleton.AddNetworkPrefab(cubeGDC2.associatedGameObject);

            MaterialsHelper.ApplyGameMaterials(cubeGDC3.associatedGameObject, true);
            ___groupsData.Add(cubeGDC3);
            uint newint3 = 204376343;
            SetGlobalObjectIdHash(cubeGDC3.associatedGameObject.GetComponent<NetworkObject>(), newint3);
            NetworkManager.Singleton.AddNetworkPrefab(cubeGDC3.associatedGameObject);
            //networking code
        }
        NetworkManager.Singleton.NetworkConfig.ForceSamePrefabs = true;
        ___groupsData.Find(e => e.id == "AttachPod").associatedGameObject.AddComponent<AttachPodMerger>();
        ___groupsData.Find(e => e.id == "AttachFPod").associatedGameObject.AddComponent<AttachPodMerger>();
    }
}
