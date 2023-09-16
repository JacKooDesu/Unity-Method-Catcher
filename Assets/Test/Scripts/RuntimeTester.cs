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

        public void InhjectCallback()
        {
            Debug.Log("Inject!!");
        }
    }
}
