using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;

namespace MethodCatcher
{
    public static class Patcher
    {
        static Harmony _harmony { get; } = new("method.catcher.harmony");

        public static UnityEvent<object> Patch(string key, MethodInfo method)
        {
            UnityEvent<object> handler = new();

            if (EventHandleDict.TryGetValue(key, out handler))
                return handler;

            handler = new();
            var original = method;
            var postfix = AccessTools.Method(
                    typeof(Patcher),
                    original.ReturnType == typeof(void) ?
                        nameof(InvokeVoid) : nameof(Invoke));

            _harmony.Patch(
                original,
                postfix: new(postfix));
            EventHandleDict.TryAdd(key, handler);

            return handler;
        }

        static void InvokeVoid(ref object __instance, MethodInfo __originalMethod)
        {
            var type = __instance.GetType();
            var key = $"{type.Assembly.GetName().Name}.{type.FullName}.{__originalMethod.Name}";

            if (EventHandleDict.TryGetValue(key, out var result))
                result.Invoke(null);
        }

        static void Invoke(ref object __instance, MethodInfo __originalMethod, ref object __result)
        {
            var type = __instance.GetType();
            var key = $"{type.Assembly.GetName().Name}.{type.FullName}.{__originalMethod.Name}";

            if (EventHandleDict.TryGetValue(key, out var result))
                result.Invoke(__result);
        }

        static Dictionary<string, UnityEvent<object>> EventHandleDict { get; } = new();
    }
}
