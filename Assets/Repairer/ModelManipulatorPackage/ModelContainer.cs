using HoloToolkit.Unity.SpatialMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit;
using HoloToolkit.Unity.InputModule;

public class ModelContainer : MonoBehaviour, IInputClickHandler , IManipulationHandler {

   
    private Vector3 startVector;
    private Vector3 aimPosition;
    private PanelController controll;

    public float minSize = 0.1f;
    public float maxSize = 2;


    #region LifeCycle
    // Use this for initialization
    void Start () {
        aimPosition = transform.position;
        controll = FindObjectOfType<PanelController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (controll.viewMode == PanelController.ViewMode.Move || controll.viewMode == PanelController.ViewMode.None)
        {
           // StandartGestureManager.Instance.SetRecognizableGestures(UnityEngine.VR.WSA.Input.GestureSettings.ManipulationTranslate | UnityEngine.VR.WSA.Input.GestureSettings.Tap);
        }
    }

    public void SetParams()
    {
        if (controll.viewMode == PanelController.ViewMode.Rotate)
        {
            //GetComponent<NetworkTransformSynchronizer>()._finalRotation = Quaternion.Euler(GetComponent<NetworkTransformSynchronizer>()._finalRotation.eulerAngles.x, -controll.SliderValue * 360, GetComponent<NetworkTransformSynchronizer>()._finalRotation.eulerAngles.z);
        }
        if (controll.viewMode == PanelController.ViewMode.Scale)
        {
           // GetComponent<NetworkTransformSynchronizer>()._finalScale = Vector3.one * (minSize + controll.SliderValue * (maxSize - minSize));
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        FindObjectOfType<PanelController>().SetMode(PanelController.ViewMode.Move);
    }

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        startVector = eventData.CumulativeDelta / 3;
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        Vector3 moveVector = Vector3.zero;
        aimPosition = transform.position;

        // 4.a: Calculate the moveVector as position - manipulationPreviousPosition.
        moveVector = eventData.CumulativeDelta - startVector;

        // 4.a: Update the manipulationPreviousPosition with the current position.
        startVector = eventData.CumulativeDelta;

        // 4.a: Increment this transform's position by the moveVector.
        //GetComponent<NetworkTransformSynchronizer>()._finalPosition += moveVector * 5;
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        //throw new NotImplementedException();
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
       // throw new NotImplementedException();
    }

    #endregion
}
