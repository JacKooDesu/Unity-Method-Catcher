using UnityEngine;
using UnityEditor;
using System.Linq;

namespace MethodCatcher
{
    public class SettingWindow : EditorWindow
    {
        Vector2 _scrollPos;
        string _filter = "";

        [MenuItem("Method Catcher/Setting")]
        static void ShowWindow()
        {
            var window = GetWindow<SettingWindow>();
            window.titleContent = new GUIContent("Method Catcher Setting");
            window.Show();
        }

        void OnGUI()
        {
            DrawBatchSetting();
            DrawAssemblis();
        }

        void DrawBatchSetting()
        {
            CatcherSetting.AutoLoadDomain = EditorGUILayout.Toggle(
                    "Auto Batch",
                    CatcherSetting.AutoLoadDomain);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Manual Get Domain Batch"))
                CatcherSetting.GetDomain();

            if (GUILayout.Button("Manual Batch Assemblies"))
                CatcherSetting.BatchAssemblies();
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Save Setting"))
            {
                CatcherSetting.SaveSetting();
                CatcherSetting.SaveConfig();
            }
            GUILayout.EndVertical();
        }

        void DrawAssemblis()
        {
            var keys = CatcherSetting.READ_ASSEMBLY.Keys.ToList();
            _filter = GUILayout.TextField(_filter);
            if (!string.IsNullOrWhiteSpace(_filter))
                keys.RemoveAll(a => !a.GetName().Name.Contains(_filter));

            if (GUILayout.Button("", GUILayout.Width(20f)))
                foreach (var a in keys)
                    CatcherSetting.READ_ASSEMBLY[a] = !CatcherSetting.READ_ASSEMBLY[a];

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            foreach (var a in keys)
                CatcherSetting.READ_ASSEMBLY[a] =
                    EditorGUILayout.ToggleLeft(
                        a.GetName().Name,
                        CatcherSetting.READ_ASSEMBLY[a]);
            GUILayout.EndScrollView();
        }
    }
}