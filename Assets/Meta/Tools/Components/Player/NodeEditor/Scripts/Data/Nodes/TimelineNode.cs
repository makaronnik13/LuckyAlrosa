using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.Tools
{
    [Serializable]
    public class TimelineNode : NodeBase
    {
        #region Variables
        public int NodeSource = -1;

        public NodeInput PlayInput;
        public NodeInput StopInput;
        public NodeInput PauseInput;

        public NodeOutput OnEndOutput;
        #endregion

        #region Constructor
        public TimelineNode()
        {
            //Debug.Log("Created NEW!!");
        }
        #endregion

        #region Input Actions
        public void PlayInputAction(object obj = null)
        {
            (ParentGraph.Player.ReferenceHolder.Get(NodeSource) as Timeline).Play();
        }

        public void StopInputAction(object obj = null)
        {
            (ParentGraph.Player.ReferenceHolder.Get(NodeSource) as Timeline).Stop();
        }

        public void PauseInputAction(object obj = null)
        {
            (ParentGraph.Player.ReferenceHolder.Get(NodeSource) as Timeline).Pause();
        }

        public void OnAnimationEnded()
        {
            OnEndOutput.Invoke();
        }
        #endregion

        #region Main Methods
        public override void InitNode()
        {
            Debug.Log("Timeline InitNode");
            NodeRect = new Rect(NodeRect.x, NodeRect.y, 260f, 140f);
            base.InitNode();
            NodeType = NodeType.Timeline;

            PlayInput = (NodeInput)ScriptableObject.CreateInstance<NodeInput>();
            StopInput = (NodeInput)ScriptableObject.CreateInstance<NodeInput>();
            PauseInput = (NodeInput)ScriptableObject.CreateInstance<NodeInput>();
            OnEndOutput = (NodeOutput)ScriptableObject.CreateInstance<NodeOutput>();

            PlayInput.Parent = this;
            StopInput.Parent = this;
            PauseInput.Parent = this;
            OnEndOutput.Parent = this;

            PlayInput.Type = IOType.Action;
            StopInput.Type = IOType.Action;
            PauseInput.Type = IOType.Action;
            OnEndOutput.Type = IOType.Action;

            PlayInput.Name = "Play";
            StopInput.Name = "Stop";
            PauseInput.Name = "Pause";
            OnEndOutput.Name = "OnEnd";

            PlayInput.IORect = new Rect(IOWidth * 0.5f, NodeRect.height * 0.5f - IOWidth * 2f, IOWidth, IOWidth);
            StopInput.IORect = new Rect(IOWidth * 0.5f, NodeRect.height * 0.5f - IOWidth * 0.5f, IOWidth, IOWidth);
            PauseInput.IORect = new Rect(IOWidth * 0.5f, NodeRect.height * 0.5f + IOWidth, IOWidth, IOWidth);
            OnEndOutput.IORect = new Rect(NodeRect.width - IOWidth * 1.5f, NodeRect.height * 0.5f - IOWidth * 0.5f, IOWidth, IOWidth);

#if UNITY_EDITOR
            AssetDatabase.AddObjectToAsset(PlayInput, ParentGraph);
            AssetDatabase.AddObjectToAsset(StopInput, ParentGraph);
            AssetDatabase.AddObjectToAsset(PauseInput, ParentGraph);
            AssetDatabase.AddObjectToAsset(OnEndOutput, ParentGraph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        public override void SubscribeIO()
        {
            //base.SubscribeInputs();

            PlayInput.Action += PlayInputAction;
            StopInput.Action += StopInputAction;
            PauseInput.Action += PauseInputAction;

            (ParentGraph.Player.ReferenceHolder.Get(NodeSource) as Timeline)._animationEnded += OnAnimationEnded;
        }

        public override void UpdateNode(Event e, Rect viewRect)
        {
            base.UpdateNode(e, viewRect);

        }

        public override void DestroyIO()
        {
            base.DestroyIO();

            for (int i = 0; i < PlayInput.Outputs.Count; i++)
            {
                PlayInput.Outputs[i].Remove(PlayInput);
            }
            for (int i = 0; i < StopInput.Outputs.Count; i++)
            {
                StopInput.Outputs[i].Remove(StopInput);
            }
            for (int i = 0; i < PauseInput.Outputs.Count; i++)
            {
                PauseInput.Outputs[i].Remove(PauseInput);
            }
            for (int i = 0; i < OnEndOutput.Inputs.Count; i++)
            {
                OnEndOutput.Inputs[i].Remove(OnEndOutput);
            }

            NodeScriptsUtils.DeleteAsset(PlayInput);
            NodeScriptsUtils.DeleteAsset(StopInput);
            NodeScriptsUtils.DeleteAsset(PauseInput);
            NodeScriptsUtils.DeleteAsset(OnEndOutput);
        }

#if UNITY_EDITOR
        public override void DrawNodeProperties(GUISkin skin)
        {
            base.DrawNodeProperties(skin);

            //EditorGUILayout.LabelField(NodeName + "'s Properties:", skin.GetStyle("RichWordWrap"));

            EditorGUILayout.BeginVertical();
            Timeline source = ParentGraph.Player.ReferenceHolder.Get(NodeSource) as Timeline;

            if (source != null)
            {
                EditorGUILayout.LabelField("Timeline name:", skin.GetStyle("PropertiesViewStyleText"), GUILayout.Width(120f));
                Rect lastRect = GUILayoutUtility.GetLastRect();
                string newName = EditorGUI.TextField(new Rect(lastRect.x + 110f, lastRect.y + skin.GetStyle("PropertiesViewStyleText").padding.top, 140f, EditorGUIUtility.singleLineHeight), source.Name);
                if (newName != source.Name)
                {
                    Undo.RecordObject(source, "New Name Applied");
                    source.Name = newName;
                }


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Play On Awake:", skin.GetStyle("PropertiesViewStyleText"), GUILayout.Width(120f));
                bool newPlayOnAwake = EditorGUILayout.Toggle(source.PlayOnAwake);
                if (newPlayOnAwake != source.PlayOnAwake)
                {
                    Undo.RecordObject(source, "Play On Awake Setting Changed");
                    source.PlayOnAwake = newPlayOnAwake;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.LabelField("Timeline Source:", skin.GetStyle("PropertiesViewStyleText"));


            EditorGUI.BeginChangeCheck();
            source = (Timeline)EditorGUILayout.ObjectField(source, typeof(Timeline), true);
            if (EditorGUI.EndChangeCheck())
            {
                if (source != null)
                {
                    Undo.RecordObject(ParentGraph.Player, "Timeline Reference Added");
                    Undo.RecordObject(this, "Timeline Nodes Source Changed");
                    NodeSource = ParentGraph.Player.ReferenceHolder.Add(source);
                    EditorUtility.SetDirty(this);
                }
            }
            EditorGUILayout.EndVertical();
        }

        public override void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin)
        {
            base.UpdateNodeGUI(e, viewRect, viewSkin);

            MarginLeft = 40f;
            Rect labelRect = new Rect();

            labelRect = PlayInput.GlobalRect;
            labelRect.x += labelRect.width;
            labelRect.width = MarginLeft;
            GUI.Label(labelRect, PlayInput.Name, viewSkin.GetStyle("IOLabelsLeft"));

            labelRect = StopInput.GlobalRect;
            labelRect.x += labelRect.width;
            labelRect.width = MarginLeft;
            GUI.Label(labelRect, StopInput.Name, viewSkin.GetStyle("IOLabelsLeft"));

            labelRect = PauseInput.GlobalRect;
            labelRect.x += labelRect.width;
            labelRect.width = MarginLeft;
            GUI.Label(labelRect, PauseInput.Name, viewSkin.GetStyle("IOLabelsLeft"));

            MarginRight = 40f;

            labelRect = OnEndOutput.GlobalRect;
            labelRect.x -= MarginRight;
            labelRect.width += MarginRight - labelRect.width;
            GUI.Label(labelRect, OnEndOutput.Name, viewSkin.GetStyle("IOLabelsRight"));

            if (GUI.Button(PlayInput.GlobalRect, "", viewSkin.GetStyle("NodeOutput")))
            {
                if (ParentGraph != null)
                {
                    if (ParentGraph.ConnectionIO != null && ParentGraph.ConnectionIO.Parent != null)
                    {
                        if (ParentGraph.ConnectionIO is NodeOutput && ParentGraph.ConnectionIO.Type == PlayInput.Type)
                        {
                            Undo.RecordObject((ParentGraph.ConnectionIO as NodeOutput), "Connection Added");
                            (ParentGraph.ConnectionIO as NodeOutput).Add(PlayInput);
                            Undo.RecordObject(PlayInput, "Connection Added");
                            PlayInput.Add((ParentGraph.ConnectionIO as NodeOutput));
                            EditorUtility.SetDirty(PlayInput);
                            EditorUtility.SetDirty((ParentGraph.ConnectionIO as NodeOutput));
                            ParentGraph.ConnectionIO = null;
                            //NodeScriptsUtils.SaveAsset();
                        }
                    }
                    else
                    {
                        ParentGraph.ConnectionIO = PlayInput;
                    }
                }
            }

            if (GUI.Button(StopInput.GlobalRect, "", viewSkin.GetStyle("NodeOutput")))
            {
                if (ParentGraph != null)
                {
                    if (ParentGraph.ConnectionIO != null && ParentGraph.ConnectionIO.Parent != null)
                    {
                        if (ParentGraph.ConnectionIO is NodeOutput && ParentGraph.ConnectionIO.Type == StopInput.Type)
                        {
                            Undo.RecordObject((ParentGraph.ConnectionIO as NodeOutput), "Connection Added");
                            (ParentGraph.ConnectionIO as NodeOutput).Add(StopInput);
                            Undo.RecordObject(StopInput, "Connection Added");
                            StopInput.Add((ParentGraph.ConnectionIO as NodeOutput));
                            EditorUtility.SetDirty(StopInput);
                            EditorUtility.SetDirty((ParentGraph.ConnectionIO as NodeOutput));
                            ParentGraph.ConnectionIO = null;
                            //NodeScriptsUtils.SaveAsset();
                        }
                    }
                    else
                    {
                        ParentGraph.ConnectionIO = StopInput;
                    }
                }
            }

            if (GUI.Button(PauseInput.GlobalRect, "", viewSkin.GetStyle("NodeOutput")))
            {
                if (ParentGraph != null)
                {
                    if (ParentGraph.ConnectionIO != null && ParentGraph.ConnectionIO.Parent != null)
                    {
                        if (ParentGraph.ConnectionIO is NodeOutput && ParentGraph.ConnectionIO.Type == PauseInput.Type)
                        {
                            Undo.RecordObject((ParentGraph.ConnectionIO as NodeOutput), "Connection Added");
                            (ParentGraph.ConnectionIO as NodeOutput).Add(PauseInput);
                            Undo.RecordObject(PauseInput, "Connection Added");
                            PauseInput.Add((ParentGraph.ConnectionIO as NodeOutput));
                            EditorUtility.SetDirty(PauseInput);
                            EditorUtility.SetDirty((ParentGraph.ConnectionIO as NodeOutput));
                            ParentGraph.ConnectionIO = null;
                            //NodeScriptsUtils.SaveAsset();
                        }
                    }
                    else
                    {
                        ParentGraph.ConnectionIO = PauseInput;
                    }
                }
            }

            if (GUI.Button(OnEndOutput.GlobalRect, "", viewSkin.GetStyle("NodeOutput")))
            {
                if (ParentGraph != null)
                {
                    if (ParentGraph.ConnectionIO != null && ParentGraph.ConnectionIO.Parent != null)
                    {
                        if (ParentGraph.ConnectionIO is NodeInput && ParentGraph.ConnectionIO.Type == OnEndOutput.Type)
                        {
                            Undo.RecordObject(OnEndOutput, "Connection Added");
                            OnEndOutput.Add(ParentGraph.ConnectionIO as NodeInput);
                            Undo.RecordObject((ParentGraph.ConnectionIO as NodeInput), "Connection Added");
                            (ParentGraph.ConnectionIO as NodeInput).Add(OnEndOutput);
                            EditorUtility.SetDirty(OnEndOutput);
                            EditorUtility.SetDirty((ParentGraph.ConnectionIO as NodeInput));
                            ParentGraph.ConnectionIO = null;
                            //NodeScriptsUtils.SaveAsset();
                        }
                    }
                    else
                    {
                        ParentGraph.ConnectionIO = OnEndOutput;
                    }
                }
            }

            //DrawDefaultLabel(viewSkin);

            GUILayout.BeginArea(NodeInnerRect);

            GUILayout.Label(NodeName, viewSkin.GetStyle("NodeLabelStyle"));

            Timeline source = ParentGraph.Player.ReferenceHolder.Get(NodeSource) as Timeline;
            GUILayout.Space(10f);
            if (source != null)
            {
                GUILayout.Label("<size=16>" + source.Name + "</size>", viewSkin.GetStyle("NodeLabelStyle"));
            }
            else
            {
                GUILayout.Label("<size=16>Empty</size>", viewSkin.GetStyle("NodeLabelStyle"));
            }

            GUILayout.EndArea();

            /*GUILayout.BeginArea(NodeInnerRect);

            GUILayout.Label(NodeName, viewSkin.GetStyle("NodeLabelStyle"));

            Timeline source = ParentGraph.Player.ReferenceHolder.Get(NodeSource) as Timeline;
            if (source != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name: ", viewSkin.GetStyle("RichWordWrap"), GUILayout.Width(50f));
                string newName = EditorGUILayout.TextField(source.Name);
                if (newName != source.Name)
                {
                    Undo.RecordObject(source, "New Name Applied");
                    source.Name = newName;
                    EditorUtility.SetDirty(source);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Source:", viewSkin.GetStyle("RichWordWrap"), GUILayout.Width(50f));
            //Timeline source = (Timeline)EditorUtility.InstanceIDToObject(NodeSource);
            EditorGUI.BeginChangeCheck();
            source = (Timeline)EditorGUILayout.ObjectField(source, typeof(Timeline), true);
            if (EditorGUI.EndChangeCheck())
            {
                if (source != null)
                {
                    Undo.RecordObject(ParentGraph.Player, "Timeline Reference Added");
                    Undo.RecordObject(this, "Timeline Nodes Source Changed");
                    NodeSource = ParentGraph.Player.ReferenceHolder.Add(source);
                    EditorUtility.SetDirty(this);
                    //NodeSource = source.GetInstanceID();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();*/

            //DrawIOLines();
        }
#endif
        #endregion

        #region Utility Methods
#if UNITY_EDITOR
        public override void DrawIOLines()
        {
            base.DrawIOLines();
            if (OnEndOutput.Inputs.Count > 0)
            {
                Handles.BeginGUI();

                Handles.color = Color.white;

                for (int i = 0; i < OnEndOutput.Inputs.Count; i++)
                {
                    BezierDrawing.DrawBezierFromTo(OnEndOutput.CenterRect,
                        OnEndOutput.Inputs[i].CenterRect,
                        Color.white,
                        Color.gray,
                        false);
                }

                Handles.EndGUI();
            }
        }
#endif

        public override NodeIO GetIOAtCoordinates(Vector2 localMousePosition)
        {
            if (PlayInput.IORect.Contains(localMousePosition))
            {
                return PlayInput;
            }
            if (StopInput.IORect.Contains(localMousePosition))
            {
                return StopInput;
            }
            if (PauseInput.IORect.Contains(localMousePosition))
            {
                return PauseInput;
            }
            if (OnEndOutput.IORect.Contains(localMousePosition))
            {
                return OnEndOutput;
            }
            return null;
        }
        #endregion
    }
}
