using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Meta.Tools
{
    public static class ArrowManager
    {
        private static bool _RMBPressed, _edit = true, _randomColor = true;
        private static Color _color = Color.red;
        private static List<Point> _points = new List<Point>();
        private static List<Arrow> _arrows = new List<Arrow>();
        private static Arrow _lastSelected;
        private static Vector3 _oldPos = Vector3.zero;

        /// <summary>
        ///     Add functionality to manage exist Arrows and create new
        /// </summary>
        public static void ManageArrows(List<Arrow> arrows = null, bool randomColor = true)
        {
            if (arrows != null) {
                _arrows.AddRange(arrows);
            }

            Event e = Event.current;
            var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            var dist = Vector3.Distance(Vector3.zero, ray.origin);
            var pos = ray.GetPoint(dist);

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1 && _edit)
                    {
                        _RMBPressed = true;
                    }
                    break;
                case EventType.MouseUp:
                    if (e.button == 1 && _edit)
                    {
                        _RMBPressed = false;
                        // add new point
                        if (_points.Count == 0)
                        {
                            if (_randomColor) {
                                _points.Add(new Point(pos));
                            }
                            else
                            {
                                _points.Add(new Point(pos, _color));
                            }
                            deselectArrows();
                        }
                        else
                        {
                            // add new arrow
                            _points.Add(new Point(pos, _points[0].Color));
                            _arrows.Add(new Arrow(_points[0].Position, _points[1].Position, _points[0].Color));
                            _points.Clear();
                            deselectArrows();
                        }
                    }
                    break;
                case EventType.KeyUp:
                    if (e.keyCode == KeyCode.D)
                    {
                        if (_arrows.Count > 0)
                        {
                            for (int i = 0; i < _arrows.Count; i++)
                            {
                                if (_arrows[i].Selected)
                                    _arrows.Remove(_arrows[i]);
                                else if (_points.Count > 0 && _points.Count % 2 != 0)
                                    _points.RemoveAt(_points.Count - 1);
                            }
                        }
                        e.Use();
                    }
                    else if (e.keyCode == KeyCode.Z)
                    {
                        undoInput();
                    }
                    break;
            }

            if (_RMBPressed && _edit)
            {
                // draw new point
                deselectArrows();
                Handles.color = Color.yellow;
                Handles.FreeMoveHandle(pos, Quaternion.identity, dist / 40, Vector3.one * 0.1f, Handles.SphereCap);
            }

            // draw exist points
            for (int i = 0; i < _points.Count; i++)
            {
                Handles.color = _points[i].Color;
                Vector3 newPos = Handles.FreeMoveHandle(_points[i].Position, Quaternion.identity, dist / 50, Vector3.one * 0.1f, Handles.SphereCap);

                if (newPos != _points[i].Position)
                {
                    _points[i].Position = newPos;
                }
            }

            // draw exist arrows
            for (int i = 0; i < _arrows.Count; i++)
            {
                var arr = _arrows[i];
                int controlID = GUIUtility.GetControlID(arr.Hash, FocusType.Passive);
                Vector3 screenPosition = Handles.matrix.MultiplyPoint(arr.End);

                switch (e.GetTypeForControl(controlID))
                {
                    case EventType.Layout:
                        HandleUtility.AddControl(
                            controlID,
                            HandleUtility.DistanceToLine(arr.Start, arr.End)
                        );
                        break;
                    case EventType.MouseDown:
                        if (HandleUtility.nearestControl == controlID && e.button == 0)
                        {
                            _lastSelected = arr;
                            selectNewArrow(_lastSelected);
                            GUIUtility.hotControl = controlID;
                            _oldPos = pos;
                            e.Use();
                        }
                        break;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == controlID && e.button == 0)
                        {
                            GUIUtility.hotControl = 0;
                            e.Use();
                            Debug.Log("id: " + controlID);
                        }
                        break;

                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == controlID && e.button == 0)
                        {
                            var fromStart = Vector3.Distance(pos, arr.Start);
                            var fromEnd = Vector3.Distance(pos, arr.End);
                            var limit = arr.Length * 0.1; // 10% from start and end point

                            if (fromStart < limit)
                            {
                                arr.Start = pos;
                            }
                            else if (fromEnd < limit)
                            {
                                arr.End = pos;
                            }
                            else
                            {
                                var delta = pos - _oldPos;
                                arr.Start = arr.Start + delta;
                                arr.End = arr.End + delta;
                                _oldPos = pos;
                            }

                            GUI.changed = true;
                            e.Use();
                        }
                        break;

                    case EventType.Repaint:
                        arr.Draw(i, dist);
                        break;
                }

                if (!arr.Selected)
                {
                    Handles.lighting = false;
                }

                Vector3
                    oldStartPoint = arr.Start,
                    startOffset = (arr.Start - arr.End).normalized * (dist / 50),
                    newStartPoint = Handles.FreeMoveHandle(
                    oldStartPoint + startOffset, Quaternion.identity, dist / 50, Vector3.one * 0.1f, Handles.SphereCap) - startOffset,

                    oldEndPoint = arr.End,
                    endOffset = (arr.End - arr.Start).normalized * ((dist / 25)),
                    newEndPoint = Handles.FreeMoveHandle(
                    oldEndPoint + endOffset, Quaternion.identity, dist / 50, Vector3.one * 0.1f, Handles.SphereCap) - endOffset;

                if (oldStartPoint != newStartPoint)
                {
                    selectNewArrow(_lastSelected);
                    arr.Start = newStartPoint;
                    arr.Length = (arr.End - arr.Start).magnitude;
                }
                else if (oldEndPoint != newEndPoint)
                {
                    selectNewArrow(_lastSelected);
                    arr.End = newEndPoint;
                    arr.Length = (arr.End - arr.Start).magnitude;
                }
            }
        }

        /// <summary>
        ///     Can user create new Arrows?
        /// </summary>
        public static bool Edit {
            get { return _edit; }
            set { _edit = value; }
        }

        /// <summary>
        ///     Create all new Arrows with random color?
        /// </summary>
        public static bool RandomColor {
            get { return _randomColor; }
            set { _randomColor = value; }
        }

        /// <summary>
        ///     Color for new arrows
        /// </summary>
        public static Color Color  {
            get { return _color; }
            set { _color = value; }
        }

        /// <summary>
        ///     Get or set Arrows
        /// </summary>
        public static List<Arrow> Arrows {
            get { return _arrows; }
            set { _arrows = value; }
        }

        /// <summary>
        ///     Append single arrow to list
        /// </summary>
        public static void AddArrow(Arrow arrow) {
            _arrows.Add(arrow);
        }

        private static void selectNewArrow(Arrow arr) {
            if (arr != null)
            {
                deselectArrows();

                if (_arrows.Count > 0)
                    _arrows[_arrows.IndexOf(arr)].Selected = true;
            }
        }

        private static void deselectArrows()
        {
            foreach (var arr in _arrows)
                arr.Selected = false;
        }

        private static void undoInput()
        {
            if (_points.Count > 0 && _points.Count % 2 != 0)
            {
                _points.RemoveAt(_points.Count - 1);
            }
            else if (_arrows.Count > 0)
            {
                _arrows.RemoveAt(_arrows.Count - 1);
            }
            SceneView.RepaintAll();
        }
    }

    public class Point
    {
        private Vector3 _position;
        private Color _color;

        public Point(Vector3 position)
        {
            _position = position;
            _color = new Color(Random.value, Random.value, Random.value, 1.0f);
        }

        public Point(Vector3 position, Color color)
        {
            _position = position;
            _color = color;
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
    }
}