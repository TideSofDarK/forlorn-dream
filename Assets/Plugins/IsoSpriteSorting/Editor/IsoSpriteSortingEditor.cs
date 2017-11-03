using UnityEngine;
using System.Collections;
using UnityEditor;

namespace CreativeSpore.SpriteSorting
{
    [CustomEditor(typeof(IsoSpriteSorting))]
    public class IsoSpriteSortingEditor : Editor
    {
        private static bool m_showStatistics = false;

        public override void OnInspectorGUI()
        {
            IsoSpriteSorting myTarget = (IsoSpriteSorting)target;
            serializedObject.Update();

            EditorGUILayout.Space();

            m_showStatistics = EditorGUILayout.Foldout(m_showStatistics, "Show Statistics");
            if( m_showStatistics )
            {
                GUIStyle style = new GUIStyle();
                style.richText = true;
                EditorGUILayout.TextArea(myTarget.GetStatistics(), style);
                Repaint();
            }

            int iVal = PlayerPrefs.GetInt(IsoSpriteSorting.k_IntParam_ExecuteInEditMode, 1);
            bool bVal = EditorGUILayout.ToggleLeft("Execute in Edit Mode", iVal != 0, EditorStyles.boldLabel);
            PlayerPrefs.SetInt(IsoSpriteSorting.k_IntParam_ExecuteInEditMode, bVal ? 1 : 0);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("SortingAxis"), new GUIContent("Sorting Axis", "Axis used for sorting"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SorterPositionOffset"), new GUIContent("Sorting Offset", "Position used for sorting, relative to gameobject position. Change it in the editor dragging the white dot square."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IncludeInactiveRenderer"), new GUIContent("Include Inactive Renderers", "If it's invalidated and IncludeInactiveRenderer is true, inactive renderers will be taking into account."));

            EditorGUILayout.HelpBox("Invalidate when adding or removing any renderer.\nInvalidateAll invalidates all objects.", MessageType.Info);
            if (GUILayout.Button("Invalidate")) myTarget.Invalidate();
            if (GUILayout.Button("InvalidateAll")) IsoSpriteSorting.InvalidateAll();

            if (GUI.changed)
            {
                SceneView.RepaintAll();
                Undo.RecordObject(target, "Updated Sorting Settings");
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        public void OnSceneGUI()
        {
            IsoSpriteSorting myTarget = (IsoSpriteSorting)target;

            myTarget.SorterPositionOffset = Handles.FreeMoveHandle(myTarget.transform.position + myTarget.SorterPositionOffset, Quaternion.identity, 0.08f * HandleUtility.GetHandleSize(myTarget.transform.position), Vector3.zero, Handles.DotCap) - myTarget.transform.position;
            if (GUI.changed)
            {
                Undo.RecordObject(target, "Updated Sorting Offset");
                EditorUtility.SetDirty(target);
            }
        }
    }
}