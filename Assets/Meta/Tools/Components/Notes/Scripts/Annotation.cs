using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Meta;

[ExecuteInEditMode]
[RequireComponent(typeof(AudioSource))]
public class Annotation : MonoBehaviour
{
    [SerializeField]
    private InputField _text;

    [SerializeField]
    private Transform _lineStartPoint;
    [SerializeField]
    private Transform _lineMiddlePoint;
    [SerializeField]
    private Transform _lineEndPoint;

    [SerializeField]
    private Transform _urPoint, _ulPoint, _drPoint, _dlPoint; // up/down/left/right
    private Transform _selectedCorner;

    private float _middlePointPercentage = 1.1f;

    private GameObject _selectedGameObject;
    private Vector3 _initialScale;
    [SerializeField]
    private float _interpolationSpeed = 0.2f;
    [SerializeField]
    private float _radius = 105;
    private float _collisionRadius;
    private float _increment;

    private float _initialDistance;
    private float _breakLineFactor = 1.5f;
    [SerializeField]
    private Color _breakLineFeedbackColor;
    private Color _defaultColor;
    private bool _isGrabbing = false;
    private AudioSource _deleteSoundFeedback;
    private FadeLines _fade;

    private TransformLineRenderer _line;
    private Material _lineMaterial;

    [SerializeField]
    private string _layerName = "Annotation";
    private int _layerID;

    // Variables to change the size of the note according to the size of the object
    float optimalSizePerMeter = 0.007f;

    private void Awake()
    {
        _line = GetComponentInChildren<TransformLineRenderer>();
        _deleteSoundFeedback = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _layerID = LayerMask.NameToLayer(_layerName);
        _lineMaterial = _line.transform.GetComponent<Renderer>().sharedMaterial;
        _defaultColor = _lineMaterial.color;

        _initialScale = transform.lossyScale;

        _fade = GetComponent<FadeLines>();
        _fade.Initialize();
    }

    public void Initialize(GameObject selectedGO)
    {
        Awake();
        Start();

        transform.SetParent(selectedGO.transform);
        _lineEndPoint.position = selectedGO.transform.position;

        SetLineTransforms();
        CalculateLinePoints(selectedGO);

        Show();
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        _fade.HideLines();
    }

    [ContextMenu("Show")]
    public void Show()
    {
        _fade.ShowLines();
    }

    public void SetAnnotationContent(string content)
    {
        _text.text = content;
    }

    private void CalculateLinePoints(GameObject selectedGO)
    {
        _selectedGameObject = selectedGO;
        CalculatePositions(_selectedGameObject.GetComponent<Renderer>());
        _initialDistance = Vector3.Distance(_lineStartPoint.position, _lineEndPoint.position);
        SetLineTransforms();
    }

    private void SetLineTransforms()
    {
        _line.SetPositions(new Transform[3] { _lineStartPoint, _lineMiddlePoint, _lineEndPoint });
    }

    private void FixedUpdate()
    {
        _selectedCorner = GetClosestPoint();
        _lineStartPoint.position = Vector3.MoveTowards(_lineStartPoint.position, _selectedCorner.position, _interpolationSpeed * Time.deltaTime);
        _lineMiddlePoint.position = (_lineStartPoint.position + _lineEndPoint.position) / 2f * _middlePointPercentage;
        SetLineTransforms();

        if (_isGrabbing)
        {
            CheckLineConnection();
        }
    }

    private void LateUpdate()
    {
        if (!Mathf.Approximately(transform.lossyScale.x, _initialScale.x) ||
            !Mathf.Approximately(transform.lossyScale.y, _initialScale.y) ||
            !Mathf.Approximately(transform.lossyScale.z, _initialScale.z))
        {
            float xFactor = transform.lossyScale.x / _initialScale.x;
            float yFactor = transform.lossyScale.y / _initialScale.y;
            float zFactor = transform.lossyScale.z / _initialScale.z;

            float x, y, z;
            if (Mathf.Approximately(transform.lossyScale.x, 0f))
            {
                x = transform.localScale.x;
            }
            else
            {
                x = transform.localScale.x / xFactor;
            }

            if (Mathf.Approximately(transform.lossyScale.y, 0f))
            {
                y = transform.localScale.y;
            }
            else
            {
                y = transform.localScale.y / yFactor;
            }

            if (Mathf.Approximately(transform.lossyScale.z, 0f))
            {
                z = transform.localScale.z;
            }
            else
            {
                z = transform.localScale.z / zFactor;
            }

            transform.localScale = new Vector3(x, y, z);
            _lineEndPoint.position = transform.parent.position;
        }
    }

    public void OnGrabNote()
    {
        _isGrabbing = true;
    }

    public void OnReleaseNote()
    {
        _isGrabbing = false;
        // Destroy the line
        if (_lineMaterial.color == _breakLineFeedbackColor)
        {
            Hide();
            //_deleteSoundFeedback.Play();
            //Destroy(gameObject, _deleteSoundFeedback.clip.length);
            Destroy(gameObject);
        }
    }

    private void CheckLineConnection()
    {
        // Set line color for feedback
        if (Vector3.SqrMagnitude(_lineStartPoint.position - _lineEndPoint.position) > _initialDistance * _breakLineFactor * _initialDistance * _breakLineFactor)
        {
            _lineMaterial.SetColor("_Color", _breakLineFeedbackColor);
        }
        else
        {
            _lineMaterial.SetColor("_Color", _defaultColor);
        }
    }

    private Transform GetClosestPoint()
    {
        if (_selectedGameObject == null) return _ulPoint;

        float ul = Vector3.SqrMagnitude(_selectedGameObject.transform.position - _ulPoint.position);
        float ur = Vector3.SqrMagnitude(_selectedGameObject.transform.position - _urPoint.position);
        float dl = Vector3.SqrMagnitude(_selectedGameObject.transform.position - _dlPoint.position);
        float dr = Vector3.SqrMagnitude(_selectedGameObject.transform.position - _drPoint.position);
        if (ul < ur && ul < dl && ul < dr)
        {
            return _ulPoint;
        }
        else if (ur < dl && ur < dr)
        {
            return _urPoint;
        }
        else if (dl < dr)
        {
            return _dlPoint;
        }
        else
        {
            return _drPoint;
        }
    }

    private void CalculatePositions(Renderer renderer)
    {
        Bounds bounds = new Bounds(renderer.transform.position, Vector3.zero);
        MeshRenderer[] renderers = renderer.transform.root.GetComponentsInChildren<MeshRenderer>();
        // create box around the renderers
        for (int i = 0; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 size = bounds.size;
        Vector3 max = bounds.max;
        Vector3 min = bounds.min;
        Vector3 avg = (max + min) / 2;

        bool isRight, isUp, isFront;
        isRight = isUp = isFront = false;

        // check if the object is on the left/right, up/down, forward/backward in relation to the whole object using the bounds to avoid pivot problems
        if (renderer.transform.position.x > avg.x)
        {
            isRight = true;
        }
        if (renderer.transform.position.y > avg.y)
        {
            isUp = true;
        }
        if (renderer.transform.position.z > avg.z)
        {
            isFront = true;
        }

        // set direction of the note
        Vector3 direction = new Vector3(isRight ? 1 : -1, isUp ? 1 : -1, isFront ? 1 : -1);


        // Change initial scale of the note top match the size of the object
        float maximumSize;
        if (bounds.size.x > bounds.size.y && bounds.size.x > bounds.size.z)
        {
            maximumSize = bounds.size.x;
        }
        else if (bounds.size.y > bounds.size.z)
        {
            maximumSize = bounds.size.y;
        }
        else
        {
            maximumSize = bounds.size.z;
        }

        float maximumScale;
        Vector3 lossyScale = transform.parent.lossyScale;
        if (lossyScale.x > lossyScale.y && lossyScale.x > lossyScale.z)
        {
            maximumScale = lossyScale.x;
        }
        else if (lossyScale.y > lossyScale.z)
        {
            maximumScale = lossyScale.y;
        }
        else
        {
            maximumScale = lossyScale.z;
        }
        transform.localScale = Vector3.one * optimalSizePerMeter / maximumSize * maximumScale;
        _initialScale = transform.lossyScale;
        
        // Change line width according to the scale of the note
        LineRenderer[] lr;
        lr = GetComponentsInChildren<LineRenderer>();
        for (int i = 0; i < lr.Length; i++)
        {
            print(lr[i].startWidth + " " + lr[i].endWidth);
            lr[i].startWidth = maximumScale / 10;
            lr[i].endWidth = maximumScale / 10;
        }



        float distance = 0;
        _collisionRadius = _radius * transform.lossyScale.x;
        _increment = _collisionRadius / 2;

        // increase the distance while the sphere is colliding with something
        while (true)
        {
            Vector3 pos = renderer.transform.position + direction * (_collisionRadius + distance);
            Collider[] hitColliders = Physics.OverlapSphere(pos, _collisionRadius);

            if (hitColliders.Length == 0)
            {
                break;
            }

            distance += _increment;
        }

        Vector3 offset = direction * (_collisionRadius + distance);

        transform.position = renderer.transform.position + offset;
        _lineMiddlePoint.position = renderer.transform.position + offset / 2;
        _lineEndPoint.position = renderer.transform.position;

        if (isUp)
        {
            if (isRight)
            {
                _lineStartPoint.position = _dlPoint.position;
            }
            else
            {
                _lineStartPoint.position = _drPoint.position;
            }
        }
        else
        {
            if (isRight)
            {
                _lineStartPoint.position = _ulPoint.position;
            }
            else
            {
                _lineStartPoint.position = _urPoint.position;
            }
        }
    }
}
