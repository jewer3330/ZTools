using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZTool.XNode;
using ZTool.XNode.Examples.StateGraph;

namespace ZTool
{
    public class DebugXNode : MonoInstance<DebugXNode>
    {
        public Dictionary<int,StateGraph> graphs = new Dictionary<int,StateGraph>();
        public void CreateGraph(int uid)
        {
            if (!graphs.ContainsKey(uid))
            {
                var graph = ScriptableObject.CreateInstance<StateGraph>();
                graphs.Add(uid, graph);
            }
        }

        public void DestroyGraph(int uid)
        {
            if (graphs.ContainsKey(uid))
            {
                Destroy(graphs[uid]);
                graphs.Remove(uid);
            }
        }


        public void CleanGraph(int uid)
        {
            StateNode excutingNode = null;
           
            if (graphs.ContainsKey(uid))
            {
                var graph = graphs[uid];
                foreach (var item in graph.nodes)
                {
                    var sn = item as StateNode;
                    if (sn.status == StateNode.Status.EXECUTING)
                    {
                        excutingNode = sn;
                    }
                    else
                    {
                        sn.childSignal = false;
                    }
                }
            }
            
            while (excutingNode != null)
            {
                excutingNode.childSignal = true;
                if (excutingNode.status == StateNode.Status.FAILURE)
                {
                    excutingNode.status = StateNode.Status.WAITING;
                }
                var port = excutingNode.Inputs.First();
                if (port != null)
                    excutingNode = port.Connection?.node as StateNode;
            }
        }

        public int AddNode(int uid,string name)
        {
            if (graphs.TryGetValue(uid, out StateGraph graph))
            {
                var node = graph.AddNode<StateNode>();
                node.name = name;
              
                return node.GetInstanceID();
            }
            return 0;
        }

        public void SetNodePosition(int uid, int instanceID,int x,int y)
        {
            if (graphs.TryGetValue(uid, out StateGraph graph))
            {
                var srcNode = graph.nodes.Find(r => r.GetInstanceID() == instanceID);
                srcNode.position = new Vector2(x, y);
            }
        }

        public void LinkNode(int uid, int srcInstanceID,int targetInstanceID)
        {
            if (graphs.TryGetValue(uid, out StateGraph graph))
            {
                var srcNode = graph.nodes.Find(r => r.GetInstanceID() == srcInstanceID);
                var targetNode = graph.nodes.Find(r => r.GetInstanceID() == targetInstanceID);
                if (srcNode != null && targetNode != null)
                    targetNode.Inputs.FirstOrDefault().Connect(srcNode.Outputs.FirstOrDefault());
            }
        }

        public void StepNode(int uid, int instanceID,int status)
        {
            if (graphs.TryGetValue(uid, out StateGraph graph))
            {
                foreach (var k in graph.nodes)
                {
                    if (k.GetInstanceID() == instanceID)
                    {
                        var sn = k as StateNode;
                        sn.status = (StateNode.Status)status;
                        sn.signal = true;
                        break;
                    }
                }
            }
        }
    }
}