using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

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

        [ContextMenu("Hello 3")]
        public async UniTask<string> Hello3()
        {
            Debug.Log("Hello3 waiting...");
            await UniTask.Delay(1000);
            Debug.Log("Hello3 called!");
            return Time.deltaTime.ToString();
        }

        [ContextMenu("Hello 4")]
        public async Task<string> Hello4()
        {
            Debug.Log("Hello4 waiting...");
            await UniTask.Delay(1000);
            Debug.Log("Hello4 called!");
            return Time.deltaTime.ToString();
        }

        public T HelloGeneric<T>(T t)
        {
            Debug.Log(t);
            return t;
        }
    }
}
