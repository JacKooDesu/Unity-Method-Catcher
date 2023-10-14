using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MethodCatcher
{
    public class MethodCatcherComponent : MonoBehaviour
    {
        [SerializeField] public CatcherSetting _setting;
        [SerializeField] UnityEvent _onInvoke = new();
        public UnityEvent<object> OriginHandler { get; private set; }

        void Awake()
        {
            if (_setting.GetEventHandler(out var handler))
                (OriginHandler = handler).AddListener(_ => _onInvoke.Invoke());
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