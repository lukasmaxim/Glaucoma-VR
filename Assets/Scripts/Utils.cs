using UnityEngine;
using Varjo;

public class Utils
{
    // converts varjo vector given by doubles to unity vector3
    public static string Double3ToString(double[] doubles)
    {
        return doubles[0].ToString() + ", " + doubles[1].ToString() + ", " + doubles[2].ToString();
    }

    // converts varjo vector given by doubles to unity vector3
    public static Vector3 Double3ToVector3(double[] doubles)
    {
        return new Vector3((float)doubles[0], (float)doubles[1], (float)doubles[2]);
    }

    // prints the current hmd pose
    public static void PrintHMDPose()
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
