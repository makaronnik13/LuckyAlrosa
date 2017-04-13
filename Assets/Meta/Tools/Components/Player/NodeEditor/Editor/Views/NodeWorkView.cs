using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace Meta.Tools.Editor
{
    [Serializable]
    public class NodeWorkView : ViewBase
    {
        #region Public Variables
        #endregion

        #region Protected Variables
        private Vector2 _mousePosition;
        #endregion

        #region Constructor
        public NodeWorkView() : base("Work View")
        {

        }
        #endregion

        #region Main Methods
        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph currentGraph)
        {
            base.UpdateView(editorRect, percentageRect, e, currentGraph);
            GUI.Box(ViewRect, ViewTitle, _viewSkin.GetStyle("ViewBG"));

            NodeUtils.DrawGrid(ViewRect, 50f, 0.15f, Color.white);

            GUILayout.BeginArea(ViewRect);
            if (_currentGraph != null)
            {
                _currentGraph.UpdateGraphGUI(e, ViewRect, _viewSkin);
            }
            GUILayout.EndArea();

            ProcessEvents(e);
        }

        public override void ProcessEvents(Event e)
        {
            _mousePosition = e.mousePosition;
            base.ProcessEvents(e);

            if (ViewRect.Contains(e.mousePosition))
            {
                if (e.button == 0)
                {
                    switch (e.type)
                    {
                        case EventType.MouseDown:
                            GUIUtility.keyboardControl = 0;
                            break;
                        case EventType.MouseDrag:
                            break;
                        case EventType.MouseUp:
                            break;
                    }
                }

                if (e.button == 1)
                {
                    switch (e.type)
                    {
                        case EventType.MouseDown:
                            GUIUtility.keyboardControl = 0;
                            ProcessContextMenu(e);
                            break;
                        case EventType.MouseDrag:
                            break;
                        case EventType.MouseUp:
                            break;
                    }
                }
            }
        }
        #endregion

        #region Utility Methods
        private void ProcessContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            //menu.AddItem(new GUIContent("Create New Graph"), false, ContextCallback, "0");
            //menu.AddItem(new GUIContent("Load Graph..."), false, ContextCallback, "1");
            //menu.AddItem(new GUIContent("Unload Graph..."), false, ContextCallback, "2");

            if (_currentGraph != null)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Float Node"), false, ContextCallback, "3");
                menu.AddItem(new GUIContent("Add Node"), false, ContextCallback, "4");
                menu.AddItem(new GUIContent("Timeline Node"), false, ContextCallback, "5");
                menu.AddItem(new GUIContent("MonoLifeCycle Node"), false, ContextCallback, "6");
            }

            menu.ShowAsContext();
            e.Use();
        }

        private void ContextCallback(object obj)
        {
            switch (obj.ToString())
            {
                case "0":
                    Debug.Log("Creating new graph");
                    NodePopupWindow.InitNodePopup();
                    break;
                case "1":
                    NodeUtils.LoadGraph();
                    Debug.Log("Loading graph");
                    break;
                case "2":
                    NodeUtils.UnloadGraph();
                    Debug.Log("Unloading graph");
                    break;
                case "3":
                    //Undo.RecordObject(_currentGraph, "Node Added");
                    NodeUtils.CreateNode(_currentGraph, NodeType.Float, _mousePosition);
                    break;
                case "4":
                    //Undo.RecordObject(_currentGraph, "Node Added");
                    NodeUtils.CreateNode(_currentGraph, NodeType.Add, _mousePosition);
                    break;
                case "5":
                    Debug.Log("_mousePosition = " + _mousePosition);
                    //Undo.RecordObject(_currentGraph, "Node Added");
                    NodeUtils.CreateNode(_currentGraph, NodeType.Timeline, _mousePosition);
                    break;
                case "6":
                    Debug.Log("_mousePosition = " + _mousePosition);
                    //Undo.RecordObject(_currentGraph, "Node Added");
                    NodeUtils.CreateNode(_currentGraph, NodeType.MonoBehaviourLifeCycle, _mousePosition);
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
