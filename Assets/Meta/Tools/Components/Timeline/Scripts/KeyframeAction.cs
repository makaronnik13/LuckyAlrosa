using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.Tools
{
    [Serializable]
    public class KeyframeAction : ICloneable
    {
        public string Name = "";

        private Action<object> _forwardAction;
        private Action<object> _backwardAction;

        public KeyframeAction(Action<object> forwardAction, Action<object> backwardAction = null)
        {
            _forwardAction = forwardAction;
            _backwardAction = backwardAction;
        }

        public void PerformForwardAction(object obj)
        {
            if (_forwardAction != null)
            {
                _forwardAction.Invoke(obj);
            }
        }

        public void PerformBackwardAction(object obj)
        {
            if (_backwardAction != null)
            {
                _backwardAction.Invoke(obj);
            }
        }

        public object Clone()
        {
            KeyframeAction clone = new KeyframeAction(_forwardAction, _backwardAction);

            clone.Name = Name;

            return clone;
        }
    }
}
