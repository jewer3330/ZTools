using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNodeEditor;

namespace ZTool
{

    [CustomEditor(typeof(DebugXNode))]
    public class DebugXNodeInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DebugXNode debugxnod = target as DebugXNode;
            foreach (var k in debugxnod.graphs)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(k.Key.ToString());
                if (GUILayout.Button("Show",GUILayout.Width(100)))
                {
                    NodeEditorWindow.Open(k.Value);
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
