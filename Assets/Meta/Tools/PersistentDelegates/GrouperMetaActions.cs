using UnityEngine;
using System;
using System.Linq;

namespace Meta.Tools
{
    [Serializable]
    public class MetaGrouperSubObjectAction
    {
        private Action<GrouperPartSubObject> action;
        [SerializeField]
        private string serializedData;
        [SerializeField]
        private string serializedTypeData;
        public object[] Targets { get { return Get().GetInvocationList().Select(d => d.Target).ToArray(); } }
        public void Invoke(GrouperPartSubObject param)
        {
            Get().Invoke(param);
        }
        public void Add(Action<GrouperPartSubObject> handler)
        {
            ChangeAction(() => action = Get() + handler);
        }
        public void Remove(Action<GrouperPartSubObject> handler)
        {
            if (Get() != null)
            {
                ChangeAction(() => action = Get() - handler);
            }
        }
        public Action<GrouperPartSubObject> Get()
        {
            if (action == null)
            {
                if (serializedData != null && serializedData.Length > 0)
                {
                    action = Utilities.DeserializeFromString<Action<GrouperPartSubObject>>(serializedData);
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

    [Serializable]
    public class MetaGrouperSubObjectIntAction
    {
        private Action<GrouperPartSubObject, int> action;
        [SerializeField]
        private string serializedData;
        [SerializeField]
        private string serializedTypeData;
        public object[] Targets { get { return Get().GetInvocationList().Select(d => d.Target).ToArray(); } }
        public void Invoke(GrouperPartSubObject param1, int param2)
        {
            Get().Invoke(param1, param2);
        }
        public void Add(Action<GrouperPartSubObject, int> handler)
        {
            ChangeAction(() => action = Get() + handler);
        }
        public void Remove(Action<GrouperPartSubObject, int> handler)
        {
            if (Get() != null)
            {
                ChangeAction(() => action = Get() - handler);
            }
        }
        public Action<GrouperPartSubObject, int> Get()
        {
            if (action == null)
            {
                if (serializedData != null && serializedData.Length > 0)
                {
                    action = Utilities.DeserializeFromString<Action<GrouperPartSubObject, int>>(serializedData);
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

    [Serializable]
    public class MetaGrouperPartSubObjectAction
    {
        private Action<GrouperPart, GrouperPartSubObject> action;
        [SerializeField]
        private string serializedData;
        [SerializeField]
        private string serializedTypeData;
        public object[] Targets { get { return Get().GetInvocationList().Select(d => d.Target).ToArray(); } }
        public void Invoke(GrouperPart param1, GrouperPartSubObject param2)
        {
            Get().Invoke(param1, param2);
        }
        public void Add(Action<GrouperPart, GrouperPartSubObject> handler)
        {
            ChangeAction(() => action = Get() + handler);
        }
        public void Remove(Action<GrouperPart, GrouperPartSubObject> handler)
        {
            if (Get() != null)
            {
                ChangeAction(() => action = Get() - handler);
            }
        }
        public Action<GrouperPart, GrouperPartSubObject> Get()
        {
            if (action == null)
            {
                if (serializedData != null && serializedData.Length > 0)
                {
                    action = Utilities.DeserializeFromString<Action<GrouperPart, GrouperPartSubObject>>(serializedData);
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

    [Serializable]
    public class MetaGrouperPartAction
    {
        private Action<GrouperPart> action;
        [SerializeField]
        private string serializedData;
        [SerializeField]
        private string serializedTypeData;
        public object[] Targets { get { return Get().GetInvocationList().Select(d => d.Target).ToArray(); } }
        public void Invoke(GrouperPart param)
        {
            Get().Invoke(param);
        }
        public void Add(Action<GrouperPart> handler)
        {
            ChangeAction(() => action = Get() + handler);
        }
        public void Remove(Action<GrouperPart> handler)
        {
            if (Get() != null)
            {
                ChangeAction(() => action = Get() - handler);
            }
        }
        public Action<GrouperPart> Get()
        {
            if (action == null)
            {
                if (serializedData != null && serializedData.Length > 0)
                {
                    action = Utilities.DeserializeFromString<Action<GrouperPart>>(serializedData);
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
