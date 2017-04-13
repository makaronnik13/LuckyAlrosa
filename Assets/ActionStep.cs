using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionStep : MonoBehaviour {
    
    [System.Serializable]
    public struct MaterialReplacement
    {
        public GameObject gameObject;
        public Material material;
    }

    public struct ObjectActivator
    {
        public GameObject gameObject;
        public bool value;
    }

    public MaterialReplacement[] materialReplacements;
    public string[] activationTriggers;
    public ObjectActivator[] activators;
}
