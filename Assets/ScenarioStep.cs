using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

//[CreateAssetMenu(menuName = "ScenarioStep")]
public class ScenarioStep
{//: ScriptableObject

    public enum StepType
    {
        Material,
        Object,
        Animation,
        AudioClip
    }

    public bool show = true;

    public StepType stepType = StepType.AudioClip;
    public bool waitTillTheEnd;
    public GameObject go;
    public bool waitCommand = false;
    public float time;
    public AudioClip clip;
    public Material replacedMaterial;

    public string animatorParameter;
    public AnimatorControllerParameterType parameterType = AnimatorControllerParameterType.Trigger;
    public bool boolType;
    public float floatType;
    public int intType;
    public bool objectActive = true;
    public bool materialActive = true;
    internal bool activated = false;
}
