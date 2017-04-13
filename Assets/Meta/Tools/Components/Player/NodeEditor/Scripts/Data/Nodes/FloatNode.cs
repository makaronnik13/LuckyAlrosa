using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.Tools
{
    [Serializable]
    public class FloatNode : NodeBase
    {
        #region Variables
        public float NodeValue;
        public NodeOutput Output;
        #endregion

        #region Constructor
        public FloatNode()
        {
        }
        #endregion

        #region Main Methods
        public override void InitNode()
        {
            NodeRect = new Rect(NodeRect.x, NodeRect.y, 150f, 150f);
            base.InitNode();
            NodeType = NodeType.Float;

            Output = new NodeOutput();
            Output.Parent = this;
            Output.Type = IOType.Numeric;
            Output.Name = "Value";
            Output.IORect = new Rect(NodeRect.width - IOWidth * 1.5f, NodeRect.height * 0.5f - IOWidth * 0.5f, IOWidth, IOWidth);
        }

        public override void UpdateNode(Event e, Rect viewRect)
        {
            base.UpdateNode(e, viewRect);
        }

#if UNITY_EDITOR
        public override void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin)
        {
            base.UpdateNodeGUI(e, viewRect, viewSkin);

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
            return null;
        }
        #endregion
    }
}
