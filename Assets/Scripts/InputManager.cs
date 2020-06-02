using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varjo;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        if(VarjoManager.Instance.GetButtonDown())
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
