using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZTool.XNode.Examples.StateGraph;
using XNodeEditor;
using ZTool.XNode;
using UnityEditor;
using System.Linq;

namespace XNodeEditor.Examples {
	[CustomNodeGraphEditor(typeof(StateGraph))]
	public class StateGraphEditor : NodeGraphEditor {

        readonly Color boolColor = new Color(0.1f, 0.6f, 0.6f);
        private List<ObjectLastOnTimer> lastOnTimers = new List<ObjectLastOnTimer>();
        private double lastFrame;
        private class ObjectLastOnTimer
        {
            public object obj;
            public double lastOnTime;

            public ObjectLastOnTimer(object obj, bool on)
            {
                this.obj = obj;
            }
        }

        /// <summary> 
        /// Overriding GetNodeMenuName lets you control if and how nodes are categorized.
        /// In this example we are sorting out all node types that are not in the XNode.Examples namespace.
        /// </summary>
        public override string GetNodeMenuName(System.Type type) {
            if (type.Namespace == "ZTool.XNode.Examples.StateGraph")
            {
                return base.GetNodeMenuName(type).Replace("Z Tool/X Node/Examples/State Graph/", "");
            }
            else return null;
        }


        /// <summary> Controls graph noodle colors </summary>
		public override Gradient GetNoodleGradient(NodePort output, NodePort input)
        {
            
            Gradient baseGradient = base.GetNoodleGradient(output, input);

            StateNode outputNode = output?.node as StateNode;
            StateNode inputNode = input?.node as StateNode;
            if (outputNode != null && inputNode != null)
            {
                bool ret = (outputNode.signal) && (inputNode.signal);
                if (ret)
                {
                    HighlightGradient(baseGradient, Color.green, input, ret);
                }
                else
                {
                    HighlightGradient(baseGradient, Color.yellow, input, outputNode.childSignal && inputNode.childSignal);
                }
            }
            return baseGradient;
        }

        /// <summary> Controls graph type colors </summary>
        public override Color GetTypeColor(System.Type type)
        {
            if (type == typeof(StateNode.Status)) return boolColor;
            else return base.GetTypeColor(type);
        }

        /// <summary> Returns the time at which an arbitrary object was last 'on' </summary>
        public double GetLastOnTime(object obj, bool high)
        {
            ObjectLastOnTimer timer = lastOnTimers.FirstOrDefault(x => x.obj == obj);
            if (timer == null)
            {
                timer = new ObjectLastOnTimer(obj, high);
                lastOnTimers.Add(timer);
            }
            if (high) timer.lastOnTime = EditorApplication.timeSinceStartup;
            return timer.lastOnTime;
        }

        /// <summary> Returns a color based on if or when an arbitrary object was last 'on' </summary>
        public Color GetLerpColor(Color off, Color on, object obj, bool high)
        {
            double lastOnTime = GetLastOnTime(obj, high);

            if (high) return on;
            else
            {
                float t = (float)(lastOnTime - EditorApplication.timeSinceStartup);
                t *= 8f;
                if (t > 0) return Color.Lerp(off, on, t);
                else return off;
            }
        }

        /// <summary> Returns a color based on if or when an arbitrary object was last 'on' </summary>
        public void HighlightGradient(Gradient gradient, Color highlightColor, object obj, bool high)
        {
            double lastOnTime = GetLastOnTime(obj, high);
            float t;

            if (high) t = 1f;
            else
            {
                t = (float)(lastOnTime - EditorApplication.timeSinceStartup);
                t *= 8f;
                t += 1;
            }
            t = Mathf.Clamp01(t);
            GradientColorKey[] colorKeys = gradient.colorKeys;
            for (int i = 0; i < colorKeys.Length; i++)
            {
                GradientColorKey colorKey = colorKeys[i];
                colorKey.color = Color.Lerp(colorKeys[i].color, highlightColor, t);
                colorKeys[i] = colorKey;
            }
            gradient.SetKeys(colorKeys, gradient.alphaKeys);
        }

        public override void OnGUI()
        {
            // Repaint each frame
            window.Repaint();

            // Timer
            if (Event.current.type == EventType.Repaint)
            {
                for (int i = 0; i < target.nodes.Count; i++)
                {
                    ITimerTick timerTick = target.nodes[i] as ITimerTick;
                    if (timerTick != null)
                    {
                        float deltaTime = (float)(EditorApplication.timeSinceStartup - lastFrame);
                    timerTick.Tick(deltaTime);
                }
            }
            }
            lastFrame = EditorApplication.timeSinceStartup;
        }
    }
}