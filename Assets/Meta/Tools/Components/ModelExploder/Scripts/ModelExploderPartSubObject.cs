using System;
using UnityEngine;
using System.Collections.Generic;

namespace Meta.Tools
{
    [Serializable]
    public class ModelExploderPartSubObject
    {
        #region Public Fields
        public List<CustomTransform> RelativeTransform = new List<CustomTransform>();
        #endregion

        #region Serilized Fields
        [SerializeField]
        private int _grouperSubObjectIndex = -1;
        [SerializeField]
        private int _modelExploderSubObjectIndex = -1;
        [SerializeField]
        private int _gameObjectIndex = -1;
        #endregion

        #region Private Fields
        [SerializeField]
        private bool _isOrigin;
        #endregion

        #region Public Properties
        public int GrouperSubObjectIndex
        {
            get
            {
                return _grouperSubObjectIndex;
            }
            set
            {
                _grouperSubObjectIndex = value;
            }
        }

        public int ModelExploderSubObjectIndex
        {
            get
            {
                return _modelExploderSubObjectIndex;
            }
            set
            {
                _modelExploderSubObjectIndex = value;
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

        public bool IsOrigin
        {
            get
            {
                return _isOrigin;
            }
            set
            {
                _isOrigin = value;
            }
        }
        #endregion

        #region Constructor
        public ModelExploderPartSubObject(int gameObjectIndex, int grouperSubObjectIndex, int modelExplodeSubOjectIndex)
        {
            GameObjectIndex = gameObjectIndex;
            GrouperSubObjectIndex = grouperSubObjectIndex;
            ModelExploderSubObjectIndex = modelExplodeSubOjectIndex;

            AddNewPositions();
        }
        #endregion

        #region Interface Methods
        public void AddNewPositions(int baseIndex = -1)
        {
            if (baseIndex >= 0)
            {
                RelativeTransform.Add(RelativeTransform[baseIndex].Clone() as CustomTransform);
            }
            else
            {
                RelativeTransform.Add(new CustomTransform(Vector3.zero, Vector3.zero, Vector3.zero));
            }
        }

        public void RemovePosiotions(int i)
        {
            RelativeTransform.RemoveAt(i);
        }
        #endregion
    }
}
