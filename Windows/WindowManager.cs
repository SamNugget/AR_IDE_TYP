using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class WindowManager : MonoBehaviour
{
    private static WindowManager singleton;

    // global control of scale?

    [SerializeField] private GameObject workspacesWindowFab;
    private static GameObject _workspacesWindowFab;
    [SerializeField] private GameObject filesWindowFab;
    private static GameObject _filesWindowFab;
    [SerializeField] private GameObject textEntryWindowFab;
    private static GameObject _textEntryWindowFab;



    private static List<Window3D> windows = new List<Window3D>();

    public static Window3D getWindowWithComponent<T>()
    {
        foreach (Window3D window in windows)
        {
            Transform w = window.transform;
            if (w.GetComponent<T>() != null || w.GetComponentInChildren<T>() != null)
                return window;

        }
        return null;
    }

    public static void spawnFilesWindow()
    {
        // replace the workspace window with a files window

        // find the workspaces window and get attributes
        Window3D workspacesWindow = getWindowWithComponent<WorkspacesButtonManager>();
        //bool isFollowing = workspacesWindow.GetComponentInChildren<RadialView>().enabled;
        Vector3 windowPos = workspacesWindow.transform.GetChild(0).position;

        // spawn new window with attributes of old window
        spawnWindow(_filesWindowFab, windowPos);
        //filesWindow.GetComponentInChildren<RadialView>().enabled = isFollowing;

        destroyWindow(workspacesWindow);
    }

    public static Window3D spawnTextInputWindow()
    {
        return spawnWindow(_filesWindowFab, Vector3.zero);
    }

    private static Window3D spawnWindow(GameObject prefab, Vector3 position)
    {
        GameObject spawned = Instantiate(prefab, singleton.transform);
        
        if (Vector3.Distance(position, Vector3.zero) > 0)
            spawned.transform.GetChild(0).position = position;

        Window3D w = spawned.GetComponent<Window3D>();
        if (w == null) Debug.Log("No Window3D component on window.");
        else windows.Add(w);

        return w;
    }

    public static void destroyWindow(Window3D window)
    {
        windows.Remove(window);
        Destroy(window.gameObject);
    }

    public static void stopAllFollowing(Window3D caller)
    {
        foreach (Window3D w in windows)
            if (w != caller) w.stopFollowing();
    }

    void Awake()
    {
        singleton = this;

        _workspacesWindowFab = workspacesWindowFab;
        _filesWindowFab = filesWindowFab;
        _textEntryWindowFab = textEntryWindowFab;

        GameObject workspacesWindow = spawnWindow(_workspacesWindowFab, new Vector3(0, 0, 1f)).gameObject;
        workspacesWindow.GetComponentInChildren<RadialView>().enabled = true;
    }
}
