using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZTool.XNode.Examples.StateGraph
{
    [CreateAssetMenu(fileName = "NewBehaviourTree", menuName = "BehaviourTree/Empty")]
    public class StateGraph : NodeGraph
    {

        public const string JsonFolderName = "Json";
        [ContextMenu("ExportJson")]
        public void ExportJson()
        {
            CheckJsonFolder();
            var root = GetRoot();
            if (string.IsNullOrEmpty(name))
                SaveJson("json.json", root);
            else
                SaveJson(name + ".json", root);
        }

        public void CheckJsonFolder()
        {
            if (!System.IO.Directory.Exists(JsonFolderName))
            {
                System.IO.Directory.CreateDirectory(JsonFolderName);
            }
        }

        public void SaveJson(string fileName, StateNode root)
        {
            using (var sw = new System.IO.StreamWriter(JsonFolderName + "/" + fileName))
            {
                TravelJson(sw, root);
            }
        }

        [ContextMenu("DumpLua")]
        public void Export()
        {
            CheckDumpFolder();
            var root = GetRoot();
            if (string.IsNullOrEmpty(name))
                SaveLua("dump.lua", root);
            else
                SaveLua(name + ".lua", root);
        }
        public const string FolderName = "Dump";
        public void CheckDumpFolder()
        {
            if (!System.IO.Directory.Exists(FolderName))
            {
                System.IO.Directory.CreateDirectory(FolderName);
            }
        }


        public void TravelForward(Node root,System.Action<Node, int> func,int depth)
        {
            if (root != null)
            {
                var op = root.GetOutputPort("exit");
                {
                    if (op.ConnectionCount > 0)
                    {
                        var cons = op.Connection.GetConnections();
                        foreach (var c in cons)
                        {
                            foreach (var k in c.GetConnections())
                            {
                                TravelForward(k.node, func,depth + 1);
                            }
                        }
                    }
                }
                func?.Invoke(root,depth);
            }
        }

        public void TravelJson(System.IO.StreamWriter writer, StateNode root, int depth = 0)
        {
            if (root != null)
            {

                WriteTab(writer, depth);
                writer.WriteLine($"{{");
                WriteTab(writer, depth + 1);
                writer.Write($"\"NodeType\" : \"{ root.nodeType }\"");
                if (!string.IsNullOrEmpty(root.nodeName))
                {

                    writer.WriteLine(',');
                    WriteTab(writer, depth + 1);
                    writer.Write($"\"NodeName\" : \"{ root.nodeName }\"");
                }
                if (!string.IsNullOrEmpty(root.operation))
                {

                    writer.WriteLine(',');
                    WriteTab(writer, depth + 1);
                    writer.Write($"\"Operation\" : \"{ root.operation }\"");
                }

                if (root.nodeType == StateNode.NodeType.ReferencedBehavior)
                {
                    root.nodeType = StateNode.NodeType.Root;
                    {
                        SaveJson(root.nodeName + ".json", root);
                    }
                    root.nodeType = StateNode.NodeType.ReferencedBehavior;

                }
                else
                {
                    var op = root.GetOutputPort("exit");
                    {
                        if (op.ConnectionCount > 0)
                        {

                            writer.WriteLine(',');
                            WriteTab(writer, depth + 1);
                            writer.Write("\"Childs\" :  [");


                            var cons = op.Connection.GetConnections();
                            int i = 0;
                            foreach (var c in cons)
                            {
                                foreach (var k in c.GetConnections())
                                {
                                    if (i > 0)
                                    {
                                        //WriteTab(writer, depth + 2);
                                        writer.WriteLine(',');
                                    }
                                    else
                                    {
                                        //WriteTab(writer, depth + 1);
                                        writer.WriteLine();
                                    }
                                    i++;
                                    TravelJson(writer, k.node as StateNode, depth + 2);
                                }
                            }
                            writer.WriteLine();
                            WriteTab(writer, depth + 1);
                            writer.Write("]");
                        }
                    }
                }
                writer.WriteLine();
                WriteTab(writer, depth);
                writer.Write("}");
            }
        }


        public void SaveLua(string fileName, StateNode root)
        {
            using (var sw = new System.IO.StreamWriter(FolderName + "/" + fileName))
            {
                sw.WriteLine("local config = {");
                Travel(sw, root);
                sw.WriteLine("}");
                sw.Write("return config");
            }
        }

        public StateNode GetRoot()
        {
            var root = nodes[0] as StateNode;

            while (root.GetInputValue<StateNode>("enter") != null)
            {
                root = root.GetInputValue<StateNode>("enter");
            }
            return root;
        }
        public void WriteTab(System.IO.StreamWriter writer, int count)
        {
            for (int i = 0; i < count; i++)
            {
                writer.Write('\t');
            }
        }
        public void Travel(System.IO.StreamWriter writer, StateNode root, int depth = 0)
        {
            if (root != null)
            {

                WriteTab(writer, depth);
                writer.WriteLine($"Node = {{");
                WriteTab(writer, depth + 1);
                writer.WriteLine($"NodeType = \"{ root.nodeType }\"");
                if (!string.IsNullOrEmpty(root.nodeName))
                {
                    WriteTab(writer, depth + 1);
                    writer.WriteLine($"NodeName = \"{ root.nodeName }\"");
                }
                if (!string.IsNullOrEmpty(root.operation))
                {
                    WriteTab(writer, depth + 1);
                    writer.WriteLine($"Operation = \"{ root.operation }\"");
                }

                if (root.nodeType == StateNode.NodeType.ReferencedBehavior)
                {
                    root.nodeType = StateNode.NodeType.Root;
                    {
                        SaveLua(root.nodeName + ".lua", root);
                    }
                    root.nodeType = StateNode.NodeType.ReferencedBehavior;

                }
                else
                {
                    var op = root.GetOutputPort("exit");
                    {
                        if (op.ConnectionCount > 0)
                        {
                            WriteTab(writer, depth + 1);
                            writer.WriteLine("Connector =  {");
                            WriteTab(writer, depth + 2);
                            writer.WriteLine("Identifier =  \"GenericChildren\",");

                            var cons = op.Connection.GetConnections();
                            foreach (var c in cons)
                            {
                                foreach (var k in c.GetConnections())
                                {
                                    Travel(writer, k.node as StateNode, depth + 2);
                                }
                            }
                            WriteTab(writer, depth + 2);
                            writer.WriteLine("}");

                        }
                    }
                }
                WriteTab(writer, depth + 1);
                writer.WriteLine("},");
            }
        }

        public static int LEAFCOUNT = 0;
        public float ResetPositionChild(Node root,int depth)
        {
            float sum = 0;
            if (root != null)
            {
                var op = root.GetOutputPort("exit");
                if (op.ConnectionCount > 0 && !root.hide)
                {
                    var cons = op.Connection.GetConnections();
                    foreach (var c in cons)
                    {
                        foreach (var k in c.GetConnections())
                        {
                            sum += ResetPositionChild(k.node, depth + 1);
                        }
                    }
                    root.position.y = sum / op.ConnectionCount;
                    root.position.x = 300 * depth;
                }
                else
                {
                    root.position.y = 200 * LEAFCOUNT;
                    root.position.x = 300 * depth;
                    LEAFCOUNT++;
                }
            }

            return root.position.y;
        }

        public void ResetPosition()
        {
            LEAFCOUNT = 0;
            var root = GetRoot();
            ResetPositionChild(root,0);
        }

        public void SetHide(Node root, bool hide)
        {
            TravelForward(root, (node,depth) =>
            {
                node.hide = hide;
                node.childHide = hide;
            },0);
        }
    }
}