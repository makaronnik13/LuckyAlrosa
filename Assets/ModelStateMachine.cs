using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using TMPro;

public class ModelStateMachine : MonoBehaviour
{
    public enum ModelState
    {
        Info = 0,
        FreeDisassembly = 1
    }
    public PartInfoPanel infoPanel;
    public TMP_Dropdown dropdown;
    private GameObject selectedPart;
    private ModelState state;
    private bool showingInfoEnabled = false;
    public AudioClip pickSound;

    // Use this for initialization
    void Start () {
        DropState();
        InitScenarious();
       // FindObjectOfType<InputManager>().AddGlobalListener(gameObject);
	}

    private void InitScenarious()
    {
        dropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (Scenario scenario in GetComponent<ScenariosPlayer>().scenarios)
        {
            options.Add(new TMP_Dropdown.OptionData(scenario.ScenarioName));
        }

        dropdown.AddOptions(options);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void ShowInfo(PartInformation currentPart = null)
    {
        GetComponent<AudioSource>().PlayOneShot(pickSound);

        if (currentPart && showingInfoEnabled)
        { 
            infoPanel.ShowInfo(currentPart);
        }
    }

    public void PlaceAllPartsBack(PartInformation currentPart = null)
    {
        if (currentPart && !currentPart.Showing)
        {
            selectedPart = currentPart.gameObject;
        }

        foreach (PartInformation part in GetComponentsInChildren<PartInformation>())
        {
            if (part!=currentPart)
            {
                part.Showing = false;
            }
        }
    }

    public void Choosestate(int _state)
    {
        Choosestate((ModelState)_state);
    }

    public void Choosestate(ModelState _state)
    {
        DropState();

        state = _state;

        switch (state)
        {
            case ModelState.Info:
                foreach (PartInformation pi in GetComponentsInChildren<PartInformation>())
                {
                    pi.IsActivate = true;
                }
                showingInfoEnabled = true;
                break;
            case ModelState.FreeDisassembly:
                foreach (HandDraggable hd in GetComponentsInChildren<HandDraggable>())
                {
                    hd.IsDraggingEnabled = true;
                }
                showingInfoEnabled = true;
                break;
        }
    }

    public void DropState()
    {
        Debug.Log("drop");
        GetComponent<Collider>().enabled = false;
        foreach (FocusHighlight hl in GetComponentsInChildren<FocusHighlight>())
        {
            hl.enabled = true;
        }
        GetComponent<Animator>().enabled = false;
        PlaceAllPartsBack();
        infoPanel.HideInfo();
        foreach (PartInformation pi in GetComponentsInChildren<PartInformation>())
        {
            pi.IsActivate = false;
        }
        foreach (HandDraggable hd in GetComponentsInChildren<HandDraggable>())
        {
            hd.IsDraggingEnabled = false;
        }
        showingInfoEnabled = false;
        GetComponent<ScenariosPlayer>().StopScenario();
    }

    public void ChoseScenario()
    {
        ChoseScenario(dropdown.value);
    }

    public void ChoseScenario(Int32 val)
    {
        DropState();
        GetComponent<Animator>().enabled = true;
        foreach (FocusHighlight hl in GetComponentsInChildren<FocusHighlight>())
        {
            hl.enabled = false;
        }
        GetComponent<ScenariosPlayer>().PlayScenario(val);
        GetComponent<Collider>().enabled = true;
    }
}
