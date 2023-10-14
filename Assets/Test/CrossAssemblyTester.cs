using System.Reflection;
using UnityEngine;

public class CrossAssemblyTester : MonoBehaviour
{
    [ContextMenu("test")]
    void Test()
    {
        var info = typeof(TestReflect).GetMethod(
            "StaticInvoke",
            (BindingFlags)int.MaxValue);

        Debug.Log(info.DeclaringType);
        info.GetParameters();
        info.Invoke(null, new object[] { 10 });
    }
}

public class TestReflect
{
    void Invoke(int x)
    {
        Debug.Log(x);
    }

    static void StaticInvoke(int x)
    {
        Debug.Log(x);
    }
}