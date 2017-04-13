using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PartInformation : MonoBehaviour, IInputClickHandler {

    public string partName;

    private Vector3 baseVector;
    private Quaternion baseRotation;

    public bool IsActivate = false;
    private bool showing = false;
    public bool Showing
    {
        get
        {
            return showing;
        }
        set
        {
            showing = value;
            moving = true;
        }
    }
    private bool moving = false;
    private Vector3 cameraPosition;
    

    public void OnInputClicked(InputClickedEventData eventData)
    {
        GetComponentInParent<ModelStateMachine>().ShowInfo(this);
        if (IsActivate)
        {
            GetComponentInParent<ModelStateMachine>().PlaceAllPartsBack(this);
            showing = !showing;
            moving = true;
            cameraPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.7f;
        }
    }

    internal string Info()
    {
        return FindObjectOfType<InfoManager>().GetInfo(partName);
    }

    private void Start()
    {
        baseVector = transform.localPosition;
        baseRotation = transform.localRotation;
    }

    private void Update()
    {
        if (moving)
        {
            if (showing)
            {
                transform.position = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime);
                if (Vector3.Distance(transform.position, cameraPosition) <0.01f)
                {
                    moving = false;
                }
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, baseVector, Time.deltaTime);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, baseRotation, Time.deltaTime);
                if (Vector3.Distance(transform.localPosition, baseVector) < 0.01f && Quaternion.Angle(transform.localRotation, baseRotation)<1)
                {
                    moving = false;
                }
            }
        }
    }
}
