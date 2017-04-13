using HoloToolkit.Unity.SpatialMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorViewer : MonoBehaviour {

    private PanelController controller;
    private List<GameObject> views = new List<GameObject>();
    private GameObject currenView;

    private Transform baseViewParent;
    private Vector3 basePosition;

	// Use this for initialization
	void Start () {
        controller = FindObjectOfType<PanelController>();
        foreach (Transform childTransform in transform)
        {
            views.Add(childTransform.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (currenView == null || currenView != views[controller.additionalMode])
        {
            if (controller.additionalMode >= 0)
            {
                ShowView(controller.additionalMode);
            }
        }
        else
        {
            if (controller.additionalMode < 0)
            {
                HideView();
            }
        }
	}

    private void HideView()
    {
        currenView.transform.SetParent(baseViewParent);
        currenView.transform.localPosition = basePosition;
        currenView = null;
    }

    private void ShowView(int v)
    {
        currenView = views[v];

        if (controller.viewMode == PanelController.ViewMode.Walk)
        {
            //Debug.Log(FindObjectOfType<SurfacePlane>().Plane.Bounds.Center);
        }

        baseViewParent = currenView.transform.parent;
        basePosition = currenView.transform.localPosition;

        currenView.transform.SetParent(null);
        currenView.transform.position = new Vector3(Camera.main.transform.position.x, FindObjectOfType<SurfacePlane>().Plane.Bounds.Center.y, Camera.main.transform.position.z);
    }
}
