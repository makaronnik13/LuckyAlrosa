using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class FocusInfo : MonoBehaviour, IFocusable
{
    public InfoManager.InfoType type;
    public int id;

    public void OnFocusEnter()
    {
        FindObjectOfType<InfoManager>().ShowLable(type, id, transform);
    }

    public void OnFocusExit()
    {
        FindObjectOfType<InfoManager>().HideLable();
    }
}
