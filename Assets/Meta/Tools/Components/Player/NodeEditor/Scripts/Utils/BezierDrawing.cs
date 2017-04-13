using UnityEngine;

namespace Meta.Tools
{
    public class BezierDrawing
    {
        public static Texture2D AALineTex = null;
        public static Texture2D LineTex = null;

        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
        {
            Color savedColor = GUI.color;
            Matrix4x4 savedMatrix = GUI.matrix;

            if (!LineTex)
            {
                LineTex = new Texture2D(1, 1, TextureFormat.ARGB32, true);
                LineTex.SetPixel(0, 1, Color.white);
                LineTex.Apply();
            }
            if (!AALineTex)
            {
                AALineTex = new Texture2D(1, 3, TextureFormat.ARGB32, true);
                AALineTex.SetPixel(0, 0, new Color(1, 1, 1, 0));
                AALineTex.SetPixel(0, 1, Color.white);
                AALineTex.SetPixel(0, 2, new Color(1, 1, 1, 0));
                AALineTex.Apply();
            }
            if (antiAlias) width *= 3;
            float angle = Vector3.Angle(pointB - pointA, Vector2.right) * (pointA.y <= pointB.y ? 1 : -1);
            float m = (pointB - pointA).magnitude;
            if (m > 0.01f)
            {
                Vector3 dz = new Vector3(pointA.x, pointA.y, 0);

                GUI.color = color;
                GUI.matrix = _translationMatrix(dz) * GUI.matrix;
                GUIUtility.ScaleAroundPivot(new Vector2(m, width), new Vector3(-0.5f, 0, 0));
                GUI.matrix = _translationMatrix(-dz) * GUI.matrix;
                GUIUtility.RotateAroundPivot(angle, Vector2.zero);
                GUI.matrix = _translationMatrix(dz + new Vector3(width / 2, -m / 2) * Mathf.Sin(angle * Mathf.Deg2Rad)) * GUI.matrix;

                if (!antiAlias)
                    GUI.DrawTexture(new Rect(0, 0, 1, 1), LineTex);
                else
                    GUI.DrawTexture(new Rect(0, 0, 1, 1), AALineTex);
            }
            GUI.matrix = savedMatrix;
            GUI.color = savedColor;
        }

        public static void BezierLine(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, bool antiAlias, int segments)
        {
            Vector2 lastV = _cubeBezier(start, startTangent, end, endTangent, 0);
            for (int i = 1; i <= segments; ++i)
            {
                Vector2 v = _cubeBezier(start, startTangent, end, endTangent, i / (float)segments);

                BezierDrawing.DrawLine(
                    lastV,
                    v,
                    color, width, antiAlias);
                lastV = v;
            }
        }

        private static Vector2 _cubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
        {
            float rt = 1 - t;
            float rtt = rt * t;
            return rt * rt * rt * s + 3 * rt * rtt * st + 3 * rtt * t * et + t * t * t * e;
        }

        private static Matrix4x4 _translationMatrix(Vector3 v)
        {
            return Matrix4x4.TRS(v, Quaternion.identity, Vector3.one);
        }

        public static void DrawBezierFromTo(Rect wr, Rect wr2, Color color, Color shadow, bool withShadow = false)
        {
            if (withShadow)
            {
                BezierLine(
                new Vector2(wr.x + wr.width, wr.y + 3 + wr.height / 2),
                new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr.y + 3 + wr.height / 2),
                new Vector2(wr2.x, wr2.y + 3 + wr2.height / 2),
                new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr2.y + 3 + wr2.height / 2), shadow, 5, true, 40);
            }
            BezierLine(
                new Vector2(wr.x + wr.width, wr.y + wr.height / 2),
                new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr.y + wr.height / 2),
                new Vector2(wr2.x, wr2.y + wr2.height / 2),
                new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr2.y + wr2.height / 2), color, 2, true, 40);
        }
    }
}
