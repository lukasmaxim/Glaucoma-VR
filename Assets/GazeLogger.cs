using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varjo;

public class GazeLogger : MonoBehaviour
{
    const string ValidString = "VALID";
    const string InvalidString = "INVALID";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        VarjoPlugin.GazeData data = VarjoPlugin.GetGaze();
        Debug.Log(data);

        string[] logData = new string[3];

        bool invalid = data.status == VarjoPlugin.GazeStatus.INVALID;
        logData[0] = invalid ? InvalidString : ValidString;
        logData[1] = invalid ? "" : Double3ToString(data.gaze.forward);
        logData[2] = invalid ? "" : Double3ToString(data.gaze.position);

        Debug.Log(logData);
    }

    public static string Double3ToString(double[] doubles)
    {
        return doubles[0].ToString() + ", " + doubles[1].ToString() + ", " + doubles[2].ToString();
    }
}
