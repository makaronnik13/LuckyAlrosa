using UnityEngine;
using System.Collections;

namespace Meta
{
    [ExecuteInEditMode]
    public class AimAtMainCamera : MonoBehaviour
    {
        private void Update()
        {
            transform.LookAt(Camera.main.transform);
            transform.forward = -transform.forward;
        }
    }
}