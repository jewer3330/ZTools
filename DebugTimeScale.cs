using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZTool
{
    public class DebugTimeScale : MonoInstance<DebugTimeScale>
    {
        public float timeScale = 1f;

        // Update is called once per frame
        void Update()
        {
            Time.timeScale = timeScale;
        }
    }
}
