using UnityEngine;
using System;

namespace Meta.Tools.Editor
{

    [Serializable]
    public class ViewBase
    {
        #region Public Variables
        public string ViewTitle;
        public Rect ViewRect;
        #endregion

        #region Protected Variables
        protected GUISkin _viewSkin;
        protected NodeGraph _currentGraph;
        #endregion

        #region Constructor
        public ViewBase(string title)
        {
            ViewTitle = title;
            GetEditorSkin();
        }
        #endregion

        #region Main Methods
        public virtual void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph currentGraph)
        {
            if (_viewSkin == null)
            {
                GetEditorSkin();
                return;
            }

            //Set the current view Graph
            _currentGraph = currentGraph;

            //Update view title
            if (currentGraph != null)
            {
                ViewTitle = currentGraph.GraphName;
            }
            else
            {
                ViewTitle = "No Graph";
            }

            //Update view rectangle
            ViewRect = new Rect(editorRect.x * percentageRect.x, editorRect.y * percentageRect.y, editorRect.width * percentageRect.width, editorRect.height * percentageRect.height);
        }

        public virtual void ProcessEvents(Event e)
        {

        }
        #endregion

        #region Utility Methods
        protected void GetEditorSkin()
        {
            _viewSkin = (GUISkin)Resources.Load("GUISkins/EditorSkins/NodeEditorSkin");
        }
        #endregion
    }
}
