using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRManager : MonoBehaviour
{
    [SerializeField]
    bool enableVR = false;

    void Awake()
    {
        if(!enableVR)
        {
            DisableVR();
        }
    }

    IEnumerator LoadDevice(string newDevice, bool enable)
    {
        XRSettings.LoadDeviceByName(newDevice);
        yield return null;
        XRSettings.enabled = enable;
    }

    void DisableVR()
    {   
        StartCoroutine(LoadDevice("", false));
        GameObject.Find("Shader Test Camera").SetActive(true);
        GameObject.Find("Shader Test Camera").GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        GameObject.Find("VarjoCameraRig").SetActive(false);
    }
}
