using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZTool
{

    public interface IGUI
    {
        void OnGUI();
    }

    public class ButtonGUI : IGUI
    {
        public string text;
        public System.Action callback;
        public void OnGUI()
        {
            if (GUILayout.Button(text))
            {
                callback?.Invoke();
            }
        }
    }

    public class PannelGUI : IGUI
    {
        public string title;
        public string content;
        public System.Action okCallback;
        public System.Action cancelCallback;

        public void OnGUI()
        {
            Rect screen = new Rect(0, 0, Screen.width, Screen.height);
            GUI.backgroundColor = Color.grey;
            GUILayout.BeginArea(screen);

            GUILayout.TextField(title);
            GUILayout.TextField(content);
            if (GUILayout.Button("ok"))
            {
                okCallback?.Invoke();
            }
            if (GUILayout.Button("cancle"))
            {
                cancelCallback?.Invoke();
            }

            GUILayout.EndArea();
        }
    }

    public class TextGUI : IGUI
    {
        public string text;
        public void OnGUI()
        {
            GUILayout.TextField(text);
        }
    }



    public class DebugGUI : MonoInstance<DebugGUI>
    {

        public List<IGUI> contents;
        public void ShowButton(string text, System.Action callback)
        {
            var button = new ButtonGUI();
            button.text = text;
            button.callback = callback;
            contents.Add(button);
        }

        public void ShowPannel(string title, string content, System.Action okcallBack, System.Action cancellCallBack)
        { 
            //TODO
        }

        public void ShowText(string content)
        { 
            //TODO
        }

        private void OnGUI()
        {
            foreach (var k in contents)
            {
                k.OnGUI();
            }
        }
    }
}