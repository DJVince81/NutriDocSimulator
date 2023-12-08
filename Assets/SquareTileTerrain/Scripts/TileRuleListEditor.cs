#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace SquareTileTerrainEditor
{
    [CustomEditor(typeof(TileRuleList))]
    public class TileRuleListEditor : Editor
    {
        SerializedProperty ruleList;
        ReorderableList ruleReorderableList;

        private void OnEnable()
        {
            /* Some Unity stuff */
            ruleList = serializedObject.FindProperty("TileList");

            ruleReorderableList = new ReorderableList(serializedObject,
                                                    ruleList,
                                                    true, true, true, true);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ruleReorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif