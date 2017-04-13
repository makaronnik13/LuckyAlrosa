using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    [CustomEditor(typeof(Tweener))]
    public class TweenerEditor : UnityEditor.Editor
    {
        private Tweener tweener;

        private void OnEnable()
        {
            tweener = target as Tweener;
        }

        public override void OnInspectorGUI()
        {
            GUIContent explosionLabelContent = new GUIContent();
            explosionLabelContent.text = "<b><i><size=18>  Tweener </size></i></b>";
            explosionLabelContent.tooltip = "Component that tweens everything!";
            EditorGUILayout.LabelField(explosionLabelContent, Utilities.RichTextStyle);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tween Target:");
            serializedObject.FindProperty("_containingMonoBehaviour").objectReferenceValue = EditorGUILayout.ObjectField(serializedObject.FindProperty("_containingMonoBehaviour").objectReferenceValue, typeof(MonoBehaviour), true) as MonoBehaviour;
            if (serializedObject.FindProperty("_containingMonoBehaviour").objectReferenceValue != null)
            {
                MonoBehaviour[] scripts = (serializedObject.FindProperty("_containingMonoBehaviour").objectReferenceValue as MonoBehaviour).GetComponents<MonoBehaviour>();
                bool founded = false;
                for (int i = 0; i < scripts.Length; i++)
                {
                    if (scripts[i] is IInputFloatValue && scripts[i] != target as MonoBehaviour)
                    {
                        founded = true;
                        tweener.Target = (scripts[i] as IInputFloatValue);
                        serializedObject.FindProperty("_containingMonoBehaviour").objectReferenceValue = scripts[i];
                        break;
                    }
                }
                if (!founded)
                {
                    tweener.Target = null;
                    serializedObject.FindProperty("_containingMonoBehaviour").objectReferenceValue = null;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            Rect lastRect = GUILayoutUtility.GetLastRect();
            EditorGUI.LabelField(new Rect(lastRect.x, lastRect.y, 50, EditorGUIUtility.singleLineHeight), "<b><Size=14>From:</size></b>", Utilities.RichTextStyle);
            tweener.From = EditorGUI.FloatField(new Rect(lastRect.x + 50, lastRect.y, 30, EditorGUIUtility.singleLineHeight), tweener.From);
            EditorGUI.LabelField(new Rect(lastRect.x + 100, lastRect.y, 30, EditorGUIUtility.singleLineHeight), "<b><Size=14>To:</size></b>", Utilities.RichTextStyle);
            tweener.To = EditorGUI.FloatField(new Rect(lastRect.x + 130, lastRect.y, 30, EditorGUIUtility.singleLineHeight), tweener.To);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            lastRect = GUILayoutUtility.GetLastRect();
            EditorGUI.LabelField(new Rect(lastRect.x, lastRect.y, 90, EditorGUIUtility.singleLineHeight), "<b><Size=14>Time (sec.):</size></b>", Utilities.RichTextStyle);
            tweener.Time = EditorGUI.FloatField(new Rect(lastRect.x + 100, lastRect.y, 30, EditorGUIUtility.singleLineHeight), tweener.Time);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Ease Type:");
            tweener.EaseType = (LeanTweenType)EditorGUILayout.EnumPopup(tweener.EaseType);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Delay:");
            tweener.Delay = EditorGUILayout.FloatField(tweener.Delay);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Loop Type:");
            tweener.LoopType = (Tweener.LoopTypes)EditorGUILayout.EnumPopup(tweener.LoopType);
            EditorGUILayout.EndHorizontal();

            if (tweener.LoopType == Tweener.LoopTypes.Regular)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Loops Count:");
                tweener.LoopsCount = EditorGUILayout.IntField(tweener.LoopsCount);
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
