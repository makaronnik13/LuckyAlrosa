using System;
using UnityEngine;
using System.Collections.Generic;

namespace Meta.Tools
{
    /// <summary>
    /// Class describing single logical part, that is a group of sub-objects, that should be treated as one.
    /// </summary>
    [Serializable]
    public class GrouperPart
    {
        #region Public Fields
        /// <summary>
        /// SubObjects in the part
        /// </summary>
        public List<GrouperPartSubObject> SubObjectsList = new List<GrouperPartSubObject>();
        #endregion

        #region Serialized Fields
        [SerializeField]
        private int _partIndex = -1;
        [SerializeField]
        private string _name = "";

#if UNITY_EDITOR
        [SerializeField]
        private bool _partObserved;
        [SerializeField]
        private bool _partMembersObserved;
        [SerializeField]
        private float _currentElementHeight;
#endif
        #endregion

        #region Public Properties
        public int PartIndex
        {
            get
            {
                return _partIndex;
            }
            set
            {
                _partIndex = value;
            }
        }

        /// <summary>
        /// Name of part
        /// </summary>
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

        public bool IsEmpty
        {
            get
            {
                return SubObjectsList.Count == 0;
            }
        }
        #endregion

        #region Constructor
        public GrouperPart(int index)
        {
            PartIndex = index;
        }
        #endregion

        #region Interface Methods
        public GrouperPartSubObject Add(int gameObjectIndex)
        {
            return initializeNewSubObject(gameObjectIndex);
        }
        #endregion

        #region Utility Methods
        private GrouperPartSubObject initializeNewSubObject(int gameObjectIndex)
        {
            GrouperPartSubObject newSubObject = new GrouperPartSubObject(gameObjectIndex, SubObjectsList.Count);

            SubObjectsList.Add(newSubObject);

            return newSubObject;
        }
        #endregion
    }
}
