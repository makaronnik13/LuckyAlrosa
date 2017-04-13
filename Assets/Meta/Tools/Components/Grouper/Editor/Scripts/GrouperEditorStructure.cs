using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Generic;

namespace Meta.Tools.Editor
{
    public partial class GrouperEditor : UnityEditor.Editor
    {
        private static float _layoutIndentAmount = 20f;
        private bool curentElementObserved;

        private GameObject[] gos;
        private ReorderableList _groupsList;
        private SerializedProperty element, _serializedParts;

        private void setCallbacks()
        {
            #region Setting callbacks

            heights.Clear();

            for (int i = 0; i < _serializedParts.arraySize; i++)
            {
                heights.Add((_serializedParts.GetArrayElementAtIndex(i).FindPropertyRelative("_partObserved").boolValue ? 1 : 0)
                    * _serializedParts.GetArrayElementAtIndex(i).FindPropertyRelative("SubObjectsList").arraySize
                    * EditorGUIUtility.singleLineHeight + 2 * EditorGUIUtility.singleLineHeight);
            }

            _groupsList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Groups: ");
            };

            _groupsList.elementHeightCallback = (int index) =>
            {
                try
                {
                    return heights[index];
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.LogWarning(e.Message);
                    return 2 * EditorGUIUtility.singleLineHeight;
                }
            };

            _groupsList.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (EditorGUIUtility.GUIToScreenPoint(rect.min + heights[index] * Vector2.one).y < 0 ||
                    EditorGUIUtility.GUIToScreenPoint(rect.min).y > Screen.height * 1.5f)
                {
                    return;
                }

                rect.y += 2;
                element = _groupsList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 96, EditorGUIUtility.singleLineHeight), "Groups's name:");

                EditorGUI.PropertyField(new Rect(rect.x + 96, rect.y, 200, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("_name"), GUIContent.none);

                curentElementObserved = _serializedParts.GetArrayElementAtIndex(index)
                .FindPropertyRelative("_partObserved").boolValue;

                _serializedParts.GetArrayElementAtIndex(index)
                .FindPropertyRelative("_partObserved").boolValue = EditorGUI.Foldout(
                    new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, 60, EditorGUIUtility.singleLineHeight),
                    curentElementObserved,
                    "Contained sub objects"
                );

                if (curentElementObserved != _serializedParts.GetArrayElementAtIndex(index)
                .FindPropertyRelative("_partObserved").boolValue)
                {
                    heights[index] = (_serializedParts.GetArrayElementAtIndex(index)
                    .FindPropertyRelative("_partObserved").boolValue ? 1 : 0)
                    * (0.5f + _serializedParts.GetArrayElementAtIndex(index).FindPropertyRelative("SubObjectsList").arraySize)
                    * EditorGUIUtility.singleLineHeight + 2 * EditorGUIUtility.singleLineHeight;
                    curentElementObserved = !curentElementObserved;
                }

                rect.x += _layoutIndentAmount;

                if (curentElementObserved)
                {
                    gos = _grouper.GetGameObjectsFromPart(_grouper.Parts[index]);

                    for (int i = 0; i < gos.Length; i++)
                    {
                        EditorGUI.ObjectField(
                            new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2
                            + EditorGUIUtility.singleLineHeight * i, 300, EditorGUIUtility.singleLineHeight),
                            serializedObject.FindProperty("_gameObjects")
                            .GetArrayElementAtIndex(_grouper.GetElementIndex(index, i)), GUIContent.none);
                    }
                }
            };

            _groupsList.onSelectCallback = (ReorderableList list) =>
            {
                GameObject[] gos = _grouper.GetGameObjectsFromPart(_grouper.Parts[list.index]);
                Utilities.DeselectAllInSceneView();
                Utilities.SelectGameObjectsInSceneView(gos, Utilities.SceneViewSelectionType.Variant3);
                serializedObject.FindProperty("CurrentlySelectedPart").intValue = list.index;
            };

            #endregion Setting callbacks
        }
    }
}