using System;
using UnityEngine;
using System.Collections.Generic;

namespace Meta.Tools
{
    [Serializable]
    public class NodeInput : NodeIO
    {
        public Action<object> Action;

        public void Invoke(object parameters = null)
        {
            if (Action != null)
            {
                Action.Invoke(parameters);
            }
        }

        [SerializeField]
        private List<NodeOutput> _outputs = new List<NodeOutput>();
        public List<NodeOutput> Outputs
        {
            get
            {
                return _outputs;
            }
        }

        public bool Add(NodeOutput output)
        {
            if (_outputs.Contains(output))
            {
                return false;
            }

            _outputs.Add(output);
            return true;
        }

        public bool Remove(NodeOutput output)
        {
            if (_outputs.Contains(output))
            {
                _outputs.Remove(output);
                return true;
            }

            return true;
        }
    }
}
