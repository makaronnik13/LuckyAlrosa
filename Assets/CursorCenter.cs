using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorCenter : MonoBehaviour {

    public void ShowState(int i)
    {
        Hide();
        GetComponentsInChildren<InfoPanelFader>()[i].Show();
    }

    public void Hide()
    {
        foreach (InfoPanelFader fader in GetComponentsInChildren<InfoPanelFader>())
        {
            fader.Hide();
        }
    }
}
