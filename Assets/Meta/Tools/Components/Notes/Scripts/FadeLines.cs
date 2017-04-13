using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class FadeLines : MonoBehaviour
{
    LineRenderer[] lines;
    [SerializeField]
    float time = 3;
    float elapsedTime = 0;
    bool fadingIn, fadingOut;
    Text[] texts;
    Material[] materials;

    private void Start()
    {
        lines = GetComponentsInChildren<LineRenderer>();
        texts = GetComponentsInChildren<Text>();

        materials = new Material[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            materials[i] = new Material(lines[i].sharedMaterial);
        }
    }

    public void Initialize()
    {
        Start();
    }

    private void Update()
    {
        if (fadingIn)
        {
            FadeIn();
        }
        else if (fadingOut)
        {
            FadeOut();
        }
    }

    public void HideLines()
    {
        fadingOut = true;
        fadingIn = false;
    }

    public void ShowLines()
    {
        fadingIn = true;
        fadingOut = false;
    }

    public void HideLinesImmediate()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].enabled = false;
            materials[i].SetFloat("_Fade1", 1f);
            materials[i].SetFloat("_Fade2", 1f);
            lines[i].sharedMaterial = materials[i];
        }
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].enabled = false;
            texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 0f);
        }
    }

    public void ShowLinesImmediate()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].enabled = true;
            materials[i].SetFloat("_Fade1", 0.5f);
            materials[i].SetFloat("_Fade2", 0.5f);
            lines[i].sharedMaterial = materials[i];
        }
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].enabled = true;
            texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 1f);
        }
    }

    private void FadeOut()
    {
        elapsedTime += Time.deltaTime;
        for (int i = 0; i < lines.Length; i++)
        {
            float fadeFactor = Mathf.Lerp(0.5f, 1, elapsedTime / time);
            materials[i].SetFloat("_Fade1", fadeFactor);
            materials[i].SetFloat("_Fade2", fadeFactor);
            lines[i].sharedMaterial = materials[i];
        }

        for (int i = 0; i < texts.Length; i++)
        {
            float fadeFactor = Mathf.Lerp(255, 0, elapsedTime / time);
            texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, fadeFactor / 255);
        }

        if (elapsedTime > time)
        {
            fadingOut = false;
            elapsedTime = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i].enabled = false;
            }
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].enabled = false;
            }
        }
    }

    private void FadeIn()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].enabled = true;
        }
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].enabled = true;
        }

        elapsedTime += Time.deltaTime;
        for (int i = 0; i < lines.Length; i++)
        {
            float fadeFactor = Mathf.Lerp(1, 0.5f, elapsedTime / time);
            materials[i].SetFloat("_Fade1", fadeFactor);
            materials[i].SetFloat("_Fade2", fadeFactor);
            lines[i].sharedMaterial = materials[i];
        }
        for (int i = 0; i < texts.Length; i++)
        {
            float fadeFactor = Mathf.Lerp(0, 255, elapsedTime / time);
            texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, fadeFactor / 255);
        }

        if (elapsedTime > time)
        {
            fadingIn = false;
            elapsedTime = 0;
        }
    }
}
