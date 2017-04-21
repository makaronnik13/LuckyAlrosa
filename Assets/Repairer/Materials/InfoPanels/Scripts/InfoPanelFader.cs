using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class InfoPanelFader : MonoBehaviour {

    public bool Opened
    {
        get
        {
            if (state != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    int state = 0;
    int wantedState = 0;
    public float ShowingTime = 0.89f;
    public List<Renderer> RenderersToFade = new List<Renderer>();
    public List<TextMeshProUGUI> TextsToFade = new List<TextMeshProUGUI>();
    public List<GameObject> GameObjectsToSetActive = new List<GameObject>();
    public List<Image> ImagesToFade = new List<Image>();
    public List<Color> BaseColors = new List<Color>();

    Material[] mats;
    Color[] initialColors;
    float[] initialAlphas;

    // Use this for initialization
    void Awake () {
        mats = new Material[RenderersToFade.Count];
        initialColors = new Color[mats.Length];
        initialAlphas = new float[initialColors.Length];
        for (int i = 0; i < initialColors.Length; i++)
        {
            mats[i] = RenderersToFade[i].material;
            initialColors[i] = mats[i].color;
            initialAlphas[i] = initialColors[i].a;
        }

        foreach (Image img in ImagesToFade)
        {
            BaseColors.Add(img.color);
        }
        HideImmediately();
    }

    public void Show(System.Action callback = null)
    {

        wantedState = 1;
        if (state == 0)
        {
            state = 1;

            for (int i = 0; i < GameObjectsToSetActive.Count; i++)
            {
                GameObjectsToSetActive[i].SetActive(true);
            }

            Color c;
            LeanTween.value(gameObject, 0, 1, ShowingTime).setEase(LeanTweenType.easeInOutCubic).setOnUpdate((float val) =>
            {
                for (int i = 0; i < mats.Length; i++)
                {
                    c = initialColors[i];
                    c.a = Mathf.Lerp(0, initialAlphas[i], val);
                    mats[i].color = c;
                }
                for (int i = 0; i < TextsToFade.Count; i++)
                {
                    c = Color.white;
                    c.a = Mathf.Lerp(0, 1, val);
                    TextsToFade[i].color = c;
                }
                for (int i = 0; i < ImagesToFade.Count; i++)
                {
                    c = BaseColors[i];
                    c.a = Mathf.Lerp(0, BaseColors[i].a, val);
                    ImagesToFade[i].color = c;
                }
            }).setOnComplete(() =>
            {
                state = 2;

                if (wantedState != 1)
                {
                    Hide();
                }

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

    public void ShowImmediately()
    {
        Debug.Log("show im");
        state = 2;

        for (int i = 0; i < GameObjectsToSetActive.Count; i++)
        {
            GameObjectsToSetActive[i].SetActive(true);
        }

        Color c;
        for (int i = 0; i < mats.Length; i++)
        {
            c = initialColors[i];
            c.a = initialAlphas[i];
            mats[i].color = c;
        }
        for (int i = 0; i < TextsToFade.Count; i++)
        {
            c = Color.white;
            TextsToFade[i].color = c;
        }
        for (int i = 0; i < ImagesToFade.Count; i++)
        {
            c = BaseColors[i];
            ImagesToFade[i].color = c;
        }
    }

    public void Hide(System.Action callback = null)
    {

        wantedState = 0;
        if (state == 2)
        {
            state = 3;
            Color c;
            LeanTween.value(gameObject, 1, 0, ShowingTime).setEase(LeanTweenType.easeInOutCubic).setOnStart(() =>
            {
                /*for (int i = 0; i < GameObjectsToSetActive.Count; i++)
                {
                    GameObjectsToSetActive[i].SetActive(false);
                }*/
            }).setOnUpdate((float val) =>
            {
                for (int i = 0; i < mats.Length; i++)
                {
                    c = initialColors[i];
                    c.a = Mathf.Lerp(0, initialAlphas[i], val);
                    mats[i].color = c;
                }
                for (int i = 0; i < TextsToFade.Count; i++)
                {
                    c = Color.white;
                    c.a = Mathf.Lerp(0, 1, val);
                    TextsToFade[i].color = c;
                }
                for (int i = 0; i < ImagesToFade.Count; i++)
                {
                    c = BaseColors[i];
                    c.a = Mathf.Lerp(0, BaseColors[i].a, val);
                    ImagesToFade[i].color = c;
                }
            }).setOnComplete(() =>
            {
                state = 0;

                for (int i = 0; i < GameObjectsToSetActive.Count; i++)
                {
                    GameObjectsToSetActive[i].SetActive(false);
                }

                if (wantedState != 0)
                {
                    Show();
                }

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

    public void HideImmediately()
    {
        state = 0;
        Color c;
        for (int i = 0; i < mats.Length; i++)
        {
            c = initialColors[i];
            c.a = 0;
            mats[i].color = c;
        }
        for (int i = 0; i < TextsToFade.Count; i++)
        {
            c = Color.white;
            c.a = 0;
            TextsToFade[i].color = c;
        }
        for (int i = 0; i < ImagesToFade.Count; i++)
        {
            c = BaseColors[i];
            c.a = 0;
            ImagesToFade[i].color = c;
        }
        for (int i = 0; i < GameObjectsToSetActive.Count; i++)
        {
            GameObjectsToSetActive[i].SetActive(false);
        }
    }
}
