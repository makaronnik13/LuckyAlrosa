using System;

namespace Meta.Tools
{
    [AttributeUsage(AttributeTargets.Class)]
    public class KeyframeFor : Attribute
    {
        public readonly Type Type;
        public readonly string[] Paths;

        public KeyframeFor(Type type, string[] paths = null)
        {
            Type = type;
            Paths = paths;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class InterpolationFor : Attribute
    {
        public readonly Type Type;
        public readonly string[] Paths;

        public InterpolationFor(Type type, string[] paths = null)
        {
            Type = type;
            Paths = paths;
        }
    }
}
