using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


[CustomEditor(typeof(ScenariosPlayer))]

public class ScenarioStepEditor: Editor {

    private ReorderableList reorderableList;
    private ReorderableList reorderableList2;

    private Scenario selectedScenario;
    private ScenarioStep selectedStep;


    private void OnEnable()
    {

        ScenariosPlayer sp = (ScenariosPlayer)target;

        reorderableList = new ReorderableList(sp.scenarios, typeof(Scenario), true, true, true, true);
        reorderableList2 = new ReorderableList(new List<ScenarioStep>(), typeof(ScenarioStep), true, true, true, true);

        // This could be used aswell, but I only advise this your class inherrits from UnityEngine.Object or has a CustomPropertyDrawer
        // Since you'll find your item using: serializedObject.FindProperty("list").GetArrayElementAtIndex(index).objectReferenceValue
        // which is a UnityEngine.Object
        // reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("list"), true, true, true, true);

        // Add listeners to draw events
        reorderableList.drawHeaderCallback += DrawHeader;
        reorderableList.drawElementCallback += DrawElement;

        reorderableList.onSelectCallback += Select;

        reorderableList.onAddCallback += AddItem;
        reorderableList.onRemoveCallback += RemoveItem;

        reorderableList2.drawHeaderCallback += DrawHeaderStep;
        reorderableList2.drawElementCallback += DrawElementStep;
        reorderableList2.onSelectCallback += SelectStep;
        reorderableList2.onAddCallback += AddItemStep;
        reorderableList2.onRemoveCallback += RemoveItemStep;
        reorderableList2.elementHeightCallback += Height;
        reorderableList2.drawElementBackgroundCallback += Background;
    }

    private void Background(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.height = Height(index);
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, new Color(0.33f, 0.66f, 1f, 0.66f));
        tex.Apply();
        if (isActive)
            GUI.DrawTexture(rect, tex as Texture);
    }

    private float Height(int index)
    {
        float height = 20;

        if (selectedStep!=null && index>-1 && index<selectedScenario.scenarioSteps.Count() && selectedScenario.scenarioSteps[index] == selectedStep)
        {
            switch (selectedStep.stepType)
            {
                case ScenarioStep.StepType.Animation:
                    height += 40;
                    break;
                case ScenarioStep.StepType.Material:
                    height += 40;
                    break;
                case ScenarioStep.StepType.Object:
                    height += 30;
                    break;
                case ScenarioStep.StepType.AudioClip:
                    height += 30;
                    break;
            }
        }
        return height;
    }

    private void Select(ReorderableList list)
    {
        selectedScenario = ((ScenariosPlayer)target).scenarios[list.index];

        reorderableList2 = new ReorderableList(selectedScenario.scenarioSteps, typeof(ScenarioStep), true, true, true, true);
        reorderableList2.drawHeaderCallback += DrawHeaderStep;
        reorderableList2.drawElementCallback += DrawElementStep;
        reorderableList2.onSelectCallback += SelectStep;
        reorderableList2.onAddCallback += AddItemStep;
        reorderableList2.onRemoveCallback += RemoveItemStep;
        reorderableList2.elementHeightCallback += Height;
        reorderableList2.drawElementBackgroundCallback += Background;
    }

    private void SelectStep(ReorderableList list)
    {
        selectedStep = selectedScenario.scenarioSteps[list.index];
    }

    private void OnDisable()
    {
        // Make sure we don't get memory leaks etc.
        reorderableList.drawHeaderCallback -= DrawHeader;
        reorderableList.drawElementCallback -= DrawElement;

        reorderableList.onSelectCallback -= Select;

        reorderableList.onAddCallback -= AddItem;
        reorderableList.onRemoveCallback -= RemoveItem;


        reorderableList2.drawHeaderCallback -= DrawHeaderStep;
        reorderableList2.drawElementCallback -= DrawElementStep;
        reorderableList2.onAddCallback -= AddItemStep;
        reorderableList2.onRemoveCallback -= RemoveItemStep;
    }

    /// <summary>
    /// Draws the header of the list
    /// </summary>
    /// <param name="rect"></param>
    private void DrawHeader(Rect rect)
    {
        GUI.Label(rect, "Scenarios");
    }

    private void DrawHeaderStep(Rect rect)
    {
        GUI.Label(rect, "Steps");
    }

    /// <summary>
    /// Draws one element of the list (ListItemExample)
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="index"></param>
    /// <param name="active"></param>
    /// <param name="focused"></param>
    private void DrawElement(Rect rect, int index, bool active, bool focused)
    {
        Scenario item = ((ScenariosPlayer)target).scenarios[index];

        EditorGUI.BeginChangeCheck();
        //item.boolValue = EditorGUI.Toggle(new Rect(rect.x, rect.y, 18, rect.height), item.boolValue);
        item.ScenarioName = EditorGUI.TextField(new Rect(rect.x + 18, rect.y, rect.width - 18, rect.height), item.ScenarioName);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
        }

        // If you are using a custom PropertyDrawer, this is probably better
        // EditorGUI.PropertyField(rect, serializedObject.FindProperty("list").GetArrayElementAtIndex(index));
        // Although it is probably smart to cach the list as a private variable ;)
    }

    private void DrawElementStep(Rect rect, int index, bool active, bool focused)
    {
        ScenarioStep item = selectedScenario.scenarioSteps[index];

        EditorGUI.BeginChangeCheck();
        //item.boolValue = EditorGUI.Toggle(new Rect(rect.x, rect.y, 18, rect.height), item.boolValue);
        item.stepType = (ScenarioStep.StepType)EditorGUI.EnumPopup(new Rect(rect.x + 18, rect.y, 160, rect.height), item.stepType);
        item.time = EditorGUI.FloatField(new Rect(rect.x + 180, rect.y, 50, rect.height), item.time);
        item.waitCommand = EditorGUI.Toggle(new Rect(rect.x + 230, rect.y, 20, rect.height), item.waitCommand);

        if (selectedStep == item)
        {
            switch (item.stepType)
            {
                case ScenarioStep.StepType.Animation:
                    item.animatorParameter = EditorGUI.TextField(new Rect(rect.x + 18, rect.y+rect.height, 160, rect.height*1.5f), "", item.animatorParameter);
                    item.parameterType = (UnityEngine.AnimatorControllerParameterType)EditorGUI.EnumPopup(new Rect(rect.x + 180, rect.y + rect.height, 80, rect.height/2), "", item.parameterType);
                    switch (item.parameterType)
                    {
                        case UnityEngine.AnimatorControllerParameterType.Bool:
                            item.boolType = EditorGUI.Toggle(new Rect(rect.x + 180, rect.y + rect.height*1.7f, 20, rect.height*0.7f), item.boolType);
                            break;
                        case UnityEngine.AnimatorControllerParameterType.Float:
                            item.floatType = EditorGUI.FloatField(new Rect(rect.x + 180, rect.y + rect.height*1.7f, 50, rect.height * 0.7f), item.floatType);
                            break;
                        case UnityEngine.AnimatorControllerParameterType.Int:
                            item.intType = EditorGUI.IntField(new Rect(rect.x + 180, rect.y + rect.height*1.7f, 50, rect.height * 0.7f), item.intType);
                            break;
                    }
                    break;
                case ScenarioStep.StepType.Material:
                    
                    item.materialActive = EditorGUI.ToggleLeft(new Rect(rect.x + 18, rect.y + rect.height, 60, rect.height), "", item.materialActive);
                    if (item.materialActive)
                    {
                        item.replacedMaterial = (Material)EditorGUI.ObjectField(new Rect(rect.x + 40, rect.y + rect.height*2, 100, rect.height), "", item.replacedMaterial, typeof(Material), false);
                    }
                    item.go = (GameObject)EditorGUI.ObjectField(new Rect(rect.x + 40, rect.y + rect.height, 100, rect.height), "", item.go, typeof(GameObject), true);
                    break;
                case ScenarioStep.StepType.Object:
                    item.objectActive = EditorGUI.Toggle(new Rect(rect.x + 18, rect.y+rect.height, 20, rect.height), item.objectActive);
                    item.go = (GameObject)EditorGUI.ObjectField(new Rect(rect.x + 40, rect.y + rect.height, 100, rect.height),"", item.go, typeof(GameObject), true);
                    break;
                case ScenarioStep.StepType.AudioClip:
                    item.clip = (AudioClip)EditorGUI.ObjectField(new Rect(rect.x + 18, rect.y+rect.height, 100, rect.height), "", item.clip, typeof(AudioClip), true);
                    //item.waitTillTheEnd = EditorGUILayout.Toggle("Wait end: ", item.waitTillTheEnd);
                    break;
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
        }

        // If you are using a custom PropertyDrawer, this is probably better
        // EditorGUI.PropertyField(rect, serializedObject.FindProperty("list").GetArrayElementAtIndex(index));
        // Although it is probably smart to cach the list as a private variable ;)
    }

    private void AddItem(ReorderableList list)
    {
        ((ScenariosPlayer)target).scenarios.Add(new Scenario());

        EditorUtility.SetDirty(target);
    }

    private void AddItemStep(ReorderableList list)
    {

        selectedScenario.scenarioSteps.Add(new ScenarioStep());

        EditorUtility.SetDirty(target);
    }

    private void RemoveItem(ReorderableList list)
    {
        ((ScenariosPlayer)target).scenarios.RemoveAt(list.index);

        EditorUtility.SetDirty(target);
    }

    private void RemoveItemStep(ReorderableList list)
    {
        selectedScenario.scenarioSteps.RemoveAt(list.index);

        EditorUtility.SetDirty(target);
    }

    public override void OnInspectorGUI()
    {
        // Actually draw the list in the inspector
        reorderableList.DoLayoutList();

        if (selectedScenario!=null)
        {
            reorderableList2.DoLayoutList();
        }
    }
}

    
   
