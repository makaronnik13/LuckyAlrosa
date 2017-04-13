using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class HitechLabelController : MonoBehaviour
{
    public bool useFader = true;
    private enum HitechLabelControllerState { Hided, Opening, Opened, Hiding }

    public float timeOfShowingAndHiding = 1.0f;

    private float counter = 0.0f;

    private Material labelMat;
    private TextMeshProUGUI text;
    private Image img;
    private Color baseColor;

    private HitechLabelControllerState state;

    private void Awake()
    {
        try
        {
            labelMat = transform.FindChild("Label").gameObject.GetComponent<Renderer>().material;
        }
        catch (Exception e)
        {

        }
        text = GetComponentInChildren<TextMeshProUGUI>();
        img = GetComponentInChildren<Image>();
        baseColor = img.color;
        HideImmediately();
        state = HitechLabelControllerState.Hided;
    }

    private void goToState(HitechLabelControllerState newState)
    {
        state = newState;

        switch (state)
        {
            case HitechLabelControllerState.Opening:
                LeanTween.cancel(gameObject);
                Color c1 = baseColor;
                c1.a = 0;
                if (text)
                {
                    text.color = c1;
                }
                if (img && !useFader)
                {
                    img.color = c1;
                }
                LeanTween.value(gameObject, counter, 1f, timeOfShowingAndHiding * (1 - counter)).setOnUpdate((float val) =>
                {
                    counter = val;
                    if (labelMat != null) labelMat.SetFloat("_PercentOfAppearing", counter);
                    c1.a = counter;
                    if (text)
                    {
                        text.color = c1;
                    }
                    if (img && !useFader)
                    {
                        img.color = c1;
                    }
                }).setOnComplete(() =>
                {
                    goToState(HitechLabelControllerState.Opened);
                });
                break;
            case HitechLabelControllerState.Hiding:
                LeanTween.cancel(gameObject);
                Color c2 = baseColor;
                c2.a = 0;
                if (text)
                {
                    text.color = c2;
                }
                if (img && !useFader)
                {
                    img.color = c2;
                }
                LeanTween.value(gameObject, counter, 0f, timeOfShowingAndHiding * counter).setOnUpdate((float val) =>
                {
                    counter = val;
                    if (labelMat != null) labelMat.SetFloat("_PercentOfAppearing", counter);
                    c2.a = counter;
                    if (text)
                    {
                        text.color = c2;
                    }
                    if (img && !useFader)
                    {
                        img.color = c2;
                    }
                }).setOnComplete(() =>
                {
                    goToState(HitechLabelControllerState.Hided);
                });
                break;
        }
    }

    public void Show(string lable)
    {
        text.text = lable;
        Show();
    }

    public void Show()
    {
        if ((state == HitechLabelControllerState.Hided) || (state == HitechLabelControllerState.Hiding))
        {
            goToState(HitechLabelControllerState.Opening);
        }
        if (GetComponent<InfoPanelFader>() && useFader)
        {
            GetComponent<InfoPanelFader>().Show();
        }
    }

    public void Hide()
    {
        if ((state == HitechLabelControllerState.Opened) || (state == HitechLabelControllerState.Opening))
        {
            goToState(HitechLabelControllerState.Hiding);
        }
        if (GetComponent<InfoPanelFader>() && useFader)
        {
            GetComponent<InfoPanelFader>().Hide();
        }
    }

    public void HideImmediately()
    {
        counter = 0;
        if (labelMat != null) labelMat.SetFloat("_PercentOfAppearing", 0f);
        Color c = baseColor;
        c.a = 0f;
        if (text)
        {
            text.color = c;
        }
        if (img && !useFader)
        {
            img.color = c;
        }
        goToState(HitechLabelControllerState.Hided);
    }
}