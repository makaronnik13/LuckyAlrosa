using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.Tools
{
    [Serializable]
    public class KeyframeActions : ICloneable
    {
        [SerializeField]
        private List<KeyframeAction> _actions = new List<KeyframeAction>();
        
        public void Add(string name, Action<object> forwardAction, Action<object> backwardAction = null)
        {
            KeyframeAction newAction = new KeyframeAction(forwardAction, backwardAction);
            newAction.Name = name;
            _actions.Add(newAction);
        }

        public void PerformForward(int index, object obj = null)
        {
            if (_actions != null && _actions.Count > index)
            {
                _actions[index].PerformForwardAction(obj);
            }
        }

        public void PerformBackward(int index, object obj = null)
        {
            if (_actions != null && _actions.Count > index)
            {
                _actions[index].PerformBackwardAction(obj);
            }
        }

        public string[] GetNames()
        {
            List<string> names = new List<string>();

            for (int i = 0; i < _actions.Count; i++)
            {
                names.Add(_actions[i].Name);
            }

            return names.ToArray();
        }

        public object Clone()
        {
            KeyframeActions clone = new KeyframeActions();

            for (int i = 0; i < _actions.Count; i++)
            {
                clone._actions.Add((KeyframeAction)_actions[i].Clone());
            }

            return clone;
        }
    }
}
