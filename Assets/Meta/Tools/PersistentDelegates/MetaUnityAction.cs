using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Object = UnityEngine.Object;
using System.Reflection;

namespace Meta.Tools
{
    [Serializable]
    public class MetaUnityAction
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

        private Action action;
        [SerializeField]
        private List<Entry> entries = new List<Entry>();

        public Object[] Targets { get { return entries.Select(e => e.target).ToArray(); } }

        public MetaUnityAction() { }
        public MetaUnityAction(Action handler) { Add(handler); }

        public void Invoke()
        {
            Get().Invoke();
        }

        private bool IsValidHandler(Action handler)
        {
            return handler != null && handler.Target is MonoBehaviour;
        }

        public void Add(Action handler)
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

        public void Remove(Action handler)
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

        public Action Get()
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
                        action += Delegate.CreateDelegate(typeof(Action), target, method) as Action;
#else
                        action += action.GetMethodInfo().CreateDelegate(typeof(Action), target) as Action;
#endif
                    }
                }
            }
            return action;
        }
    }
}
