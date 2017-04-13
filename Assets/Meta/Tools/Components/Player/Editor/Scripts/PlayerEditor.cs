using UnityEngine;
using UnityEditor;

namespace Meta.Tools.Editor
{
    [CustomEditor(typeof(Player))]
    public class PlayerEditor : UnityEditor.Editor
    {
        private Player _player;
        public void OnEnable()
        {
            _player = target as Player;
            if (_player.Script == null)
            {
                Undo.RecordObject(target, "New Story Created");
                _player.Script = NodeUtils.CreateNewGraph("New Story", (_player.GetInstanceID() + _player.GetHashCode() + Random.value*1000f).GetHashCode().ToString());
                serializedObject.FindProperty("_scriptName").stringValue = _player.Script.FileName;
            }
        }

        public override void OnInspectorGUI()
        {
            if (_player.Script != null)
            {
                _player.Script.Player = _player;
                //EditorGUILayout.LabelField("file name: " + _player.Script.name);
                string newStoryName = EditorGUILayout.TextField("Story name: ", _player.Script.GraphName);
                if (newStoryName != _player.Script.GraphName)
                {
                    Undo.RecordObject(_player.Script, "Script's Name Changed");
                    _player.Script.GraphName = newStoryName;
                }
                if (GUILayout.Button("Edit script"))
                {
                    Debug.Log("Button pressed");
                    NodeEditorWindow.InitEditorWindow(_player);
                }
            }
            /*EditorGUI.BeginChangeCheck();
            _player.Script = (NodeGraph)EditorGUILayout.ObjectField("Scenario: ", _player.Script, typeof(NodeGraph), false);
            if (EditorGUI.EndChangeCheck())
            {
                NodeGraph temp = _player.Script;
                _player.Script = null;
                Undo.RecordObject(target, "Apply scenario");
                _player.Script = temp;
            }*/
        }
    }
}
