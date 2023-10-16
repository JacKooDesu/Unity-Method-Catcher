using System;
using System.Linq;
using System.Reflection;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace MethodCatcher
{
    public partial class CatcherSetting
    {
#if UNITY_EDITOR
        internal static Dictionary<Assembly, bool> READ_ASSEMBLY { get; } = new();
        public static Dictionary<string, Dictionary<string, string[]>> _FlattenDict { get; } = new();

        public static bool AutoLoadDomain { get; internal set; } = false;

        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnReloadScripts()
        {
            LoadConfig();
            GetDomain();
            if (!AutoLoadDomain)
                return;

            BatchAssemblies();
        }

        internal static void GetDomain()
        {
            var domain = AppDomain.CurrentDomain;
            var backup = LoadSetting();
            foreach (var a in domain.GetAssemblies())
            {
                var b = backup?.Contains(a.GetName().Name) ?? false;
                READ_ASSEMBLY.TryAdd(a, b);
            }
        }

        internal static void BatchAssemblies()
        {
            _FlattenDict.Clear();
            Debug.LogWarning("[Condition Checker] Batching Assembly!");

            var assems = READ_ASSEMBLY
                .Where(kv => kv.Value)
                .Select(kv => kv.Key);

            var enumerable = assems
                .Select(a =>
                        (name: a.GetName().Name,
                         types: a.GetTypes().Select(t =>
                            (name: t.FullName,
                             methods: t.GetMethods((BindingFlags)int.MaxValue)))));

            foreach (var a in enumerable)
            {
                if (a.types.Count() is 0)
                    continue;
                Dictionary<string, string[]> typeMethodsDict = new();
                foreach (var t in a.types)
                {
                    if (t.methods.Length is 0)
                        continue;
                    typeMethodsDict.TryAdd(
                        t.name,
                        t.methods.Select(x => x.Name).ToArray());
                }
                _FlattenDict.TryAdd(a.name, typeMethodsDict);
            }
        }

        internal static void LoadConfig()
        {
            var root = Application.dataPath + "/../jackoo-dev-MethodCatcher/";
            var filePath = root + "/config.txt";
            if (!File.Exists(filePath))
                return;

            List<string> result = new();
            using (FileStream fs = File.OpenRead(filePath))
            {
                StreamReader sr = new(fs);
                while (sr.Peek() >= 0)
                    AutoLoadDomain = Convert.ToBoolean(sr.ReadLine());
                sr.Close();
            }
        }

        internal static void SaveConfig()
        {
            var root = Application.dataPath + "/../jackoo-dev-MethodCatcher/";
            Directory.CreateDirectory(root);
            var filePath = root + "/config.txt";
            using (FileStream fs = File.Create(filePath))
            {
                StreamWriter sw = new(fs);
                sw.WriteLine(AutoLoadDomain);
                sw.Close();
            }
        }

        internal static List<string> LoadSetting()
        {
            var root = Application.dataPath + "/../jackoo-dev-MethodCatcher/";
            var filePath = root + "/targets.txt";
            if (!File.Exists(filePath))
                return null;

            List<string> result = new();
            using (FileStream fs = File.OpenRead(filePath))
            {
                StreamReader sr = new(fs);
                while (sr.Peek() >= 0)
                    result.Add(sr.ReadLine());
                sr.Close();
            }

            return result;
        }

        internal static void SaveSetting()
        {
            var root = Application.dataPath + "/../jackoo-dev-MethodCatcher/";
            Directory.CreateDirectory(root);
            var filePath = root + "/targets.txt";
            using (FileStream fs = File.Create(filePath))
            {
                StreamWriter sw = new(fs);
                foreach (var (k, v) in READ_ASSEMBLY)
                    if (v) sw.WriteLine(k.GetName().Name);
                sw.Close();
            }
        }
#endif
    }
}
