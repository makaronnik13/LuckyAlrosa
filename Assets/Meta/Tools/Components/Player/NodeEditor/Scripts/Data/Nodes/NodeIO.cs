using System;
using UnityEngine;

namespace Meta.Tools
{
    [Serializable]
    public class NodeIO : ScriptableObject
    {
        public string Name;
        public Rect IORect;
        public Rect GlobalRect
        {
            get
            {
                return new Rect(Parent.NodeRect.x + IORect.x,
                    Parent.NodeRect.y + IORect.y,
                    IORect.width,
                    IORect.height);
            }
        }
        public Rect CenterRect
        {
            get
            {
                return new Rect(Parent.NodeRect.x + IORect.x + IORect.width * 0.4f,
                    Parent.NodeRect.y + IORect.y + IORect.height * 0.4f,
                    0f,
                    0f);
            }
        }
        public IOType Type;
        public NodeBase Parent;
    }
}