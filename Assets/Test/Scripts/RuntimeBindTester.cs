using UnityEngine;
using MethodCatcher;

namespace Test
{
    public class RuntimeBindTester : MonoBehaviour
    {
#if UNITY_EDITOR
        int bindedCount = 0;
        void OnGUI()
        {
            foreach (var (a, av) in CatcherSetting._FlattenDict)
                foreach (var (t, tv) in av)
                    foreach (var m in tv)
                        if (GUILayout.Button($"Bind To {a}.{t}.{m}"))
                        {
                            if (CatcherSetting.GetEventHandler(new(
                                a, t, m), out var handler))
                            {
                                int x = bindedCount;
                                handler.AddListener(() =>
                                    Debug.Log($"Binder `{x}`"));

                                Debug.Log($"Registed `{bindedCount}`");
                                bindedCount++;
                            }
                        }
        }
#endif
    }
}
