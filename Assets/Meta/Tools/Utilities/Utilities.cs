using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization;
#if UNITY_EDITOR
using System.Runtime.Serialization.Formatters.Binary;
#endif
using System.Linq;

namespace Meta.Tools
{
    public static class Utilities
    {
        #region GUI Layout

        private static GUISkin _timelineSkin;
        private static GUISkin _exploderSkin;
 
         public static GUISkin ExploderSkin
         {
             get
             {
                 if (_exploderSkin == null)
                 {
                     _exploderSkin = (GUISkin)Resources.Load("GUISkins/EditorSkins/ModelExploderSkin");
                 }
                 return _exploderSkin;
             }
         }

        public static GUISkin TimelineSkin
        {
            get
            {
                if (_timelineSkin == null)
                {
                    _timelineSkin = (GUISkin)Resources.Load("GUISkins/EditorSkins/TimelineEditorSkin");
                }
                return _timelineSkin;
            }
        }

        public static GUIStyle TitleStyle
        {
            get
            {
                var guiTitleStyle = new GUIStyle();
                guiTitleStyle.fontSize = 16;
                guiTitleStyle.richText = true;
                return guiTitleStyle;
            }
        }

        public static GUIStyle RichTextStyle
        {
            get
            {
                var guiTitleStyle = new GUIStyle();
                guiTitleStyle.richText = true;
                return guiTitleStyle;
            }
        }

        public static GUIStyle RichClippedLeftCenterTextStyle
        {
            get
            {
                var guiTitleStyle = new GUIStyle();
                guiTitleStyle.richText = true;
                guiTitleStyle.clipping = TextClipping.Clip;
                guiTitleStyle.alignment = TextAnchor.MiddleLeft;
                return guiTitleStyle;
            }
        }

        public static GUIStyle RichWordWrapCenterTextStyle
        {
            get
            {
                var guiTitleStyle = new GUIStyle();
                guiTitleStyle.richText = true;
                guiTitleStyle.wordWrap = true;
                guiTitleStyle.alignment = TextAnchor.MiddleCenter;
                return guiTitleStyle;
            }
        }

        public static string Hex(this Color color)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}",
                (int)(color.r * 255),
                (int)(color.g * 255),
                (int)(color.b * 255));
        }

        public static Color ActiveElementColor = new Color(0.8941f, 0.8980f, 0.8941f);

        public static Color InactiveElementColor = Color.white * 0.92f;

        public static Color UniqueActiveElementColor = new Color(0.8941f, 0.54f, 0.38f);

        public static Color UniqueInactiveActiveElementColor = new Color(0.92f, 0.84f, 0.78f);

        public static Color GoodResultColor = new Color(0f, 0.5f, 0f);

        public static Color AverageResultColor = new Color(0.5f, 0f, 0f);

        public static Color BadResultColor = new Color(0f, 0f, 0.5f);

        #endregion GUI Layout

        #region GameObject's Statistics
        #region Objects Counters

        /// <summary>
        /// Returns overall count of children of a passed GameObject.
        /// </summary>
        /// <param name="GO">Root of hierarchy</param>
        /// <param name="includeInactive">If true - inactive children will also be counted</param>
        /// <returns></returns>
        public static int GetGameObjectsCount(GameObject gameObject, bool includeInactive = true)
        {
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>(includeInactive);
            return transforms.Length;
        }
        /// <summary>
        /// Returns all Transforms of a passed GameObject and it's children.
        /// </summary>
        /// <param name="GO">Root of hierarchy</param>
        /// <param name="includeInactive">If true - inactive children will also be counted</param>
        /// <returns></returns>
        public static Transform[] GetGameObjectsTransforms(GameObject gameObject, bool includeInactive = true)
        {
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>(includeInactive);
            return transforms;
        }

        /// <summary>
        /// Returns overall count of children that renders itself
        /// </summary>
        /// <param name="GO">Root of hierarchy</param>
        /// <param name="includeInactive">If true - inactive children will also be counted</param>
        /// <param name="includeDisabled">If true - disabled renderers will also be counted</param>
        /// <returns></returns>
        public static int GetMeshRenderedGameObjectsCount(GameObject gameObject, bool includeInactive = true, bool includeDisabled = true)
        {
            Renderer[] transforms = gameObject.GetComponentsInChildren<Renderer>(includeInactive);

            int counter = 0;
            if (includeDisabled)
            {
                for (int i = 0; i < transforms.Length; i++)
                {
                    if (transforms[i].GetComponent<Renderer>() != null)
                    {
                        counter++;
                    }
                }
            }
            else
            {
                Renderer rend;
                for (int i = 0; i < transforms.Length; i++)
                {
                    rend = transforms[i].GetComponent<Renderer>();
                    if (rend != null && rend.enabled)
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }

        /// <summary>
        /// Returns overall count of children that renders itself
        /// </summary>
        /// <param name="GO">Root of hierarchy</param>
        /// <param name="includeInactive">If true - inactive children will also be counted</param>
        /// <param name="includeDisabled">If true - disabled renderers will also be counted</param>
        /// <returns></returns>
        public static GameObject[] GetMeshRenderedGameObjects(GameObject gameObject, bool includeInactive = true, bool includeDisabled = true)
        {
            Renderer[] transforms = gameObject.GetComponentsInChildren<Renderer>(includeInactive);
            List<GameObject> gos = new List<GameObject>();

            if (includeDisabled)
            {
                for (int i = 0; i < transforms.Length; i++)
                {
                    if (transforms[i].GetComponent<Renderer>() != null)
                    {
                        gos.Add(transforms[i].gameObject);
                    }
                }
            }
            else
            {
                Renderer rend;
                for (int i = 0; i < transforms.Length; i++)
                {
                    rend = transforms[i].GetComponent<Renderer>();
                    if (rend != null && rend.enabled)
                    {
                        gos.Add(transforms[i].gameObject);
                    }
                }
            }

            return gos.ToArray();
        }

        public static Vector3 AveragePosition(GameObject[] gameObjects)
        {
            Vector3 pos = Vector3.zero;
            int i = 0;
            for (; i < gameObjects.Length; i++)
            {
                pos += gameObjects[i].transform.position;
            }
            return pos / i;
        }

        public static Vector3 AverageLocalPosition(GameObject[] gameObjects)
        {
            Vector3 pos = Vector3.zero;
            int i = 0;
            for (; i < gameObjects.Length; i++)
            {
                pos += gameObjects[i].transform.localPosition;
            }
            return pos / i;
        }

        #endregion Objects Counters

        #region Bounding Boxes

        public static Vector3 GetGameObjectsAABBBoundingBoxsDimensions(GameObject gameObject, bool includeInactive = true, bool includeDisabled = true)
        {
            Bounds bounds = new Bounds();

            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(includeInactive);
            if (includeDisabled)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
            }
            else
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i].enabled)
                    {
                        bounds.Encapsulate(renderers[i].bounds);
                    }
                }
            }

            return bounds.extents * 2f;
        }

        public static Bounds GetGameObjectsAABBBoundingBox(GameObject gameObject, bool includeInactive = true, bool includeDisabled = true)
        {
            Bounds bounds = new Bounds();

            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(includeInactive);
            if (includeDisabled)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
            }
            else
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i].enabled)
                    {
                        bounds.Encapsulate(renderers[i].bounds);
                    }
                }
            }

            return bounds;
        }

        #endregion Bounding Boxes
        #endregion GameObject's Statistics

        #region Scene-view Selection system

        /*
         * Selection system will be implemented using following rules:
         * 1.) When we asked to select some object's, we save them in the array.
         * 2.) If we asked to deselect object's, we deselect them (cool, huh?)
         */

        private static List<GameObject> selectedInSceneView = new List<GameObject>();

        public enum SceneViewSelectionType
        {
            Default,
            Variant1,
            Variant2,
            Variant3,
        }

        /// <summary>
        /// As soon as there is no updates supported to _Time variable in editor-mode, we must provide time variable by ourselves
        /// </summary>
        /// <param name="value">Current in-editor time</param>
        public static void UpdatePhasesOfSelectionMaterials(float value)
        {
            SceneViewSelectionType.Default.Material().SetFloat("_Phase", value);
            SceneViewSelectionType.Variant1.Material().SetFloat("_Phase", value);
            SceneViewSelectionType.Variant2.Material().SetFloat("_Phase", value);
            SceneViewSelectionType.Variant3.Material().SetFloat("_Phase", value);
        }

        /// <summary>
        /// Removes all existing selections of all types form scene view
        /// </summary>
        public static void DeselectAllInSceneView()
        {
            while (selectedInSceneView.Count > 0)
            {
                deselect(selectedInSceneView[0], SceneViewSelectionType.Default);
                deselect(selectedInSceneView[0], SceneViewSelectionType.Variant1);
                deselect(selectedInSceneView[0], SceneViewSelectionType.Variant2);
                deselect(selectedInSceneView[0], SceneViewSelectionType.Variant3);
                selectedInSceneView.RemoveAt(0);
            }
        }

        /// <summary>
        /// Removes all types of selection from sertain GameObjects. User in game view
        /// </summary>
        /// <param name="gameObjects">GameObjects to deselect</param>
        public static void DeselectInGame(GameObject[] gameObjects)
        {
            if (gameObjects != null)
            {
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    deselect(gameObjects[i], SceneViewSelectionType.Default);
                    deselect(gameObjects[i], SceneViewSelectionType.Variant1);
                    deselect(gameObjects[i], SceneViewSelectionType.Variant2);
                    deselect(gameObjects[i], SceneViewSelectionType.Variant3);
                }
                /*for (int i = 0; i < gameObjects.Length; i++)
                {
                    deselect(gameObjects[i], SceneViewSelectionType.Variant1);
                }
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    deselect(gameObjects[i], SceneViewSelectionType.Variant2);
                }
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    deselect(gameObjects[i], SceneViewSelectionType.Variant3);
                }*/
            }
        }

        /// <summary>
        /// Removes all types of selection from sertain GameObjects. User in game view
        /// </summary>
        /// <param name="gameObjects">GameObjects to deselect</param>
        public static void DeselectInGame(GameObject gameObject)
        {
            if (gameObject != null)
            {
                deselect(gameObject, SceneViewSelectionType.Default);
                deselect(gameObject, SceneViewSelectionType.Variant1);
                deselect(gameObject, SceneViewSelectionType.Variant2);
                deselect(gameObject, SceneViewSelectionType.Variant3);
            }
        }

        /// <summary>
        /// Removes all existing selections of particular type form scene view
        /// </summary>
        /// <param name="selectionType">Wich type of selection to consider</param>
        public static void DeselectAllSelectionsInSceneView(SceneViewSelectionType selectionType)
        {
            while (selectedInSceneView.Count > 0)
            {
                deselect(selectedInSceneView[0], selectionType);
            }
        }

        /// <summary>
        /// Removes specific selection of particular type form scene view
        /// </summary>
        /// <param name="gameObject">Target GameObject</param>
        /// <param name="selectionType">Type of selection</param>
        public static void DeselectInSceneView(GameObject gameObject, SceneViewSelectionType selectionType)
        {
            deselect(gameObject, selectionType);
        }

        /// <summary>
        /// Applies specific type of selection to an array of GameObjects.
        /// </summary>
        /// <param name="gameObjects">Array of target GameObjects</param>
        /// <param name="selectionType">Type of selection</param>
        public static void SelectGameObjectsInSceneView(GameObject[] gameObjects, SceneViewSelectionType selectionType)
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                select(gameObjects[i], selectionType);
            }
        }

        private static void select(GameObject go, SceneViewSelectionType selectionType)
        {
            if (selectionPossible(go))
            {
                Material targetMaterial = selectionType.Material();
                if (targetMaterial != null)
                {
                    Material[] materials = go.GetComponent<Renderer>().sharedMaterials;

                    if (materials.IndexOf(targetMaterial) < 0)
                    {
                        Material[] newMaterials = new Material[materials.Length + 1];

                        int i = 0;
                        for (; i < materials.Length; i++)
                        {
                            newMaterials[i] = materials[i];
                        }

                        newMaterials[i] = targetMaterial;
                        go.GetComponent<Renderer>().sharedMaterials = newMaterials;
                    }

                    if (selectedInSceneView.IndexOf(go) < 0)
                    {
                        selectedInSceneView.Add(go);
                    }
                }
            }
        }

        private static void deselect(GameObject go, SceneViewSelectionType selectionType)
        {
            if (selectionPossible(go))
            {
                Material targetMaterial = selectionType.Material();
                if (targetMaterial != null)
                {
                    Material[] materials = go.GetComponent<Renderer>().sharedMaterials;
                    int index = materials.IndexOf(targetMaterial);
                    if (index >= 0)
                    {
                        Material[] newMaterials = new Material[materials.Length - 1];

                        for (int i = 0, j = 0; i < newMaterials.Length; i++, j++)
                        {
                            if (materials[j] == targetMaterial)
                            {
                                j++;
                            }

                            newMaterials[i] = materials[j];
                        }

                        go.GetComponent<Renderer>().sharedMaterials = newMaterials;
                    }
                }
            }
        }

        private static bool selectionPossible(GameObject go)
        {
            return go != null && go.GetComponent<Renderer>() != null;
        }

        private static int IndexOf(this Material[] mats, Material mat)
        {
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] == mat)
                {
                    return i;
                }
            }

            return -1;
        }

        private static int IndexOf(this GameObject[] gos, GameObject go)
        {
            for (int i = 0; i < gos.Length; i++)
            {
                if (gos[i] == go)
                {
                    return i;
                }
            }

            return -1;
        }

        public static Material Material(this SceneViewSelectionType mat)
        {
            switch (mat)
            {
                case SceneViewSelectionType.Variant1:
                    //Debug.Log("Variant1");
                    return (Material)Resources.Load("Materials/Variant1SelectionMaterial");
                case SceneViewSelectionType.Variant2:
                    //Debug.Log("Variant2");
                    return (Material)Resources.Load("Materials/Variant2SelectionMaterial");
                case SceneViewSelectionType.Variant3:
                    //Debug.Log("Variant3");
                    return (Material)Resources.Load("Materials/Variant3SelectionMaterial");
                case SceneViewSelectionType.Default:
                    //Debug.Log("Default");
                    return (Material)Resources.Load("Materials/DefaultSelectionMaterial");
            }
            return null;
        }

        #endregion Scene-view Selection system

        public static bool IsModelPivotsOk(GameObject go)
        {
            bool result = true;

            foreach (GameObject part in GetMeshRenderedGameObjects(go))
            {
                result = result && IsPivotOk(part);
            }

            return result;
        }

        public static bool IsPivotOk(GameObject go)
        {
            Renderer rend = go.GetComponent<Renderer>();
            if (rend as MeshRenderer)
            {
                MeshFilter filter = go.GetComponent<MeshFilter>();
                if (filter && filter.sharedMesh)
                {
                    if (Vector3.Distance(Vector3.zero, filter.sharedMesh.bounds.center) > 0.01f)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

            }

            if (rend as SkinnedMeshRenderer)
            {
                SkinnedMeshRenderer filter = go.GetComponent<SkinnedMeshRenderer>();
                if (filter && filter.sharedMesh)
                {
                    if (Vector3.Distance(go.transform.position, filter.sharedMesh.bounds.center) > 0.01f)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

            }

            return true;
        }

        public static Bounds TransformBounds(this Transform _transform, Bounds _localBounds)
        {
            var center = _transform.TransformPoint(_localBounds.center);

            // transform the local extents' axes
            var extents = _localBounds.extents;
            var axisX = _transform.TransformVector(extents.x, 0, 0);
            var axisY = _transform.TransformVector(0, extents.y, 0);
            var axisZ = _transform.TransformVector(0, 0, extents.z);

            // sum their absolute value to get the world extents
            extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
            extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
            extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

            return new Bounds { center = center, extents = extents };
        }

        public static void FixModelPivot(GameObject model)
        {
  
            foreach (GameObject go in GetMeshRenderedGameObjects(model))
            {
                Renderer rend = go.GetComponent<Renderer>();
                if (rend as MeshRenderer)
                {
                    Vector3 center;
                    MeshFilter filter = go.GetComponent<MeshFilter>();
                    Mesh meshCopy = Mesh.Instantiate(filter.sharedMesh) as Mesh;  //make a deep copy
                    if (filter && meshCopy)
                    { 
                        center = go.GetComponent<MeshRenderer>().bounds.center;
                        Vector3[] oldVertPositions = meshCopy.vertices;
                        for (int i = 0; i < oldVertPositions.Length; i++)
                        {
                            oldVertPositions[i] -= meshCopy.bounds.center;
                        }
                        meshCopy.SetVertices(oldVertPositions.ToList());
                        meshCopy.RecalculateBounds();                
                        go.transform.position = center;
                        filter.mesh = meshCopy;
                    }
                }

                if (rend as SkinnedMeshRenderer)
                {
                    Vector3 center;
                    SkinnedMeshRenderer skin = go.GetComponent<SkinnedMeshRenderer>();
                    Mesh meshCopy = Mesh.Instantiate(skin.sharedMesh) as Mesh;  //make a deep copy
                    if (skin && meshCopy)
                    {
                        center = go.GetComponent<SkinnedMeshRenderer>().bounds.center;
                        Vector3[] oldVertPositions = meshCopy.vertices;
                        for (int i = 0; i < oldVertPositions.Length; i++)
                        {
                            oldVertPositions[i] -= meshCopy.bounds.center;
                        }
                        meshCopy.SetVertices(oldVertPositions.ToList());
                        meshCopy.RecalculateBounds();
                        go.transform.position = center;
                        skin.BakeMesh(meshCopy);
                    }
                }
            }
        }

        public static Vector3 Divide(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static bool AbsBetween(float a, float b, float target)
        {
            return (target >= a && target < b) || (target >= b && target < a);
        }

        public static Rect Encapsulate(Rect rect1, Rect rect2)
        {
            Rect result = new Rect();
            Vector2 topLeft1 = new Vector2(rect1.x, rect1.y);
            Vector2 bottomRight1 = new Vector2(rect1.x + rect1.width, rect1.y + rect1.height);
            Vector2 topLeft2 = new Vector2(rect2.x, rect2.y);
            Vector2 bottomRight2 = new Vector2(rect2.x + rect2.width, rect2.y + rect2.height);
            if (topLeft1.x < topLeft2.x)
            {
                result.x = topLeft1.x;
            }
            else
            {
                result.x = topLeft2.x;
            }
            if (topLeft1.y < topLeft2.y)
            {
                result.y = topLeft1.y;
            }
            else
            {
                result.y = topLeft2.y;
            }
            if (bottomRight1.x > bottomRight2.x)
            {
                result.width = bottomRight1.x - result.x;
            }
            else
            {
                result.width = bottomRight2.x - result.x;
            }
            if (bottomRight1.y > bottomRight2.y)
            {
                result.height = bottomRight1.y - result.y;
            }
            else
            {
                result.height = bottomRight2.y - result.y;
            }
            return result;
        }


        /// <summary>
        /// Serializes 'value' to a string, using BinaryFormatter
        /// </summary>
        public static string SerializeToString<T>(T value)
        {
            using (var stream = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(stream, value);

                //(new BinaryFormatter()).Serialize(stream, value);
                stream.Flush();
                return Convert.ToBase64String(stream.ToArray());
            }
        }
        /// <summary>
        /// Deserializes an object of type T from the string 'data'
        /// </summary>
        public static T DeserializeFromString<T>(string data)
        {
            byte[] bytes = Convert.FromBase64String(data);
            using (var stream = new MemoryStream(bytes))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                var record = (T)serializer.ReadObject(stream);

                return record;//(T)(new BinaryFormatter()).Deserialize(stream);
            }
        }

    }
}