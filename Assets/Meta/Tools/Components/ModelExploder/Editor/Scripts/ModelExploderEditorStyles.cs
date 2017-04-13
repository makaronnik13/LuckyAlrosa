using UnityEditor;
using UnityEngine;
using System;
using UnityEditorInternal;

namespace Meta.Tools.Editor
{
    public partial class ModelExploderEditor : UnityEditor.Editor
    {
        private GUIStyle _uniqueButtonStyle = new GUIStyle();
        private GUIStyle _commonButtonStyle = new GUIStyle();

        private GUIContent _originLabelContent = new GUIContent();
        private GUIContent _distanceContent = new GUIContent();
        private GUIContent _explosionLabelContent = new GUIContent();
        private GUIContent _explosionSettingsBundlesContent = new GUIContent();
        private GUIContent _uniqueContent = new GUIContent();
        private GUIContent _grouperContent = new GUIContent();
        private GUIContent _axisWiseAxisContent = new GUIContent();
        private GUIContent _previewLabelContent = new GUIContent();
        
        private void initializingStyleAndContents()
        {
            Texture2D uniqueButtonImg = (Texture2D)Resources.Load("Textures/met-tools-red-light1");
            _uniqueButtonStyle.active.background = uniqueButtonImg;
            _uniqueButtonStyle.hover.background = uniqueButtonImg;
            _uniqueButtonStyle.normal.background = uniqueButtonImg;

            Texture2D commonButtonImg = (Texture2D)Resources.Load("Textures/met-tools-green-light1");
            _commonButtonStyle.active.background = commonButtonImg;
            _commonButtonStyle.hover.background = commonButtonImg;
            _commonButtonStyle.normal.background = commonButtonImg;

            _originLabelContent.text = "<b><i><size=18>  Origin: </size></i></b>";
            _originLabelContent.tooltip = "Here you can set an origin for whole object.";
            
            _distanceContent.text = "Distance: ";
            _distanceContent.tooltip = "This value is responsible for distance between an initial positions of a part and it's final position.";

            _explosionLabelContent.text = "<b><i><size=18>  Explosion: </size></i></b>";
            _explosionLabelContent.tooltip = "Here you can adjust required explosion, including setting automatic explosion scenario and performing manual adjustments.";

            _explosionSettingsBundlesContent.text = "<b><i><size=12>  Explosion settings bundle: </size></i></b>";
            _explosionSettingsBundlesContent.tooltip = "Here you can manage bundles of explosion settings.";

            _uniqueContent.text = "<size=9><i>Unique</i></size>";
            _uniqueContent.tooltip = "Set this setting unique";

            _grouperContent.text = "Grouper: ";

            _axisWiseAxisContent.text = "Axis used: ";

            _previewLabelContent.text = "<b><size=12> Preview slider:</size></b>";
            _previewLabelContent.tooltip = "Manipulating slider allow to see the result of model explosion in the scene view";
        }
    }
}