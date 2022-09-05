using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZTool
{
    public class MonoInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;


        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    var go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }
    }
}
