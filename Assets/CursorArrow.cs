using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorArrow : MonoBehaviour {

    public Color activeColor;
    private Color aimColor;
    private Image img;
    private bool active = false;
    private bool hidden = true;
    public bool Active
    {
        get
        {
            return active;
        }
        set
        {
            if (!hidden) {
                active = value;
                if (active)
                {
                    aimColor = activeColor;
                }
                else
                {
                    aimColor = Color.white;
                }
            }
        }
    }

    private void Awake()
    {
        img = GetComponent<Image>();

        aimColor = img.color;
    }

    private void Update()
    {
        img.color = Color.Lerp(img.color, aimColor, Time.deltaTime * 10);
    }


	public void HideAdditional()
    {
        foreach (Image fader in GetComponentsInChildren<Image>())
        {
            Color c = fader.color;
            c.a = 0;
            fader.color = c;
        }
    }

    public void ShowAdditional(int i)
    {
        HideAdditional();
        Color c = GetComponentsInChildren<Image>()[i].color;
        c.a = 1;
        GetComponentsInChildren<Image>()[i].color = c;
    }

    public void Hide()
    {
        Color c = img.color;
        c.a = 0;
        aimColor = c;
        hidden = true;
    }

    public void Show()
    {
        Color c = img.color;
        c.a = 1;
        aimColor = c;
        hidden = false;
    }

}
