using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DisableVR();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadDevice(string newDevice, bool enable)
    {
        XRSettings.LoadDeviceByName(newDevice);
        yield return null;
        XRSettings.enabled = enable;
    }

    void EnableVR()
    {
        StartCoroutine(LoadDevice("daydream", true));
    }

    void DisableVR()
    {
        StartCoroutine(LoadDevice("", false));
    }
}
