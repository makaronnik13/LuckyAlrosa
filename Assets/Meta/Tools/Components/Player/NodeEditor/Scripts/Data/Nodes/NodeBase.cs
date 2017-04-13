using System;
using UnityEngine;

namespace Meta.Tools
{
    [Serializable]
    public class NodeBase : ScriptableObject
    {
        #region Public Variables
        public bool IsSelected = false;
        public string NodeName;
        public float IOWidth = 16f;
        public NodeGraph ParentGraph;
        public NodeType NodeType;

        public Rect NodeRect;
        public Rect NodeInnerRect;
        #endregion

        #region Protected Variables
        protected float MarginLeft = 0f;
        protected float MarginRight = 0f;
        protected float MarginTop = 0f;
        protected float MarginBottom = 0f;
        #endregion

        #region Main Methods
        public virtual void InitNode()
        {

        }

        public virtual void UpdateNode(Event e, Rect viewRect)
        {

        }

        public virtual NodeIO GetIOAtCoordinates(Vector2 localMousePosition)
        {
            return null;
        }

        public virtual void SubscribeIO()
        {

        }

        public virtual void DestroyIO()
        {

        }

        public virtual void DrawNodeProperties(GUISkin skin)
        {
            GUILayout.Space(40f);
        }

        public virtual void DrawIOLines()
        {

        }

#if UNITY_EDITOR
        public virtual void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin)
        {
            ProcessEvents(e, viewRect);

            NodeInnerRect = new Rect(NodeRect.x + IOWidth * 2f + MarginLeft,
                NodeRect.y + IOWidth * 2f + MarginTop,
                NodeRect.width - IOWidth * 4f - MarginLeft - MarginRight,
                NodeRect.height - IOWidth * 4f - MarginTop - MarginBottom);

            if (!IsSelected)
            {
                GUI.Box(NodeRect, "", viewSkin.GetStyle("NodeDefault"));
            }
            else
            {
                GUI.Box(NodeRect, "", viewSkin.GetStyle("NodeSelected"));
            }

            /*GUILayout.BeginArea(NodeInnerRect);

            GUILayout.Label(NodeName, NodeLabelStyle);

            GUILayout.EndArea();*/

            //EditorUtility.SetDirty(this);
        }

        protected void DrawDefaultLabel(GUISkin viewSkin)
        {
            GUILayout.BeginArea(NodeInnerRect);

            GUILayout.Label(NodeName, viewSkin.GetStyle("NodeLabelStyle"));

            GUILayout.EndArea();
        }
#endif

        #endregion

        #region Utility Methods
        private void ProcessEvents(Event e, Rect viewRect)
        {
            if (IsSelected)
            {
                if (e.type == EventType.MouseDrag)
                {
                    NodeRect.x += e.delta.x;
                    NodeRect.y += e.delta.y;
                }
            }
        }
        #endregion
    }
}
