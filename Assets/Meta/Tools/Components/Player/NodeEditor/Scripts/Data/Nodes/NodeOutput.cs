using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.Tools
{
    [Serializable]
    public class NodeOutput : NodeIO
    {
        [SerializeField]
        private List<NodeInput> _inputs = new List<NodeInput>();
        public List<NodeInput> Inputs
        {
            get
            {
                return _inputs;
            }
        }

        public bool Add(NodeInput input)
        {
            if (_inputs.Contains(input))
            {
                return false;
            }

            _inputs.Add(input);
            return true;
        }

        public bool Remove(NodeInput input)
        {
            if (_inputs.Contains(input))
            {
                _inputs.Remove(input);
                return true;
            }

            return true;
        }

        public void Invoke(object parameters = null)
        {
            for (int i = 0; i < _inputs.Count; i++)
            {
                _inputs[i].Invoke(parameters);
            }
        }
    }
}
