using System.Text;
using UnityEngine;

namespace Meta.Tools
{
    public partial class Grouper : MonoBehaviour
    {
        /// <summary>
        /// Wrapper-related function, that calculates Hash of GameObject, considering related Objects of all children.
        /// Currently related Objects:
        /// 1. Transform
        /// </summary>
        /// <returns>Hash of GameObject</returns>
        private int calculateHashCodeOfGameObject()
        {
            Transform[] children = GetComponentsInChildren<Transform>();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < children.Length; i++)
            {
                sb.Append(children[i].GetInstanceID());
            }

            return sb.ToString().GetHashCode();
        }
    }
}
