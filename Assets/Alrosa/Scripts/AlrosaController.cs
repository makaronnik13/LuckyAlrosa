using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlrosaController : MonoBehaviour {

    public InfoPanelFader panel, panel2;

    public void StartScene()
    {
        LeanTween.scale(transform.gameObject, Vector3.one, 3);
        LeanTween.rotateAroundLocal(transform.GetChild(0).gameObject, Vector3.up, 360, 4);
        panel.Show();
        panel2.Show();
        Vector3 fvd = Camera.main.transform.forward;
        fvd.y = 0;
        fvd = fvd.normalized * 2;
        transform.position = Camera.main.transform.position + fvd;

        transform.LookAt(Camera.main.transform.position);
    }

	// Use this for initialization
	void Start () {
        transform.localScale = Vector3.zero;
        panel.Show();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
