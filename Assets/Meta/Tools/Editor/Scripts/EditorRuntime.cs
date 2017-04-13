using UnityEditor;

namespace Meta.Tools.Editor
{
    [InitializeOnLoad]
    public class EditorRuntime
    {
        static EditorRuntime()
        {
            EditorApplication.update += Update;
        }

        // Update is called once per frame
        static void Update()
        {
            Utilities.UpdatePhasesOfSelectionMaterials((float)EditorApplication.timeSinceStartup);
        }
    }
}
