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
        [HideInInspector]
        public Status status = Status.START;

        [HideInInspector]
        public bool signal;
        [HideInInspector]
        public bool childSignal;
        public void MoveNext() {
			StateGraph fmGraph = graph as StateGraph;


			NodePort exitPort = GetOutputPort("exit");

			if (!exitPort.IsConnected) {
				Debug.LogWarning("Node isn't connected");
				return;
			}

			StateNode node = exitPort.Connection.node as StateNode;
			node.OnEnter();
		}

		public void OnEnter() {
			
			
		}
        [HideInInspector]
        private float _duration = 0;
        public void Tick(float delta)
        {
            _duration += delta;
            if (_duration > 2)
            {
                signal = false;
            }
            
        }

        [Serializable]
		public class Empty { }
	}
}