using System;
using System.Linq;
using System.Reflection;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

using HarmonyLib;

namespace MethodCatcher
{
    [Serializable]
    public partial class CatcherSetting
    {
        public string TargetAssembly;
        public string TargetType;
        public string TargetMethod;
        public string GetKey() => $"{TargetAssembly}.{TargetType}.{TargetMethod}";
        static Harmony _harmony { get; } = new("method.catcher.harmony");

        public static bool GetEventHandler(CatcherSetting setting, out UnityEvent handler)
        {
            var key = setting.GetKey();
            if (!EventHandleDict.TryGetValue(key, out var result))
            {
                if ((result = Patch(setting)) is not null)
                    EventHandleDict.TryAdd(key, result);
            }

            return (handler = result) is not null;
        }

        static UnityEvent Patch(CatcherSetting setting)
        {
            UnityEvent handler = null;
            if (!_assemblies.TryGetValue(setting.TargetAssembly, out var assembly))
                return handler;

            handler = new();
            var original = AccessTools.Method(
                    assembly.GetType(setting.TargetType),
                    setting.TargetMethod);
            var postfix = AccessTools.Method(
                    typeof(CatcherSetting),
                    nameof(Invoke));
            Debug.Log(postfix);
            _harmony.Patch(
                original,
                postfix: new(postfix));

            return handler;
        }

        // static void Invoke(ref object __instance, MethodInfo __originalMethod)
        static void Invoke(ref object __instance, MethodInfo __originalMethod)
        {
            var type = __instance.GetType();
            var key = $"{type.Assembly.GetName().Name}.{type.FullName}.{__originalMethod.Name}";

            if (EventHandleDict.TryGetValue(key, out var result))
                result.Invoke();
        }

        static Dictionary<string, UnityEvent> EventHandleDict { get; } = new();
        static Dictionary<string, Assembly> _assemblies;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void LoadAssembly()
        {
            _assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .ToDictionary(x => x.GetName().Name);

            Debug.Log(_assemblies.Count);
        }
    }
}
