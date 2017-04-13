using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.Tools
{
    [RequireComponent(typeof(Grouper)), Serializable]
    public class ModelExploder : MonoBehaviour, I01InputFloatValue
    {
        #region Public Fields
        //Editor properties, that define modelexploder editor's visual appearence
#if UNITY_EDITOR
        public bool GridEnabled = true;
        public Color GridColor = new Color(100f / 255f, 114f / 255f, 100f / 255, 1f);
        public Color GridBackgroundColor = new Color(0.870588f, 0.870588f, 0.870588f, 1f);
        public CameraAxisSide TargetCameraAxisSide = CameraAxisSide.PlusZ;
        public ProjectionAplliedEffect ProjectionAppliedEffect = ProjectionAplliedEffect.TransparentColor;
        public Color AppliedMaterialColor = new Color(0.68235f, 0.66474f, 0.66474f, 0.45882f);
        public float MinCameraPaddingMultiplier = 0.16f;
        public float GridDimension = 15f;
        public bool ProjectionControlsObserved = false;
#endif
        #endregion

        #region Serialized Fields
        //reference for a grouper component wich we are bonded to
        [SerializeField]
        private Grouper _grouper;
        //Index of currently user Explosion Settins Bundle
        [SerializeField]
        private int _currentExplosionSettingsBundle = 1;
        //Collection of all Explosion Setting Bundles
        [SerializeField]
        public List<ExplosionSettingsBundle> ExplosionSettingsBundles = new List<ExplosionSettingsBundle>();
        //Default settings
        //[SerializeField]
        //private ExplosionSettingsBundle _defaultExplosionSettingsBundle;
        //Temporary settings
        //[SerializeField]
        //private ExplosionSettingsBundle _temporaryExplosionSettingsBundle;
        [SerializeField]
        private int _initialExplosionSettingsBundleIndex = 0;
        [SerializeField]
        private int _finalExplosionSettingsBundleIndex = 1;
        [SerializeField]
        private int _currentlySelectedSettingsBundleIndex = 0;
        //Global Origin Gameobject of a model
        [SerializeField]
        private GameObject _originGameObject;
        //Axis-Aligned Bounding Box of a model
        [SerializeField]
        private SerializableBounds _aabb = new SerializableBounds(Vector3.zero, Vector3.zero);
        //Do we need to make a recalculation of AABB. Used when we changed model's structure somehow
        [SerializeField]
        private bool boundsToBeReCalculated = true;
        //Collection of Parts
        [SerializeField]
        private List<ModelExploderPart> _parts = new List<ModelExploderPart>();
        //Current point of explosion between 0 and 1
        [SerializeField]
        private float _currentAnimationRatio = 0f;
        //original scale of a model. Used as reference point in explosions
        [SerializeField]
        private Vector3 initialScale;

#if UNITY_EDITOR
        //Currently selected Part's index
        public int CurrentlySelectedPart = -1;
        //Is some subobject selected
        [SerializeField]
        private bool _currentlySelectedSubObject = false;
#endif
        #endregion

        #region Private Fields

        /// <summary>
        /// Explosion settings that applied to all parts by default. Used as Prototype for every default explosion settings bundle
        /// </summary>
        private ExplosionSettings.Fields _defaultExplosionFields = new ExplosionSettings.Fields()
        {
            ExplosionRule = ExplosionRule.Radial,
            ExplosionTimings = new Range01FloatValues() { Value0 = 0f, Value1 = 1f },
            Axis = Axis.X,
            TransformToUse = TransformToUse.Local,
            Distance = 2.5f,
            UseCustomOrigin = false,
            CustomOrigin = new CustomTransform(Vector3.zero, Vector3.zero, Vector3.zero)
        };

        private bool _initialized = false;

        #endregion

        #region Public Properties
#if UNITY_EDITOR
        public static ModelExploder ModelExploderRelated
        {
            get
            {
                ModelExploder result = null;

                if (Selection.activeGameObject != null)
                {
                    result = Selection.activeGameObject.GetComponent<ModelExploder>();
                    if (result == null)
                    {
                        Transform parent = Selection.activeGameObject.transform.parent;
                        while (parent != null)
                        {
                            result = parent.gameObject.GetComponent<ModelExploder>();
                            if (result != null)
                            {
                                break;
                            }
                            else
                            {
                                parent = parent.parent;
                            }
                        }
                    }
                }

                return result;
            }
        }
#endif

        public float OptimalExplosionDistance
        {
            get
            {
                float maxSide = AABB.extents.x;
                if (AABB.extents.y > maxSide)
                {
                    maxSide = AABB.extents.y;
                }
                if (AABB.extents.z > maxSide)
                {
                    maxSide = AABB.extents.z;
                }

                return maxSide * 3f;
            }
        }

        public int SelectedStateIndex
        {
            get
            {
                return _currentlySelectedSettingsBundleIndex;
            }
            set
            {
                _currentlySelectedSettingsBundleIndex = value;

                int initial = -1;

                for (int i = 0; i < ExplosionSettingsBundles.Count; i++)
                {
                    if (ExplosionSettingsBundles[value].sourceID == ExplosionSettingsBundles[i].ID)
                    {
                        initial = i;
                        break;
                    }
                }

                if (initial >= 0)
                {
                    InitialStateIndex = initial;
                }
            }
        }

        public int InitialStateIndex
        {
            get
            {
                return _initialExplosionSettingsBundleIndex;
            }
            set
            {
                _initialExplosionSettingsBundleIndex = value;
            }
        }

        public int FinalStateIndex
        {
            get
            {
                return _finalExplosionSettingsBundleIndex;
            }
            set
            {
                _finalExplosionSettingsBundleIndex = value;
            }
        }

        public ExplosionSettings.Fields CurrentFields(int indexOfPart = -1)
        {
            ExplosionSettings.Fields fields = null;

            if (indexOfPart >= 0)
            {
                if (_parts[indexOfPart].CurrentExplosionSettings[CurrentExplosionSettingsBundle].UniqueExplosionRule)
                {
                    fields = _parts[indexOfPart].CurrentExplosionSettings[CurrentExplosionSettingsBundle].Uniques;
                }
                else
                {
                    fields = ExplosionSettingsBundles[CurrentExplosionSettingsBundle].DefaultSettings;
                }
            }
            else
            {
                fields = ExplosionSettingsBundles[CurrentExplosionSettingsBundle].DefaultSettings;
            }

            return fields;
        }

        public GameObject OriginGameObject
        {
            get
            {
                if (_originGameObject == null)
                {
                    _originGameObject = gameObject;
                }
                return _originGameObject;
            }
        }

        public Transform Origin
        {
            get
            {
                return OriginGameObject.transform;
            }
        }

        public Vector3 OriginPosition
        {
            get
            {
                return OriginGameObject.transform.position;
            }
        }

        //ModelExploder doesn't store references to gameObjects. Instead of it he rely on groupers model structure
        public List<GameObject> GameObjects
        {
            get
            {
                return _grouper.GameObjects;
            }
        }

        public ExplosionSettingsBundle TemporaryExplosionSettingsBundle
        {
            get
            {
                return ExplosionSettingsBundles[0];
            }
        }

        public int CurrentExplosionSettingsBundle
        {
            get
            {
                return _currentExplosionSettingsBundle;
            }
            set
            {
                _currentExplosionSettingsBundle = value;
            }
        }

        public List<string> ExplosionSettingsBundlesNames
        {
            get
            {
                List<string> names = new List<string>();

                for (int i = 0; i < ExplosionSettingsBundles.Count; i++)
                {
                    if (i > 1)
                    {
                        names.Add(ExplosionSettingsBundles[i].Name);
                    }
                }

                return names;
            }
        }

        //Current Axis-Aligned Boundeing Box of a model
        public Bounds AABB
        {
            get
            {
                if (boundsToBeReCalculated)
                {
                    RecalculateAABB();
                }

                Bounds result = new Bounds();
                result.center = _aabb.Center.Extract();
                result.extents = _aabb.Extents.Extract();

                return result;
            }
        }

        public List<ModelExploderPart> Parts
        {
            get
            {
                return _parts;
            }
        }

        public Grouper Grouper
        {
            get
            {
                return _grouper;
            }
            set
            {
                /*
                 * The moment we set grouper is the moment we must perform integration of it. Also
                 * that may mean resetting, so we must clear ourselves to the uninitilized state.
                 */

                if (_grouper != value)
                {
                    if (_grouper != null)
                    {
                        _grouper._unityPartAdd.Remove(onGrouperPartAdded);
                        _grouper._unityPartDestroy.Remove(onGrouperPartDestroyed);
                        _grouper._unitySubObjectAdd.Remove(onGrouperSubObjectAdded);
                        _grouper._unitySubObjectDestroy.Remove(onGrouperSubObjectDestroyed);

                        _grouper._unityDestroy.Remove(onGrouperDestroy);

                        Clear();
                    }

                    _grouper = value;
                    if (value != null)
                    {
                        _grouper._unityPartAdd.Add(onGrouperPartAdded);
                        _grouper._unityPartDestroy.Add(onGrouperPartDestroyed);
                        _grouper._unitySubObjectAdd.Add(onGrouperSubObjectAdded);
                        _grouper._unitySubObjectDestroy.Add(onGrouperSubObjectDestroyed);

                        _grouper._unityDestroy.Add(onGrouperDestroy);

                        integrateAllGroupsFromGrouper();
                    }
                }
            }
        }

        //CurrentAnimationRatio can be used for animating explosion as it explodes model in it's setter
        public float CurrentAnimationRatio
        {
            get
            {
                return _currentAnimationRatio;
            }
            set
            {
                if (value != _currentAnimationRatio)
                {
                    _currentAnimationRatio = value;
                    explode(InitialStateIndex, FinalStateIndex, _currentAnimationRatio);
                }
            }
        }

        #endregion

        #region Monobehaviour Lifecycle
        private void Awake()
        {
#if UNITY_EDITOR
            //it happens when parts wasn't deselected in editor mode despite our actions
            if (CurrentlySelectedPart >= 0 || _currentlySelectedSubObject)
            {
                Utilities.DeselectInGame(Grouper.GameObjects.ToArray());
                CurrentlySelectedPart = -1;
            }
#endif
        }

        private void OnDestroy()
        {
            Destroy(_grouper);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// ModelExploder constructor.
        /// </summary>
        public ModelExploder()
        {
            //in constructor we just creating first settings bundle
            AddExplosionSettingsBundle(null, "Default");
        }
        #endregion

        #region Interface Methods

        public GameObject[] GetGameObjectsFromPart(ModelExploderPart part)
        {
            return Grouper.GetGameObjectsFromPart(Grouper.Parts[part.GrouperPartIndex]);
        }

        public Transform GetPartsOrigin(ModelExploderPart part)
        {
            return _grouper.GameObjects[part.SubObjectsList[part.OriginSubObjectIndex].GameObjectIndex].transform;
        }

        public GameObject GetPartsOriginGameObject(ModelExploderPart part)
        {
            return _grouper.GameObjects[part.SubObjectsList[part.OriginSubObjectIndex].GameObjectIndex];
        }

        public Transform GetSubObjectsOrigin(ModelExploderPartSubObject subObject)
        {
            return _grouper.GameObjects[subObject.GameObjectIndex].transform;
        }

        public ExplosionSettingsBundle AddExplosionSettingsBundle(ExplosionSettingsBundle prototype = null, string name = "")
        {
            ExplosionSettingsBundle result = new ExplosionSettingsBundle();

            int lastID = 0;
            for (int i = 0; i < ExplosionSettingsBundles.Count; i++)
            {
                if (lastID < ExplosionSettingsBundles[i].ID)
                {
                    lastID = ExplosionSettingsBundles[i].ID;
                }
            }
            result.ID = lastID + 1;

            if (name == "")
            {
                result.Name = "State №" + (ExplosionSettingsBundles.Count - 1);
            }
            else
            {
                result.Name = name;
            }

            if (prototype != null)
            {
                result.DefaultSettings = prototype.DefaultSettings.Clone() as ExplosionSettings.Fields;
            }
            else
            {
                result.DefaultSettings = _defaultExplosionFields.Clone() as ExplosionSettings.Fields;
            }

            int baseIndex = -1;
            if (prototype != null && ExplosionSettingsBundles.Contains(prototype))
            {
                baseIndex = ExplosionSettingsBundles.IndexOf(prototype);
            }

            for (int i = 0; i < _parts.Count; i++)
            {
                _parts[i].AddExplosionSettings(result.DefaultSettings);
                _parts[i].AddNewPositions(baseIndex);

                for (int j = 0; j < _parts[i].SubObjectsList.Count; j++)
                {
                    _parts[i].SubObjectsList[j].AddNewPositions(baseIndex);
                }
            }

            ExplosionSettingsBundles.Add(result);
            //CurrentExplosionSettingsBundle = ExplosionSettingsBundles.Count - 1;

            return result;
        }

        public bool RemoveExplosionSettingsBundle(int index)
        {
            //We cannot delete all bundles, we must leave at least one. So let it be the one default settings bundle that will not be deletable.
            //And removing temporary bundle also have no sense
            if (index > 1 && ExplosionSettingsBundles.Count > index)
            {
                for (int i = 0; i < _parts.Count; i++)
                {
                    _parts[i].RemoveExplosionSettings(index);
                    _parts[i].RemovePosiotions(index);

                    for (int j = 0; j < _parts[i].SubObjectsList.Count; j++)
                    {
                        _parts[i].SubObjectsList[j].RemovePosiotions(index);
                    }
                }

                ExplosionSettingsBundles.RemoveAt(index);

                //CurrentExplosionSettingsBundle--;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Clears ModelExploder's parts collection
        /// </summary>
        public void Clear()
        {
            _parts.Clear();
        }

        /// <summary>
        /// Access part by it's index in collection
        /// </summary>
        /// <param name="grouperPartIndex"></param>
        /// <returns></returns>
        public ModelExploderPart GetPart(int grouperPartIndex)
        {
            for (int i = 0; i < Parts.Count; i++)
            {
                if (Parts[i].GrouperPartIndex == grouperPartIndex)
                {
                    return Parts[i];
                }
            }
            return null;
        }

        public ModelExploderPartSubObject GetSubObject(int grouperPartIndex, int grouperSubObjectIndex)
        {
            ModelExploderPart part = GetPart(grouperPartIndex);
            if (part != null)
            {
                for (int i = 0; i < part.SubObjectsList.Count; i++)
                {
                    if (part.SubObjectsList[i].GrouperSubObjectIndex == grouperSubObjectIndex)
                    {
                        return part.SubObjectsList[i];
                    }
                }
            }
            return null;
        }

        #endregion

        #region Grouper Events Listeners
        private void onGrouperPartAdded(GrouperPart grouperPart)
        {
            addPart(grouperPart.PartIndex, grouperPart.Name);
        }

        private void onGrouperPartDestroyed(GrouperPart grouperPart)
        {
            destroyPart(GetPart(grouperPart.PartIndex));
        }

        private void onGrouperSubObjectAdded(GrouperPart grouperPart, GrouperPartSubObject grouperPartSubObject)
        {
            addSubObject(GetPart(grouperPart.PartIndex), grouperPartSubObject.GameObjectIndex, grouperPartSubObject.SubObjectIndex);
        }

        private void onGrouperSubObjectDestroyed(GrouperPart grouperPart, GrouperPartSubObject grouperPartSubObject)
        {
            destroySubObject(GetPart(grouperPart.PartIndex), GetSubObject(grouperPart.PartIndex, grouperPartSubObject.SubObjectIndex));
        }

        private void onGrouperDestroy()
        {
            /*
             * when grouper is destroyed there's no sense of ModelExploder, so we destroying ourselves
             */

            if (Grouper != null)
            {
                _grouper._unityPartAdd.Remove(onGrouperPartAdded);
                _grouper._unityPartDestroy.Remove(onGrouperPartDestroyed);
                _grouper._unitySubObjectAdd.Remove(onGrouperSubObjectAdded);
                _grouper._unitySubObjectDestroy.Remove(onGrouperSubObjectDestroyed);

                _grouper._unityDestroy.Remove(onGrouperDestroy);
            }
            Destroy(this);
        }
        #endregion

        #region Main Methods

        public void Save(int index, int sourceIndex = -1)
        {
            int newIndex = index;
            if (index == 1)
            {
                AddExplosionSettingsBundle(ExplosionSettingsBundles[1], "");
                newIndex = ExplosionSettingsBundles.Count - 1;
            }
            //UpdateInitialTransforms(index);
            //RecalculateAllExplosionTargets(InitialStateIndex, index);

            SavePlacesToFinal(newIndex);

            if (index == 1)
            {
                if (sourceIndex >= 0)
                {
                    ExplosionSettingsBundles[newIndex].sourceID = ExplosionSettingsBundles[sourceIndex].ID;
                }
                else
                {
                    //ExplosionSettingsBundles[newIndex].sourceID = ExplosionSettingsBundles[SelectedStateIndex].ID;
                    ExplosionSettingsBundles[newIndex].sourceID = ExplosionSettingsBundles[0].ID;
                }
            }

            FinalStateIndex = newIndex;
        }

        /// <summary>
        /// Function called on start. There may be 2 scenarious. In first ModelExploder is newely added, and hasn't been 
        /// linked to grouper yet. In that case we must perform initial integration of what we see in grouper. In second 
        /// scenario we already have all parts linked and need to do nothing.
        /// </summary>
        public void Initialize()
        {
            if (!_initialized && Grouper == null)
            {
                _initialized = true;
                
                if (!Utilities.IsModelPivotsOk(gameObject))
                {
                    Utilities.FixModelPivot(gameObject);
                }

                performInitialIntegration(GetComponent<Grouper>());
                _defaultExplosionFields.Distance = OptimalExplosionDistance;

                ExplosionSettingsBundles[0].DefaultSettings.Distance = _defaultExplosionFields.Distance;
                ExplosionSettingsBundles[1].DefaultSettings.Distance = _defaultExplosionFields.Distance;
            }
        }

        private void performInitialIntegration(Grouper grouper = null)
        {
            /*
             * Initial integration of grouper's content means subscribing on related events, and integrating all the parts
             * that grouper contains, and that's it - the rest will be performed automatically. And all of that will be 
             * done in the grouper's setter.
             */

            if (grouper != null)
            {
                Grouper = grouper;
                Grouper.hideFlags = HideFlags.HideInInspector;

                Grouper.PerformAutoDistribution();

                /*ExplosionSettingsBundles[0].DefaultSettings.Distance = 1f;
                //UpdateInitialTransforms(0);
                //RecalculateAllExplosionTargets(0, 0);
                SavePlacesToFinal(0);
                ExplosionSettingsBundles[0].DefaultSettings.Distance = _defaultExplosionFields.Distance;*/
                SaveAsDefault();

                AddExplosionSettingsBundle(ExplosionSettingsBundles[0], "Temp");
            }
        }

        public void SaveAsDefault()
        {
            SavePlacesToFinal(0);
        }

        public void SaveToNew()
        {
            Save(1);
        }

        public void SetDefault()
        {
            explode(0, 0, 0f);
        }

        public void UpdateInitialTransforms(int index)
        {
            /*
             * When updating initial transforms we currently updating whole hierarchy. We must accept the 
             * current configuration of transforms as what we want to be in initial state. So we walk recursively
             * through all objects, and remembering all their relative offsets in the internal structs.
             */

            /*if (_parts.Count == 0)
            {
                return;
            }
            ExplosionSettingsBundles[index].DefaultSettings.InitialScale.Convert(OriginGameObject.transform.localScale);
            //initialScale = OriginGameObject.transform.localScale;

            for (int i = 0; i < _parts.Count; i++)
            {
                Transform childTransform = GetPartsOrigin(_parts[i]);

                _parts[i].InitialTransform[index].Position.Convert(_originGameObject.transform.InverseTransformPoint(childTransform.position));
                _parts[i].InitialTransform[index].Rotation.Convert((Quaternion.Inverse(_originGameObject.transform.rotation) * childTransform.rotation).eulerAngles);
                _parts[i].InitialTransform[index].Scale.Convert(Utilities.Divide(childTransform.localScale, _originGameObject.transform.localScale));

                //Debug.Log("_parts[" + i + "] initial: " + _parts[i].InitialTransform[CurrentExplosionSettingsBundle].Scale.ToString());

                for (int j = 0; j < _parts[i].SubObjectsList.Count; j++)
                {
                    Transform grandChildTransform = GetSubObjectsOrigin(_parts[i].SubObjectsList[j]);

                    _parts[i].SubObjectsList[j].RelativeTransform[index].Position.Convert(childTransform.InverseTransformPoint(grandChildTransform.position));
                    _parts[i].SubObjectsList[j].RelativeTransform[index].Rotation.Convert((Quaternion.Inverse(childTransform.rotation) * grandChildTransform.rotation).eulerAngles);
                    _parts[i].SubObjectsList[j].RelativeTransform[index].Scale.Convert(Utilities.Divide(grandChildTransform.localScale, childTransform.localScale));

                    //Debug.Log("_parts[" + i + "] SubObjectsList[" + j + "] relative: " + _parts[i].SubObjectsList[j].RelativeTransform[CurrentExplosionSettingsBundle].Scale.ToString());
                }
            }*/
        }

        //In that function we perform calculations for every part to define the final state of it
        public void RecalculateAllExplosionTargets(int startIndex, int endIndex)
        {
            if (_parts == null)
            {
                return;
            }

            for (int i = 0; i < _parts.Count; i++)
            {
                ExplosionRule requiredExplosionRule = _parts[i].CurrentExplosionSettings[endIndex].UniqueExplosionRule ? 
                    _parts[i].CurrentExplosionSettings[endIndex].Uniques.ExplosionRule : 
                    ExplosionSettingsBundles[endIndex].DefaultSettings.ExplosionRule;

                float requiredDistance = _parts[i].CurrentExplosionSettings[endIndex].UniqueDistance ?
                    _parts[i].CurrentExplosionSettings[endIndex].Uniques.Distance :
                    ExplosionSettingsBundles[endIndex].DefaultSettings.Distance;

                CustomTransform requiredOrigin = new CustomTransform(Vector3.zero, Vector3.zero, Vector3.zero);
                Vector3 requiredCenterPosition;
                Transform requiredCenterTransform;
                if (_parts[i].CurrentExplosionSettings[endIndex].UniqueUseCustomOrigin ?
                            _parts[i].CurrentExplosionSettings[endIndex].Uniques.UseCustomOrigin :
                            ExplosionSettingsBundles[endIndex].DefaultSettings.UseCustomOrigin)
                {
                    requiredOrigin = _parts[i].CurrentExplosionSettings[endIndex].UniqueCustomOrigin ?
                        _parts[i].CurrentExplosionSettings[endIndex].Uniques.CustomOrigin :
                        ExplosionSettingsBundles[endIndex].DefaultSettings.CustomOrigin;

                    requiredCenterPosition = Origin.transform.TransformPoint(requiredOrigin.Position.Extract());
                    requiredCenterTransform = Origin.transform;
                }
                else
                {
                    requiredCenterPosition = Origin.transform.position;
                    requiredCenterTransform = Origin.transform;
                }

                Transform partsTransform = GetPartsOrigin(_parts[i]);
                CustomTransform partsCustomTransform = _parts[i].FinalTransformOffset[startIndex];
                
                switch (requiredExplosionRule)
                {
                    case ExplosionRule.AxisWise:
                        Vector3 finalOffset = Vector3.zero;

                        Axis requiredAxis = _parts[i].CurrentExplosionSettings[endIndex].UniqueAxis ?
                            _parts[i].CurrentExplosionSettings[endIndex].Uniques.Axis :
                            ExplosionSettingsBundles[endIndex].DefaultSettings.Axis;

                        Vector3 fullOffset = Vector3.zero;

                        //fullOffset = requiredDistance * Vector3.Scale(_parts[i].InitialTransform[CurrentExplosionSettingsBundle].Position.Extract(), initialScale);

                        Vector3 originPoint = Origin.TransformPoint(_parts[i].FinalTransformOffset[startIndex].Position.Extract());
                        Vector3 offset = originPoint - requiredCenterPosition;
                        //Vector3 offset = originPoint - Origin.position;
                        offset = Quaternion.Inverse(requiredCenterTransform.rotation) * offset;

                        fullOffset = offset;
                        fullOffset = /*requiredCenterPosition + */fullOffset * requiredDistance;
                        //fullOffset = /*requiredCenterPosition + */Vector3.Scale(Utilities.Divide(ExplosionSettingsBundles[startIndex].DefaultSettings.InitialScale.Extract(), Origin.localScale), fullOffset * requiredDistance);
                        if (FlagsHelper.IsSet(requiredAxis, Axis.X))
                        {
                            finalOffset.x = fullOffset.x;
                        }
                        if (FlagsHelper.IsSet(requiredAxis, Axis.Y))
                        {
                            finalOffset.y = fullOffset.y;
                        }
                        if (FlagsHelper.IsSet(requiredAxis, Axis.Z))
                        {
                            finalOffset.z = fullOffset.z;
                        }

                        partsTransform.position = originPoint + requiredCenterTransform.rotation * finalOffset;
                        /*finalOffset = requiredCenterTransform.rotation * finalOffset;
                        finalOffset = requiredCenterPosition + finalOffset;

                        partsTransform.position = requiredCenterTransform.rotation * _parts[i].FinalTransformOffset[startIndex].Position.Extract() + finalOffset;*/
                        break;
                    case ExplosionRule.Radial:

                        Vector3 originPoint1 = Origin.TransformPoint(_parts[i].FinalTransformOffset[startIndex].Position.Extract());
                        Vector3 offset1 = originPoint1 - requiredCenterPosition;
                        //Vector3 offset1 = originPoint1 - Origin.position;
                        offset = Quaternion.Inverse(requiredCenterTransform.rotation) * offset1;

                        Vector3 fullOffset1 = offset * requiredDistance;

                        partsTransform.position = originPoint1 + requiredCenterTransform.rotation * fullOffset1;

                        //partsTransform.position = /*requiredCenterPosition + requiredDistance * (partsTransform.position - requiredCenterPosition)*/requiredCenterPosition + partsCustomTransform.Position.Extract() * requiredDistance;
                        break;
                    case ExplosionRule.Sphere:

                        Vector3 originPoint2 = Origin.TransformPoint(_parts[i].FinalTransformOffset[startIndex].Position.Extract());
                        Vector3 offset2 = originPoint2 - requiredCenterPosition;
                        //Vector3 offset2 = originPoint2 - Origin.position;
                        offset = Quaternion.Inverse(requiredCenterTransform.rotation) * offset2;

                        Vector3 fullOffset2 = offset.normalized * requiredDistance;

                        partsTransform.position = Origin.position + requiredCenterTransform.rotation * (0.5f * fullOffset2);


                        //partsTransform.position = requiredCenterPosition + 0.5f * requiredDistance * (partsTransform.position - requiredCenterPosition).normalized;
                        break;
                }

                //_parts[i].FinalPointCalculated = true;

                //partsTransform.position = _parts[i].FinalTransformOffset[endIndex].Position.Extract();
            }
        }

        private void SavePlacesToFinal(int index)
        {
            if (_parts == null)
            {
                return;
            }

            for (int i = 0; i < _parts.Count; i++)
            {
                Transform partsTransform = GetPartsOrigin(_parts[i]);

                _parts[i].FinalTransformOffset[index].Position.Convert(Origin.InverseTransformPoint(partsTransform.position));

                _parts[i].FinalPointCalculated = true;
            }
        }

        public void Explode(int initialState, int finalState, float ratio)
        {
            explode(initialState, finalState, ratio);
        }

        /// <summary>
        /// This function sets parts accordingly to their setting rules and current animation ratio
        /// </summary>
        private void explode(int initialState, int finalState, float ratio)
        {
            //if (initialScale.x != 0f && initialScale.y != 0f && initialScale.z != 0f)
            //{

                for (int i = 0; i < _parts.Count; i++)
                {
                    /*ExplosionRule requiredExplosionRule = _parts[i].CurrentExplosionSettings[finalState].UniqueExplosionRule ?
                        _parts[i].CurrentExplosionSettings[finalState].Uniques.ExplosionRule :
                        ExplosionSettingsBundles[finalState].DefaultSettings.ExplosionRule;*/

                    if (_parts[i].Active && _parts[i].FinalPointCalculated)
                    {
                        Transform partsTransform = GetPartsOrigin(_parts[i]);

                    /*switch (requiredExplosionRule)
                    {
                        case ExplosionRule.AxisWise:
                            partsTransform.position = Origin.TransformPoint(_parts[i].FinalTransformOffset[initialState].Position.Extract() + Vector3.Scale(Utilities.Divide(_originGameObject.transform.localScale, initialScale), _parts[i].FinalTransformOffset[finalState].Position.Extract() * _currentAnimationRatio));
                            break;
                        default:
                            partsTransform.position = Origin.TransformPoint(Vector3.Lerp(_parts[i].FinalTransformOffset[initialState].Position.Extract(), Vector3.Scale(Utilities.Divide(_originGameObject.transform.localScale, initialScale), _parts[i].FinalTransformOffset[finalState].Position.Extract()), _currentAnimationRatio));
                            break;
                    }*/
                    /*Vector3 pos = Vector3.Lerp(_parts[i].FinalTransformOffset[initialState].Position.Extract(), _parts[i].FinalTransformOffset[finalState].Position.Extract(), ratio);
                    pos = Vector3.Scale(Utilities.Divide(ExplosionSettingsBundles[finalState].DefaultSettings.InitialScale.Extract(), Origin.localScale), pos);
                    pos = Origin.TransformPoint(pos);
                    partsTransform.position = pos;*/
                    //partsTransform.position = Origin.TransformPoint(Vector3.Lerp(_parts[i].FinalTransformOffset[initialState].Position.Extract(), /*Vector3.Scale(Utilities.Divide(_originGameObject.transform.localScale, initialScale),*/ _parts[i].FinalTransformOffset[finalState].Position.Extract()/*)*/, ratio));
                    partsTransform.position = Origin.TransformPoint(Vector3.Lerp(_parts[i].FinalTransformOffset[initialState].Position.Extract(), _parts[i].FinalTransformOffset[finalState].Position.Extract(), ratio));

                    /*for (int j = 0; j < _parts[i].SubObjectsList.Count; j++)
                    {
                        Transform subObjectsTransform = GetSubObjectsOrigin(_parts[i].SubObjectsList[j]);

                        subObjectsTransform.position = partsTransform.TransformPoint(_parts[i].SubObjectsList[j].RelativeTransform[finalState].Position.Extract());
                    }*/
                }
                }
            //}
        }
        #endregion

        #region Utility Methods
        public int GetActiveGroupsCount()
        {
            int activeGroupsCount = 0;
            for (int i = 0; i < Parts.Count; i++)
            {
                if (Parts[i].Active) activeGroupsCount++;
            }
            return activeGroupsCount;
        }

        public void RecalculateAABB()
        {
            Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                return;
            }
            Bounds tempoBounds = renderer.bounds;
            foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>())
            {
                tempoBounds.Encapsulate(rend.bounds);
            }
            _aabb.Center.Convert(tempoBounds.center);
            _aabb.Extents.Convert(tempoBounds.extents);
        }


        public CustomTransform GetRelativeCustomTransform(Transform parent, Transform child)
        {
            CustomTransform result = new CustomTransform();
            result.Position.Convert(parent.InverseTransformPoint(child.position));
            result.Rotation.Convert((Quaternion.Inverse(parent.rotation) * child.rotation).eulerAngles);
            result.Scale.Convert(Utilities.Divide(child.localScale, parent.localScale));

            return result;
        }

        public void ConnectToDefaultController()
        {
            Tweener tweener = gameObject.GetComponent<Tweener>();
            if (tweener == null)
            {
                tweener = gameObject.AddComponent<Tweener>();
            }

            Adapter01 adapter01 = gameObject.GetComponent<Adapter01>();
            if (adapter01 == null)
            {
                adapter01 = gameObject.AddComponent<Adapter01>();
            }

            tweener.Target = adapter01;
            adapter01.Target = this;
        }

        private void integrateAllGroupsFromGrouper()
        {
            for (int i = 0; i < Grouper.Parts.Count; i++)
            {
                addPart(Grouper.Parts[i].PartIndex, Grouper.Parts[i].Name);

                for (int j = 0; j < Grouper.Parts[i].SubObjectsList.Count; j++)
                {
                    addSubObject(GetPart(Grouper.Parts[i].PartIndex), Grouper.Parts[i].SubObjectsList[j].GameObjectIndex, Grouper.Parts[i].SubObjectsList[j].SubObjectIndex);
                }
            }
        }

        private void addPart(int grouperPartIndex, string name)
        {
            ModelExploderPart explodedPart = new ModelExploderPart(grouperPartIndex, Parts.Count, ExplosionSettingsBundles);

            Parts.Add(explodedPart);

            explodedPart.Name = name;
        }

        private void destroyPart(ModelExploderPart part)
        {
            Parts.Remove(part);
        }

        private void addSubObject(ModelExploderPart part, int gameObjectIndex, int grouperSubObjectIndex)
        {
            ModelExploderPartSubObject subObject = new ModelExploderPartSubObject(gameObjectIndex, grouperSubObjectIndex, part.SubObjectsList.Count);

            part.SubObjectsList.Add(subObject);

            if (part.OriginSubObjectIndex < 0)
            {
                part.OriginSubObjectIndex = 0;

                part.SubObjectsList[0].IsOrigin = true;
            }
        }

        private void destroySubObject(ModelExploderPart part, ModelExploderPartSubObject subObject)
        {
            part.SubObjectsList.Remove(subObject);

            if (part.SubObjectsList.Count == 0)
            {
                part.OriginSubObjectIndex = -1;
            }
        }
        #endregion

        #region Interfaces Implementations
        public float InputValue
        {
            get
            {
                return _currentAnimationRatio;
            }
            set
            {
                if (value != _currentAnimationRatio)
                {
                    _currentAnimationRatio = value;
                    explode(InitialStateIndex, FinalStateIndex, _currentAnimationRatio);
                }
            }
        }
        #endregion
    }
}

