using UnityEditor;
using UnityEngine;
using System;
using UnityEditorInternal;
using System.Collections.Generic;

namespace Meta.Tools.Editor
{
    public partial class ModelExploderEditor : UnityEditor.Editor
    {
        private GUIContent _numberOfGroupsContent = new GUIContent();
        private GUIContent _numberOfActiveGroupsContent = new GUIContent();
        private Texture _centeringTexture;
        private List<Vector2> _rombsPositions = new List<Vector2>();
        private Vector2 _torusPosition;

        private int partsCount = 0;

        private void drawSummary()
        {

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("<i><b>    Summary:</b></i>", Utilities.TitleStyle);
            GUILayout.Space(5);

            EditorGUI.indentLevel++;

            /* Explosing info about groups count */

            int groupsCount = _modelExploder.Parts.Count;
            string groupsCountColor = Utilities.GoodResultColor.Hex();
            if (groupsCount == 0)
            {
                _numberOfGroupsContent.tooltip = "There are no active groups out there. ModelExploder will have nothing to explode.";
                groupsCountColor = Utilities.AverageResultColor.Hex();
            }
            else if (groupsCount == 1)
            {
                _numberOfGroupsContent.tooltip = "There is only one group is going to be animated.";
                groupsCountColor = Utilities.BadResultColor.Hex();
            }
            _numberOfGroupsContent.text = "Number of parts: <color=" + groupsCountColor + "><b>" + groupsCount + "</b></color>";
            EditorGUILayout.LabelField(_numberOfGroupsContent, Utilities.RichTextStyle);

            /* Explosing info about active groups count */

            int activeGroupsCount = _modelExploder.GetActiveGroupsCount();
            string activeGroupsCountColor = Utilities.GoodResultColor.Hex();

            if (activeGroupsCount == 0)
            {
                _numberOfActiveGroupsContent.tooltip = "There are no active groups out there. ModelExploder will have nothing to explode.";
                activeGroupsCountColor = Utilities.AverageResultColor.Hex();
            }
            else if (activeGroupsCount == 1)
            {
                _numberOfActiveGroupsContent.tooltip = "There is only one group is going to be animated.";
                activeGroupsCountColor = Utilities.BadResultColor.Hex();
            }
            _numberOfActiveGroupsContent.text = "Active groups: <color=" + activeGroupsCountColor + "><b>" + activeGroupsCount + "</b></color>";
            //EditorGUILayout.LabelField(_numberOfActiveGroupsContent, Utilities.RichTextStyle);

            #region Summary 3D-View region

            Rect lastRect = GUILayoutUtility.GetLastRect();
            
            float textureWidth = EditorGUIUtility.currentViewWidth - 80f;
            float textureHeight = 200f;
            if (EditorScreenShooter.Resolution.x != textureWidth)
            {
                screenshootCameraChanged = true;
            }
            EditorScreenShooter.Resolution = new Vector2(textureWidth, textureHeight);
            Rect textureRect = new Rect(lastRect.x + 20, lastRect.y + EditorGUIUtility.singleLineHeight, textureWidth, textureHeight);

            if (Event.current.type == EventType.MouseUp && textureRect.Contains(Event.current.mousePosition))
            {
                Utilities.DeselectAllInSceneView();
                serializedObject.FindProperty("CurrentlySelectedPart").intValue = -1;
                _modelExploder.CurrentlySelectedPart = -1;
                screenshootCameraChanged = true;
                updateScreenshoots();
            }
               

            GUI.Label(textureRect, _centeringTexture);

            Vector2 basePosition = new Vector2(lastRect.x + 20f, lastRect.y + EditorGUIUtility.singleLineHeight);


            float buttonSize = 15 - 10 * serializedObject.FindProperty("MinCameraPaddingMultiplier").floatValue;

            int id = 0;
            int pushedObj = -1;

            GUI.Label(new Rect(textureRect.center.x - buttonSize * 1.5f - textureRect.width / 175 + _torusPosition.x, // new Rect(basePosition.x + 143 - (0.5f - pos.x) * 387+buttonSize* serializedObject.FindProperty("MinCameraPaddingMultiplier").floatValue, 
                    textureRect.center.y - buttonSize * 1.5f - _torusPosition.y * textureRect.height,
                    buttonSize * 3, buttonSize * 3), "", Utilities.ExploderSkin.label);

            foreach (Vector2 pos in _rombsPositions)
            {
                if (GUI.Button(new Rect(textureRect.center.x - buttonSize / 2 - textureRect.width / 175 + pos.x, // new Rect(basePosition.x + 143 - (0.5f - pos.x) * 387+buttonSize* serializedObject.FindProperty("MinCameraPaddingMultiplier").floatValue, 
                    textureRect.center.y - buttonSize / 2 - pos.y * textureRect.height,
                    buttonSize, buttonSize), "", Utilities.ExploderSkin.button))
                {
                    pushedObj = id;
                    GameObject[] gos = _modelExploder.Grouper.GetGameObjectsFromPart(_modelExploder.Grouper.Parts[id]);
                    Utilities.DeselectAllInSceneView();
                    Utilities.SelectGameObjectsInSceneView(gos, Utilities.SceneViewSelectionType.Variant3);

                    serializedObject.FindProperty("CurrentlySelectedPart").intValue = id;
                    _modelExploder.CurrentlySelectedPart = id;
                }
                id++;
            }

            if (pushedObj > -1)
            {
                screenshootCameraChanged = true;
                updateScreenshoots();
            }

            GUILayout.Space(200);

            GUILayout.BeginHorizontal();

            GUILayout.Label("  Projection:", GUILayout.Width(72f));
            CameraAxisSide oldCameraAxisSide = (CameraAxisSide)serializedObject.FindProperty("TargetCameraAxisSide").enumValueIndex;
            serializedObject.FindProperty("TargetCameraAxisSide").enumValueIndex = (int)(CameraAxisSide)EditorGUILayout.EnumPopup((CameraAxisSide)serializedObject.FindProperty("TargetCameraAxisSide").enumValueIndex, _guiSkin.GetStyle("DropDown"), GUILayout.Width(120f));
            if (oldCameraAxisSide != (CameraAxisSide)serializedObject.FindProperty("TargetCameraAxisSide").enumValueIndex)
            {
                screenshootCameraChanged = true;
                updateScreenshoots();
            }

            GUILayout.EndHorizontal();

            /*EditorGUILayout.LabelField("<b>  Projection Controls</b>", Utilities.RichTextStyle);
            lastRect = GUILayoutUtility.GetLastRect();
            serializedObject.FindProperty("ProjectionControlsObserved").boolValue = EditorGUI.Foldout(new Rect(lastRect.x + 6, lastRect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), serializedObject.FindProperty("ProjectionControlsObserved").boolValue, GUIContent.none);

            if (serializedObject.FindProperty("ProjectionControlsObserved").boolValue) {
                bool oldGridEnabled = serializedObject.FindProperty("GridEnabled").boolValue;
                serializedObject.FindProperty("GridEnabled").boolValue = EditorGUILayout.Toggle("Enable Grid:", serializedObject.FindProperty("GridEnabled").boolValue);

                Color oldGridColor = serializedObject.FindProperty("GridColor").colorValue;
                Color oldGridBackgroundColor = serializedObject.FindProperty("GridBackgroundColor").colorValue;
                float oldGridDimension = serializedObject.FindProperty("GridDimension").floatValue;

                if (_modelExploder.GridEnabled) {
                    serializedObject.FindProperty("GridColor").colorValue = EditorGUILayout.ColorField("Grid Color:", serializedObject.FindProperty("GridColor").colorValue);
                    serializedObject.FindProperty("GridBackgroundColor").colorValue = EditorGUILayout.ColorField("Background Color:", serializedObject.FindProperty("GridBackgroundColor").colorValue);

                    serializedObject.FindProperty("GridDimension").floatValue = EditorGUILayout.FloatField("Grid Dimension:", serializedObject.FindProperty("GridDimension").floatValue);
                }

                CameraAxisSide oldCameraAxisSide = (CameraAxisSide)serializedObject.FindProperty("TargetCameraAxisSide").enumValueIndex;
                serializedObject.FindProperty("TargetCameraAxisSide").enumValueIndex = (int)(CameraAxisSide)EditorGUILayout.EnumPopup("Camera view:", (CameraAxisSide)serializedObject.FindProperty("TargetCameraAxisSide").enumValueIndex);

                ProjectionAplliedEffect oldProjectionAplliedEffect = (ProjectionAplliedEffect)serializedObject.FindProperty("ProjectionAppliedEffect").enumValueIndex;
                serializedObject.FindProperty("ProjectionAppliedEffect").enumValueIndex = (int)(ProjectionAplliedEffect)EditorGUILayout.EnumPopup("Applied material:", (ProjectionAplliedEffect)serializedObject.FindProperty("ProjectionAppliedEffect").enumValueIndex);

                Color oldAppliedMaterialColor = serializedObject.FindProperty("AppliedMaterialColor").colorValue;
                serializedObject.FindProperty("AppliedMaterialColor").colorValue = EditorGUILayout.ColorField("Material color:", serializedObject.FindProperty("AppliedMaterialColor").colorValue);

                EditorGUILayout.Space();
                float oldMinCameraPaddingMultiplier = serializedObject.FindProperty("MinCameraPaddingMultiplier").floatValue;
                serializedObject.FindProperty("MinCameraPaddingMultiplier").floatValue = EditorGUILayout.Slider("Projection padding:", serializedObject.FindProperty("MinCameraPaddingMultiplier").floatValue, 0f, 1f);

                if (oldGridEnabled != serializedObject.FindProperty("GridEnabled").boolValue || oldGridColor != serializedObject.FindProperty("GridColor").colorValue ||
                    oldGridBackgroundColor != serializedObject.FindProperty("GridBackgroundColor").colorValue || oldCameraAxisSide != (CameraAxisSide)serializedObject.FindProperty("TargetCameraAxisSide").enumValueIndex ||
                    oldProjectionAplliedEffect != (ProjectionAplliedEffect)serializedObject.FindProperty("ProjectionAppliedEffect").enumValueIndex || oldAppliedMaterialColor != serializedObject.FindProperty("AppliedMaterialColor").colorValue ||
                    oldMinCameraPaddingMultiplier != serializedObject.FindProperty("MinCameraPaddingMultiplier").floatValue || oldGridDimension != serializedObject.FindProperty("GridDimension").floatValue)
                {
                    screenshootCameraChanged = true;
                    updateScreenshoots();
                }
            }*/

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            #endregion Summary 3D-View region
            EditorGUI.indentLevel--;
        }

        private void updateScreenshoots()
        {
            if (screenshootCameraChanged)
            {

            }
            else if (_modelExploder.Parts.Count == 0 || partsCount == _modelExploder.Parts.Count)
            {
                return;
            }
            partsCount = _modelExploder.Parts.Count;
            screenshootCameraChanged = false;

            _modelExploder.RecalculateAABB();

            _screenShooter.GridColor = serializedObject.FindProperty("GridColor").colorValue;
            _screenShooter.GridBackgroundColor = serializedObject.FindProperty("GridBackgroundColor").colorValue;
            _screenShooter.TargetCameraAxisSide = (CameraAxisSide)serializedObject.FindProperty("TargetCameraAxisSide").enumValueIndex;
            _screenShooter.ProjectionAppliedEffect = (ProjectionAplliedEffect)serializedObject.FindProperty("ProjectionAppliedEffect").enumValueIndex;
            _screenShooter.AppliedMaterialColor = serializedObject.FindProperty("AppliedMaterialColor").colorValue;
            _screenShooter.cameraViewFinder.MinCameraPaddingMultiplier = serializedObject.FindProperty("MinCameraPaddingMultiplier").floatValue;
            _screenShooter.GridDimension = serializedObject.FindProperty("GridDimension").floatValue;

            _centeringTexture = _screenShooter.TakeScreenShoot(_modelExploder,
                new Color(174f / 255f, 169f / 255f, 169f / 255f, 117f / 255f), serializedObject.FindProperty("GridEnabled").boolValue,
                (Camera camera) =>
                {
                    Vector3 pos;
                    _rombsPositions.Clear();
                    for (int i = 0; i < _modelExploder.Parts.Count; i++)
                    {
                        pos = _modelExploder.GetPartsOrigin(_modelExploder.Parts[i]).position;
                        _rombsPositions.Add(FromSpaceToSummary(pos, camera));
                    }
                    pos = _modelExploder.OriginPosition;
                    //_screenShooter.PlaceTorus(_modelExploder.OriginPosition, OrtoLookAt(_modelExploder.AABB.center, camera.transform.position), Vector3.one * 0.2f * camera.orthographicSize * (1.2f - serializedObject.FindProperty("MinCameraPaddingMultiplier").floatValue), Color.red);
                    _torusPosition = FromSpaceToSummary(pos, camera);
                }, true, 60, CameraClearFlags.Color);
        }

        private Vector2 FromSpaceToSummary(Vector3 spacePosition, Camera camera)
        {
            return new Vector2((camera.WorldToScreenPoint(spacePosition).x - camera.pixelWidth / 2) * 0.37f, camera.WorldToViewportPoint(spacePosition).y - 0.5f);
        }

        private Quaternion OrtoLookAt(Vector3 objPos, Vector3 camPos)
        {
            Vector3 v = objPos - camPos;

            if (!((v.x == 0f ? true : false) && (v.z == 0f ? true : false)))
            {
                if (v.x == 0f ? true : false)
                {
                    return Quaternion.Euler(new Vector3(90, 0, 0));
                }
                return Quaternion.Euler(new Vector3(0, 0, 90));
            }
            return Quaternion.identity;
        }
    }
}
