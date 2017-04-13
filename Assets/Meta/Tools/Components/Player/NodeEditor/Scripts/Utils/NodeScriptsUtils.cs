using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.Tools
{
    public static class NodeScriptsUtils
    {
        public static void DeleteAsset(ScriptableObject scrObject)
        {
#if UNITY_EDITOR
            if (scrObject != null)
            {
                GameObject.DestroyImmediate(scrObject, true);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
#endif
        }
        public static void SaveAsset()
        {
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }
}
