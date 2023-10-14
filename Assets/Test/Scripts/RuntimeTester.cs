using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class RuntimeTester : MonoBehaviour
    {
        [ContextMenu("Hello")]
        void Hello()
        {
            Debug.Log("Hello called!");
        }

        public void InjectCallback()
        {
            Debug.Log("Inject!!");
        }

        [ContextMenu("Hello 2")]
        public string Hello2()
        {
            Debug.Log("Hello2 called!");
            return Time.deltaTime.ToString();
        }

        public T HelloGeneric<T>(T t)
        {
            Debug.Log(t);
            return t;
        }
    }
}
