using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class HoloButton : MonoBehaviour, IInputClickHandler
{
    public UnityEvent action;
    public AudioClip clip;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        action.Invoke();
        GetComponent<AudioSource>().PlayOneShot(clip);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
