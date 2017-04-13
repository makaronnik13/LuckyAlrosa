using UnityEngine;
using System;
using System.Linq;

namespace Meta.Tools
{
    [Serializable]
    public class MetaAction
    {
        private Action action;
        [SerializeField]
        private string serializedData;
        public object[] Targets { get { return Get().GetInvocationList().Select(d => d.Target).ToArray(); } }
        public void Invoke()
        {
            Get().Invoke();
        }
        public void Add(Action handler)
        {
            ChangeAction(() => action = Get() + handler);
        }
        public void Remove(Action handler)
        {
            ChangeAction(() => action = Get() - handler);
        }
        public Action Get()
        {
            if (action == null)
            {
                if (serializedData != null)
                {
                    action = Utilities.DeserializeFromString<Action>(serializedData);
                }
            }
            return action;
        }
        private void ChangeAction(Action change)
        {
            change();
            serializedData = Utilities.SerializeToString(action);
        }
    }
}
