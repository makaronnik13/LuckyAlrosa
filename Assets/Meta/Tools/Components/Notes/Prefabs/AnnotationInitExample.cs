using UnityEngine;
using System.Collections;
using Meta;

public class AnnotationInitExample : MonoBehaviour
{
    [SerializeField]
    private GameObject _annotationPrefab;
    [SerializeField]
    private GameObject _selectedGO;

    [ContextMenu("Initialize")]
    private void Initialize ()
    {
        GameObject annotationGO = GameObject.Instantiate(_annotationPrefab);

        annotationGO.transform.position = transform.position;

        Annotation annotation = annotationGO.GetComponentInChildren<Annotation>();
        annotation.Initialize(_selectedGO);
        //annotation.SetAnnotationContent("Content");
    }
}
