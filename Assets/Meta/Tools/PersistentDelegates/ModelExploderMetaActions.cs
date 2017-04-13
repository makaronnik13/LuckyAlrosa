using UnityEngine;
using System;
using System.Linq;

namespace Meta.Tools
{
    /*[Serializable]
    public class MetaModelExploderSubObjectAction
    {
        private Action<ModelExploder.Part.SubObject> action;
        [SerializeField]
        private string serializedData;
        [SerializeField]
        private string serializedTypeData;
        public object[] Targets { get { return Get().GetInvocationList().Select(d => d.Target).ToArray(); } }
        public void Invoke(ModelExploder.Part.SubObject param)
        {
            Get().Invoke(param);
        }
        public void Add(Action<ModelExploder.Part.SubObject> handler)
        {
            ChangeAction(() => action = Get() + handler);
        }
        public void Remove(Action<ModelExploder.Part.SubObject> handler)
        {
            if (Get() != null)
            {
                ChangeAction(() => action = Get() - handler);
            }
        }
        public Action<ModelExploder.Part.SubObject> Get()
        {
            if (action == null)
            {
                if (serializedData != null)
                {
                    action = Utilities.DeserializeFromString<Action<ModelExploder.Part.SubObject>>(serializedData);
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
    public class MetaModelExploderPartSubObjectAction
    {
        private Action<ModelExploder.Part, ModelExploder.Part.SubObject> action;
        [SerializeField]
        private string serializedData;
        [SerializeField]
        private string serializedTypeData;
        public object[] Targets { get { return Get().GetInvocationList().Select(d => d.Target).ToArray(); } }
        public void Invoke(ModelExploder.Part param1, ModelExploder.Part.SubObject param2)
        {
            Get().Invoke(param1, param2);
        }
        public void Add(Action<ModelExploder.Part, ModelExploder.Part.SubObject> handler)
        {
            ChangeAction(() => action = Get() + handler);
        }
        public void Remove(Action<ModelExploder.Part, ModelExploder.Part.SubObject> handler)
        {
            if (Get() != null)
            {
                ChangeAction(() => action = Get() - handler);
            }
        }
        public Action<ModelExploder.Part, ModelExploder.Part.SubObject> Get()
        {
            if (action == null)
            {
                if (serializedData != null)
                {
                    action = Utilities.DeserializeFromString<Action<ModelExploder.Part, ModelExploder.Part.SubObject>>(serializedData);
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
    public class MetaModelExploderPartAction
    {
        private Action<ModelExploder.Part> action;
        [SerializeField]
        private string serializedData;
        [SerializeField]
        private string serializedTypeData;
        public object[] Targets { get { return Get().GetInvocationList().Select(d => d.Target).ToArray(); } }
        public void Invoke(ModelExploder.Part param)
        {
            Get().Invoke(param);
        }
        public void Add(Action<ModelExploder.Part> handler)
        {
            ChangeAction(() => action = Get() + handler);
        }
        public void Remove(Action<ModelExploder.Part> handler)
        {
            if (Get() != null)
            {
                ChangeAction(() => action = Get() - handler);
            }
        }
        public Action<ModelExploder.Part> Get()
        {
            if (action == null)
            {
                if (serializedData != null)
                {
                    action = Utilities.DeserializeFromString<Action<ModelExploder.Part>>(serializedData);
                }
            }
            return action;
        }
        private void ChangeAction(Action change)
        {
            change();
            serializedData = Utilities.SerializeToString(action);
        }
    }*/





    [Serializable]
    public class MetaModelExploderSubObjectAction
    {
        private Action<ModelExploderPartSubObject> action;
        [SerializeField]
        private string serializedData;
        [SerializeField]
        private string serializedTypeData;
        public object[] Targets { get { return Get().GetInvocationList().Select(d => d.Target).ToArray(); } }
        public void Invoke(ModelExploderPartSubObject param)
        {
            Get().Invoke(param);
        }
        public void Add(Action<ModelExploderPartSubObject> handler)
        {
            ChangeAction(() => action = Get() + handler);
        }
        public void Remove(Action<ModelExploderPartSubObject> handler)
        {
            if (Get() != null)
            {
                ChangeAction(() => action = Get() - handler);
            }
        }
        public Action<ModelExploderPartSubObject> Get()
        {
            if (action == null)
            {
                if (serializedData != null)
                {
                    action = Utilities.DeserializeFromString<Action<ModelExploderPartSubObject>>(serializedData);
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
    public class MetaModelExploderPartSubObjectAction
    {
        private Action<ModelExploderPart, ModelExploderPartSubObject> action;
        [SerializeField]
        private string serializedData;
        [SerializeField]
        private string serializedTypeData;
        public object[] Targets { get { return Get().GetInvocationList().Select(d => d.Target).ToArray(); } }
        public void Invoke(ModelExploderPart param1, ModelExploderPartSubObject param2)
        {
            Get().Invoke(param1, param2);
        }
        public void Add(Action<ModelExploderPart, ModelExploderPartSubObject> handler)
        {
            ChangeAction(() => action = Get() + handler);
        }
        public void Remove(Action<ModelExploderPart, ModelExploderPartSubObject> handler)
        {
            if (Get() != null)
            {
                ChangeAction(() => action = Get() - handler);
            }
        }
        public Action<ModelExploderPart, ModelExploderPartSubObject> Get()
        {
            if (action == null)
            {
                if (serializedData != null)
                {
                    action = Utilities.DeserializeFromString<Action<ModelExploderPart, ModelExploderPartSubObject>>(serializedData);
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
    public class MetaModelExploderPartAction
    {
        private Action<ModelExploderPart> action;
        [SerializeField]
        private string serializedData;
        [SerializeField]
        private string serializedTypeData;
        public object[] Targets { get { return Get().GetInvocationList().Select(d => d.Target).ToArray(); } }
        public void Invoke(ModelExploderPart param)
        {
            Get().Invoke(param);
        }
        public void Add(Action<ModelExploderPart> handler)
        {
            ChangeAction(() => action = Get() + handler);
        }
        public void Remove(Action<ModelExploderPart> handler)
        {
            if (Get() != null)
            {
                ChangeAction(() => action = Get() - handler);
            }
        }
        public Action<ModelExploderPart> Get()
        {
            if (action == null)
            {
                if (serializedData != null)
                {
                    action = Utilities.DeserializeFromString<Action<ModelExploderPart>>(serializedData);
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
