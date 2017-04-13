using UnityEngine;
using System.Collections;

public enum HitechTextPromoterState { Static, ScrollingDown, ScrollingUp}

public class HitechTextPromoter : MonoBehaviour {

    [Range(0.001f, 1f)]
    public float ScrollSpeed = 0.1f;

    public GameObject ScrolledObject;
    private Vector2 _initialPos = Vector2.zero;
    private Vector2 _endPosition;
    public float EndScrollYPosition = 1f;
    private Vector2 _currentPosition;

    private HitechTextPromoterState state;

    void Start()
    {
        _initialPos = ScrolledObject.GetComponent<RectTransform>().anchoredPosition;
        _currentPosition = _initialPos;
        _endPosition = new Vector2(_initialPos.x, EndScrollYPosition);

        state = HitechTextPromoterState.Static;
    }

	public void ScrollUp()
    {
        state = HitechTextPromoterState.ScrollingUp;
        _currentPosition.y -= Time.deltaTime * ScrollSpeed;
        _currentPosition.y = Mathf.Clamp(_currentPosition.y, _initialPos.y, _endPosition.y);
        ScrolledObject.GetComponent<RectTransform>().anchoredPosition = _currentPosition;
    }

    public void ScrollDown()
    {
        state = HitechTextPromoterState.ScrollingDown;
        _currentPosition.y += Time.deltaTime * ScrollSpeed;
        _currentPosition.y = Mathf.Clamp(_currentPosition.y, _initialPos.y, _endPosition.y);
        ScrolledObject.GetComponent<RectTransform>().anchoredPosition = _currentPosition;
    }

    void Update()
    {
        switch (state)
        {
            case HitechTextPromoterState.Static:
                break;
            case HitechTextPromoterState.ScrollingUp:
                break;
            case HitechTextPromoterState.ScrollingDown:
                break;
        }
    }
}