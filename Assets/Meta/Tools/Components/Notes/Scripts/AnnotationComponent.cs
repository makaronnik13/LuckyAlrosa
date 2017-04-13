using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta;

public class AnnotationComponent : MonoBehaviour {

    [SerializeField, HideInInspector]
    private GameObject _gameObject;

    public Annotation GetAnnotation()
    {
        if (_gameObject == null)
        {
            Initialize();
        }
        Annotation annotation = _gameObject.GetComponentInChildren<Annotation>();
        return annotation;
    }

    public void Show()
    {
        if (_gameObject == null)
        {
            Initialize();
        }
        Annotation annotation = _gameObject.GetComponentInChildren<Annotation>();
        if (annotation != null)
        {
            annotation.Show();
        }
    }

    public void Hide()
    {
        if (_gameObject == null)
        {
            Initialize();
        }
        Annotation annotation = _gameObject.GetComponentInChildren<Annotation>();
        if (annotation != null)
        {
            annotation.Hide();
        }
    }

    internal void SetContent(string content)
    {
        if (_gameObject == null)
        {
            Initialize();
        }
        Annotation annotation = _gameObject.GetComponentInChildren<Annotation>();
        if (annotation != null)
        {
            annotation.SetAnnotationContent(content);
        }
    }

    internal void Initialize()
    {
        if (_gameObject != null)
        {
            return;
        }
        _gameObject = (GameObject)Instantiate(Resources.Load("AnnotationPrefabs/Annotation"));

        _gameObject.transform.parent = transform;
        _gameObject.transform.position = transform.position;

        Annotation annotation = _gameObject.GetComponentInChildren<Annotation>();

        //_gameObject.GetComponentInChildren<TransformLineRenderer>().Initialize();

        //annotation.CalculateLinePoints(gameObject);
        annotation.Initialize(gameObject);
    }
}
