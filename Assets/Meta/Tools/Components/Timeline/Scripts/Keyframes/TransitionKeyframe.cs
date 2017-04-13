using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Meta.Tools
{
    [KeyframeFor(typeof(GameObject))]
    public class TransitionKeyframe : MetaKeyframeBase
    {
        public static float ContextMenuPriority = 7f;

        #region Public Fields

        public int MaterialCount = 0;

        public bool EnableOpacity = true;
        public float Opacity = 1;

        public List<MeshRenderer> renderersList = new List<MeshRenderer>();
        public List<Material> materialsCopy = new List<Material>();

        #endregion

        #region Private Fields

        private static float _height = 25f;

        private static Shader standardShader;

        private static Shader unlitColor;
        private static Shader unlitTexture;
        private static Shader unlitTransparent;

        private static Shader vertexLit;
        private static Shader vertexLitTransparent;

        private static Shader diffuse;
        private static Shader diffuseTransparent;

        private static Shader bumpedDiffuse;
        private static Shader bumpedDiffuseTransparent;

        private static Shader bumpedSpecular;
        private static Shader bumpedSpecularTransparent;

        #endregion

        #region Public Properties
        public override UnityEngine.Object Target
        {
            get
            {
                return base.Target;
            }

            set
            {
                if (value is GameObject)
                {
                    base.Target = value;
                }
            }
        }

#if UNITY_EDITOR
        public override string TargetsName
        {
            get
            {
                return "Transition";
            }
        }

        public override string ParametersWindowTitle
        {
            get
            {
                return "Transition State";
            }
        }

        public override float ParametersWindowHeight
        {
            get
            {
                return base.ParametersWindowHeight + _height;
            }
        }

        public override float ParametersWindowWidth
        {
            get
            {
                return 300f;
            }
        }
#endif

        public override int AnimationQueuePriority
        {
            get
            {
                return 100;
            }
        }
        #endregion

        #region MonoBehaviour Lifecycle
        protected override void Awake()
        {
            base.Awake();
        }
        #endregion

        #region Main Methods
        public override void Init(UnityEngine.Object target, string path)
        {
            base.Init(target, path);

            #region Get Shaders

            standardShader = Shader.Find("Standard");

            unlitColor = Shader.Find("Unlit/Color");
            unlitTexture = Shader.Find("Unlit/Texture");
            unlitTransparent = Shader.Find("Meta/Tools/Unlit/Transparent");

            vertexLit = Shader.Find("Legacy Shaders/VertexLit");
            vertexLitTransparent = Shader.Find("Legacy Shaders/Transparent/VertexLit");

            diffuse = Shader.Find("Legacy Shaders/Diffuse");
            diffuseTransparent = Shader.Find("Legacy Shaders/Transparent/Diffuse");

            bumpedDiffuse = Shader.Find("Legacy Shaders/Bumped Diffuse");
            bumpedDiffuseTransparent = Shader.Find("Legacy Shaders/Transparent/Bumped Diffuse");

            bumpedSpecular = Shader.Find("Legacy Shaders/Bumped Specular");
            bumpedSpecularTransparent = Shader.Find("Legacy Shaders/Transparent/Bumped Specular");

            #endregion

#if UNITY_EDITOR
            _keyframeColor = new Color(122f / 255f, 190f / 255f, 224f / 255f);
#endif
        }

        public override void Fetch()
        {
            if (_target == null)
            {
                return;
            }
            // Get all MeshRenderer components which belong to parent
            if (renderersList.Count == 0)
            {
                var arr = (Target as GameObject).GetComponentsInChildren<MeshRenderer>();
                renderersList.AddRange(arr);
                // Loop across all renderers
                for (int i = 0; i < renderersList.Count; i++)
                {
                    var renderer = renderersList[i];

                    if (renderer != null)
                    {

                        materialsCopy.Clear();

                        // Loop across all renderer materials
                        for (int j = 0; j < renderer.sharedMaterials.Length; j++)
                        {
                            var mat = renderer.sharedMaterials[j];

                            if (mat != null)
                            {
                                if (Opacity == 1)
                                {
                                    Opacity = mat.color.a; // Set opacity
                                }

                                MaterialCount++; // Count materials
                                materialsCopy.Add(new Material(mat)); // Create a temporary copy of material
                            }
                        }

                        renderer.sharedMaterials = materialsCopy.ToArray();
                    }
                }
            }

            base.Fetch();
        }

        public override void Apply(bool forward = true)
        {
            if (_target == null)
            {
                return;
            }

            base.Apply();

            for (int i = 0; i < renderersList.Count; i++)
            {
                var renderer = renderersList[i];

                if (renderer != null)
                {
                    for (int j = 0; j < renderer.sharedMaterials.Length; j++)
                    {
                        var newMat = renderer.sharedMaterials[j];

                        if (newMat != null)
                        {
                            renderer.sharedMaterials[j] = SupportShaderOpacity(newMat); // return material with opacity
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR
        public override void DrawParametersWindow()
        {
            EditorGUILayout.BeginVertical();

            var labelStyle = Utilities.RichTextStyle;
            labelStyle.fontSize = 12;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.normal.textColor = Color.white;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Object materials: " + MaterialCount, labelStyle, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            labelStyle.fontSize = 10;
            labelStyle.fontStyle = FontStyle.Normal;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Enabled: ", labelStyle, GUILayout.Width(50));
            EnableOpacity = EditorGUILayout.Toggle(EnableOpacity, GUILayout.Width(16));

            EditorGUILayout.LabelField("Opacity: ", labelStyle, GUILayout.Width(50));
            Opacity = EditorGUILayout.Slider(Opacity, 0f, 1f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            //base.DrawParametersWindow();
        }
#endif
        #endregion

        #region Interface Implementations

        public override object Clone()
        {
            TransitionKeyframe clone = gameObject.AddComponent<TransitionKeyframe>();

            clone.Target = Target;

            clone.EnableOpacity = EnableOpacity;
            clone.Opacity = Opacity;

#if UNITY_EDITOR
            clone._keyframeColor = _keyframeColor;
            clone._caption = _caption;
#endif

            clone.Timing = Timing;
            clone.hideFlags = hideFlags;
            clone.Init(_target, "");

            return clone;
        }
        #endregion

        #region Utility Methods

        public Material SupportShaderOpacity(Material targetMaterial)
        {
            /*
            Supported shaders:
             - Standart (specular setup),
             - Unlit,
             - Vertext Lit,
             - Bumped Diffuse,
             - Bumped Specular,
             - Diffuse
            */
            (Target as GameObject).SetActive(true);
            string shaderName = targetMaterial.shader.name;

            if (Opacity > 0 && Opacity < 1f)
            {
                if (shaderName.Contains("Standard"))
                {
                    StandardShaderUtils.ChangeRenderMode(targetMaterial, StandardShaderUtils.BlendMode.Fade);
                }
                else if (shaderName.Contains("Unlit") && !shaderName.Contains("Transparent"))
                {
                    targetMaterial.shader = unlitTransparent;
                }
                else if (shaderName.Contains("VertexLit") && !shaderName.Contains("Transparent"))
                {
                    targetMaterial.shader = vertexLitTransparent;
                }
                else if (shaderName.Contains("Bumped Diffuse") && !shaderName.Contains("Transparent"))
                {
                    targetMaterial.shader = bumpedDiffuseTransparent;
                }
                else if (shaderName.Contains("Bumped Specular") && !shaderName.Contains("Transparent"))
                {
                    targetMaterial.shader = bumpedSpecularTransparent;
                }
                else if (shaderName.Contains("Diffuse") && !shaderName.Contains("Transparent"))
                {
                    targetMaterial.shader = diffuseTransparent;
                }
            }
            else if (Opacity == 1f)
            {
                if (shaderName.Contains("Standard"))
                {
                    StandardShaderUtils.ChangeRenderMode(targetMaterial, StandardShaderUtils.BlendMode.Opaque);
                }
                else if (shaderName.Contains("Unlit") && shaderName.Contains("Transparent"))
                {
                    if (targetMaterial.mainTexture != null)
                    {
                        targetMaterial.shader = unlitTexture;
                    }
                    else
                    {
                        targetMaterial.shader = unlitColor;
                    }
                }
                else if (shaderName.Contains("VertexLit") && shaderName.Contains("Transparent"))
                {
                    targetMaterial.shader = vertexLit;
                }
                else if (shaderName.Contains("Bumped Diffuse") && shaderName.Contains("Transparent"))
                {
                    targetMaterial.shader = bumpedDiffuse;
                }
                else if (shaderName.Contains("Bumped Specular") && shaderName.Contains("Transparent"))
                {
                    targetMaterial.shader = bumpedSpecular;
                }
                else if (shaderName.Contains("Diffuse") && shaderName.Contains("Transparent"))
                {
                    targetMaterial.shader = diffuse;
                }
            }

            if (EnableOpacity)
            {
                Color newColor = targetMaterial.color;
                newColor.a = Opacity;
                targetMaterial.color = newColor;
            }

            return targetMaterial;
        }

        #endregion
    }
}
