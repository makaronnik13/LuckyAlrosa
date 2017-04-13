using UnityEngine;
using System;

namespace Meta.Tools.Editor
{
    [Serializable]
    public class NodePropertyView : ViewBase
    {
        #region Public Variables
        #endregion

        #region Protected Variables
        #endregion

        #region Constructor
        public NodePropertyView() : base("Property View")
        {

        }
        #endregion

        #region Main Methods
        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph currentGraph)
        {
            base.UpdateView(editorRect, percentageRect, e, currentGraph);
            GUI.Box(ViewRect, "Properties", _viewSkin.GetStyle("ViewBG"));

            GUILayout.BeginArea(ViewRect);

            if (currentGraph.SelectedNode != null)
            {
                currentGraph.SelectedNode.DrawNodeProperties(_viewSkin);
            }

            GUILayout.EndArea();

            ProcessEvents(e);
        }

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);

            switch (e.type)
            {
                case EventType.KeyDown:
                    switch (e.keyCode)
                    {
                        case KeyCode.Escape:
                            break;
                    }
                    break;
            }

            if (ViewRect.Contains(e.mousePosition))
            {

            }
        }
        #endregion

        #region Utility Methods
        #endregion

    }
}
