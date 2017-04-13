using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace Meta.Tools
{
    [Serializable]
    public class NodeGraph : ScriptableObject
    {
        #region Public Variables
        //public ReferenceResolver ReferenceResolver;
        public Player Player;

        public string GraphName = "New Graph";
        public string FileName = "";
        [SerializeField]
        public List<NodeBase> Nodes;
        public NodeBase SelectedNode;

        public NodeIO ConnectionIO = null;
        #endregion

        #region Constructor
        public NodeGraph()
        {
            Nodes = new List<NodeBase>();
        }
        #endregion

        #region Main Methods
        private void OnEnable()
        {
            //Nodes = new List<NodeBase>();

        }

        public void SubscribeInputs()
        {
            if (Nodes.Count > 0)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].SubscribeIO();
                }
            }
        }

        public void InitGraph()
        {
            if (Nodes.Count > 0)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].InitNode();
                }
            }
        }

        public void UpdateGraph()
        {
            if (Nodes.Count > 0)
            {

            }
        }

#if UNITY_EDITOR
        public void UpdateGraphGUI(Event e, Rect viewRect, GUISkin viewSkin)
        {
            if (Nodes.Count > 0)
            {
                ProcessEvents(e, viewRect);

                for (int i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].UpdateNodeGUI(e, viewRect, viewSkin);
                }
                for (int i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].DrawIOLines();
                }
            }

            //Lets look for connection mode
            if (ConnectionIO != null && ConnectionIO.Parent != null)
            {
                DrawConnectionToMouse(e.mousePosition);
            }

            //EditorUtility.SetDirty(this);
        }


        private void DrawConnectionToMouse(Vector2 mousePosition)
        {
            Handles.BeginGUI();

            Handles.color = Color.white;
            BezierDrawing.DrawBezierFromTo(ConnectionIO.CenterRect,
                new Rect(mousePosition.x - 1f, mousePosition.y - 1f, 1f, 1f),
                Color.white,
                Color.gray,
                false);
            /*Handles.DrawLine(new Vector3(ConnectionIO.Parent.NodeRect.x + ConnectionIO.IORect.center.x,
                ConnectionIO.Parent.NodeRect.y + ConnectionIO.IORect.center.y, 0f),
                new Vector3(mousePosition.x, mousePosition.y, 0f));*/

            Handles.EndGUI();
        }
#endif
        #endregion

        #region Utility Methods
#if UNITY_EDITOR
        private void ProcessEvents(Event e, Rect viewRect)
        {
            if (viewRect.Contains(e.mousePosition))
            {
                switch (e.type)
                {
                    case EventType.KeyDown:
                        switch (e.keyCode)
                        {
                            case KeyCode.Delete:
                                if (SelectedNode != null)
                                {
                                    DeleteNode(SelectedNode);
                                }
                                break;
                        }
                        break;
                    case EventType.MouseDown:
                        switch (e.button)
                        {
                            case 0:
                                DeselectAllNodes();

                                //bool nodeIOSelected = false;
                                bool clickedNotOnButton = true;
                                for (int i = 0; i < Nodes.Count; i++)
                                {
                                    if (Nodes[i].NodeRect.Contains(e.mousePosition))
                                    {
                                        Vector2 localMousePosition = Vector2.zero;
                                        localMousePosition.x = e.mousePosition.x - Nodes[i].NodeRect.x;
                                        localMousePosition.y = e.mousePosition.y - Nodes[i].NodeRect.y;
                                        NodeIO nodeIO = Nodes[i].GetIOAtCoordinates(localMousePosition);

                                        //We want to perform some actions here only if there's no connection mode, otherwise we'll prevent connections
                                        if (ConnectionIO == null || ConnectionIO.Parent == null)
                                        {
                                            //We select whole node only if there's no IO selected
                                            if (nodeIO == null || nodeIO.Parent == null)
                                            {
                                                Nodes[i].IsSelected = true;
                                                SelectedNode = Nodes[i];
                                            }
                                            else
                                            {
                                                //nodeIOSelected = true;
                                            }
                                        }

                                        if (nodeIO != null && nodeIO.Parent != null)
                                        {
                                            clickedNotOnButton = false;
                                        }
                                        break;
                                    }
                                }

                                if (clickedNotOnButton)
                                {
                                    ConnectionIO = null;
                                }
                                break;
                        }
                        break;
                }
            }
        }

        private void DeselectAllNodes()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].IsSelected = false;
            }

            SelectedNode = null;
        }

        public void DeleteNode(NodeBase node)
        {
            if (node != null)
            {
                node.DestroyIO();

                if (Nodes.Count > 0)
                {
                    int nodeID = Nodes.IndexOf(node);
                    if (nodeID >= 0)
                    {
                        GameObject.DestroyImmediate(Nodes[nodeID], true);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        Nodes.RemoveAt(nodeID);
                    }
                }
            }
        }
#endif
        #endregion
    }
}
