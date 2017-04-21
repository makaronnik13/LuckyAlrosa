using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartInfoPanel : MonoBehaviour {

	public void ShowInfo(PartInformation part)
    {
        GetComponentInChildren<InfoPanelFader>().Show();
        GetComponentInChildren<TextMeshProUGUI>().text = part.Info();
        transform.SetParent(part.gameObject.transform);
    }

    public void ShowInfo(string inf, GameObject go)
    {
        GetComponentInChildren<InfoPanelFader>().Show();
        GetComponentInChildren<TextMeshProUGUI>().text = inf;
        transform.SetParent(go.transform);
    }

    public void HideInfo()
    {
        GetComponentInChildren<InfoPanelFader>().Hide();
    }

    private void Start()
    {
        HideInfo();
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime);
    }
}
