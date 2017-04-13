using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HighlightingSystem;

[RequireComponent(typeof(Highlighter))]
public class FocusHighlight : MonoBehaviour, IFocusable {
    public void OnFocusEnter()
    {
        GetComponent<Highlighter>().ConstantOn(new Color(0, 120, 250, 1));
    }

    public void OnFocusExit()
    {
        GetComponent<Highlighter>().ConstantOff();
    }
}
