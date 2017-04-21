using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FocusSound : MonoBehaviour, IFocusable
{
    public AudioClip focusClip;

    public void OnFocusEnter()
    {
        GetComponentInParent<AudioSource>().PlayOneShot(focusClip);
    }

    public void OnFocusExit()
    {
        //throw new NotImplementedException();
    }
}
