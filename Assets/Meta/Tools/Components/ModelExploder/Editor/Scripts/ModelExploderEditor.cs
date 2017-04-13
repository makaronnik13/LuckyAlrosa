using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    [CustomEditor(typeof(ModelExploder))]
    public partial class ModelExploderEditor : UnityEditor.Editor
    {
        [NonSerialized]
        private bool _adjustSphere;
        [NonSerialized]
        private bool _adjustCenter;

        private ModelExploder _modelExploder;
        private EditorScreenShooter _screenShooter;
        private bool _autoExplosionVisible = true;
        private bool _previewVisible;

        private GUISkin _guiSkin;

        private bool screenshootCameraChanged;

        private void OnEnable()
        {
            _guiSkin = (GUISkin)Resources.Load("GUISkins/ModelExploderGUISkin");
            _modelExploder = target as ModelExploder;
            _screenShooter = new EditorScreenShooter();

            initializingStyleAndContents();
            initializeGroupsList();

            _modelExploder.Initialize();

            if (_modelExploder && serializedObject.FindProperty("CurrentlySelectedPart").intValue!=-1)
            {
                GameObject[] gos = _modelExploder.Grouper.GetGameObjectsFromPart(_modelExploder.Grouper.Parts[serializedObject.FindProperty("CurrentlySelectedPart").intValue]);
                Utilities.DeselectAllInSceneView();
                Utilities.SelectGameObjectsInSceneView(gos, Utilities.SceneViewSelectionType.Variant3);
            }
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            Grouper oldGrouper = _modelExploder.Grouper;

            /*_modelExploder.Grouper = EditorGUILayout.ObjectField(
                "Grouper: ", 
                _modelExploder.Grouper, 
                typeof(Grouper), 
                true
                ) as Grouper;*/

            if (_modelExploder.Grouper == null) {
                return;
            }

            if (oldGrouper != _modelExploder.Grouper) {
                updateScreenshoots();
            }

            drawSummary();

            GUISkin tempGUI = GUI.skin;
            GUI.skin = _guiSkin;

            #region States View
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("<b><size=18>States Collection</size></b>", Utilities.RichTextStyle);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(12f);

            List<string> availableStatesNames = _modelExploder.ExplosionSettingsBundlesNames;
            List<int> availableStatesIndexes = new List<int>();
            int j = 2;
            for (int i = 0; i < availableStatesNames.Count; i++)
            {
                availableStatesIndexes.Add(j);
                j++;
            }

            //EditorGUILayout.LabelField("Choose state:", Utilities.RichTextStyle, GUILayout.Width(80f));
            Rect lastRect = GUILayoutUtility.GetLastRect();
            int numberOfstates = availableStatesNames.Count;
            if (numberOfstates > 5)
            {
                numberOfstates = 5;
            }
            float totalHeigth = numberOfstates * ModelExploderEditorGUI.StateHeight;

            Rect targetRect = new Rect(0f, lastRect.y, EditorGUIUtility.currentViewWidth - 40f, totalHeigth);
            string nameEdited = null;
            int nameEditedIndex = -1;
            int saveButtonClicked = -1;
            int newSelectedStateIndex = ModelExploderEditorGUI.StatesList(targetRect, _modelExploder.SelectedStateIndex, availableStatesNames.ToArray(), availableStatesIndexes.ToArray(), _guiSkin, out nameEdited, out nameEditedIndex, out saveButtonClicked);

            if (nameEditedIndex >= 0)
            {
                Undo.RecordObject(_modelExploder, "State's Name Changed");
                _modelExploder.ExplosionSettingsBundles[nameEditedIndex].Name = nameEdited;
                //ChangeStatesNameWindow.Init(_modelExploder.ExplosionSettingsBundles[editButtonClicked], _modelExploder, serializedObject);
            }
            if (saveButtonClicked >= 0)
            {
                Undo.RecordObject(_modelExploder, "State Saved");
                _modelExploder.Save(saveButtonClicked);
                serializedObject.Update();
            }

            if (newSelectedStateIndex != _modelExploder.SelectedStateIndex)
            {
                Undo.RecordObject(_modelExploder, "New State Selected");
                _modelExploder.SelectedStateIndex = newSelectedStateIndex;
                _modelExploder.Explode(newSelectedStateIndex, newSelectedStateIndex, 0f);

                screenshootCameraChanged = true;
            }

            float buttonsWidth = 16f;
            Rect resetButtonRect = new Rect(16f, targetRect.y + targetRect.height + buttonsWidth + 8f, 110f, buttonsWidth);
            if (GUI.Button(resetButtonRect, new GUIContent() { text = "Reset Model", tooltip = "Default State" }))
            {
                _modelExploder.SetDefault();
                serializedObject.Update();

                screenshootCameraChanged = true;
            }

            /*Rect defaultButtonRect = new Rect(EditorGUIUtility.currentViewWidth - buttonsWidth * 3f - 16f, targetRect.y + targetRect.height + buttonsWidth + 8f, buttonsWidth, buttonsWidth);
            if (GUI.Button(defaultButtonRect, new GUIContent() { tooltip = "Default State" }, _guiSkin.GetStyle("StatesDefaultButton")))
            {
                _modelExploder.SetDefault();
                serializedObject.Update();

                screenshootCameraChanged = true;
            }*/
            Rect deleteButtonRect = new Rect(EditorGUIUtility.currentViewWidth - buttonsWidth * 3f - 16f, targetRect.y + targetRect.height + buttonsWidth + 8f, buttonsWidth, buttonsWidth);
            if (GUI.Button(deleteButtonRect, new GUIContent() { tooltip = "Remove State" }, _guiSkin.GetStyle("StatesDeleteButton")))
            {
                if (newSelectedStateIndex > 1)
                {
                    Undo.RecordObject(_modelExploder, "State Deleted");
                    _modelExploder.RemoveExplosionSettingsBundle(newSelectedStateIndex);
                    serializedObject.Update();
                }
            }
            Rect saveButtonRect = new Rect(EditorGUIUtility.currentViewWidth - buttonsWidth * 4.5f - 16f, targetRect.y + targetRect.height + buttonsWidth + 8f, buttonsWidth, buttonsWidth);
            if (GUI.Button(saveButtonRect, new GUIContent() { tooltip = "Save State" }, _guiSkin.GetStyle("StatesSaveToCurrentButtonSelected")))
            {
                if (newSelectedStateIndex > 1)
                {
                    Undo.RecordObject(_modelExploder, "State Saved");
                    _modelExploder.Save(newSelectedStateIndex);
                    serializedObject.Update();
                }
            }
            Rect saveToNewButtonRect = new Rect(EditorGUIUtility.currentViewWidth - buttonsWidth * 6f - 16f, targetRect.y + targetRect.height + buttonsWidth + 8f, buttonsWidth, buttonsWidth);
            if (GUI.Button(saveToNewButtonRect, new GUIContent() { tooltip = "Save To New State" }, _guiSkin.GetStyle("StatesSaveToNewButton")))
            {
                Undo.RecordObject(_modelExploder, "Saved to new state");
                _modelExploder.SaveToNew();
                _modelExploder.SelectedStateIndex = _modelExploder.ExplosionSettingsBundles.Count - 1;
                serializedObject.Update();
            }

            GUILayout.Space(36f);


            GUILayout.Box(GUIContent.none, _guiSkin.GetStyle("Splitter"), new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.currentViewWidth - 40f), GUILayout.Height(1f) } );


            //GUILayout.Space(120f);

            /*GUILayout.Space(6f);

            if (_modelExploder.SelectedStateIndex > 1)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("<b><size=15><i>" + _modelExploder.ExplosionSettingsBundles[_modelExploder.SelectedStateIndex].Name + "</i></size></b>", Utilities.RichTextStyle);

                EditorGUILayout.EndHorizontal();
            }

            if (newSelectedStateIndex > 1)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Edit name:", Utilities.RichTextStyle, GUILayout.Width(80f));
                _modelExploder.ExplosionSettingsBundles[newSelectedStateIndex].Name = EditorGUILayout.TextField(_modelExploder.ExplosionSettingsBundles[newSelectedStateIndex].Name, _guiSkin.GetStyle("TextFieldStyle"));

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(6f);*/



            /*GUILayout.Space(6f);

            float minButtonWidth = 140f;
            float buttonsHeight = 50f;

            EditorGUILayout.BeginHorizontal();

            if (availableStatesIndexes.Count > 0 && newSelectedStateIndex > 1)
            {
                if (GUILayout.Button("Save to current state", new[] { GUILayout.Height(buttonsHeight), GUILayout.MinWidth(minButtonWidth) }))
                {
                    Undo.RecordObject(_modelExploder, "Saved to current state");
                    _modelExploder.Save(newSelectedStateIndex);
                    serializedObject.Update();
                }
                EditorGUILayout.LabelField("", GUILayout.Width(6f));
                if (GUILayout.Button("Save to new state", new[] { GUILayout.Height(buttonsHeight), GUILayout.MinWidth(minButtonWidth) }))
                {
                    //serializedObject.ApplyModifiedProperties();
                    Undo.RecordObject(_modelExploder, "Saved to new state");
                    _modelExploder.SaveToNew();
                    _modelExploder.SelectedStateIndex = _modelExploder.ExplosionSettingsBundles.Count - 1;
                    serializedObject.Update();
                }
            }
            else
            {
                if (GUILayout.Button("Save to new state", GUILayout.Height(buttonsHeight)))
                {
                    //serializedObject.ApplyModifiedProperties();
                    Undo.RecordObject(_modelExploder, "Saved to new state");
                    _modelExploder.SaveToNew();
                    _modelExploder.SelectedStateIndex = _modelExploder.ExplosionSettingsBundles.Count - 1;
                    serializedObject.Update();
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (availableStatesIndexes.Count > 0 && newSelectedStateIndex > 1)
            {
                if (GUILayout.Button("Set default state", new[] { GUILayout.Height(buttonsHeight), GUILayout.MinWidth(minButtonWidth) }))
                {
                    _modelExploder.SetDefault();
                    serializedObject.Update();

                    screenshootCameraChanged = true;
                }
                EditorGUILayout.LabelField("", GUILayout.Width(6f));
                if (GUILayout.Button("Delete current state", new[] { GUILayout.Height(buttonsHeight), GUILayout.MinWidth(minButtonWidth) }))
                {
                    //serializedObject.ApplyModifiedProperties();
                    Undo.RecordObject(_modelExploder, "State Deleted");
                    _modelExploder.RemoveExplosionSettingsBundle(newSelectedStateIndex);
                    _modelExploder.SelectedStateIndex = _modelExploder.ExplosionSettingsBundles.Count - 1;
                    serializedObject.Update();
                }
            }
            else
            {
                if (GUILayout.Button("Set default state", GUILayout.Height(buttonsHeight)))
                {
                    _modelExploder.Explode(0, 0, 0f);
                    serializedObject.Update();

                    screenshootCameraChanged = true;
                }
            }

            EditorGUILayout.EndHorizontal();*/

            #endregion States View

            #region Explosion Settings View

            GUILayout.Space(10f);
            EditorGUILayout.LabelField("<b><size=18> Auto-Explosion</size></b>", Utilities.RichTextStyle);
            GUILayout.Space(6f);

            lastRect = GUILayoutUtility.GetLastRect();
            _autoExplosionVisible = EditorGUI.Foldout(
                new Rect(lastRect.x + 2, lastRect.y - 12, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight),
                _autoExplosionVisible, GUIContent.none);

            int newInitialStateIndex;
            List<string> names;
            List<int> indexes;
            if (_autoExplosionVisible)
            {
                drawingExplosionSettings();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Choose state from wich to explode:");

                names = _modelExploder.ExplosionSettingsBundlesNames;
                names.Insert(0, "Default");
                indexes = new List<int>();
                indexes.Add(0);
                j = 2;
                for (int i = 0; i < names.Count; i++)
                {
                    indexes.Add(j);
                    j++;
                }
                newInitialStateIndex = EditorGUILayout.IntPopup(_modelExploder.InitialStateIndex, names.ToArray(), indexes.ToArray(), _guiSkin.GetStyle("DropDown"));
                if (newInitialStateIndex != _modelExploder.InitialStateIndex)
                {
                    Undo.RecordObject(_modelExploder, "New Initial State Choosed");
                    _modelExploder.InitialStateIndex = newInitialStateIndex;
                }

                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Explode", GUILayout.Width(120f), GUILayout.Height(48f)))
                {
                    serializedObject.ApplyModifiedProperties();

                    _modelExploder.UpdateInitialTransforms(1);
                    _modelExploder.RecalculateAllExplosionTargets(newInitialStateIndex, 1);
                    serializedObject.Update();

                    updateScreenshoots();
                }
                //drawModelStructure();
            }

            #endregion Explosion Settings View

            #region Explosion Preview

            GUILayout.Space(4f);
            EditorGUILayout.LabelField("<b><size=18> Preview</size></b>", Utilities.RichTextStyle);
            GUILayout.Space(6f);

            lastRect = GUILayoutUtility.GetLastRect();
            _previewVisible = EditorGUI.Foldout(
                new Rect(lastRect.x + 2, lastRect.y - 12, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight),
                _previewVisible, GUIContent.none);

            if (_previewVisible)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("<b><size=16>State 1</size></b>", Utilities.RichTextStyle, GUILayout.MinWidth(80f));
                EditorGUILayout.LabelField("", GUILayout.Width(80f));
                EditorGUILayout.LabelField("<b><size=16>State 2</size></b>", Utilities.RichTextStyle, GUILayout.MinWidth(80f));

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(6f);

                EditorGUILayout.BeginHorizontal();

                names = _modelExploder.ExplosionSettingsBundlesNames;
                indexes = new List<int>();
                indexes.Add(0);
                j = 2;
                for (int i = 0; i < names.Count; i++)
                {
                    indexes.Add(j);
                    j++;
                }
                names.Insert(0, "Default");
                newInitialStateIndex = EditorGUILayout.IntPopup(_modelExploder.InitialStateIndex, names.ToArray(), indexes.ToArray(), _guiSkin.GetStyle("DropDown"));
                if (newInitialStateIndex != _modelExploder.InitialStateIndex)
                {
                    Undo.RecordObject(_modelExploder, "New Initial State Choosed");
                    _modelExploder.InitialStateIndex = newInitialStateIndex;
                }
                EditorGUILayout.LabelField("", GUILayout.Width(80f));
                names = _modelExploder.ExplosionSettingsBundlesNames;
                indexes = new List<int>();
                indexes.Add(0);
                j = 2;
                for (int i = 0; i < names.Count; i++)
                {
                    indexes.Add(j);
                    j++;
                }
                names.Insert(0, "Default");
                int newFinalStateIndex = EditorGUILayout.IntPopup(_modelExploder.FinalStateIndex, names.ToArray(), indexes.ToArray(), _guiSkin.GetStyle("DropDown"));
                //int newFinalStateIndex = EditorGUILayout.Popup(_modelExploder.FinalStateIndex, _modelExploder.ExplosionSettingsBundlesNames);
                if (newFinalStateIndex != _modelExploder.FinalStateIndex)
                {
                    Undo.RecordObject(_modelExploder, "New Final State Choosed");
                    _modelExploder.FinalStateIndex = newFinalStateIndex;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                _modelExploder.CurrentAnimationRatio = EditorGUILayout.Slider(_modelExploder.CurrentAnimationRatio, 0f, 1f);
            }

            #endregion Explosion Preview

            GUILayout.Space(20);
            updateScreenshoots();

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_modelExploder, "Revert Changes");
            }
            GUI.skin = tempGUI;
        }

        private void OnSceneGUI()
        {
            if (_adjustSphere) {
                drawSphere();
            }
            
            if (_adjustCenter)
            {
                drawCenter();
            }
        }

        private void OnDisable()
        {
            Utilities.DeselectAllInSceneView();
            if (serializedObject.targetObject != null) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
