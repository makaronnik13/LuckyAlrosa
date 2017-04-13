using System;
using UnityEngine;

namespace Meta.Tools
{
    /// <summary>
    /// Class describing part's sub-object.
    /// </summary>
    [Serializable]
    public class GrouperPartSubObject
    {
        #region Serialized Fields
        [SerializeField]
        private int _subObjectIndex = -1;
        [SerializeField]
        private int _gameObjectIndex = -1;
        #endregion

        #region Public Properties
        public int SubObjectIndex
        {
            get
            {
                return _subObjectIndex;
            }
            set
            {
                _subObjectIndex = value;
            }
        }

        public int GameObjectIndex
        {
            get
            {
                return _gameObjectIndex;
            }
            set
            {
                _gameObjectIndex = value;
            }
        }
        #endregion

        #region Constructor
        public GrouperPartSubObject(int gameObjectIndex, int subObjectIndex)
        {
            SubObjectIndex = subObjectIndex;
            GameObjectIndex = gameObjectIndex;
        }
        #endregion
    }
}
