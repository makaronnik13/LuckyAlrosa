using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ModelPlacer : MonoBehaviour {

    public PlaneTypes planeType;

	// Use this for initialization
	void Awake () {
        transform.localScale = Vector3.zero;
        SurfaceMeshesToPlanes.Instance.MakePlanesComplete += Place;
    }

    private void Place(object source, EventArgs args)
    {
        List<GameObject> planes = SurfaceMeshesToPlanes.Instance.GetActivePlanes(planeType);

        float d = Mathf.Infinity;
        foreach (GameObject go in planes)
        {
            if (Vector3.Distance(go.transform.position, Camera.main.transform.position)<d)
            {
                d = Vector3.Distance(go.transform.position, Camera.main.transform.position);
                transform.position = go.transform.position;
            }
        }
        transform.localScale = Vector3.one;
    }

    private void Update()
    {

    }


}
