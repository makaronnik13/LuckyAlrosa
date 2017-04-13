#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Meta.Tools.Editor
{
    public static class NodeUtils
    {
        public static NodeGraph CreateNewGraph(string wantedName, string wantedFileName)
        {
            NodeGraph newGraph = ScriptableObject.CreateInstance<NodeGraph>();
            if (newGraph != null)
            {
                newGraph.GraphName = wantedName;
                newGraph.FileName = wantedFileName;
                newGraph.InitGraph();

                AssetDatabase.CreateAsset(newGraph, "Assets/Meta/Tools/Components/Player/NodeEditor/Database/" + wantedFileName + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                /*NodeEditorWindow currentWindow = (NodeEditorWindow)EditorWindow.GetWindow<NodeEditorWindow>();
                if (currentWindow != null)
                {
                    currentWindow.CurrentGraph = newGraph;
                }*/
            }
            else
            {
                newGraph = null;
                EditorUtility.DisplayDialog("Node Message", "Unable to create new graph", "OK");
            }

            return newGraph;
        }

        public static NodeGraph LoadGraph(string fileName)
        {
            NodeGraph loadedGraph = null;

            string pathToGraph = Application.dataPath + "/Meta/Tools/Components/Player/NodeEditor/Database/" + fileName + ".asset";

            int appPathLen = Application.dataPath.Length;
            if (appPathLen > 0)
            {
                string finalPath = pathToGraph.Substring(appPathLen - 6);

                loadedGraph = (NodeGraph)AssetDatabase.LoadAssetAtPath(finalPath, typeof(NodeGraph));
            }

            return loadedGraph;
        }

        public static void LoadGraph()
        {
            NodeGraph loadedGraph = null;
            string pathToGraph = EditorUtility.OpenFilePanel("Load Graph", Application.dataPath + "/Meta/Tools/Components/Player/NodeEditor/Database/", "asset");

            int appPathLen = Application.dataPath.Length;
            if (appPathLen > 0)
            {
                string finalPath = pathToGraph.Substring(appPathLen - 6);

                loadedGraph = (NodeGraph)AssetDatabase.LoadAssetAtPath(finalPath, typeof(NodeGraph));
                if (loadedGraph != null)
                {
                    NodeEditorWindow currentWindow = (NodeEditorWindow)EditorWindow.GetWindow<NodeEditorWindow>();
                    if (currentWindow != null)
                    {
                        currentWindow.CurrentGraph = loadedGraph;
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Node Message", "Unable to load selected graph", "OK");
                }
            }
        }

        public static void UnloadGraph()
        {
            NodeEditorWindow currentWindow = (NodeEditorWindow)EditorWindow.GetWindow<NodeEditorWindow>();
            if (currentWindow != null)
            {
                currentWindow.CurrentGraph = null;
            }
        }

        public static void CreateNode(NodeGraph graph, NodeType nodeType, Vector2 mousePosition)
        {
            if (graph != null)
            {
                Undo.RecordObject(graph, "Node Added");
                NodeBase newNode = null;
                switch (nodeType)
                {
                    case NodeType.Add:
                        newNode = (AddNode)ScriptableObject.CreateInstance<AddNode>();
                        newNode.NodeName = "Add Node";
                        break;
                    case NodeType.Float:
                        newNode = (FloatNode)ScriptableObject.CreateInstance<FloatNode>();
                        newNode.NodeName = "Float Node";
                        break;
                    case NodeType.Timeline:
                        newNode = (TimelineNode)ScriptableObject.CreateInstance<TimelineNode>();
                        newNode.NodeName = "Timeline Node";
                        break;
                    case NodeType.MonoBehaviourLifeCycle:
                        newNode = (MonoBehaviourLifeCycleNode)ScriptableObject.CreateInstance<MonoBehaviourLifeCycleNode>();
                        newNode.NodeName = "MonoBehaviour Lifecycle Node";
                        break;
                    default:
                        break;
                }

                if (newNode != null)
                {
                    newNode.NodeRect.x = mousePosition.x;
                    newNode.NodeRect.y = mousePosition.y;
                    Debug.Log("mousePosition = " + mousePosition);
                    Debug.Log("newNode.NodeRect = " + newNode.NodeRect);
                    newNode.ParentGraph = graph;
                    newNode.InitNode();

                    graph.Nodes.Add(newNode);

                    AssetDatabase.AddObjectToAsset(newNode, graph);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        public static void DeleteNode(NodeBase node, NodeGraph graph)
        {
            if (node != null && graph != null)
            {
                if (graph.Nodes.Count > 0)
                {
                    int nodeID = graph.Nodes.IndexOf(node);
                    if (nodeID >= 0)
                    {
                        NodeScriptsUtils.DeleteAsset(graph.Nodes[nodeID]);
                    }
                }
            }
        }

        public static void DrawGrid(Rect viewRect, float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(viewRect.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(viewRect.height / gridSpacing);

            Handles.BeginGUI();

            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, 0f, 0f), new Vector3(gridSpacing * i, viewRect.height, 0f));
            }
            for (int i = 0; i < heightDivs; i++)
            {
                Handles.DrawLine(new Vector3(0f, gridSpacing * i, 0f), new Vector3(viewRect.width, gridSpacing * i, 0f));
            }

            Handles.EndGUI();
        }
    }
}
