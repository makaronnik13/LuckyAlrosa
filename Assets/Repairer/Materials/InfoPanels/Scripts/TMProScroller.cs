using UnityEngine;
using System.Collections;

public class TMProScroller : MonoBehaviour {

    [Range(0f, 1f)]
    public float openedArea = 1f;
    [Range(0f, 1f)]
    public float offset = 0f;

    private Vector3 _localPos = Vector3.zero;

    private Vector3 initialPos = Vector3.zero;

    public float ScrollSpeed = 1f;

    private float _scrollPos = 0f;
    public float ScrollPos
    {
        get { return _scrollPos; }
        set
        {
            _scrollPos = value;
            //GetComponent<TMPro.TextMeshProUGUI>().UpdateMaskCoords(_scrollPos, openedArea);
            _localPos.y = _scrollPos;
            transform.localPosition = initialPos + _localPos;
        }
    }

    public float MaxScrollPos = 1;

    public void ScrollDown()
    {
        ScrollPos += Time.deltaTime * ScrollSpeed;
        if (ScrollPos > MaxScrollPos)
        {
            ScrollPos = MaxScrollPos;
        }
    }

    public void ScrollUp()
    {
        ScrollPos -= Time.deltaTime * ScrollSpeed;
        if (ScrollPos < 0)
        {
            ScrollPos = 0f;
        }
    }

    public void Scroll(float value)
    {
        ScrollPos += value;

        if (ScrollPos < 0)
        {
            ScrollPos = 0f;
        }
        else if (ScrollPos > MaxScrollPos)
        {
            ScrollPos = MaxScrollPos;
        }
    }

    void Awake()
    {
        initialPos = transform.localPosition;
    }
}
