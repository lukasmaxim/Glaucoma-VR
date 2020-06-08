using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Varjo;

public class Utils : MonoBehaviour
{
    public static string Double3ToString(double[] doubles)
    {
        return doubles[0].ToString() + ", " + doubles[1].ToString() + ", " + doubles[2].ToString();
    }

    public static void PrintDebug()
    {
        VarjoManager vm = GameObject.Find("VarjoCameraRig").GetComponent<VarjoManager>();
        Quaternion hmdOrientation = vm.GetHMDOrientation(VarjoPlugin.PoseType.CENTER);
        Vector3 hmdPosition = vm.GetHMDPosition(VarjoPlugin.PoseType.CENTER);

        Vector3 camPosition = GameObject.Find("VarjoCamera").transform.position;
        Quaternion camOrientation = GameObject.Find("VarjoCamera").transform.rotation;

        Debug.Log("HMD Pos: " + hmdPosition.ToString("F3") + " CAM Pos: " + camPosition);
        Debug.Log("HMD Rot: " + hmdOrientation.ToString("F3") + " CAM Rot: " + camOrientation);
    }
}