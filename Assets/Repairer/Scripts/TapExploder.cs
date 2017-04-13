using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit;
using HoloToolkit.Unity.InputModule;
using System;
using Meta.Tools;

public class TapExploder : MonoBehaviour, IInputClickHandler {

    public int from;
    public int to;
    public float time;
    private float value = 0;



    public void OnInputClicked(InputClickedEventData eventData)
    {
        value = 1;
        int v = from;
        from = to;
        to = v;
    }

	// Update is called once per frame
	void Update () {
        if (value>0)
        {
            value -= Time.deltaTime / time;
        }
        GetComponent<ModelExploder>().Explode(from, to, value);
    }
}
