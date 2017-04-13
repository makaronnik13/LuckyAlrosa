using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Object = UnityEngine.Object;
using System.Reflection;

namespace Meta.Tools
{
    /*[Serializable]
    public class MetaUnityModelExploderSubObjectAction
    {
        [Serializable]
        private class Entry
        {
            public Object target;
            public List<string> methodsNames = new List<string>();
            public Entry() { }
            public Entry(Object target, string methodName)
            {
                this.target = target;
                methodsNames.Add(methodName);
            }
        }

        private Action<ModelExploder.Part.SubObject> action;
        [SerializeField]
        private List<Entry> entries = new List<Entry>();

        public Object[] Targets { get { return entries.Select(e => e.target).ToArray(); } }

        public MetaUnityModelExploderSubObjectAction() { }
        public MetaUnityModelExploderSubObjectAction(Action<ModelExploder.Part.SubObject> handler) { Add(handler); }

        public void Invoke(ModelExploder.Part.SubObject param)
        {
            if (Get() != null)
            {
                Get().Invoke(param);
            }
        }

        private bool IsValidHandler(Action<ModelExploder.Part.SubObject> handler)
        {
            return handler != null && handler.Target is MonoBehaviour;
        }

        public void Add(Action<ModelExploder.Part.SubObject> handler)
        {
            if (!IsValidHandler(handler)) return;

            var target = handler.Target as Object;

            if (!Targets.Contains(target))
            {
                entries.Add(new Entry(target, handler.Method.Name));
            }
            else
            {
                entries.First(e => e.target == target).methodsNames.Add(handler.Method.Name);
            }

            if (action == null)
            {
                action = Get();
            }
            else
            {
                action = Get() + handler;
            }
        }

        public void Remove(Action<ModelExploder.Part.SubObject> handler)
        {
            if (!IsValidHandler(handler)) return;

            var target = handler.Target as Object;
            int index = Targets.ToList().IndexOf(target);
            if (index == -1) return;
            entries[index].methodsNames.Remove(handler.Method.Name);
            if (entries[index].methodsNames.Count == 0)
                entries.RemoveAt(index);

            action = Get() - handler;
        }

        public Action<ModelExploder.Part.SubObject> Get()
        {
            if (action == null)
            {
                foreach (var entry in entries)
                {
                    var target = entry.target;
                    if (target == null) continue;
                    foreach (var method in entry.methodsNames)
                    {
                        action += Delegate.CreateDelegate(typeof(Action<ModelExploder.Part.SubObject>), target, method) as Action<ModelExploder.Part.SubObject>;
                    }
                }
            }
            return action;
        }
    }

    [Serializable]
    public class MetaUnityModelExploderPartSubObjectAction
    {
        [Serializable]
        private class Entry
        {
            public Object target;
            public List<string> methodsNames = new List<string>();
            public Entry() { }
            public Entry(Object target, string methodName)
            {
                this.target = target;
                methodsNames.Add(methodName);
            }
        }

        private Action<ModelExploder.Part, ModelExploder.Part.SubObject> action;
        [SerializeField]
        private List<Entry> entries = new List<Entry>();

        public Object[] Targets { get { return entries.Select(e => e.target).ToArray(); } }

        public MetaUnityModelExploderPartSubObjectAction() { }
        public MetaUnityModelExploderPartSubObjectAction(Action<ModelExploder.Part, ModelExploder.Part.SubObject> handler) { Add(handler); }

        public void Invoke(ModelExploder.Part param1, ModelExploder.Part.SubObject param2)
        {
            if (Get() != null)
            {
                Get().Invoke(param1, param2);
            }
        }

        private bool IsValidHandler(Action<ModelExploder.Part, ModelExploder.Part.SubObject> handler)
        {
            return handler != null && handler.Target is MonoBehaviour;
        }

        public void Add(Action<ModelExploder.Part, ModelExploder.Part.SubObject> handler)
        {
            if (!IsValidHandler(handler)) return;

            var target = handler.Target as Object;

            if (!Targets.Contains(target))
            {
                entries.Add(new Entry(target, handler.Method.Name));
            }
            else
            {
                entries.First(e => e.target == target).methodsNames.Add(handler.Method.Name);
            }

            if (action == null)
            {
                action = Get();
            }
            else
            {
                action = Get() + handler;
            }
        }

        public void Remove(Action<ModelExploder.Part, ModelExploder.Part.SubObject> handler)
        {
            if (!IsValidHandler(handler)) return;

            var target = handler.Target as Object;
            int index = Targets.ToList().IndexOf(target);
            if (index == -1) return;
            entries[index].methodsNames.Remove(handler.Method.Name);
            if (entries[index].methodsNames.Count == 0)
                entries.RemoveAt(index);

            action = Get() - handler;
        }

        public Action<ModelExploder.Part, ModelExploder.Part.SubObject> Get()
        {
            if (action == null)
            {
                foreach (var entry in entries)
                {
                    var target = entry.target;
                    if (target == null) continue;
                    foreach (var method in entry.methodsNames)
                    {
                        action += Delegate.CreateDelegate(typeof(Action<ModelExploder.Part, ModelExploder.Part.SubObject>), target, method) as Action<ModelExploder.Part, ModelExploder.Part.SubObject>;
                    }
                }
            }
            return action;
        }
    }

    [Serializable]
    public class MetaUnityModelExploderPartAction
    {
        [Serializable]
        private class Entry
        {
            public Object target;
            public List<string> methodsNames = new List<string>();
            public Entry() { }
            public Entry(Object target, string methodName)
            {
                this.target = target;
                methodsNames.Add(methodName);
            }
        }

        private Action<ModelExploder.Part> action;
        [SerializeField]
        private List<Entry> entries = new List<Entry>();

        public Object[] Targets { get { return entries.Select(e => e.target).ToArray(); } }

        public MetaUnityModelExploderPartAction() { }
        public MetaUnityModelExploderPartAction(Action<ModelExploder.Part> handler) { Add(handler); }

        public void Invoke(ModelExploder.Part param)
        {
            if (Get() != null)
            {
                Get().Invoke(param);
            }
        }

        private bool IsValidHandler(Action<ModelExploder.Part> handler)
        {
            return handler != null && handler.Target is MonoBehaviour;
        }

        public void Add(Action<ModelExploder.Part> handler)
        {
            if (!IsValidHandler(handler)) return;

            var target = handler.Target as Object;

            if (!Targets.Contains(target))
            {
                entries.Add(new Entry(target, handler.Method.Name));
            }
            else
            {
                entries.First(e => e.target == target).methodsNames.Add(handler.Method.Name);
            }

            if (action == null)
            {
                action = Get();
            }
            else
            {
                action = Get() + handler;
            }
        }

        public void Remove(Action<ModelExploder.Part> handler)
        {
            if (!IsValidHandler(handler)) return;

            var target = handler.Target as Object;
            int index = Targets.ToList().IndexOf(target);
            if (index == -1) return;
            entries[index].methodsNames.Remove(handler.Method.Name);
            if (entries[index].methodsNames.Count == 0)
                entries.RemoveAt(index);

            action = Get() - handler;
        }

        public Action<ModelExploder.Part> Get(bool info = false)
        {
            if (action == null)
            {
                if (info)
                {
                    Debug.Log("action == null");
                    Debug.Log("entries.Count = " + entries.Count);
                }
                foreach (var entry in entries)
                {
                    if (info)
                    {
                        Debug.Log("entry.methodsNames.Count = " + entry.methodsNames.Count);
                    }
                    var target = entry.target;
                    if (target == null) continue;
                    foreach (var method in entry.methodsNames)
                    {
                        action += Delegate.CreateDelegate(typeof(Action<ModelExploder.Part>), target, method) as Action<ModelExploder.Part>;
                    }
                }
            }
            return action;
        }
    }*/







    [Serializable]
    public class MetaUnityModelExploderSubObjectAction
    {
        [Serializable]
        private class Entry
        {
            public Object target;
            public List<string> methodsNames = new List<string>();
            public Entry() { }
            public Entry(Object target, string methodName)
            {
                this.target = target;
                methodsNames.Add(methodName);
            }
        }

        private Action<ModelExploderPartSubObject> action;
        [SerializeField]
        private List<Entry> entries = new List<Entry>();

        public Object[] Targets { get { return entries.Select(e => e.target).ToArray(); } }

        public MetaUnityModelExploderSubObjectAction() { }
        public MetaUnityModelExploderSubObjectAction(Action<ModelExploderPartSubObject> handler) { Add(handler); }

        public void Invoke(ModelExploderPartSubObject param)
        {
            if (Get() != null)
            {
                Get().Invoke(param);
            }
        }

        private bool IsValidHandler(Action<ModelExploderPartSubObject> handler)
        {
            return handler != null && handler.Target is MonoBehaviour;
        }

        public void Add(Action<ModelExploderPartSubObject> handler)
        {
            if (!IsValidHandler(handler)) return;

            var target = handler.Target as Object;

            if (!Targets.Contains(target))
            {
#if UNITY_EDITOR
                entries.Add(new Entry(target, handler.Method.Name));
#else
                entries.Add(new Entry(target, handler.GetMethodInfo().Name));
#endif
            }
            else
            {
#if UNITY_EDITOR
                entries.First(e => e.target == target).methodsNames.Add(handler.Method.Name);
#else
                entries.First(e => e.target == target).methodsNames.Add(handler.GetMethodInfo().Name);
#endif
            }

            if (action == null)
            {
                action = Get();
            }
            else
            {
                action = Get() + handler;
            }
        }

        public void Remove(Action<ModelExploderPartSubObject> handler)
        {
            if (!IsValidHandler(handler)) return;

            var target = handler.Target as Object;
            int index = Targets.ToList().IndexOf(target);
            if (index == -1) return;
#if UNITY_EDITOR
            entries[index].methodsNames.Remove(handler.Method.Name);
#else
            entries[index].methodsNames.Remove(handler.GetMethodInfo().Name);
#endif
            if (entries[index].methodsNames.Count == 0)
                entries.RemoveAt(index);

            action = Get() - handler;
        }

        public Action<ModelExploderPartSubObject> Get()
        {
            if (action == null)
            {
                foreach (var entry in entries)
                {
                    var target = entry.target;
                    if (target == null) continue;
                    foreach (var method in entry.methodsNames)
                    {
#if UNITY_EDITOR
                        action += Delegate.CreateDelegate(typeof(Action<ModelExploderPartSubObject>), target, method) as Action<ModelExploderPartSubObject>;
#else
                        action += action.GetMethodInfo().CreateDelegate(typeof(Action<ModelExploderPartSubObject>), target) as Action<ModelExploderPartSubObject>;
#endif
                    }
                }
            }
            return action;
        }
    }

    [Serializable]
    public class MetaUnityModelExploderPartSubObjectAction
    {
        [Serializable]
        private class Entry
        {
            public Object target;
            public List<string> methodsNames = new List<string>();
            public Entry() { }
            public Entry(Object target, string methodName)
            {
                this.target = target;
                methodsNames.Add(methodName);
            }
        }

        private Action<ModelExploderPart, ModelExploderPartSubObject> action;
        [SerializeField]
        private List<Entry> entries = new List<Entry>();

        public Object[] Targets { get { return entries.Select(e => e.target).ToArray(); } }

        public MetaUnityModelExploderPartSubObjectAction() { }
        public MetaUnityModelExploderPartSubObjectAction(Action<ModelExploderPart, ModelExploderPartSubObject> handler) { Add(handler); }

        public void Invoke(ModelExploderPart param1, ModelExploderPartSubObject param2)
        {
            if (Get() != null)
            {
                Get().Invoke(param1, param2);
            }
        }

        private bool IsValidHandler(Action<ModelExploderPart, ModelExploderPartSubObject> handler)
        {
            return handler != null && handler.Target is MonoBehaviour;
        }

        public void Add(Action<ModelExploderPart, ModelExploderPartSubObject> handler)
        {
            if (!IsValidHandler(handler)) return;

            var target = handler.Target as Object;

            if (!Targets.Contains(target))
            {
#if UNITY_EDITOR
                entries.Add(new Entry(target, handler.Method.Name));
#else
                entries.Add(new Entry(target, handler.GetMethodInfo().Name));
#endif
            }
            else
            {
#if UNITY_EDITOR
                entries.First(e => e.target == target).methodsNames.Add(handler.Method.Name);
#else
                entries.First(e => e.target == target).methodsNames.Add(handler.GetMethodInfo().Name);
#endif
            }

            if (action == null)
            {
                action = Get();
            }
            else
            {
                action = Get() + handler;
            }
        }

        public void Remove(Action<ModelExploderPart, ModelExploderPartSubObject> handler)
        {
            if (!IsValidHandler(handler)) return;

            var target = handler.Target as Object;
            int index = Targets.ToList().IndexOf(target);
            if (index == -1) return;
#if UNITY_EDITOR
            entries[index].methodsNames.Remove(handler.Method.Name);
#else
            entries[index].methodsNames.Remove(handler.GetMethodInfo().Name);
#endif
            if (entries[index].methodsNames.Count == 0)
                entries.RemoveAt(index);

            action = Get() - handler;
        }

        public Action<ModelExploderPart, ModelExploderPartSubObject> Get()
        {
            if (action == null)
            {
                foreach (var entry in entries)
                {
                    var target = entry.target;
                    if (target == null) continue;
                    foreach (var method in entry.methodsNames)
                    {
#if UNITY_EDITOR
                        action += Delegate.CreateDelegate(typeof(Action<ModelExploderPart, ModelExploderPartSubObject>), target, method) as Action<ModelExploderPart, ModelExploderPartSubObject>;
#else
                        action += action.GetMethodInfo().CreateDelegate(typeof(Action<ModelExploderPart, ModelExploderPartSubObject>), target) as Action<ModelExploderPart, ModelExploderPartSubObject>;
#endif
                    }
                }
            }
            return action;
        }
    }

    [Serializable]
    public class MetaUnityModelExploderPartAction
    {
        [Serializable]
        private class Entry
        {
            public Object target;
            public List<string> methodsNames = new List<string>();
            public Entry() { }
            public Entry(Object target, string methodName)
            {
                this.target = target;
                methodsNames.Add(methodName);
            }
        }

        private Action<ModelExploderPart> action;
        [SerializeField]
        private List<Entry> entries = new List<Entry>();

        public Object[] Targets { get { return entries.Select(e => e.target).ToArray(); } }

        public MetaUnityModelExploderPartAction() { }
        public MetaUnityModelExploderPartAction(Action<ModelExploderPart> handler) { Add(handler); }

        public void Invoke(ModelExploderPart param)
        {
            if (Get() != null)
            {
                Get().Invoke(param);
            }
        }

        private bool IsValidHandler(Action<ModelExploderPart> handler)
        {
            return handler != null && handler.Target is MonoBehaviour;
        }

        public void Add(Action<ModelExploderPart> handler)
        {
            if (!IsValidHandler(handler)) return;

            var target = handler.Target as Object;

            if (!Targets.Contains(target))
            {
#if UNITY_EDITOR
                entries.Add(new Entry(target, handler.Method.Name));
#else
                entries.Add(new Entry(target, handler.GetMethodInfo().Name));
#endif
            }
            else
            {
#if UNITY_EDITOR
                entries.First(e => e.target == target).methodsNames.Add(handler.Method.Name);
#else
                entries.First(e => e.target == target).methodsNames.Add(handler.GetMethodInfo().Name);
#endif
            }

            if (action == null)
            {
                action = Get();
            }
            else
            {
                action = Get() + handler;
            }
        }

        public void Remove(Action<ModelExploderPart> handler)
        {
            if (!IsValidHandler(handler)) return;

            var target = handler.Target as Object;
            int index = Targets.ToList().IndexOf(target);
            if (index == -1) return;
#if UNITY_EDITOR
            entries[index].methodsNames.Remove(handler.Method.Name);
#else
            entries[index].methodsNames.Remove(handler.GetMethodInfo().Name);
#endif
            if (entries[index].methodsNames.Count == 0)
                entries.RemoveAt(index);

            action = Get() - handler;
        }

        public Action<ModelExploderPart> Get(bool info = false)
        {
            if (action == null)
            {
                if (info)
                {
                    Debug.Log("action == null");
                    Debug.Log("entries.Count = " + entries.Count);
                }
                foreach (var entry in entries)
                {
                    if (info)
                    {
                        Debug.Log("entry.methodsNames.Count = " + entry.methodsNames.Count);
                    }
                    var target = entry.target;
                    if (target == null) continue;
                    foreach (var method in entry.methodsNames)
                    {
#if UNITY_EDITOR
                        action += Delegate.CreateDelegate(typeof(Action<ModelExploderPart>), target, method) as Action<ModelExploderPart>;
#else
                        action += action.GetMethodInfo().CreateDelegate(typeof(Action<ModelExploderPart>), target) as Action<ModelExploderPart>;
#endif
                    }
                }
            }
            return action;
        }
    }
}