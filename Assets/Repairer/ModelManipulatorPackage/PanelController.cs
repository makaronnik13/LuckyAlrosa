using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanelController : MonoBehaviour {

    private Vector3 neededPosition;
    private List<GameObject> menues = new List<GameObject>();
    private Slider slider;
    private bool slidingActive = false;
    public int animationMode = 0;
    public int additionalMode = 0;
    public ViewMode viewMode = ViewMode.Move;

    public enum ViewMode
    {
        None = -1,
        Move = 0,
        Rotate = 1,
        Scale = 2,
        Animate = 3,
        Sharing = 4,
        Walk = 5
    }

    public float SliderValue
    {
        get
        {
            return slider.value;
        }
        set
        {
            slider.value = value;
        }
    }

    private void Start()
    {
        foreach (Transform childTransform in transform)
        {
            menues.Add(childTransform.gameObject);
        }
        slider = GetComponentInChildren<Slider>();

      
        neededPosition = transform.position;

        SetMode(ViewMode.Move);
    }


    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, neededPosition, 1f);
    }


    public void SetGaze(Toggle tg)
    {
       // FindObjectOfType<SharingManager>().IsGazeOn = tg.isOn;
    }

    private void ActivateSlider()
    {
        if (viewMode == ViewMode.Rotate)
        {
           float angle = -FindObjectOfType<ModelContainer>().transform.GetChild(0).rotation.eulerAngles.y;
           slider.value = (1 + angle / 360);
        }
        if (viewMode == ViewMode.Scale)
        {
            slider.value = (FindObjectOfType<ModelContainer>().transform.localScale.x - FindObjectOfType<ModelContainer>().minSize) / (FindObjectOfType<ModelContainer>().maxSize - FindObjectOfType<ModelContainer>().minSize);
        }

       // StandartGestureManager.Instance.SetRecognizableGestures(UnityEngine.VR.WSA.Input.GestureSettings.NavigationX | UnityEngine.VR.WSA.Input.GestureSettings.Tap);
       // InputManager.Instance.naviga += SetSlider;
    }

    private void DeactivateSlider()
    {
       // StandartGestureManager.Instance.SetRecognizableGestures(UnityEngine.VR.WSA.Input.GestureSettings.Tap);
       // StandartGestureManager.Instance.NavigationUpdateEvent -= SetSlider;
    }

    private void SetSlider(Vector3 cumulativeData)
    {
        slider.value += cumulativeData.x/100;
    }

    public void ShowMenu(int i)
    {
        neededPosition = Camera.main.transform.position + Camera.main.transform.forward/1.5f - Camera.main.transform.up*0.05F;
        foreach (GameObject menu in menues)
        {
            menu.SetActive(false);
        }

        if (i >= 0)
        {
            menues[i].SetActive(true);
        }
    }

    public void SetMode(int i)
    {
        SetMode((ViewMode)i);
    }

    public void SetMode(ViewMode mode)
    {
        viewMode = mode;
        switch (viewMode)
        {
            case ViewMode.None:
                ShowMenu(-1);
                break;
            case ViewMode.Move:
               // StandartGestureManager.Instance.SetRecognizableGestures(UnityEngine.VR.WSA.Input.GestureSettings.ManipulationTranslate  | UnityEngine.VR.WSA.Input.GestureSettings.Tap);
                ShowMenu(0);
                break;
            case ViewMode.Rotate:
                ShowMenu(1);
                break;
            case ViewMode.Scale:
                ShowMenu(1);
                break;
            case ViewMode.Animate:
                ShowMenu(2);
                break;
            case ViewMode.Sharing:
                ShowMenu(3);
                break;
            case ViewMode.Walk:
                ShowMenu(4);
                break;
        }

        if (slider.transform.parent.gameObject.activeInHierarchy && !slidingActive)
        {
            ActivateSlider();
        }
        if (!slider.transform.parent.gameObject.activeInHierarchy && slidingActive)
        {
            DeactivateSlider();
        }

        if (viewMode != ViewMode.Walk)
        {
            additionalMode = -1;
        }

        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
    }

    public void SetAnimationMode(int i)
    {
        animationMode = i;
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
       // FindObjectOfType<AnimatorStatesController>().SetMode(animationMode);
    }

    public void SetAdditionalMode(int i)
    {
        additionalMode = i;
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
    }
}
