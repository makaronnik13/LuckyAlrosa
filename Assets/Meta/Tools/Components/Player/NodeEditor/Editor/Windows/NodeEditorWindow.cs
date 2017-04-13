using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    public class NodeEditorWindow : EditorWindow
    {
        #region Variables
        public static NodeEditorWindow Instance;

        public NodePropertyView PropertyView;
        public NodeWorkView WorkView;

        public NodeGraph CurrentGraph = null;
        public Player Player;

        public float ViewPercentage = 0.75f;
        #endregion

        #region Main Methods
        public static void InitEditorWindow(Player player)
        {
            Debug.Log("InitEditorWindow");
            FindWindow();
            Instance.Player = player;
            Instance.CurrentGraph = player.Script;
            CreateViews();
        }

        private void OnEnabled()
        {
            //Debug.Log("Enabled window!");
        }

        private void OnDestroy()
        {
            //Debug.Log("Disabled window!");
        }

        private void Update()
        {
            //Debug.Log("Updating window!");
        }

        private void OnGUI()
        {
            //check for null views
            if (PropertyView == null || WorkView == null)
            {
                CreateViews();
                return;
            }

            //Saving and processing current event
            Event e = Event.current;
            ProcessEvents(e);

            //Update views
            WorkView.UpdateView(position, new Rect(0, 0, ViewPercentage, 1f), e, CurrentGraph);
            PropertyView.UpdateView(new Rect(position.width, position.height, position.width, position.height), new Rect(ViewPercentage, 0f, 1f - ViewPercentage, 1f), e, CurrentGraph);

            Repaint();
        }

        #endregion

        #region Utility Methods

        private static void FindWindow()
        {
            Instance = (NodeEditorWindow)GetWindow<NodeEditorWindow>();

            GUIContent title = new GUIContent();
            title.text = "Node tool";
            Instance.titleContent = title;
        }

        private static void CreateViews()
        {
            if (Instance == null)
            {
                FindWindow();
            }

            Instance.PropertyView = new NodePropertyView();
            Instance.WorkView = new NodeWorkView();
        }

        private void ProcessEvents(Event e)
        {
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftArrow)
            {
                ViewPercentage -= 0.01f;
            }
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.RightArrow)
            {
                ViewPercentage += 0.01f;
            }
        }

        #endregion
    }
}
