using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.Tools
{
    [Serializable]
    public class ModelExploderPart
    {
        #region Public Fields
        /// <summary>
        /// Explosion target is final appearence of part. Explosion animation is simply interpolation between initial part's transformation
        /// and final transformation. Initial transformation is defined by _originSubObject's transform. Final is evaluated and assigned by 
        /// algorythms of explosion.
        /// </summary>
        public List<CustomTransform> FinalTransformOffset = new List<CustomTransform>();
        public List<CustomTransform> InitialTransform = new List<CustomTransform>();
        /// <summary>
        /// Switcher to manually activate-deactivate current Part
        /// </summary>
        public bool Active = true;
        public bool FinalPointCalculated = false;
        public List<ModelExploderPartSubObject> SubObjectsList = new List<ModelExploderPartSubObject>();
        #endregion

        #region Serialized Fields
        [SerializeField]
        private int _grouperPartIndex = -1;
        [SerializeField]
        private int _ModelExploderPartIndex = -1;
        [SerializeField]
        private string _name = "Empty Part";
        [SerializeField]
        private int _originSubObjectIndex = -1;
        [SerializeField]
        private List<ExplosionSettings> _currentExplosionSettings = new List<ExplosionSettings>();
        [SerializeField]
        private SerializableBounds _aabb = new SerializableBounds(Vector3.zero, Vector3.zero);

#if UNITY_EDITOR
        /*[SerializeField]
        private bool _observed = false;*/
        /*[SerializeField]
        private float _currentElementHeight = 0f;*/
#endif
        #endregion

        #region Public Properties
        public bool Observed { get; set; }

        public int GrouperPartIndex
        {
            get
            {
                return _grouperPartIndex;
            }
            set
            {
                _grouperPartIndex = value;
            }
        }

        public int ModelExploderPartIndex
        {
            get
            {
                return _ModelExploderPartIndex;
            }
            set
            {
                _ModelExploderPartIndex = value;
            }
        }

        /// <summary>
        /// Can be checked to evaluate if part contains some real objects. Used for optimization.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return SubObjectsList.Count == 0;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public int OriginSubObjectIndex
        {
            get
            {
                return _originSubObjectIndex;
            }
            set
            {
                _originSubObjectIndex = value;
            }
        }

        public List<ExplosionSettings> CurrentExplosionSettings
        {
            get
            {
                return _currentExplosionSettings;
            }
            set
            {
                _currentExplosionSettings = value;
            }
        }
        #endregion

        #region Constructor
        /*
         * We cannot create parts without related Grouper.Part, because it have no sense, although it can be empty if
         * we remove all sub-objects from it, but it will still be bond with Grouper's part. We just can manipulate only 
         * with sub-objects that are represented in the related Grouper's part.
         */

        /// <summary>
        /// ModelExploder.Part constructor.
        /// </summary>
        /// <param name="GrouperPart">Reference to the related Grouper.Part instance</param>
        /// <param name="defaults">Reference to global default behaviour for this ModelExploder</param>
        public ModelExploderPart(int grouperPartIndex, int modelExploderPartIndex, List<ExplosionSettingsBundle> bundles)
        {
            //_grouperPart = GrouperPart;
            GrouperPartIndex = grouperPartIndex;
            ModelExploderPartIndex = modelExploderPartIndex;

            for (int i = 0; i < bundles.Count; i++)
            {
                AddExplosionSettings(bundles[i].DefaultSettings);
                AddNewPositions();
            }
        }
        #endregion

        #region Interface Methods
        public void AddExplosionSettings(ExplosionSettings.Fields defaults)
        {
            _currentExplosionSettings.Add(new ExplosionSettings(defaults));
        }

        public void AddNewPositions(int baseIndex = -1)
        {
            if (baseIndex >= 0)
            {
                FinalTransformOffset.Add(FinalTransformOffset[baseIndex].Clone() as CustomTransform);
                InitialTransform.Add(InitialTransform[baseIndex].Clone() as CustomTransform);
            }
            else
            {
                FinalTransformOffset.Add(new CustomTransform(Vector3.zero, Vector3.zero, Vector3.zero));
                InitialTransform.Add(new CustomTransform(Vector3.zero, Vector3.zero, Vector3.zero));
            }
        }

        public void RemoveExplosionSettings(int i)
        {
            _currentExplosionSettings.RemoveAt(i);
        }

        public void RemovePosiotions(int i)
        {
            FinalTransformOffset.RemoveAt(i);
            InitialTransform.RemoveAt(i);
        }
        #endregion

        #region Utility Methods
        private void checkCenteringOnSubObjectRemoved(ModelExploderPartSubObject subObject)
        {
            /*
             * if some objects was removed we must check if that was an origin-object, and reset origin if it is.
             * Also we must update AABB if deleted object was rendered object and if we choosed bounding box centering mode
             */

            if (SubObjectsList.Count > 1)
            {
                if (subObject.IsOrigin)
                {
                    /*
                     * if origin gameObject was deleted we just assign first gameObject as origin
                     */

                    _originSubObjectIndex = 0;
                    SubObjectsList[0].IsOrigin = true;
                }
            }
            else
            {
                _originSubObjectIndex = -1;
            }
        }
        #endregion
    }
}
