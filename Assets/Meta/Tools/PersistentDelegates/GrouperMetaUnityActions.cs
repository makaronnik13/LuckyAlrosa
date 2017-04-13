using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Object = UnityEngine.Object;
using System.Reflection;

namespace Meta.Tools
{
    [Serializable]
    public class MetaUnityGrouperSubObjectAction
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

        private Action<GrouperPartSubObject> action;
        [SerializeField]
        private List<Entry> entries = new List<Entry>();

        public Object[] Targets { get { return entries.Select(e => e.target).ToArray(); } }

        public MetaUnityGrouperSubObjectAction() { }
        public MetaUnityGrouperSubObjectAction(Action<GrouperPartSubObject> handler) { Add(handler); }

        public void Invoke(GrouperPartSubObject param)
        {
            if (Get() != null)
            {
                Get().Invoke(param);
            }
        }

        private bool IsValidHandler(Action<GrouperPartSubObject> handler)
        {
            if (handler == null)
            {
                return false;
            }

            string[] nestedClasses = handler.Target.GetType().ToString().Split('+');
            for (int i = 0; i < nestedClasses.Length; i++)
            {
                if (handler.Target.GetType().GetTypeInfo().BaseType == typeof(MonoBehaviour))
                {
                    return true;
                }
            }
            return false;
        }

        public void Add(Action<GrouperPartSubObject> handler)
        {
            if (!IsValidHandler(handler)) return;

/*
            if (handler.Method.Name == "onSubObjectRemoved" || handler.Method.Name == "onSourceDestroyed")
            {
                Debug.Log("");
            }
*/
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

        public void Remove(Action<GrouperPartSubObject> handler)
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

        public Action<GrouperPartSubObject> Get()
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
                        action += Delegate.CreateDelegate(typeof(Action<GrouperPartSubObject>), target, method) as Action<GrouperPartSubObject>;
#else
                        action += action.GetMethodInfo().CreateDelegate(typeof(Action<GrouperPartSubObject>), target) as Action<GrouperPartSubObject>;
#endif
                    }
                }
            }
            return action;
        }
    }

    [Serializable]
    public class MetaUnityGrouperSubObjectIntAction
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

        private Action<GrouperPartSubObject, int> action;
        [SerializeField]
        private List<Entry> entries = new List<Entry>();

        public Object[] Targets { get { return entries.Select(e => e.target).ToArray(); } }

        public MetaUnityGrouperSubObjectIntAction() { }
        public MetaUnityGrouperSubObjectIntAction(Action<GrouperPartSubObject, int> handler) { Add(handler); }

        public void Invoke(GrouperPartSubObject param1, int param2)
        {
            if (Get() != null)
            {
                Get().Invoke(param1, param2);
            }
        }

        private bool IsValidHandler(Action<GrouperPartSubObject, int> handler)
        {
            if (handler == null)
            {
                return false;
            }

            string[] nestedClasses = handler.Target.GetType().ToString().Split('+');
            for (int i = 0; i < nestedClasses.Length; i++)
            {
                if (handler.Target.GetType().GetTypeInfo().BaseType == typeof(MonoBehaviour))
                {
                    return true;
                }
            }
            return false;
        }

        public void Add(Action<GrouperPartSubObject, int> handler)
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

        public void Remove(Action<GrouperPartSubObject, int> handler)
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

        public Action<GrouperPartSubObject, int> Get()
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
                        action += Delegate.CreateDelegate(typeof(Action<GrouperPartSubObject, int>), target, method) as Action<GrouperPartSubObject, int>;
#else
                        action += action.GetMethodInfo().CreateDelegate(typeof(Action<GrouperPartSubObject, int>), target) as Action<GrouperPartSubObject, int>;
#endif
                    }
                }
            }
            return action;
        }
    }

    [Serializable]
    public class MetaUnityGrouperPartSubObjectAction
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

        private Action<GrouperPart, GrouperPartSubObject> action;
        [SerializeField]
        private List<Entry> entries = new List<Entry>();

        public Object[] Targets { get { return entries.Select(e => e.target).ToArray(); } }

        public MetaUnityGrouperPartSubObjectAction() { }
        public MetaUnityGrouperPartSubObjectAction(Action<GrouperPart, GrouperPartSubObject> handler) { Add(handler); }

        public void Invoke(GrouperPart param1, GrouperPartSubObject param2)
        {
            if (Get() != null)
            {
                Get().Invoke(param1, param2);
            }
        }

        private bool IsValidHandler(Action<GrouperPart, GrouperPartSubObject> handler)
        {
            if (handler == null)
            {
                return false;
            }

            string[] nestedClasses = handler.Target.GetType().ToString().Split('+');
            for (int i = 0; i < nestedClasses.Length; i++)
            {
                if (handler.Target.GetType().GetTypeInfo().BaseType == typeof(MonoBehaviour))
                {
                    return true;
                }
            }
            return false;
        }

        public void Add(Action<GrouperPart, GrouperPartSubObject> handler)
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

        public void Remove(Action<GrouperPart, GrouperPartSubObject> handler)
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

        public Action<GrouperPart, GrouperPartSubObject> Get()
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
                        action += Delegate.CreateDelegate(typeof(Action<GrouperPart, GrouperPartSubObject>), target, method) as Action<GrouperPart, GrouperPartSubObject>;
#else
                        action += action.GetMethodInfo().CreateDelegate(typeof(Action<GrouperPart, GrouperPartSubObject>), target) as Action<GrouperPart, GrouperPartSubObject>;
#endif
                    }
                }
            }
            return action;
        }
    }

    [Serializable]
    public class MetaUnityGrouperPartAction
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

        private Action<GrouperPart> action;
        [SerializeField]
        private List<Entry> entries = new List<Entry>();

        public Object[] Targets { get { return entries.Select(e => e.target).ToArray(); } }

        public MetaUnityGrouperPartAction() { }
        public MetaUnityGrouperPartAction(Action<GrouperPart> handler) { Add(handler); }

        public void Invoke(GrouperPart param)
        {
            if (Get() != null)
            {
                Get().Invoke(param);
            }
        }

        private bool IsValidHandler(Action<GrouperPart> handler)
        {
            if (handler == null)
            {
                return false;
            }

            string[] nestedClasses = handler.Target.GetType().ToString().Split('+');
            for (int i = 0; i < nestedClasses.Length; i++)
            {
/*
                if (Type.GetType(nestedClasses[i]).BaseType == typeof(MonoBehaviour))
                {
                    return true;
                }
*/
                if (handler.Target.GetType().GetTypeInfo().BaseType == typeof(MonoBehaviour))
                {
                    return true;
                }
        }
            return false;
        }

        public void Add(Action<GrouperPart> handler)
        {
            if (!IsValidHandler(handler)) return;

            var target = handler.Target as Object;

            /*Debug.Log("Add: handler.Target.ToString() = " + handler.Target.ToString());
            Debug.Log("Add: target.name = " + target.name);
            Debug.Log("Add: handler.Method.Name = " + handler.Method.Name);*/

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

        public void Remove(Action<GrouperPart> handler)
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

        public Action<GrouperPart> Get(bool info = false)
        {
            if (action == null)
            {
                foreach (var entry in entries)
                {
                    if (info)
                    {
                        Debug.Log("");
                        Debug.Log("entry.target = " + entry.target);
                        Debug.Log("entry.target = " + entry.target.name);
                        Debug.Log("entry.methodsNames.Count = " + entry.methodsNames.Count);
                    }
                    var target = entry.target;
                    if (target == null) continue;
                    foreach (var method in entry.methodsNames)
                    {
#if UNITY_EDITOR
                        action += Delegate.CreateDelegate(typeof(Action<GrouperPart>), target, method) as Action<GrouperPart>;
#else
                        action += action.GetMethodInfo().CreateDelegate(typeof(Action<GrouperPart>), target) as Action<GrouperPart>;
#endif
                        if (info)
                        {
                            Debug.Log("method = " + method);
                        }
                    }
                }
            }

            return action;
        }
    }
}
