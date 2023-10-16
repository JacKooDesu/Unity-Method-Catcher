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
    }
}