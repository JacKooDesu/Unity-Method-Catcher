using UnityEngine;
using MethodCatcher;
using System.Reflection;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

namespace Test
{
    public class RuntimeBindTester : MonoBehaviour
    {
        int bindedCount = 0;
        string _assembly = "";
        string _type = "";
        string _method = "";

        Dictionary<CatcherSetting, UnityAction<object>> _binded = new();
        Queue<CatcherSetting> _pendingRemove = new();
        int width = 0, height = 0;
        void Awake()
        {
            width = Screen.width;
            height = Screen.height;
        }
        void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.Width(width));
            _assembly = GUILayout.TextArea(_assembly);
            _type = GUILayout.TextArea(_type);
            _method = GUILayout.TextArea(_method);
            if (GUILayout.Button("Bind!"))
            {
                CatcherSetting setting = new(_assembly, _type, _method);
                if (setting.GetEventHandler(out var handler))
                {
                    var current = bindedCount;
                    UnityAction<object> action = _ => Debug.Log($"Binded `{current}`, result: {_}");
                    _binded.Add(setting, action);
                    handler.AddListener(action);

                    bindedCount++;
                }
            }

            foreach (var (setting, action) in _binded)
            {
                if (GUILayout.Button(setting.GetKey()) &&
                    setting.GetEventHandler(out var handler))
                {
                    handler.RemoveListener(action);
                    _pendingRemove.Enqueue(setting);
                }
            }

            GUILayout.EndVertical();

            while (_pendingRemove.TryDequeue(out var setting))
                _binded.Remove(setting);
        }

        void RuntimeInject(string assembly, string type, string method)
        {
            if (new CatcherSetting(assembly, type, method)
                    .GetEventHandler(out var handler))
            {
                handler.AddListener(_ =>
                    Debug.Log("Runtime Inject!!"));
            }
        }
    }
}
