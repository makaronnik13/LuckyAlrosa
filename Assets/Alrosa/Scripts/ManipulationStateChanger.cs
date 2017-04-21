using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ManipulationStateChanger : MonoBehaviour, IFocusable {
    public ManipulationCursor.ManipulationMode mode;

    public void OnFocusEnter()
    {
        if (FindObjectOfType<ManipulationCursor>())
        {
            FindObjectOfType<ManipulationCursor>().AddMode(mode);
        }
    }

    public void OnFocusExit()
    {
        if (FindObjectOfType<ManipulationCursor>())
        {
            FindObjectOfType<ManipulationCursor>().AddMode(ManipulationCursor.ManipulationMode.None);
        }
    }
}
