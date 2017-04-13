using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Scroller : MonoBehaviour, IManipulationHandler {

    private TMProScroller scroller;

    private void Start()
    {
        scroller = GetComponentInChildren<TMProScroller>();
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        //throw new NotImplementedException();
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        //throw new NotImplementedException();
    }

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        //throw new NotImplementedException();
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        scroller.Scroll(eventData.CumulativeDelta.y/50);
    }

}
