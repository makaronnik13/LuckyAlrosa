using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
using System;

namespace Meta.Tools.Editor
{
    public partial class ModelExploderEditor : UnityEditor.Editor
    {
        private bool curentElementObserved;
        private bool lastGroupsVisible;
        private bool _groupsVisible = false;
        private bool _originVisible = false;
        private string textColor;

        private ReorderableList _groupsList;
        private List<float> heights = new List<float>();
        private SerializedProperty _serializedParts;
        private GameObject[] gos;
        private Texture2D tex;
        private Color elementBackgroundColor;
        private GUIContent partNameContent, activeContent;

        private void initializeGroupsList() {
            _serializedParts = serializedObject.FindProperty("_parts");

            _groupsList = new ReorderableList(serializedObject, _serializedParts,
                true, false, false, false);
            
            _groupsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
            {
                #region drawing background

                if (heights.Count <= index)
                {
                    heights.Clear();

                    for (int i = 0; i < _serializedParts.arraySize; i++)
                    {
                        heights.Add(EditorGUIUtility.singleLineHeight);
                    }
                }

                if (EditorGUIUtility.GUIToScreenPoint(rect.min + heights[index] * Vector2.one).y < 0 ||
                    EditorGUIUtility.GUIToScreenPoint(rect.min).y > Screen.height * 1.5f) {
                    return;
                }

                tex = new Texture2D(1, 1);
                if (!_modelExploder.Parts[index]
                .CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].HasSomethingUnique)
                {
                    if (_modelExploder.Parts[index].Active == true)
                    {
                        elementBackgroundColor = Utilities.ActiveElementColor;
                    }
                    else {
                        elementBackgroundColor = Utilities.InactiveElementColor;
                    }
                }
                else {
                    if (_modelExploder.Parts[index].Active == true)
                    {
                        elementBackgroundColor = Utilities.UniqueActiveElementColor;
                    }
                    else {
                        elementBackgroundColor = Utilities.UniqueInactiveActiveElementColor;
                    }
                }

                tex.SetPixel(0, 0, elementBackgroundColor);
                tex.Apply();
                Rect elementRect = rect;
                elementRect.width += 4;
                GUI.DrawTexture(elementRect, tex as Texture);

                #endregion drawing background

                rect.y += 2;

                #region drawing main line

                textColor = "#000000";

                if (_modelExploder.Parts[index].Active == false) {
                    textColor = "#888888";
                }

                partNameContent = new GUIContent();
                partNameContent.text = "<color=" + textColor + "><b> " + _modelExploder.Parts[index].Name + "</b></color>";

                EditorGUI.LabelField(
                    new Rect(rect.x + 8, rect.y, 150, EditorGUIUtility.singleLineHeight), 
                    partNameContent, 
                    Utilities.RichTextStyle
                );

                activeContent = new GUIContent();
                activeContent.text = "<color=" + textColor + ">Active: </color>";

                EditorGUI.LabelField(
                    new Rect(rect.x + 260, rect.y, 150, EditorGUIUtility.singleLineHeight),
                    activeContent,
                    Utilities.RichTextStyle
                );

                _groupsList.serializedProperty.GetArrayElementAtIndex(index)
                .FindPropertyRelative("Active").boolValue = EditorGUI.Toggle(
                    new Rect(rect.x + 300, rect.y, 140, EditorGUIUtility.singleLineHeight),
                    _modelExploder.Parts[index].Active
                );

                curentElementObserved = EditorGUI.Foldout(
                    new Rect(rect.x + 10, rect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight),
                    _modelExploder.Parts[index].Observed,
                    GUIContent.none
                );

                if (curentElementObserved != _modelExploder.Parts[index].Observed)
                {
                    if (curentElementObserved)
                    {
                        heights[index] = (2 + _modelExploder.GetGameObjectsFromPart(_modelExploder.Parts[index]).Length)
                        * EditorGUIUtility.singleLineHeight;
                    }
                    else {
                        heights[index] = EditorGUIUtility.singleLineHeight;
                    }
                }

                _modelExploder.Parts[index].Observed = curentElementObserved;

                #endregion drawing main line

                #region drawing subobjects

                int originHasChanged = -1;
                
                if (_modelExploder.Parts[index].Observed)
                {
                    EditorGUI.LabelField(
                        new Rect(rect.x + 8, rect.y += EditorGUIUtility.singleLineHeight, 150, EditorGUIUtility.singleLineHeight),
                        "Sub-objects list: ",
                        Utilities.RichTextStyle
                    );
                    
                    gos = _modelExploder.GetGameObjectsFromPart(_modelExploder.Parts[index]);

                    for (int i = 0; i < gos.Length; i++) {
                        EditorGUI.LabelField(
                            new Rect(rect.x + 16, rect.y += EditorGUIUtility.singleLineHeight,
                            150, EditorGUIUtility.singleLineHeight),
                            gos[i].name, 
                            Utilities.RichTextStyle
                        );
                        EditorGUI.LabelField(
                            new Rect(rect.x + 230, rect.y, 36, EditorGUIUtility.singleLineHeight),
                            "<size=9><i>Center: </i></size>", Utilities.RichTextStyle
                        );

                        bool oldIsOrigin = _groupsList.serializedProperty.GetArrayElementAtIndex(index)
                        .FindPropertyRelative("SubObjectsList").GetArrayElementAtIndex(i)
                        .FindPropertyRelative("_isOrigin").boolValue;

                        _groupsList.serializedProperty.GetArrayElementAtIndex(index)
                        .FindPropertyRelative("SubObjectsList").GetArrayElementAtIndex(i)
                        .FindPropertyRelative("_isOrigin").boolValue = EditorGUI.Toggle(
                            new Rect(rect.x + 266, rect.y, 140, EditorGUIUtility.singleLineHeight),
                            _modelExploder.Parts[index].SubObjectsList[i].IsOrigin);

                        if (oldIsOrigin != _groupsList.serializedProperty.GetArrayElementAtIndex(index)
                        .FindPropertyRelative("SubObjectsList").GetArrayElementAtIndex(i)
                        .FindPropertyRelative("_isOrigin").boolValue) {
                            originHasChanged = i;
                        }
                    }
                    
                    if (originHasChanged >= 0)
                    {
                        _groupsList.serializedProperty.GetArrayElementAtIndex(index)
                        .FindPropertyRelative("_originSubObjectIndex").intValue = originHasChanged;

                        for (int i = 0; i < _modelExploder.Parts[index].SubObjectsList.Count; i++)
                        {
                            if (i == originHasChanged)
                            {
                                _groupsList.serializedProperty.GetArrayElementAtIndex(index)
                                .FindPropertyRelative("SubObjectsList").GetArrayElementAtIndex(i)
                                .FindPropertyRelative("_isOrigin").boolValue = true;
                            }
                            else
                            {
                                _groupsList.serializedProperty.GetArrayElementAtIndex(index)
                                .FindPropertyRelative("SubObjectsList").GetArrayElementAtIndex(i)
                                .FindPropertyRelative("_isOrigin").boolValue = false;
                            }
                        }

                        serializedObject.ApplyModifiedProperties();
                        screenshootCameraChanged = true;
                        updateScreenshoots();
                    }
                }

                #endregion drawing subobjects
            };
            
            _groupsList.elementHeightCallback = (int index) => 
            {
                if (heights != null && heights.Count > index)
                {
                    return heights[index];
                }
                else
                {
                    return EditorGUIUtility.singleLineHeight;
                }
            };
            
            _groupsList.onSelectCallback = (ReorderableList list) => 
            {
                gos = _modelExploder.GetGameObjectsFromPart(_modelExploder.Parts[list.index]);
                _selectedPartIndex = list.index;
                Utilities.DeselectAllInSceneView();
                Utilities.SelectGameObjectsInSceneView(gos, Utilities.SceneViewSelectionType.Variant3);
                serializedObject.FindProperty("CurrentlySelectedPart").intValue = list.index;
            };
        }

        private void drawModelStructure()
        {
            #region drawing main origin

            Rect lastRect;

            EditorGUILayout.LabelField(_originLabelContent, Utilities.RichTextStyle);

            lastRect = GUILayoutUtility.GetLastRect();
            _originVisible = EditorGUI.Foldout(
                new Rect(lastRect.x + 10, lastRect.y + 4, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight),
                _originVisible, GUIContent.none);
            
            if (_originVisible)
            {
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Center GameObject: ", Utilities.RichTextStyle, GUILayout.Width(120));

                EditorGUI.BeginChangeCheck();
                serializedObject.FindProperty("_originGameObject")
                    .objectReferenceValue = EditorGUILayout.ObjectField(_modelExploder.OriginGameObject, typeof(GameObject), true);

                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(_modelExploder, "Revert Changes");
                    screenshootCameraChanged = true;
                    serializedObject.ApplyModifiedProperties();
                    updateScreenshoots();
                }

                EditorGUILayout.EndHorizontal();
            }

            #endregion drawing main origin

            #region drawing groups

            GUIContent groupsLabelContent = new GUIContent();
            groupsLabelContent.text = "<b><i><size=18>  Groups: </size></i></b>";
            groupsLabelContent.tooltip = "Groups helping you to keep well-structured representation of your model. In the list below are all groups that ModelExploder will be working with.";
            EditorGUILayout.LabelField(groupsLabelContent, Utilities.RichTextStyle);

            lastRect = GUILayoutUtility.GetLastRect();
            _groupsVisible = EditorGUI.Foldout(new Rect(lastRect.x + 10, lastRect.y + 4, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), _groupsVisible, GUIContent.none);

            if (_groupsVisible) {

                if (lastGroupsVisible != _groupsVisible) {
                    heights.Clear();
                    
                    for (int i = 0; i < _serializedParts.arraySize; i++)
                    {
                        heights.Add(EditorGUIUtility.singleLineHeight);
                    }
                }

                GUILayout.Space(5);

                _groupsList.DoLayoutList();
                EditorGUILayout.HelpBox("You can modify groups through the Grouper Component.", MessageType.Info);
                GUILayout.Space(5);
            }

            lastGroupsVisible = _groupsVisible;

            #endregion drawing groups
        }
    }
}