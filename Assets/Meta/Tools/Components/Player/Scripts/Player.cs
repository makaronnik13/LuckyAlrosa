using System.Collections.Generic;
using UnityEngine;

namespace Meta.Tools
{
    public class Player : MonoBehaviour
    {
        public string Name = "New Story";
        //[SerializeField]
        //private string _scriptName = "";
        public NodeGraph Script;
        //private ReferenceResolver _referenceResolver;
        public ReferencesHolder ReferenceHolder = new ReferencesHolder();

        private void Awake()
        {
            Debug.Log("Script = " + Script);
            //_referenceResolver = new ReferenceResolver(Script);
            Script.Player = this;
            Script.SubscribeInputs();
        }
        
        private void Start()
        {
            Debug.Log("Start");
            if (Script != null)
            {
                Debug.Log("Script.Nodes.Count = " + Script.Nodes.Count);
                for (int i = 0; i < Script.Nodes.Count; i++)
                {
                    if (Script.Nodes[i].NodeType == NodeType.MonoBehaviourLifeCycle)
                    {
                        Debug.Log("Start Invoking");
                        (Script.Nodes[i] as MonoBehaviourLifeCycleNode).StartOutput.Invoke();
                    }
                }
            }
        }
        
        private void Update()
        {

        }
    }
}
