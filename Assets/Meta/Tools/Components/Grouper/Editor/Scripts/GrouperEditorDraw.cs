using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Generic;

namespace Meta.Tools.Editor
{
    public partial class GrouperEditor : UnityEditor.Editor
    {
        private bool _groupsVisible;

        private Rect lastRect;
        private List<float> heights = new List<float>();

        private void drawSummary()
        {
            #region Summary

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("<i><b>    Summary:</b></i>", Utilities.TitleStyle);
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("GameObjects: <b>" + Utilities.GetGameObjectsCount(_grouper.gameObject) + "</b>", Utilities.RichTextStyle);
            EditorGUILayout.LabelField("Rendered GameObjects <b>: " + Utilities.GetMeshRenderedGameObjectsCount(_grouper.gameObject) + "</b>", Utilities.RichTextStyle);
            EditorGUILayout.LabelField("Groups: <b>" + _serializedParts.arraySize + "</b>", Utilities.RichTextStyle);
            Vector3 bounds = Utilities.GetGameObjectsAABBBoundingBoxsDimensions(_grouper.gameObject);
            decimal boundsX = Math.Round((decimal)bounds.x, 2, MidpointRounding.AwayFromZero);
            decimal boundsY = Math.Round((decimal)bounds.y, 2, MidpointRounding.AwayFromZero);
            decimal boundsZ = Math.Round((decimal)bounds.z, 2, MidpointRounding.AwayFromZero);
            EditorGUILayout.LabelField("Real-world dimensions (meters): ( <b>X:</b> " + boundsX.ToString() + ", <b>Y:</b> " + boundsY.ToString() + ", <b>Z:</b> " + boundsZ.ToString() + " )", Utilities.RichTextStyle);
            lastRect = GUILayoutUtility.GetLastRect();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            #endregion Summary
        }

        private void drawModeSelection()
        {
            #region Auto Distribution Mode

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Auto-Distribution Mode: ", GUILayout.Width(140));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_autoDistributionMode"), GUIContent.none, GUILayout.Width(146));
            if (GUILayout.Button("Apply"))
            {
                _grouper.PerformAutoDistribution();
                serializedObject.Update();
                heights.Clear();
                for (int i = 0; i < _serializedParts.arraySize; i++)
                {
                    heights.Add((_serializedParts.GetArrayElementAtIndex(i).FindPropertyRelative("_partObserved").boolValue ? 1 : 0) * _serializedParts.GetArrayElementAtIndex(i).FindPropertyRelative("SubObjectsList").arraySize * EditorGUIUtility.singleLineHeight + 2 * EditorGUIUtility.singleLineHeight);
                }
            }
            EditorGUILayout.EndHorizontal();

            string helpText = "";
            switch (serializedObject.FindProperty("_autoDistributionMode").enumValueIndex)
            {
                case (int)Grouper.AutoDistributionMode.Single:
                    helpText = "This type of auto-distribution simply allocates a group for each gameObject in the hierarchy that contains Renderer.";
                    break;
                case (int)Grouper.AutoDistributionMode.Hierarchy:
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Hierarchy Level: ");
                    serializedObject.FindProperty("_hierarchyLevelOfAutoDistributionRequired").intValue = EditorGUILayout.IntField(serializedObject.FindProperty("_hierarchyLevelOfAutoDistributionRequired").intValue);

                    EditorGUILayout.EndHorizontal();

                    if (serializedObject.FindProperty("_hierarchyLevelOfAutoDistributionRequired").intValue < 0)
                    {
                        serializedObject.FindProperty("_hierarchyLevelOfAutoDistributionRequired").intValue = 0;
                    }

                    helpText = "With this type of auto-distribution a level in the hierarchy is used to distribute GameObjects to groups. You must specify what level of hierarchy to use.";
                    break;
            }

            EditorGUILayout.HelpBox(helpText, MessageType.Info);

            #endregion Auto Distribution Mode
        }

        private void drawGroups()
        {
            #region Groups

            EditorGUILayout.LabelField(_groupsLabelContent, Utilities.RichTextStyle);
            lastRect = GUILayoutUtility.GetLastRect();
            _groupsVisible = EditorGUI.Foldout(
                new Rect(lastRect.x + 10, lastRect.y + 4, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight),
                _groupsVisible, GUIContent.none
            );

            if (_groupsVisible)
            {
                GUILayout.Space(5);
                _groupsList.DoLayoutList();
            }

            GUILayout.Space(5);

            #endregion Groups
        }
    }
}