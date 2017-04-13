using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Generic;

namespace Meta.Tools.Editor
{
    /// <summary>
    /// As soon as Grouper was added to a GameObject, it preforms initial analize and assignment of 
    /// founded GameObjects to groups according to a default rule.
    /// </summary>
    [CustomEditor(typeof(Grouper))]
    public partial class GrouperEditor : UnityEditor.Editor
    {
        private Grouper _grouper;

        private void OnEnable()
        {
            _grouper = target as Grouper;
            setStylesAndContents();
            setCallbacks();
        }

        public override void OnInspectorGUI()
        {
            drawSummary();
            drawModeSelection();
            drawGroups();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnDisable()
        {
            Utilities.DeselectAllInSceneView();
        }
    }
}