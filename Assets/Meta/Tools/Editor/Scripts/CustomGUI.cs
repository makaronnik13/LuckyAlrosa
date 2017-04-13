using System;
using System.Collections;
using System.Collections.Generic;
using Meta.Tools;
using UnityEngine;
using UnityEditor;


namespace Meta.Tools.Editor
{
    public static class CustomGUI
    {
        private static GUISkin _customSkin;
        private static Texture2D _sliderImgOut;
        private static Texture2D _sliderImgIn;
        private static Texture2D _popupArrow;
        private static Dictionary<Rect, bool> _enumPopups = new Dictionary<Rect, bool>();

        private static Texture2D PopupArrow
        {
            get
            {
                if (_popupArrow == null)
                {
                    _popupArrow = (Texture2D)Resources.Load("Textures/Icon_Arrow"); ;
                }
                return _popupArrow;
            }
        }

        private static GUISkin CustomSkin
        {
            get
            {
                if (_customSkin == null)
                {
                    _customSkin = (GUISkin)Resources.Load("GUISkins/CustomGui");

                    var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    texture.SetPixel(0, 0, new Color(59.0f / 255, 66.0f / 255, 74.0f / 255));
                    texture.Apply();
                    _customSkin.GetStyle("popupButton").active.background = texture;
                    _customSkin.GetStyle("popupButton").focused.background = texture;
                    _customSkin.GetStyle("popupButton").normal.background = texture;
                    texture = new Texture2D(1, 1);
                    texture.SetPixel(0, 0, new Color(123.0f / 255, 126.0f / 255, 133.0f / 255));
                    texture.Apply();
                    _customSkin.GetStyle("popupButton").hover.background = texture;
                }
                return _customSkin;
            }
        }

        private static Texture2D SliderImgOut
        {
            get
            {
                if (_sliderImgOut == null)
                {
                    _sliderImgOut = (Texture2D)Resources.Load("Textures/Zoom In_Out/Zoom_Out");
                }
                return _sliderImgOut;
            }
        }

        private static Texture2D SliderImgIn
        {
            get
            {
                if (_sliderImgIn == null)
                {
                    _sliderImgIn = (Texture2D)Resources.Load("Textures/Zoom In_Out/Zoom_In");
                }
                return _sliderImgIn;
            }
        }


        public static bool DrawToggle(Rect rect, bool val)
        {
            bool value;
            GUILayout.BeginArea(rect);

            value = GUILayout.Toggle(val, "", CustomSkin.toggle);

            GUILayout.EndArea();
            return value;
        }

        public static float DrawSlider(Rect rect, float val, float min, float max)
        {
            float result;
            GUILayout.BeginArea(rect);
            GUILayout.BeginHorizontal();


            CustomSkin.GetStyle("SliderLeftImg").overflow.top = -Mathf.FloorToInt(rect.width * 0.1f);
            CustomSkin.GetStyle("SliderRightImg").overflow.top = -Mathf.FloorToInt(rect.width * 0.05f);

            CustomSkin.horizontalSliderThumb.overflow.top = -Mathf.FloorToInt(rect.width * 0.05f);

            GUILayout.Label("", CustomSkin.GetStyle("SliderLeftImg"), GUILayout.Width(rect.width * 0.1f));

            GUILayout.FlexibleSpace();
            result = GUILayout.HorizontalSlider(val, max, min, CustomSkin.horizontalSlider, CustomSkin.horizontalSliderThumb, GUILayout.Width(rect.width*0.7f));
            GUILayout.FlexibleSpace();
            GUILayout.Label("", CustomSkin.GetStyle("SliderRightImg"), GUILayout.Width(rect.width * 0.15f));

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            return result;
        }

        public static Enum EnumPopup(Rect rect, Rect container, Enum onKeynoteStartAction)
        {
            GUI.color = Color.white;
            GUILayout.BeginArea(rect);
            GUILayout.BeginHorizontal();
            float fontSize = rect.width/4;

            CustomSkin.GetStyle("popup").overflow.left = Mathf.FloorToInt(-rect.width + rect.height / 6);
            CustomSkin.GetStyle("popup").overflow.top = Mathf.FloorToInt(-rect.height / 2.5f);
            CustomSkin.GetStyle("popup").overflow.bottom = Mathf.FloorToInt(-rect.height / 3.5f);

            GUILayout.Toggle(PopupValue(container), "<color=#cccccc><size=" + fontSize + ">" + onKeynoteStartAction + "</size></color>", CustomSkin.GetStyle("popup"));
            
            int maxString = 0;
            foreach (System.Object ob in Enum.GetValues(onKeynoteStartAction.GetType()))
            {
                if (ob.ToString().Length > maxString)
                {
                    maxString = ob.ToString().Length;
                }
            }
            Rect leftVoidRect = new Rect(rect.x + rect.width, rect.y, Mathf.Max(maxString * fontSize, rect.width), Enum.GetValues(onKeynoteStartAction.GetType()).Length * rect.height/2);

            
            if (new Rect(0, 0, rect.width, rect.height).Contains(Event.current.mousePosition) || (new Rect(rect.width, 0, leftVoidRect.width, leftVoidRect.height).Contains(Event.current.mousePosition) && PopupValue(container)))
            {
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
                GUILayout.BeginArea(leftVoidRect);
                EditorGUILayout.BeginVertical();
                foreach (System.Object ob in Enum.GetValues(onKeynoteStartAction.GetType()))
                {
                    if (GUILayout.Button("<color=#cccccc><size=" + fontSize + ">" + ob.ToString() + "</size></color>", CustomSkin.GetStyle("popupButton"), GUILayout.Height(rect.height/2)))
                    {
                        _enumPopups[container] = false;
                        return (Enum)ob;
                    }
                }
                EditorGUILayout.EndVertical();
                GUILayout.EndArea();
                _enumPopups[container] = true;
            }
            else
            {
                _enumPopups[container] = false;
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
            
            return onKeynoteStartAction;
        }


        private static bool PopupValue(Rect rect)
        {
            if (!_enumPopups.ContainsKey(rect))
            {
                _enumPopups.Add(rect, false);
            }
            return _enumPopups[rect];
        }
    }
}
