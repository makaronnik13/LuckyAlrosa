using UnityEditor;
using UnityEngine;
using System;

namespace Meta.Tools.Editor
{
    public partial class ModelExploderEditor : UnityEditor.Editor
    {
        [NonSerialized]
        private int _selectedPartIndex = -1;
        //[NonSerialized]
        //private bool _enterNameVisible = false;
        [NonSerialized]
        private string _enteredName;

        private bool _customOrigin;

        private void drawingExplosionSettings()
        {
            #region drawing explosion settings

            EditorGUILayout.BeginHorizontal();

            if (_selectedPartIndex >= 0)
            {
                if (GUILayout.Button(GUIContent.none, _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueExplosionRule ? _uniqueButtonStyle : _commonButtonStyle, new[] { GUILayout.Width(20), GUILayout.Height(20) }))
                {
                    serializedObject.FindProperty("_parts").GetArrayElementAtIndex(_selectedPartIndex).FindPropertyRelative("_currentExplosionSettings").GetArrayElementAtIndex(_modelExploder.CurrentExplosionSettingsBundle).FindPropertyRelative("UniqueExplosionRule").boolValue = !serializedObject.FindProperty("_parts").GetArrayElementAtIndex(_selectedPartIndex).FindPropertyRelative("_currentExplosionSettings").GetArrayElementAtIndex(_modelExploder.CurrentExplosionSettingsBundle).FindPropertyRelative("UniqueExplosionRule").boolValue;
                }
            }

            /*
             * What we exposing in editor are actual settings. It's usually common settings. But if we selected 
             * some part that have unique settings, we must show and be able to modify that unique setting. Also
             * there are bundles of settings that we must consider too.
             */

            ExplosionRule explosionRule;
            if (_selectedPartIndex >= 0)
            {
                explosionRule = _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueExplosionRule ?
                    _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.ExplosionRule :
                    _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.ExplosionRule;
            }
            else
            {
                explosionRule = _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.ExplosionRule;
            }

            EditorGUILayout.LabelField("Auto-Explosion Mode: ", GUILayout.Width(128f));

            int oldExplosionRuleIndex = (int)explosionRule;
            int newExplosionRuleIndex = (int)(ExplosionRule)EditorGUILayout.EnumPopup(explosionRule, _guiSkin.GetStyle("DropDown"), GUILayout.Width(100f));

            if (oldExplosionRuleIndex != newExplosionRuleIndex)
            {
                Undo.RecordObject(target, "New ExplosionRule Setted");

                _modelExploder.CurrentFields(_selectedPartIndex).ExplosionRule = (ExplosionRule)newExplosionRuleIndex;
            }

            GUILayout.FlexibleSpace();

            if (_customOrigin)
            {
                EditorGUILayout.LabelField("Visualize", GUILayout.MaxWidth(70f));
                switch(newExplosionRuleIndex)
                {
                    case (int)ExplosionRule.Sphere:
                        _adjustSphere = EditorGUILayout.Toggle(_adjustSphere, _guiSkin.GetStyle("CheckBox"), GUILayout.MaxWidth(20f));
                        break;
                    default:
                        _adjustCenter = EditorGUILayout.Toggle(_adjustCenter, _guiSkin.GetStyle("CheckBox"), GUILayout.MaxWidth(20f));
                        break;
                }
            }
            else
            {
                _adjustSphere = false;
                _adjustCenter = false;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            string helpText = "";
            switch (newExplosionRuleIndex)
            {
                case (int)ExplosionRule.AxisWise:

                    EditorGUILayout.BeginHorizontal();

                    Axis axis;

                    if (_selectedPartIndex >= 0)
                    {
                        axis = _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueAxis ?
                            _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.Axis :
                            _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.Axis;
                    }
                    else
                    {
                        axis = _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.Axis;
                    }

                    if (_selectedPartIndex >= 0)
                    {
                        if (GUILayout.Button(GUIContent.none, _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueAxis ? _uniqueButtonStyle : _commonButtonStyle, new[] { GUILayout.Width(20), GUILayout.Height(20) }))
                        {
                            serializedObject.FindProperty("_parts").GetArrayElementAtIndex(_selectedPartIndex).FindPropertyRelative("_currentExplosionSettings").GetArrayElementAtIndex(_modelExploder.CurrentExplosionSettingsBundle).FindPropertyRelative("UniqueAxis").boolValue = !serializedObject.FindProperty("_parts").GetArrayElementAtIndex(_selectedPartIndex).FindPropertyRelative("_currentExplosionSettings").GetArrayElementAtIndex(_modelExploder.CurrentExplosionSettingsBundle).FindPropertyRelative("UniqueAxis").boolValue;
                        }
                    }
                    EditorGUILayout.LabelField(_axisWiseAxisContent, Utilities.RichTextStyle);

                    int oldAxis = (int)axis;
                    int newAxis = (int)(Axis)EditorGUILayout.EnumMaskField(axis, _guiSkin.GetStyle("DropDown"));

                    if (oldAxis != newAxis)
                    {
                        Undo.RecordObject(target, "New ExplosionRule Setted");

                        if (_selectedPartIndex >= 0)
                        {
                            if (_modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueAxis)
                            {
                                _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.Axis = (Axis)newAxis;
                            }
                            else
                            {
                                _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.Axis = (Axis)newAxis;
                            }
                        }
                        else
                        {
                            _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.Axis = (Axis)newAxis;
                        }
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();

                    TransformToUse transformToUse;

                    if (_selectedPartIndex >= 0)
                    {
                        transformToUse = _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueTransformToUse ?
                            _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.TransformToUse :
                            _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.TransformToUse;
                    }
                    else
                    {
                        transformToUse = _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.TransformToUse;
                    }

                    if (_selectedPartIndex >= 0)
                    {
                        if (GUILayout.Button(GUIContent.none, _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueTransformToUse ? _uniqueButtonStyle : _commonButtonStyle, new[] { GUILayout.Width(20), GUILayout.Height(20) }))
                        {
                            serializedObject.FindProperty("_parts").GetArrayElementAtIndex(_selectedPartIndex).FindPropertyRelative("_currentExplosionSettings").GetArrayElementAtIndex(_modelExploder.CurrentExplosionSettingsBundle).FindPropertyRelative("UniqueTransformToUse").boolValue = !serializedObject.FindProperty("_parts").GetArrayElementAtIndex(_selectedPartIndex).FindPropertyRelative("_currentExplosionSettings").GetArrayElementAtIndex(_modelExploder.CurrentExplosionSettingsBundle).FindPropertyRelative("UniqueTransformToUse").boolValue;
                        }
                    }
                    EditorGUILayout.LabelField("Transform to use: ", Utilities.RichTextStyle);

                    int oldTransformToUse = (int)transformToUse;
                    int newTransformToUse = (int)EditorGUILayout.Popup((int)transformToUse, new[] { "Global", "Local" }, _guiSkin.GetStyle("DropDown"));

                    if (oldTransformToUse != newTransformToUse)
                    {
                        Undo.RecordObject(target, "New Transform To Use Setted");

                        if (_selectedPartIndex >= 0)
                        {
                            if (_modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueTransformToUse)
                            {
                                _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.TransformToUse = (TransformToUse)newTransformToUse;
                            }
                            else
                            {
                                _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.TransformToUse = (TransformToUse)newTransformToUse;
                            }
                        }
                        else
                        {
                            _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.TransformToUse = (TransformToUse)newTransformToUse;
                        }
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    /*if (drawCustomOrigin())
                    {
                        if (_adjustCenter ? GUILayout.Button("Complete") : GUILayout.Button("Adjust"))
                        {
                            _adjustCenter = !_adjustCenter;
                        }
                    }
                    else
                    {
                        _adjustCenter = false;
                    }*/

                    helpText = "This type of explosion spreads parts respectively to an axis it was farther from the origin in the begining of animation.";
                    break;
                /*case (int)ExplosionRule.FreeTrajectory:
                    helpText = "This type of explosion allows you to specify trajectory wich you want to use as a pattern to move parts.";
                    break;*/
                case (int)ExplosionRule.Radial:
                    helpText = "This type of explosion spreads parts in radial derections from the center of the model.";

                    /*EditorGUILayout.BeginHorizontal();
                    drawDistance();
                    EditorGUILayout.EndHorizontal();*/

                    /*if (drawCustomOrigin())
                    {
                        if (_adjustCenter ? GUILayout.Button("Complete") : GUILayout.Button("Adjust"))
                        {
                            _adjustCenter = !_adjustCenter;
                        }
                    }
                    else
                    {
                        _adjustCenter = false;
                    }*/
                    break;
                /*case (int)ExplosionRule.Round:
                    helpText = "This type of explosion spreads parts so they become positioned accroding ring pattern.";
                    break;*/
                case (int)ExplosionRule.Sphere:
                    helpText = "This type of explosion spreads parts on the sphere's surface.";

                    //EditorGUILayout.LabelField("<b>Sphere</b>", Utilities.RichTextStyle);

                    /*EditorGUILayout.BeginHorizontal();
                    drawDistance();
                    EditorGUILayout.EndHorizontal();*/

                    /*if (_adjustSphere ? GUILayout.Button("Complete") : GUILayout.Button("Adjust"))
                    {
                        _adjustSphere = !_adjustSphere;
                    }

                    drawCustomOrigin();*/
                    break;
                /*case (int)ExplosionRule.SingleAxis:
                    helpText = "This type of explosion spreads parts according to a single axis.";
                    break;
                case (int)ExplosionRule.SingleVector:
                    helpText = "This type of explosion spreads parts according to a single vector.";
                    break;
                case (int)ExplosionRule.VectorWise:
                    helpText = "This type of explosion spreads parts respectively to a vectors that you choose.";
                    break;*/
            }


            EditorGUILayout.BeginHorizontal();
            drawDistance();
            GUILayout.FlexibleSpace();
            _customOrigin = drawCustomOrigin();
            EditorGUILayout.EndHorizontal();
            if (_customOrigin)
            {
                drawCustomOriginsPosition();
            }

            EditorGUILayout.HelpBox(helpText, MessageType.Info);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns true if origin is custom</returns>
        private bool drawCustomOrigin()
        {
            if (_selectedPartIndex >= 0)
            {
                if (GUILayout.Button(GUIContent.none, _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueUseCustomOrigin ? _uniqueButtonStyle : _commonButtonStyle, new[] { GUILayout.Width(20), GUILayout.Height(20) }))
                {
                    serializedObject.FindProperty("_parts").GetArrayElementAtIndex(_selectedPartIndex).FindPropertyRelative("_currentExplosionSettings").GetArrayElementAtIndex(_modelExploder.CurrentExplosionSettingsBundle).FindPropertyRelative("UniqueUseCustomOrigin").boolValue = !serializedObject.FindProperty("_parts").GetArrayElementAtIndex(_selectedPartIndex).FindPropertyRelative("_currentExplosionSettings").GetArrayElementAtIndex(_modelExploder.CurrentExplosionSettingsBundle).FindPropertyRelative("UniqueUseCustomOrigin").boolValue;
                }
            }

            bool useCustomOrigin;
            if (_selectedPartIndex >= 0)
            {
                useCustomOrigin = _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueUseCustomOrigin ?
                    _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.UseCustomOrigin :
                    _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.UseCustomOrigin;
            }
            else
            {
                useCustomOrigin = _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.UseCustomOrigin;
            }

            bool newUseCustomOrigin = EditorGUILayout.Toggle("Use Custom Origin: ", useCustomOrigin, _guiSkin.GetStyle("CheckBox"));
            if (newUseCustomOrigin != useCustomOrigin)
            {
                Undo.RecordObject(_modelExploder, "New Custom Origin Settings Applied");

                if (_selectedPartIndex >= 0)
                {
                    if (_modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueUseCustomOrigin)
                    {
                        _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.UseCustomOrigin = newUseCustomOrigin;
                    }
                    else
                    {
                        _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.UseCustomOrigin = newUseCustomOrigin;
                    }
                }
                else
                {
                    _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.UseCustomOrigin = newUseCustomOrigin;
                }
            }

            return newUseCustomOrigin;
        }

        private void drawCustomOriginsPosition()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            Vector3 centerPosition;

            if (_selectedPartIndex >= 0)
            {
                centerPosition = _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueCustomOrigin ?
                    _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.CustomOrigin.Position.Extract() :
                    _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.CustomOrigin.Position.Extract();
            }
            else
            {
                centerPosition = _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.CustomOrigin.Position.Extract();
            }

            if (_selectedPartIndex >= 0)
            {
                if (GUILayout.Button(GUIContent.none, _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueCustomOrigin ? _uniqueButtonStyle : _commonButtonStyle, new[] { GUILayout.Width(20), GUILayout.Height(20) }))
                {
                    serializedObject.FindProperty("_parts").GetArrayElementAtIndex(_selectedPartIndex).FindPropertyRelative("_currentExplosionSettings").GetArrayElementAtIndex(_modelExploder.CurrentExplosionSettingsBundle).FindPropertyRelative("UniqueCustomOrigin").boolValue = !serializedObject.FindProperty("_parts").GetArrayElementAtIndex(_selectedPartIndex).FindPropertyRelative("_currentExplosionSettings").GetArrayElementAtIndex(_modelExploder.CurrentExplosionSettingsBundle).FindPropertyRelative("UniqueCustomOrigin").boolValue;
                }
            }

            float x = centerPosition.x;
            float y = centerPosition.y;
            float z = centerPosition.z;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Relative Position:", GUILayout.Width(104f));
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("X", GUILayout.Width(12f));
            x = EditorGUILayout.FloatField(x, _guiSkin.GetStyle("TextField"), GUILayout.Width(52f));
            EditorGUILayout.LabelField("Y", GUILayout.Width(12f));
            y = EditorGUILayout.FloatField(y, _guiSkin.GetStyle("TextField"), GUILayout.Width(52f));
            EditorGUILayout.LabelField("Z", GUILayout.Width(12f));
            z = EditorGUILayout.FloatField(z, _guiSkin.GetStyle("TextField"), GUILayout.Width(52f));
            GUILayout.EndHorizontal();
            //Vector3 newCenterPosition = EditorGUILayout.Vector3Field("Relative position: ", centerPosition);
            Vector3 newCenterPosition = new Vector3(x, y, z);

            if (newCenterPosition != centerPosition)
            {
                Undo.RecordObject(target, "New Center Position Setted");

                if (_selectedPartIndex >= 0)
                {
                    if (_modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueCustomOrigin)
                    {
                        _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.CustomOrigin.Position.Convert(newCenterPosition);
                    }
                    else
                    {
                        _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.CustomOrigin.Position.Convert(newCenterPosition);
                    }
                }
                else
                {
                    _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.CustomOrigin.Position.Convert(newCenterPosition);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void drawDistance() {
            float distance;

            if (_selectedPartIndex >= 0)
            {
                distance = _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueDistance ?
                    _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.Distance :
                    _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.Distance;
            }
            else
            {
                distance = _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.Distance;
            }

            if (_selectedPartIndex >= 0)
            {
                if (GUILayout.Button(GUIContent.none, _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueDistance ? _uniqueButtonStyle : _commonButtonStyle, new[] { GUILayout.Width(20), GUILayout.Height(20) }))
                {
                    serializedObject.FindProperty("_parts").GetArrayElementAtIndex(_selectedPartIndex).FindPropertyRelative("_currentExplosionSettings").GetArrayElementAtIndex(_modelExploder.CurrentExplosionSettingsBundle).FindPropertyRelative("UniqueDistance").boolValue = !serializedObject.FindProperty("_parts").GetArrayElementAtIndex(_selectedPartIndex).FindPropertyRelative("_currentExplosionSettings").GetArrayElementAtIndex(_modelExploder.CurrentExplosionSettingsBundle).FindPropertyRelative("UniqueDistance").boolValue;
                }
            }

            EditorGUILayout.LabelField(_distanceContent, Utilities.RichTextStyle, GUILayout.Width(70f));

            float oldDistance = distance;
            float newDistance = EditorGUILayout.FloatField(distance, _guiSkin.GetStyle("TextField"), GUILayout.Width(70f));

            if (oldDistance != newDistance)
            {
                Undo.RecordObject(target, "New Distance Setted");

                if (_selectedPartIndex >= 0)
                {
                    if (_modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueDistance)
                    {
                        _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.Distance = newDistance;
                    }
                    else
                    {
                        _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.Distance = newDistance;
                    }
                }
                else
                {
                    _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.Distance = newDistance;
                }

                _modelExploder.UpdateInitialTransforms(1);
                _modelExploder.RecalculateAllExplosionTargets(_modelExploder.InitialStateIndex, 1);
                if (_modelExploder.SelectedStateIndex > 1)
                {
                    _modelExploder.Save(_modelExploder.SelectedStateIndex);
                }
            }
        }

        private CustomTransform drawCenter()
        {
            CustomTransform result = new CustomTransform(Vector3.zero, Vector3.zero, Vector3.zero);

            bool originIsCustom;
            if (_selectedPartIndex >= 0)
            {
                originIsCustom = _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueUseCustomOrigin ?
                    _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.UseCustomOrigin :
                    _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.UseCustomOrigin;
            }
            else
            {
                originIsCustom = _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.UseCustomOrigin;
            }

            if (originIsCustom)
            {
                if (_selectedPartIndex >= 0)
                {
                    result = _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueCustomOrigin ?
                        _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.CustomOrigin :
                        _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.CustomOrigin;
                }
                else
                {
                    result = _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.CustomOrigin;
                }
            }


            Handles.color = Color.red;

            Vector3 centerPosition = result.Position.Extract();

            MyHandles.DragHandleResult dhResult;
            bool altPressed = false;
            bool ctrlPressed = false;
            Vector3 newPosition = MyHandles.DragHandle(_modelExploder.transform.TransformPoint(centerPosition), 0.2f, Handles.SphereCap, /*new Color(0.68f, 0.274912f, 0.7105f, 0.3f)*/Color.red, out dhResult, out altPressed, out ctrlPressed);

            switch (dhResult)
            {
                case MyHandles.DragHandleResult.LMBDrag:
                    if (originIsCustom && !altPressed)
                    {
                        if (_selectedPartIndex >= 0)
                        {
                            if (_modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueCustomOrigin)
                            {
                                _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.CustomOrigin.Position.Convert(_modelExploder.Origin.InverseTransformPoint(newPosition));
                            }
                            else
                            {
                                _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.CustomOrigin.Position.Convert(_modelExploder.Origin.InverseTransformPoint(newPosition));
                            }
                        }
                        else
                        {
                            _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.CustomOrigin.Position.Convert(_modelExploder.Origin.InverseTransformPoint(newPosition));
                        }
                    }
                    break;
                case MyHandles.DragHandleResult.ScrollWheel:
                    break;
            }

            return result;
        }

        private void drawSphere()
        {
            Vector3 spherePosition = drawCenter().Position.Extract();

            float distance;

            if (_selectedPartIndex >= 0)
            {
                distance = _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueDistance ?
                    _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.Distance :
                    _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.Distance;
            }
            else
            {
                distance = _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.Distance;
            }
            
            Handles.color = Color.white;

            EditorGUI.BeginChangeCheck();
            float newDistance = Handles.RadiusHandle(Quaternion.identity, _modelExploder.transform.TransformPoint(spherePosition), distance / 2f) * 2f;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Radius of the Sphere");

                if (_selectedPartIndex >= 0)
                {
                    if (_modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].UniqueDistance)
                    {
                        _modelExploder.Parts[_selectedPartIndex].CurrentExplosionSettings[_modelExploder.CurrentExplosionSettingsBundle].Uniques.Distance = newDistance;
                    }
                    else
                    {
                        _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.Distance = newDistance;
                    }
                }
                else
                {
                    _modelExploder.ExplosionSettingsBundles[_modelExploder.CurrentExplosionSettingsBundle].DefaultSettings.Distance = newDistance;
                }

                _modelExploder.UpdateInitialTransforms(1);
                _modelExploder.RecalculateAllExplosionTargets(_modelExploder.InitialStateIndex, 1);
                if (_modelExploder.SelectedStateIndex > 1)
                {
                    _modelExploder.Save(_modelExploder.SelectedStateIndex);
                }
            }
        }
    }
}