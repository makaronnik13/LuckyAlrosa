using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Meta.Extensions;
using UnityEngine.UI;

namespace Meta
{
    public class Fade : MonoBehaviour
    {
        [SerializeField]
        private bool _includeChildren = true;

        [SerializeField]
        private Color _targetColor = Color.white;

        [SerializeField]
        private float _multiplier = 8f;

        [SerializeField]
        private bool _disableRendererOnAlphaFadeOut = true;

        private Renderer[] _renderers;
        private Material[] _materials;
        private Graphic[] _graphics;
        private Color[] _initialMaterialColors;
        private Color[] _initialGraphicColors;
        private Color[] _startMaterialColors;
        private Color[] _startGraphicColors;
        private Coroutine _coroutine;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            _renderers = _includeChildren ? GetComponentsInChildren<Renderer>() : GetComponents<Renderer>();
            List<Material> materialList = new List<Material>();

            for (int i = 0; i < _renderers.Length; ++i)
            {
                materialList.AddRange(_renderers[i].sharedMaterials);
            }

            _materials = materialList.ToArray();
            _initialMaterialColors = new Color[_materials.Length];
            _startMaterialColors = new Color[_materials.Length];

            for (int i = 0; i < _materials.Length; ++i)
            {
                _initialMaterialColors[i] = _materials[i].color;
                _startMaterialColors[i] = _materials[i].color;
            }

            _graphics = _includeChildren ? GetComponentsInChildren<Graphic>() : GetComponents<Graphic>();
            _initialGraphicColors = new Color[_graphics.Length];
            _startGraphicColors = new Color[_graphics.Length];
            for (int i = 0; i < _graphics.Length; ++i)
            {
                _startGraphicColors[i] = _graphics[i].color;
            }
        }

        public void FadeToTargetColor()
        {
            FadeToTargetColor(_multiplier);
        }

        public void FadeToTargetColor(float multiplier)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(FadeToTargetColorCoroutine(_targetColor, multiplier));
        }

        public void FadeToStartColor()
        {
            FadeToStartColor(_multiplier);
        }

        public void FadeToStartColor(float multiplier)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(FadeToStartColorCoroutine(multiplier));
        }

        [ContextMenu("Fade In")]
        public void FadeIn()
        {
            FadeIn(_multiplier, null);
        }

        public void FadeIn(float multiplier)
        {
            FadeIn(multiplier, null);
        }

        public void FadeIn(Action finish)
        {
            FadeIn(_multiplier, finish);
        }

        public void FadeIn(float multiplier, Action finish)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            if (_disableRendererOnAlphaFadeOut)
            {
                for (int i = 0; i < _renderers.Length; i++)
                {
                    _renderers[i].enabled = true;
                }
            }

            _coroutine = StartCoroutine(FadeAlphaCoroutine(1f, multiplier, finish));
        }

        public void FadeInImmediate()
        {
            for (int i = 0; i < _materials.Length; ++i)
            {
                _materials[i].color = _materials[i].color.SetAlpha(1);
            }
            for (int i = 0; i < _graphics.Length; ++i)
            {
                _graphics[i].color = _graphics[i].color.SetAlpha(1);
            }
        }

        [ContextMenu("Fade Out")]
        public void FadeOut()
        {
            FadeOut(_multiplier, null);
        }

        public void FadeOut(float multiplier)
        {
            FadeOut(_multiplier, null);
        }

        public void FadeOut(Action finish)
        {
            FadeOut(_multiplier, finish);
        }

        public void FadeOut(float multiplier, Action finish)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            Action tempFinish = () =>
            {
                if (_disableRendererOnAlphaFadeOut)
                {
                    for (int i = 0; i < _renderers.Length; i++)
                    {
                        _renderers[i].enabled = false;
                    }
                }
            };

            tempFinish += finish;

            _coroutine = StartCoroutine(FadeAlphaCoroutine(0f, multiplier, tempFinish));
        }

        public void FadeOutImmediate()
        {
            for (int i = 0; i < _materials.Length; ++i)
            {
                _materials[i].color = _materials[i].color.SetAlpha(0);
            }
            for (int i = 0; i < _graphics.Length; ++i)
            {
                _graphics[i].color = _graphics[i].color.SetAlpha(0);
            }
        }

        private IEnumerator FadeToStartColorCoroutine(float multiplier)
        {
            float time = 0;
            for (int i = 0; i < _materials.Length; ++i)
            {
                _initialMaterialColors[i] = _materials[i].color;
            }
            for (int i = 0; i < _graphics.Length; ++i)
            {
                _initialGraphicColors[i] = _graphics[i].color;
            }
            while (time < 1)
            {
                yield return null;
                time += Time.deltaTime * multiplier;
                for (int i = 0; i < _materials.Length; ++i)
                {
                    _materials[i].color = Color.Lerp(_initialMaterialColors[i], _startMaterialColors[i], time);
                }
                for (int i = 0; i < _graphics.Length; ++i)
                {
                    _graphics[i].color = Color.Lerp(_initialGraphicColors[i], _startGraphicColors[i], time);
                }
            }
            for (int i = 0; i < _materials.Length; ++i)
            {
                _materials[i].color = _startMaterialColors[i];
            }
            for (int i = 0; i < _graphics.Length; ++i)
            {
                _graphics[i].color = _startGraphicColors[i];
            }
        }

        private IEnumerator FadeToTargetColorCoroutine(Color targetColor, float multiplier)
        {
            float time = 0;
            for (int i = 0; i < _materials.Length; ++i)
            {
                _initialMaterialColors[i] = _materials[i].color;
            }
            for (int i = 0; i < _graphics.Length; ++i)
            {
                _initialGraphicColors[i] = _graphics[i].color;
            }
            while (time < 1)
            {
                yield return null;
                time += Time.deltaTime * multiplier;
                for (int i = 0; i < _materials.Length; ++i)
                {
                    _materials[i].color = Color.Lerp(_initialMaterialColors[i], targetColor, time);
                }
                for (int i = 0; i < _graphics.Length; ++i)
                {
                    _graphics[i].color = Color.Lerp(_initialGraphicColors[i], targetColor, time);
                }
            }
            for (int i = 0; i < _materials.Length; ++i)
            {
                _materials[i].color = _targetColor;
            }
            for (int i = 0; i < _graphics.Length; ++i)
            {
                _graphics[i].color = _targetColor;
            }
        }

        private IEnumerator FadeAlphaCoroutine(float target, float multiplier, Action finish)
        {
            float time = 0;
            for (int i = 0; i < _materials.Length; ++i)
            {
                _initialMaterialColors[i] = _materials[i].color;
            }
            for (int i = 0; i < _graphics.Length; ++i)
            {
                _initialGraphicColors[i] = _graphics[i].color;
            }
            while (time < 1)
            {
                yield return null;
                time += Time.deltaTime * multiplier;
                for (int i = 0; i < _materials.Length; ++i)
                {
                    _materials[i].color = _materials[i].color.SetAlpha(Mathf.Lerp(_initialMaterialColors[i].a, target, time));
                }
                for (int i = 0; i < _graphics.Length; ++i)
                {
                    _graphics[i].color = _graphics[i].color.SetAlpha(Mathf.Lerp(_initialGraphicColors[i].a, target, time));
                }
            }
            for (int i = 0; i < _materials.Length; ++i)
            {
                _materials[i].color = _materials[i].color.SetAlpha(target);
            }
            for (int i = 0; i < _graphics.Length; ++i)
            {
                _graphics[i].color = _graphics[i].color.SetAlpha(target);
            }
            if (finish != null)
            {
                finish();
            }
        }
    }
}