using UnityEngine;
using HoloToolkit.Unity;

public class Stone3DViewController : Singleton<Stone3DViewController> {

    public GameObject Stone;
    Material mat;

    private bool brought = false;
    public void BringToScene(Vector3 targetSize, System.Action callback = null)
    {
        if (!brought)
        {
            brought = true;
            LeanTween.scale(transform.GetChild(0).gameObject, targetSize, 1f).setOnComplete(() =>
            {
                if (callback != null)
                {
                    callback.Invoke();
                }
            });
        }
        else
        {
            if (callback != null)
            {
                callback.Invoke();
            }
        }
    }

    public void RemoveFromScene(System.Action callback = null)
    {
        if (brought)
        {
            brought = true;
            collider.enabled = false;
            GetComponent<InfoPanelFader>().Hide();
            LeanTween.scale(transform.GetChild(0).gameObject, Vector3.one * 0.01f, 1f).setOnComplete(() =>
            {
                if (callback != null)
                {
                    callback.Invoke();
                }
            });
        }
        else
        {
            if (callback != null)
            {
                callback.Invoke();
            }
        }
    }

    private new BoxCollider collider;
    private void Awake()
    {
        mat = Stone.GetComponent<Renderer>().material;
        collider = GetComponent<BoxCollider>();
        collider.enabled = false;
    }

    public void Show(string text = "")
    {
        if (text != "")
        {
            collider.enabled = true;
            Texture tex = Resources.Load(text) as Texture;
            mat.SetTexture("_MainTex", tex);
            GetComponent<InfoPanelFader>().Show();
        }
        else
        {
            collider.enabled = false;
            GetComponent<InfoPanelFader>().Hide();
        }
    }

    public void Hide(System.Action callback = null)
    {
        collider.enabled = false;
        GetComponent<InfoPanelFader>().Hide(callback);
    }
}
