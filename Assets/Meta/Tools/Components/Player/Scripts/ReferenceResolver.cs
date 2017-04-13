using System.Collections.Generic;
using UnityEngine;
using System;

namespace Meta.Tools
{
    [Serializable]
    public class ReferenceResolver
    {
        private Dictionary<int, Component> _catalogue = new Dictionary<int, Component>();
        private GameObject[] _gameObjects;

        public ReferenceResolver(NodeGraph graph = null)
        {
            if (graph != null)
            {
                Initialize(graph);
                //graph.ReferenceResolver = this;
            }
        }

        public Component Get(int id)
        {
            return _catalogue[id];
        }

        public void Initialize(NodeGraph graph)
        {
            _gameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            for (int i = 0; i < graph.Nodes.Count; i++)
            {
                if (graph.Nodes[i].NodeType == NodeType.Timeline)
                {
                    processID((graph.Nodes[i] as TimelineNode).NodeSource);
                }
            }
        }

        private void processID(int id)
        {
            if (!_catalogue.ContainsKey(id))
            {
                for (int i = 0; i < _gameObjects.Length; i++)
                {
                    Component[] components = _gameObjects[i].GetComponents<Component>();
                    for (int j = 0; j < components.Length; j++)
                    {
                        if (components[j].GetInstanceID() == id)
                        {
                            _catalogue.Add(id, components[j]);
                        }
                    }
                }
            }
        }
    }
}
