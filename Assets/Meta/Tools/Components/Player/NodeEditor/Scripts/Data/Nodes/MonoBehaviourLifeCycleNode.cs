using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.Tools
{
    [Serializable]
    public class MonoBehaviourLifeCycleNode : NodeBase
    {
        #region Variables
        public NodeOutput AwakeOutput;
        public NodeOutput StartOutput;
        public NodeOutput UpdateOutput;
        #endregion

        #region Constructor
        public MonoBehaviourLifeCycleNode()
        {
        }
        #endregion

        #region Main Methods
        public override void InitNode()
        {
            //Debug.Log("MonoBehaviourLifeCycleNode INIT!!!");
            NodeRect = new Rect(NodeRect.x, NodeRect.y, 250f, 150f);
            base.InitNode();
            NodeType = NodeType.MonoBehaviourLifeCycle;

            AwakeOutput = (NodeOutput)ScriptableObject.CreateInstance<NodeOutput>();
            StartOutput = (NodeOutput)ScriptableObject.CreateInstance<NodeOutput>();
            UpdateOutput = (NodeOutput)ScriptableObject.CreateInstance<NodeOutput>();

            AwakeOutput.Parent = this;
            StartOutput.Parent = this;
            UpdateOutput.Parent = this;

            AwakeOutput.Type = IOType.Action;
            StartOutput.Type = IOType.Action;
            UpdateOutput.Type = IOType.Action;

            AwakeOutput.Name = "Awake";
            StartOutput.Name = "Start";
            UpdateOutput.Name = "Update";

            AwakeOutput.IORect = new Rect(NodeRect.width - IOWidth * 1.5f, NodeRect.height * 0.5f - IOWidth * 2f, IOWidth, IOWidth);
            StartOutput.IORect = new Rect(NodeRect.width - IOWidth * 1.5f, NodeRect.height * 0.5f - IOWidth * 0.5f, IOWidth, IOWidth);
            UpdateOutput.IORect = new Rect(NodeRect.width - IOWidth * 1.5f, NodeRect.height * 0.5f + IOWidth, IOWidth, IOWidth);
            
#if UNITY_EDITOR
            AssetDatabase.AddObjectToAsset(AwakeOutput, ParentGraph);
            AssetDatabase.AddObjectToAsset(StartOutput, ParentGraph);
            AssetDatabase.AddObjectToAsset(UpdateOutput, ParentGraph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif

            Debug.Log("NodeRect = " + NodeRect);
        }

        public override void UpdateNode(Event e, Rect viewRect)
        {
            base.UpdateNode(e, viewRect);

        }

        public override void DestroyIO()
        {
            base.DestroyIO();

            for (int i = 0; i < AwakeOutput.Inputs.Count; i++)
            {
                AwakeOutput.Inputs[i].Remove(AwakeOutput);
            }
            for (int i = 0; i < StartOutput.Inputs.Count; i++)
            {
                StartOutput.Inputs[i].Remove(StartOutput);
            }
            for (int i = 0; i < UpdateOutput.Inputs.Count; i++)
            {
                UpdateOutput.Inputs[i].Remove(UpdateOutput);
            }

            NodeScriptsUtils.DeleteAsset(AwakeOutput);
            NodeScriptsUtils.DeleteAsset(StartOutput);
            NodeScriptsUtils.DeleteAsset(UpdateOutput);
        }

#if UNITY_EDITOR
        public override void DrawNodeProperties(GUISkin skin)
        {
            base.DrawNodeProperties(skin);

            EditorGUILayout.LabelField(@"In Unity scripting, there are a number of event functions that get executed in a predetermined order as a script executes. This execution order is described below:

<b>Awake:</b> This function is always called before any Start functions and also just after a prefab is instantiated. (If a GameObject is inactive during start up Awake is not called until it is made active.)

<b>Start:</b> Start is called before the first frame update only if the script instance is enabled.

<b>Update:</b> Update is called once per frame. It is the main workhorse function for frame updates.",
                skin.GetStyle("PropertiesViewStyleText"));
        }

        public override void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin)
        {
            base.UpdateNodeGUI(e, viewRect, viewSkin);

            MarginRight = 40f;
            Rect labelRect = new Rect();

            labelRect = AwakeOutput.GlobalRect;
            labelRect.x -= MarginRight;
            labelRect.width += MarginRight - labelRect.width;
            GUI.Label(labelRect, AwakeOutput.Name, viewSkin.GetStyle("IOLabelsRight"));

            labelRect = StartOutput.GlobalRect;
            labelRect.x -= MarginRight;
            labelRect.width += MarginRight - labelRect.width;
            GUI.Label(labelRect, StartOutput.Name, viewSkin.GetStyle("IOLabelsRight"));

            labelRect = UpdateOutput.GlobalRect;
            labelRect.x -= MarginRight;
            labelRect.width += MarginRight - labelRect.width;
            GUI.Label(labelRect, UpdateOutput.Name, viewSkin.GetStyle("IOLabelsRight"));

            //Awake
            if (GUI.Button(AwakeOutput.GlobalRect, "", viewSkin.GetStyle("NodeOutput")))
            {
                if (ParentGraph != null)
                {
                    if (ParentGraph.ConnectionIO != null && ParentGraph.ConnectionIO.Parent != null)
                    {
                        if (ParentGraph.ConnectionIO is NodeInput && ParentGraph.ConnectionIO.Type == AwakeOutput.Type)
                        {
                            Undo.RecordObject(AwakeOutput, "Connection Added");
                            AwakeOutput.Add(ParentGraph.ConnectionIO as NodeInput);
                            Undo.RecordObject((ParentGraph.ConnectionIO as NodeInput), "Connection Added");
                            (ParentGraph.ConnectionIO as NodeInput).Add(AwakeOutput);
                            EditorUtility.SetDirty(AwakeOutput);
                            EditorUtility.SetDirty((ParentGraph.ConnectionIO as NodeInput));
                            ParentGraph.ConnectionIO = null;
                            //NodeScriptsUtils.SaveAsset();
                        }
                    }
                    else
                    {
                        ParentGraph.ConnectionIO = AwakeOutput;
                    }
                }
            }

            //Start
            if (GUI.Button(StartOutput.GlobalRect, "", viewSkin.GetStyle("NodeOutput")))
            {
                if (ParentGraph != null)
                {
                    if (ParentGraph.ConnectionIO != null && ParentGraph.ConnectionIO.Parent != null)
                    {
                        if (ParentGraph.ConnectionIO is NodeInput && ParentGraph.ConnectionIO.Type == StartOutput.Type)
                        {
                            Undo.RecordObject(StartOutput, "Connection Added");
                            StartOutput.Add(ParentGraph.ConnectionIO as NodeInput);
                            Undo.RecordObject((ParentGraph.ConnectionIO as NodeInput), "Connection Added");
                            (ParentGraph.ConnectionIO as NodeInput).Add(StartOutput);
                            EditorUtility.SetDirty(StartOutput);
                            EditorUtility.SetDirty((ParentGraph.ConnectionIO as NodeInput));
                            ParentGraph.ConnectionIO = null;
                            //NodeScriptsUtils.SaveAsset();
                        }
                    }
                    else
                    {
                        ParentGraph.ConnectionIO = StartOutput;
                    }
                }
            }

            //Update
            if (GUI.Button(UpdateOutput.GlobalRect, "", viewSkin.GetStyle("NodeOutput")))
            {
                if (ParentGraph != null)
                {
                    if (ParentGraph.ConnectionIO != null && ParentGraph.ConnectionIO.Parent != null)
                    {
                        if (ParentGraph.ConnectionIO is NodeInput && ParentGraph.ConnectionIO.Type == UpdateOutput.Type)
                        {
                            Undo.RecordObject(this, "Connection Added");
                            Undo.RecordObject(UpdateOutput, "Connection Added");
                            UpdateOutput.Add(ParentGraph.ConnectionIO as NodeInput);
                            Undo.RecordObject((ParentGraph.ConnectionIO as NodeInput), "Connection Added");
                            (ParentGraph.ConnectionIO as NodeInput).Add(UpdateOutput);
                            EditorUtility.SetDirty(UpdateOutput);
                            EditorUtility.SetDirty((ParentGraph.ConnectionIO as NodeInput));
                            ParentGraph.ConnectionIO = null;
                            //NodeScriptsUtils.SaveAsset();
                        }
                    }
                    else
                    {
                        ParentGraph.ConnectionIO = UpdateOutput;
                    }
                }
            }

            DrawDefaultLabel(viewSkin);

            //DrawIOLines();
        }
#endif
        #endregion

        #region Utility Methods
#if UNITY_EDITOR
        public override void DrawIOLines()
        {
            base.DrawIOLines();

            if (AwakeOutput.Inputs.Count > 0 || StartOutput.Inputs.Count > 0 || UpdateOutput.Inputs.Count > 0)
            {
                Handles.BeginGUI();

                Handles.color = Color.white;

                for (int i = 0; i < AwakeOutput.Inputs.Count; i++)
                {
                    BezierDrawing.DrawBezierFromTo(AwakeOutput.CenterRect,
                        AwakeOutput.Inputs[i].CenterRect,
                        Color.white,
                        Color.gray,
                        false);
                }
                for (int i = 0; i < StartOutput.Inputs.Count; i++)
                {
                    BezierDrawing.DrawBezierFromTo(StartOutput.CenterRect,
                        StartOutput.Inputs[i].CenterRect,
                        Color.white,
                        Color.gray,
                        false);
                }
                for (int i = 0; i < UpdateOutput.Inputs.Count; i++)
                {
                    BezierDrawing.DrawBezierFromTo(UpdateOutput.CenterRect,
                        UpdateOutput.Inputs[i].CenterRect,
                        Color.white,
                        Color.gray,
                        false);
                }

                /*Handles.DrawLine(new Vector3(ConnectionIO.Parent.NodeRect.x + ConnectionIO.IORect.center.x,
                    ConnectionIO.Parent.NodeRect.y + ConnectionIO.IORect.center.y, 0f),
                    new Vector3(mousePosition.x, mousePosition.y, 0f));*/

                Handles.EndGUI();
            }
        }
#endif

        public override NodeIO GetIOAtCoordinates(Vector2 localMousePosition)
        {
            if (AwakeOutput.IORect.Contains(localMousePosition))
            {
                return AwakeOutput;
            }
            if (StartOutput.IORect.Contains(localMousePosition))
            {
                return StartOutput;
            }
            if (UpdateOutput.IORect.Contains(localMousePosition))
            {
                return UpdateOutput;
            }
            return null;
        }
        #endregion
    }
}
