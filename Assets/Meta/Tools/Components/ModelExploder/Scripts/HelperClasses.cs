using System;
using UnityEngine;

namespace Meta.Tools
{

    [Serializable]
    public struct SerializableBounds
    {
        public SerializableBounds(Vector3 center, Vector3 extents)
        {
            Center = new SerializableVector3();
            Extents = new SerializableVector3();
            Center.Convert(center);
            Extents.Convert(extents);
        }

        public SerializableVector3 Center;
        public SerializableVector3 Extents;
    }

    [Serializable]
    public struct SerializableRect
    {
        public SerializableRect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rect Extract()
        {
            return new Rect(X, Y, Width, Height);
        }

        public void Convert(Rect source)
        {
            X = source.x;
            Y = source.y;
            Width = source.width;
            Height = source.height;
        }

        public bool Contains(Vector2 point)
        {
            return point.x >= X && point.y >= Y && point.x < X + Width && point.y < Y + Height;
        }

        public float X;
        public float Y;
        public float Width;
        public float Height;
    }

    [Serializable]
    public class SerializableVector3 : ICloneable
    {
        public float X;
        public float Y;
        public float Z;

        public object Clone()
        {
            SerializableVector3 clone = new SerializableVector3();

            clone.X = X;
            clone.Y = Y;
            clone.Z = Z;

            return clone;
        }

        public void Convert(Vector3 source)
        {
            X = source.x;
            Y = source.y;
            Z = source.z;
        }

        public Vector3 Extract()
        {
            return new Vector3(X, Y, Z);
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }
    }

    [Serializable]
    public struct SerializableColor
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public Color Extract()
        {
            return new Color(R, G, B, A);
        }

        public void Convert(Color source)
        {
            R = source.r;
            G = source.g;
            B = source.b;
            A = source.a;
        }

        public override string ToString()
        {
            return "(" + R + ", " + G + ", " + B + ", " + A + ")";
        }
    }

    [Serializable]
    public enum CameraAxisSide
    {
        PlusX, MinusX,
        PlusY, MinusY,
        PlusZ, MinusZ
    }

    [Serializable]
    public enum ProjectionAplliedEffect
    {
        //SelectionDefault,
        WireFrame,
        TransparentColor
    }

    [Flags, Serializable]
    public enum Axis
    {
        X = 1,
        Y = 2,
        Z = 4
    }

    [Serializable]
    public class CustomTransform : ICloneable
    {
        public CustomTransform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Position = new SerializableVector3();
            Rotation = new SerializableVector3();
            Scale = new SerializableVector3();

            Position.Convert(position);
            Rotation.Convert(rotation);
            Scale.Convert(scale);
        }

        public CustomTransform()
        {
            Position = new SerializableVector3();
            Rotation = new SerializableVector3();
            Scale = new SerializableVector3();

            Position.Convert(Vector3.zero);
            Rotation.Convert(Vector3.zero);
            Scale.Convert(Vector3.zero);
        }

        public SerializableVector3 Position;
        public SerializableVector3 Rotation;
        public SerializableVector3 Scale;

        public override string ToString()
        {
            return "Position: " + Position.ToString() + "; Rotation: " + Rotation.ToString() + "; Scale: " + Scale.ToString();
        }

        public void Convert(Transform source)
        {
            Position.Convert(source.position);
            Rotation.Convert(source.rotation.eulerAngles);
            Scale.Convert(source.localScale);
        }

        public object Clone()
        {
            CustomTransform clone = new CustomTransform(Position.Extract(), Rotation.Extract(), Scale.Extract());

            return clone;
        }
    }

    [Serializable]
    public enum CenteringMode
    {
        /// <summary>
        /// To use arbitrary gameObject's cordinates as a center
        /// </summary>
        GameObject,
        /// <summary>
        /// To use center of bounding box as a center
        /// </summary>
        BoundingBox
    }

    [Serializable]
    public enum TransformToUse
    {
        Global = 0,
        Local
    }

    [Serializable]
    public class ExplosionSettings
    {
        [Serializable]
        public class Fields : ICloneable
        {
            public SerializableVector3 InitialScale = new SerializableVector3();
            public ExplosionRule ExplosionRule;
            public Range01FloatValues ExplosionTimings;
            public Axis Axis;
            public TransformToUse TransformToUse;
            public float Distance;
            public bool UseCustomOrigin;
            public CustomTransform CustomOrigin;

            public object Clone()
            {
                Fields clone = new Fields();
                clone.InitialScale = (SerializableVector3)InitialScale.Clone();
                clone.Axis = Axis;
                clone.TransformToUse = TransformToUse;
                clone.Distance = Distance;
                clone.ExplosionRule = ExplosionRule;
                clone.ExplosionTimings = (Range01FloatValues)ExplosionTimings.Clone();
                clone.UseCustomOrigin = UseCustomOrigin;
                clone.CustomOrigin = (CustomTransform)CustomOrigin.Clone();
                return clone;
            }
        }

        [SerializeField]
        private Fields _uniques;
        public Fields Uniques
        {
            get
            {
                return _uniques;
            }
            set
            {
                _uniques = value;
            }
        }

        public ExplosionSettings(Fields defaults)
        {
            Uniques = (Fields)defaults.Clone();
        }

        public bool UniqueExplosionRule = false;
        public bool UniqueExplosionTimings = false;
        public bool UniqueAxis = false;
        public bool UniqueTransformToUse = false;
        public bool UniqueDistance = false;
        public bool UniqueUseCustomOrigin = false;
        public bool UniqueCustomOrigin = false;

        public bool HasSomethingUnique
        {
            get
            {
                return UniqueExplosionRule || UniqueExplosionTimings || UniqueAxis || UniqueTransformToUse || UniqueDistance || UniqueUseCustomOrigin || UniqueCustomOrigin;
            }
        }
    }

    [Serializable]
    public enum ExplosionRule
    {
        Radial,
        Sphere,
        /*Round,
        SingleAxis,*/
        AxisWise/*,
        SingleVector,
        VectorWise,
        FreeTrajectory*/
    }

    [Serializable]
    public class ExplosionSettingsBundle
    {
        public string Name;
        public int ID = 0;
        public int sourceID = 0;
        public ExplosionSettings.Fields DefaultSettings;
    }
}
