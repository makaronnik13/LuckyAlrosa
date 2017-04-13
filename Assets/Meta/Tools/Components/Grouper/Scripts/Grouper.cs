using System;
using UnityEngine;
using System.Collections.Generic;

namespace Meta.Tools
{
    /// <summary>
    /// Class describing simple single-leveled grouping. It's responsible for categorization of multiple gameObjects, to a system
    /// of logical parts. It contains some simple automatic categorization rules, but you can also easely manage goups and it's 
    /// sub-objects by your own.
    /// </summary>
    [Serializable]
    public partial class Grouper : MonoBehaviour
    {

        #region Public Enumerations
        /// <summary>
        /// The mode of auto-distribution enumeration
        /// </summary>
        [Serializable]
        public enum AutoDistributionMode
        {
            //Single mode is when every rendered gameobject is sorted to individual group
            Single = 0,
            //Hierarhy mode makes sorting considering specified level of hierarchy
            Hierarchy
        }
        #endregion

        #region Public Fields
        //classes with MetaUnity "prefix" are custom Serializable events
        /// <summary>
        /// On destroying part
        /// </summary>
        public MetaUnityAction _unityDestroy = new MetaUnityAction();

        public MetaUnityGrouperPartAction _unityPartAdd = new MetaUnityGrouperPartAction();

        public MetaUnityGrouperPartAction _unityPartDestroy = new MetaUnityGrouperPartAction();

        public MetaUnityGrouperPartAction _unityPartModified = new MetaUnityGrouperPartAction();

        public MetaUnityGrouperPartSubObjectAction _unitySubObjectAdd = new MetaUnityGrouperPartSubObjectAction();

        public MetaUnityGrouperPartSubObjectAction _unitySubObjectDestroy = new MetaUnityGrouperPartSubObjectAction();

        public MetaUnityGrouperPartSubObjectAction _unitySubObjectModified = new MetaUnityGrouperPartSubObjectAction();
        #endregion

        #region Serialized Fields
        //collection of parts
        [SerializeField]
        private List<GrouperPart> _parts = new List<GrouperPart>();
        //collection of gameobjects in parts. They're not inside of parts because of serialization system
        [SerializeField]
        private List<GameObject> _gameObjects = new List<GameObject>();
        [SerializeField]
        private AutoDistributionMode _autoDistributionMode = AutoDistributionMode.Single;
        [SerializeField]
        private int _hierarchyLevelOfAutoDistributionRequired = 1;
#if UNITY_EDITOR
        //which part is selected now
        [SerializeField]
        private int _currentlySelectedPart = -1;
#endif
        #endregion

        #region Public Properties
        public List<GameObject> GameObjects
        {
            get
            {
                return _gameObjects;
            }
            private set
            {
                _gameObjects = value;
            }
        }

        public List<GrouperPart> Parts
        {
            get
            {
                return _parts;
            }
            set
            {
                _parts = value;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _parts.Count == 0;
            }
        }
        #endregion

        #region Monobehaviour Lifecycle
        private void Awake()
        {
#if UNITY_EDITOR
            //it happens when parts wasn't deselected in editor mode despite our actions
            if (_currentlySelectedPart >= 0)
            {
                Utilities.DeselectInGame(GameObjects.ToArray());
                _currentlySelectedPart = -1;
            }
#endif
        }
        #endregion

        #region Main Methods
        //main function of auto-distribution
        public void PerformAutoDistribution()
        {
            Clear();

            GameObjects.Clear();

            switch (_autoDistributionMode)
            {
                case AutoDistributionMode.Single:
                    GameObject[] gameObjects = Utilities.GetMeshRenderedGameObjects(gameObject);
                    for (int i = 0; i < gameObjects.Length; i++)
                    {
                        SkinnedMeshRenderer skinned = gameObjects[i].GetComponent<SkinnedMeshRenderer>();
                        if (skinned != null)
                        {
                            GameObject rootBone = skinned.rootBone.gameObject;
                            if (rootBone != null)
                            {

                                if (!GameObjects.Contains(rootBone))
                                {
                                    AddPart(new GameObject[] { rootBone }, rootBone.name);
                                }
                            }
                        }

                        AddPart(new GameObject[] { gameObjects[i] }, gameObjects[i].name);
                    }
                    break;
                case AutoDistributionMode.Hierarchy:
                    int level = 0;
                    addAllGOsOnLevel(gameObject, _parts, ref level, _hierarchyLevelOfAutoDistributionRequired);
                    break;
            }
        }

        //Recursive function of heirarchy auto-distribution
        private void addAllGOsOnLevel(GameObject root, List<GrouperPart> list, ref int currentLevel, int requiredLevel)
        {
            if (currentLevel == requiredLevel || root.transform.childCount == 0)
            {
                AddPart(Utilities.GetMeshRenderedGameObjects(root), root.name);
                return;
            }

            currentLevel++;
            for (int i = 0; i < root.transform.childCount; i++)
            {
                addAllGOsOnLevel(root.transform.GetChild(i).gameObject, list, ref currentLevel, requiredLevel);
            }
            currentLevel--;
        }

        public void Destroy()
        {
            /*
             * Like GrouperPart, even if Grouper is empty we don't destroy it, until user explicitly ask for it
             * When destroying Grouper we first destroying all parts, then destroying ourselves
             */

            Clear();

            if (_unityDestroy.Get() != null)
            {
                _unityDestroy.Invoke();
            }
        }
        #endregion

        #region Interface Methods
        public void Clear()
        {
            while (Parts.Count > 0)
            {
                while (Parts[0].SubObjectsList.Count > 0)
                {
                    RemoveSubObject(Parts[0], Parts[0].SubObjectsList[0]);
                }

                RemovePart(Parts[0]);
            }
        }

        /// <summary>
        /// Removes Part from parts collection of Grouper
        /// </summary>
        /// <param name="part"></param>
        public void RemovePart(GrouperPart part)
        {
            if (_unityPartDestroy.Get() != null)
            {
                _unityPartDestroy.Invoke(part);
            }

            Parts.Remove(part);
        }

        /// <summary>
        /// Removes Subobject from specific part of Grouper
        /// </summary>
        /// <param name="part"></param>
        /// <param name="subObject"></param>
        public void RemoveSubObject(GrouperPart part, GrouperPartSubObject subObject)
        {
            if (_unitySubObjectDestroy.Get() != null)
            {
                _unitySubObjectDestroy.Invoke(part, subObject);
            }

            part.SubObjectsList.Remove(subObject);
        }

        /// <summary>
        /// Creates new Part
        /// </summary>
        /// <returns></returns>
        public GrouperPart AddPart()
        {
            GrouperPart newPart = new GrouperPart(_parts.Count);
            _parts.Add(newPart);

            if (_unityPartAdd.Get() != null)
            {
                _unityPartAdd.Invoke(newPart);
            }

            return newPart;
        }

        /// <summary>
        /// Creates new Part with specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GrouperPart AddPart(string name)
        {
            GrouperPart newPart = new GrouperPart(_parts.Count);
            newPart.Name = name;
            _parts.Add(newPart);

            if (_unityPartAdd.Get() != null)
            {
                _unityPartAdd.Invoke(newPart);
            }

            return newPart;
        }

        /// <summary>
        /// Creates new Part with specified gameObjects and optional name
        /// </summary>
        /// <param name="initialSources"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public GrouperPart AddPart(GameObject[] initialSources, string name = "")
        {
            GrouperPart newPart = new GrouperPart(_parts.Count);
            newPart.Name = name;
            _parts.Add(newPart);

            if (_unityPartAdd.Get() != null)
            {
                _unityPartAdd.Invoke(newPart);
            }

            if (initialSources.Length > 0)
            {
                for (int i = 0; i < initialSources.Length; i++)
                {
                    if (!GameObjects.Contains(initialSources[i]))
                    {
                        GameObjects.Add(initialSources[i]);
                    }

                    if (newPart.Name == "")
                    {
                        newPart.Name = initialSources[i].name;
                    }

                    GrouperPartSubObject subObject = newPart.Add(GameObjects.IndexOf(initialSources[i]));

                    if (_unitySubObjectAdd.Get() != null)
                    {
                        _unitySubObjectAdd.Invoke(newPart, subObject);
                    }
                }
            }

            return newPart;
        }

        /// <summary>
        /// Adds SubObject to Part
        /// </summary>
        /// <param name="part"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public GrouperPartSubObject AddSubObject(GrouperPart part, int index)
        {
            GrouperPartSubObject newSubObject = part.Add(index);

            if (_unitySubObjectAdd.Get() != null)
            {
                _unitySubObjectAdd.Invoke(part, newSubObject);
            }

            return newSubObject;
        }

        /// <summary>
        /// Adds SubObject to Part
        /// </summary>
        /// <param name="part"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public GrouperPartSubObject AddSubObject(GrouperPart part, GameObject gameObject)
        {
            if (!GameObjects.Contains(gameObject))
            {
                GameObjects.Add(gameObject);
            }

            GrouperPartSubObject newSubObject = part.Add(GameObjects.IndexOf(gameObject));

            if (_unitySubObjectAdd.Get() != null)
            {
                _unitySubObjectAdd.Invoke(part, newSubObject);
            }

            return newSubObject;
        }

        /// <summary>
        /// Set Parts Name
        /// </summary>
        /// <param name="part"></param>
        /// <param name="name"></param>
        public void SetPartsName(GrouperPart part, string name)
        {
            part.Name = name;

            if (_unityPartModified.Get() != null)
            {
                _unityPartModified.Invoke(part);
            }
        }

        /// <summary>
        /// Replace Source gameObject of SubObject to a gameObject from Grouper's collection of gameObject
        /// </summary>
        /// <param name="part"></param>
        /// <param name="subObject"></param>
        /// <param name="index"></param>
        public void ReplaceSourceOfSubObject(GrouperPart part, GrouperPartSubObject subObject, int index)
        {
            subObject.SubObjectIndex = index;

            if (_unitySubObjectModified.Get() != null)
            {
                _unitySubObjectModified.Invoke(part, subObject);
            }
        }

        /// <summary>
        /// Replace Source gameObject of SubObject to a specified gameObject
        /// </summary>
        /// <param name="part"></param>
        /// <param name="subObject"></param>
        /// <param name="gameObject"></param>
        public void ReplaceSourceOfSubObject(GrouperPart part, GrouperPartSubObject subObject, GameObject gameObject)
        {
            if (!GameObjects.Contains(gameObject))
            {
                GameObjects.Add(gameObject);
            }

            subObject.SubObjectIndex = GameObjects.IndexOf(gameObject);

            if (_unitySubObjectModified.Get() != null)
            {
                _unitySubObjectModified.Invoke(part, subObject);
            }
        }

        /// <summary>
        /// Get all gameObject that are included in that part
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public GameObject[] GetGameObjectsFromPart(GrouperPart part)
        {
            List<GameObject> result = new List<GameObject>();

            for (int i = 0; i < part.SubObjectsList.Count; i++)
            {
                result.Add(GameObjects[part.SubObjectsList[i].GameObjectIndex]);
            }

            return result.ToArray();
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Gets index of gameObject in a Groupers collection
        /// </summary>
        /// <param name="partIndex"></param>
        /// <param name="elementIndex"></param>
        /// <returns></returns>
        public int GetElementIndex(int partIndex, int elementIndex)
        {
            return _parts[partIndex].SubObjectsList[elementIndex].GameObjectIndex;
        }
        #endregion
    }
}
