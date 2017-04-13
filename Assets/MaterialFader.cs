using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialFader : MonoBehaviour {

    private float currentAlpha = 1;

    private void Update()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            foreach (Material m in r.materials)
            {
                if (m.HasProperty("_Color"))
                {
                    Color c = m.GetColor("_Color");
                    c.a = currentAlpha;
                    m.SetColor("_Color", c);
                }
            }
        } 
    }

    IEnumerator LerpFade(float aim, float time)
    {
        float elapsedTime = 0;
        float startingValue = currentAlpha;
        while (elapsedTime < time)
        {
            currentAlpha = Mathf.Lerp(startingValue, aim, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    public void FadeIn(float time = 1, bool immideatly = false)
    {
        FadeTo(1, time, immideatly);
    }

    public void FadeOut(float time = 1, bool immideatly = false)
    {
        FadeTo(0, time, immideatly);
    }

    public void FadeTo(float aim, float time = 1, bool immideatly = false)
    {
        if (!immideatly)
        {
            StartCoroutine(LerpFade(aim, time));
        }
        else
        {
            currentAlpha = aim;
        }
    }
}
