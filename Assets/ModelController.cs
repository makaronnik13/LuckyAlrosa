using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ModelController : MonoBehaviour, IManipulationHandler {

    public GameObject arrow;
    private bool open = false;
    public float min = 0.5f, max = 2;

    public enum ManipulationMode
    {
        None = 0,
        Rotation = 1,
        Scale = 2,
        Move = 3
    }

    private ManipulationMode mode = ManipulationMode.None;

    public void SetMode(ManipulationMode mode)
    {
        this.mode = mode;
    }

    public void SetMode(int mode)
    {
        SetMode((ManipulationMode)mode);
    }

    public void ChangeState()
    {
        if (open)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

	private void Open()
    {
        open = true;
        LeanTween.moveLocalY(GetComponentInChildren<MaterialFader>().gameObject, 500, 1);
        GetComponentInChildren<MaterialFader>().FadeTo(0.1f, time: 2f);
    }

    private void Close()
    {
        open = false;
        LeanTween.moveLocalY(GetComponentInChildren<MaterialFader>().gameObject, 9.14f, 1);
        GetComponentInChildren<MaterialFader>().FadeTo(1f, time: 2f);
    }

    #region lifecycle
    private void Start()
    {
        InputManager.Instance.AddGlobalListener(gameObject);
    }
    #endregion

    #region handlers
    public void OnManipulationStarted(ManipulationEventData eventData)
    {

    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        switch (mode)
        {
            case ManipulationMode.Rotation:
                transform.Rotate(new Vector3(0, -eventData.CumulativeDelta.x, 0));
                break;
            case ManipulationMode.Move:
                transform.parent.position += eventData.CumulativeDelta/25;
                break;
            case ManipulationMode.Scale:
                if (eventData.CumulativeDelta.x>0)
                {
                    transform.localScale += Vector3.one * (max-min)/5 *Time.deltaTime;
                }
                else
                {
                    transform.localScale -= Vector3.one * (max-min)/5 * Time.deltaTime;
                }

                transform.localScale = Vector3.one * Mathf.Clamp(transform.localScale.x, min, max);
                break;      
        }
        
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
       
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
 
    }
    #endregion
}
