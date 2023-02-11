using UnityEngine;
using FileManagement;

public class CompilationHelper : MonoBehaviour
{
    public static CompilationHelper singleton;

    public string cubeJSON;

    void Awake()
    {
        singleton = this;
    }

    void OnApplicationQuit()
    {
        CompilationManager.UnloadDomain();
    }
}
