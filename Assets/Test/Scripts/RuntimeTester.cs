using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class RuntimeTester : MonoBehaviour
    {
        void Hello2()
        {
            Debug.Log("Hello called!");
        }
        [ContextMenu("Hello")]
        void Hello()
        {
            Debug.Log("Hello called!");
        }

        public void InhjectCallback()
        {
            Debug.Log("Inject!!");
        }

        public T HelloGeneric<T>(T t)
        {
            Debug.Log(t);
            return t;
        }
    }
}
