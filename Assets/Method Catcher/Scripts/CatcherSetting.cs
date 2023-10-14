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
        public CatcherSetting(
            string assembly, string type, string method)
        {
            TargetAssembly = assembly;
            TargetType = type;
            TargetMethod = method;
        }

        public bool GetEventHandler(out UnityEvent<object> handler)
        {
            handler = null;
            if (!_assemblies.TryGetValue(TargetAssembly, out var assembly))
                return false;

            var original = AccessTools.Method(
                    assembly.GetType(TargetType),
                    TargetMethod);

            handler = Patcher.Patch(GetKey(), original);
            return handler is not null;
        }

        static Dictionary<string, Assembly> _assemblies;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void LoadAssembly()
        {
            _assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .ToDictionary(x => x.GetName().Name);
        }
    }
}
