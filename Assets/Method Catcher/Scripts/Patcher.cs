using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
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
            var invoker = nameof(Invoke);
            if (original.ReturnType == typeof(void))
                invoker = nameof(InvokeVoid);
            else if (IsAsync(original))
                invoker = nameof(InvokeAsync);

            var postfix = AccessTools.Method(
                    typeof(Patcher),
                    invoker);

            _harmony.Patch(
                original,
                postfix: new(postfix));
            EventHandleDict.TryAdd(key, handler);

            return handler;
        }

        static bool IsAsync(MethodInfo info)
        {
            var a = typeof(AsyncStateMachineAttribute);
            return (info.GetCustomAttribute(a) as AsyncStateMachineAttribute) is not null;
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

        static void InvokeAsync(ref object __instance, MethodInfo __originalMethod, ref object __result)
        {
            const string M_AWAITER_GETTER = nameof(UniTask.GetAwaiter);
            const string M_RESULT_GETTER = nameof(UniTask.Awaiter.GetResult);
            const string M_ON_COMPLETED = nameof(UniTask.Awaiter.OnCompleted);

            var type = __instance.GetType();
            var key = $"{type.Assembly.GetName().Name}.{type.FullName}.{__originalMethod.Name}";
            var awaiter = __result.GetType().GetMethod(
                M_AWAITER_GETTER,
                BindingFlags.Public | BindingFlags.Instance)
                .Invoke(__result, Array.Empty<object>());

            if (EventHandleDict.TryGetValue(key, out var result))
            {
                var aType = awaiter.GetType();
                Action action = () => result.Invoke(
                    aType.GetMethod(M_RESULT_GETTER, (BindingFlags)int.MaxValue)
                        .Invoke(awaiter, Array.Empty<object>()));

                var m = aType.GetMethod(M_ON_COMPLETED, (BindingFlags)int.MaxValue);
                m.Invoke(awaiter, new object[] { action });
            }
        }

        static Dictionary<string, UnityEvent<object>> EventHandleDict { get; } = new();
    }
}
