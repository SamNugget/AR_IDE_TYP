using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public static float scale = 1f;

    public static GameObject textEntryFab;
    [SerializeField] private GameObject textEntryWindowFab;
    
    public static GameObject spawnWindow(GameObject prefab)
    {
        return Instantiate(prefab);
    }

    public static void destroyWindow(GameObject spawned)
    {
        Destroy(spawned);
    }

    void Awake()
    {
        textEntryFab = textEntryWindowFab;
    }
}
