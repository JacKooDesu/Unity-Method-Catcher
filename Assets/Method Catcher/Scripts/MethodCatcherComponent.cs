using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MethodCatcher
{
    public class MethodCatcherComponent : MonoBehaviour
    {
        [SerializeField] public List<CatcherSetting> _settings;
        [SerializeField] UnityEvent _onInvoke = new();
        public UnityEvent OnInvoke => _onInvoke;

        void Awake()
        {
            _settings.ForEach(s =>
            {
                if (CatcherSetting.GetEventHandler(s, out var handler))
                    handler.AddListener(_onInvoke.Invoke);
            });
        }

#if UNITY_EDITOR
        [ContextMenu("Test")]
        void Test()
        {
            var dict = CatcherSetting._FlattenDict;
            // ConditionCheckSetting.BatchDomain();
            foreach (var (a, ts) in dict)
                foreach (var (t, ms) in ts)
                    foreach (var m in ms)
                        Debug.Log($"{a}.{t}.{m}");
        }
#endif
    }
}