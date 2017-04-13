using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlrosaController : MonoBehaviour {

    public InfoPanelFader panel;

	// Use this for initialization
	void Start () {
        panel.Show();
        transform.GetChild(0).localScale = Vector3.zero;



        LeanTween.scale(transform.GetChild(0).gameObject, Vector3.one*0.0005f, 2);
        LeanTween.rotateAroundLocal(transform.GetChild(0).gameObject, Vector3.up, 360, 2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
