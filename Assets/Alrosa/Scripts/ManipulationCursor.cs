using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ManipulationCursor : MonoBehaviour, IManipulationHandler, IInputHandler {

    private ManipulationMode lastMode;

    private List<CursorArrow> arrows = new List<CursorArrow>();
    private CursorCenter center;

    private ManipulationMode currentMode;

	public enum ManipulationMode
    {
        None = 0,
        Move = 1,
        ScrollVertical = 2,
        ZoomHorizontal = 3,
        RotateHorizontal = 4
    }

    private void Update()
    {

    }

    private void Start()
    {
        arrows.Add(transform.GetChild(0).GetChild(0).GetComponent<CursorArrow>());
        arrows.Add(transform.GetChild(0).GetChild(1).GetComponent<CursorArrow>());
        arrows.Add(transform.GetChild(0).GetChild(2).GetComponent<CursorArrow>());
        arrows.Add(transform.GetChild(0).GetChild(3).GetComponent<CursorArrow>());
    
        center = GetComponentInChildren<CursorCenter>();
        InputManager.Instance.AddGlobalListener(gameObject);
        ActivateMode(ManipulationMode.None);
    }


    public void AddMode(int mode)
    {
        AddMode((ManipulationMode)mode);
    }

    public void AddMode(ManipulationMode mode)
    {      
        lastMode = mode;
    }

    private void ActivateMode(ManipulationMode mode)
    {
        currentMode = mode;
        switch (mode)
        {
            case ManipulationMode.None:
                foreach (CursorArrow arrow in arrows)
                {
                    arrow.Hide();
                    arrow.HideAdditional();
                }
                center.Hide();
                break;
            case ManipulationMode.Move:
                foreach (CursorArrow arrow in arrows)
                {
                    arrow.Show();
                }
                center.Hide();
                //maybe show center icon
                break;
            case ManipulationMode.RotateHorizontal:
                arrows[1].Show();
                arrows[3].Show();
                arrows[0].Hide();
                arrows[2].Hide();
                center.ShowState(0);
                break;
            case ManipulationMode.ScrollVertical:
                arrows[0].Show();
                arrows[2].Show();
                arrows[1].Hide();
                arrows[3].Hide();
                center.Hide();
                break;
            case ManipulationMode.ZoomHorizontal:
                arrows[1].Show();
                arrows[3].Show();
                arrows[2].Hide();
                arrows[0].Hide();
                center.ShowState(1);
                arrows[3].ShowAdditional(2);
                arrows[1].ShowAdditional(1);
                break;
        }
    }

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
       
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        arrows[1].Active = false;
        arrows[3].Active = false;
        arrows[0].Active = false;
        arrows[2].Active = false;

        if (eventData.CumulativeDelta.x < -0.03f)
        {
            arrows[1].Active = true;
        }
        if (eventData.CumulativeDelta.x > 0.03f)
        {
            arrows[3].Active = true;
        }
        if (eventData.CumulativeDelta.y > 0.03f)
        {
            arrows[0].Active = true;
        }
        if (eventData.CumulativeDelta.y < -0.03f)
        {
            arrows[2].Active = true;
        }
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {

    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        
    }


    public void OnInputUp(InputEventData eventData)
    {
        CancelInvoke("ActivateLast");
        ActivateMode(ManipulationMode.None);
    }

    public void OnInputDown(InputEventData eventData)
    {
        Invoke("ActivateLast", 0.3f);
    }

    void ActivateLast()
    {
        ActivateMode(lastMode);
    }

}
