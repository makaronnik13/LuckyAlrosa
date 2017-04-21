using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Starter : MonoBehaviour, IInputClickHandler {
    public void OnInputClicked(InputClickedEventData eventData)
    {
       FindObjectOfType<ModelController>().PlayGuiSound();
       GetComponent<InfoPanelFader>().Hide(()=>{ FindObjectOfType<AlrosaController>().StartScene(); Destroy(gameObject); });
    }

    // Use this for initialization
    void Start () {
        GetComponent<InfoPanelFader>().Show();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
