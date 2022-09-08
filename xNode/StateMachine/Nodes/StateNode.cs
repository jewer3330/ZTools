using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZTool.XNode.Examples.StateGraph {

    public interface ITimerTick
    {
        void Tick(float delta);
    }


    public class StateNode : Node,ITimerTick
    {

		[Input] public Empty enter;
		[Output] public Empty exit;

        public enum Status
        {
                START,
                FAILURE,
                SUCCESS,
                EXECUTING,
                WAITING,
        }
        
        public Status status = Status.START;

        [HideInInspector]
        public bool signal;
        [HideInInspector]
        public bool childSignal;
        


        [HideInInspector]
        private float _duration = 0;
        public void Tick(float delta)
        {
            _duration += delta;
            if (_duration > 1)
            {
                signal = false;
            }
            
        }

        [Serializable]
		public class Empty { }


        public enum NodeType
        {
            Root,
            ReferencedBehavior,
            Selector,
            Sequence,
            Decorator,
            Action,
            Condition,
        }

        public NodeType nodeType = NodeType.Root;

        public string nodeName;
        public string operation;


    }
}