using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Generic;

namespace Meta.Tools.Editor
{
    public partial class GrouperEditor : UnityEditor.Editor
    {
        private GUIContent _nameContent = new GUIContent();
        private GUIContent _groupsLabelContent = new GUIContent();

        private void setStylesAndContents()
        {
            _serializedParts = serializedObject.FindProperty("_parts");
            _groupsList = new ReorderableList(serializedObject, _serializedParts, true, true, true, true);

            _nameContent.text = "Part's name:";
            _nameContent.tooltip = "This will be the identifier for object or a group of objects that will be contained in this logical part";

            _groupsLabelContent.text = "<b><i><size=18>  Groups: </size></i></b>";
            _groupsLabelContent.tooltip = "Groups helping you to keep well-structured representation of your model. In the list below are all groups that ModelExploder will be working with.";
        }
    }
}