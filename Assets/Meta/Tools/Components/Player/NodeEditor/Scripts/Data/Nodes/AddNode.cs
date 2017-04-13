using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.Tools
{
    [Serializable]
    public class AddNode : NodeBase
    {
        #region Variables
        //[SerializeField]
        //private float _valueA;
        //[SerializeField]
        //private float _valueB;
        public float NodeSum;
        public NodeOutput Output;
        public NodeInput InputA;
        public NodeInput InputB;
        #endregion

        #region Constructor
        public AddNode()
        {
        }
        #endregion

        #region Input Actions
        public void InputAAction(object obj = null)
        {
            if (obj != null)
            {
                //_valueA = (float)obj;
            }
        }

        public void InputBAction(object obj = null)
        {
            if (obj != null)
            {
                //_valueB = (float)obj;
            }
        }
        #endregion

        #region Main Methods
        public override void InitNode()
        {
            NodeRect = new Rect(NodeRect.x, NodeRect.y, 160f, 80f);
            base.InitNode();
            NodeType = NodeType.Add;

            Output = new NodeOutput();
            InputA = new NodeInput();
            InputB = new NodeInput();

            Output.Parent = this;
            InputA.Parent = this;
            InputB.Parent = this;

            Output.Type = IOType.Numeric;
            InputA.Type = IOType.Numeric;
            InputB.Type = IOType.Numeric;

            Output.Name = "Result";
            InputA.Name = "Value A";
            InputB.Name = "Value B";

            Output.IORect = new Rect(NodeRect.width - IOWidth * 1.5f, NodeRect.height * 0.5f - IOWidth * 0.5f, IOWidth, IOWidth);
            InputA.IORect = new Rect(IOWidth * 0.5f, NodeRect.height * 0.5f - IOWidth * 1.5f, IOWidth, IOWidth);
            InputB.IORect = new Rect(IOWidth * 0.5f, NodeRect.height * 0.5f + IOWidth * 0.5f, IOWidth, IOWidth);
        }

        public override void SubscribeIO()
        {
            base.SubscribeIO();

            InputA.Action += InputAAction;
            InputB.Action += InputBAction;
        }

        public override void UpdateNode(Event e, Rect viewRect)
        {
            base.UpdateNode(e, viewRect);

        }

#if UNITY_EDITOR
        public override void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin)
        {
            base.UpdateNodeGUI(e, viewRect, viewSkin);

            //Output
            if (GUI.Button(new Rect(NodeRect.x + Output.IORect.x, NodeRect.y + Output.IORect.y, Output.IORect.width, Output.IORect.height), "", viewSkin.GetStyle("NodeOutput")))
            {
                if (ParentGraph != null)
                {
                    if (ParentGraph.ConnectionIO != null && ParentGraph.ConnectionIO.Parent != null)
                    {
                        if (ParentGraph.ConnectionIO is NodeInput && ParentGraph.ConnectionIO.Type == Output.Type)
                        {
                            Output.Add(ParentGraph.ConnectionIO as NodeInput);
                            ParentGraph.ConnectionIO = null;
                        }
                    }
                    else
                    {
                        ParentGraph.ConnectionIO = Output;
                    }
                }
            }

            //InputA
            if (GUI.Button(new Rect(NodeRect.x + InputA.IORect.x, NodeRect.y + InputA.IORect.y, InputA.IORect.width, InputA.IORect.height), "", viewSkin.GetStyle("NodeOutput")))
            {
                if (ParentGraph != null)
                {
                    if (ParentGraph.ConnectionIO != null && ParentGraph.ConnectionIO.Parent != null)
                    {
                        if (ParentGraph.ConnectionIO is NodeOutput && ParentGraph.ConnectionIO.Type == InputA.Type)
                        {
                            (ParentGraph.ConnectionIO as NodeOutput).Add(InputA);
                            ParentGraph.ConnectionIO = null;
                        }
                    }
                    else
                    {
                        ParentGraph.ConnectionIO = InputA;
                    }
                }
            }

            //InputB
            if (GUI.Button(new Rect(NodeRect.x + InputB.IORect.x, NodeRect.y + InputB.IORect.y, InputB.IORect.width, InputB.IORect.height), "", viewSkin.GetStyle("NodeOutput")))
            {
                if (ParentGraph != null)
                {
                    if (ParentGraph.ConnectionIO != null && ParentGraph.ConnectionIO.Parent != null)
                    {
                        if (ParentGraph.ConnectionIO is NodeOutput && ParentGraph.ConnectionIO.Type == InputB.Type)
                        {
                            (ParentGraph.ConnectionIO as NodeOutput).Add(InputB);
                            ParentGraph.ConnectionIO = null;
                        }
                    }
                    else
                    {
                        ParentGraph.ConnectionIO = InputB;
                    }
                }
            }

            DrawDefaultLabel(viewSkin);

            DrawIOLines();
        }
#endif
        #endregion

        #region Utility Methods
#if UNITY_EDITOR
        public override void DrawIOLines()
        {
            if (Output.Inputs.Count > 0)
            {
                Handles.BeginGUI();

                Handles.color = Color.white;

                for (int i = 0; i < Output.Inputs.Count; i++)
                {
                    BezierDrawing.DrawBezierFromTo(Output.CenterRect,
                        Output.Inputs[i].CenterRect,
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
            if (Output.IORect.Contains(localMousePosition))
            {
                return Output;
            }
            if (InputA.IORect.Contains(localMousePosition))
            {
                return InputA;
            }
            if (InputB.IORect.Contains(localMousePosition))
            {
                return InputB;
            }
            return null;
        }
        #endregion
    }
}
