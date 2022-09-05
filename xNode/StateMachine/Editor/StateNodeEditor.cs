using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZTool.XNode;
using ZTool.XNode.Examples.StateGraph;

namespace XNodeEditor.Examples {
	[CustomNodeEditor(typeof(StateNode))]
	public class StateNodeEditor : NodeEditor {

        private StateGraphEditor graphEditor;
        private StateNode node;
        public override void OnHeaderGUI()
        {
            if (node == null)
            {
                node = target as StateNode;
                graphEditor = NodeGraphEditor.GetEditor(target.graph, window) as StateGraphEditor;
            }


            GUI.color = Color.white;
			node = target as StateNode;
			StateGraph graph = node.graph as StateGraph;
            switch (node.status)
            {
                case StateNode.Status.START:
                    GUI.backgroundColor = Color.white;
                    break;
                case StateNode.Status.FAILURE:
                    GUI.backgroundColor = Color.red;
                    break;
                case StateNode.Status.SUCCESS:
                    GUI.backgroundColor = Color.green;
                    break;
                case StateNode.Status.EXECUTING:
                    GUI.backgroundColor = Color.yellow;
                    break;
                default:
                    break;
            }
          
			string title = target.name;
			GUILayout.Label(title, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
			GUI.color = Color.white;


            Rect dotRect = GUILayoutUtility.GetLastRect();
            dotRect.size = new Vector2(16, 16);
            dotRect.y += 6;

            Color c_target = Color.green;
            bool flag = false;
            if (node.status == StateNode.Status.WAITING)
            {
                c_target = Color.yellow;
                flag = true;
            }
            else if (node.status != StateNode.Status.FAILURE)
            {
                c_target = Color.green;
                flag = true;
            }

            GUI.color = graphEditor.GetLerpColor(Color.red, c_target, node, flag);
            GUI.DrawTexture(dotRect, NodeEditorResources.dot);
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
        }

		public override void OnBodyGUI() {
			//base.OnBodyGUI();
			StateNode node = target as StateNode;
            NodePort input = target.GetPort("input");
            NodePort output = target.GetPort("output");

            GUILayout.BeginHorizontal();
            if (input != null) NodeEditorGUILayout.PortField(GUIContent.none, input, GUILayout.MinWidth(0));
            if (output != null) NodeEditorGUILayout.PortField(GUIContent.none, output, GUILayout.MinWidth(0));
            GUILayout.EndHorizontal();
            base.OnBodyGUI();
            GUILayout.TextArea(node.status.ToString());
            EditorGUIUtility.labelWidth = 60;
            
        }
	}
}