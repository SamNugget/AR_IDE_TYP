using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class WindowManager : MonoBehaviour
{
    private static WindowManager singleton;

    public Transform fileWindowParent;

    [SerializeField] private float _blockScale = 1f;
    public static float blockScale { get { return singleton._blockScale; } }

    [SerializeField] private GameObject workspacesWindowFab;
    private static GameObject _workspacesWindowFab;
    [SerializeField] private GameObject filesWindowFab;
    private static GameObject _filesWindowFab;
    [SerializeField] private GameObject fileWindowFab;
    private static GameObject _fileWindowFab;
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

    private static void swapWindows(Window3D existingWindow, GameObject newWindowFab)
    {
        // find the workspaces window and get attributes
        bool isFollowing = existingWindow.GetComponentInChildren<RadialView>().enabled;

        // spawn new window with attributes of old window
        Window3D newWindow = spawnWindow(newWindowFab, Vector3.zero, existingWindow.transform);
        newWindow.GetComponentInChildren<RadialView>().enabled = isFollowing;

        destroyWindow(existingWindow);
    }

    public static void spawnFilesWindow()
    {
        // replace the workspace window with a files window
        Window3D workspacesWindow = getWindowWithComponent<WorkspacesButtonManager>();
        swapWindows(workspacesWindow, _filesWindowFab);

        singleton.fileWindowParent.gameObject.SetActive(true);
    }

    public static void spawnWorkspacesWindow()
    {
        // replace the files window with a workspace window
        Window3D filesWindow = getWindowWithComponent<FilesButtonManager>();
        swapWindows(filesWindow, _workspacesWindowFab);

        singleton.fileWindowParent.gameObject.SetActive(false);
    }

    public static Window3D spawnFileWindow()
    {
        // find the workspaces window and get attributes
        Window3D filesWindow = getWindowWithComponent<FilesButtonManager>();

        // spawn new window with attributes of old window
        return spawnWindow(_fileWindowFab, new Vector3(0f, 0f, -0.1f), filesWindow.transform, singleton.fileWindowParent);
    }

    public static Window3D spawnTextInputWindow()
    {
        return spawnWindow(_filesWindowFab, Vector3.zero, null);
    }

    private static Window3D spawnWindow(GameObject prefab, Vector3 offset, Transform toCopy = null, Transform parentOverride = null)
    {
        Transform spawned = Instantiate(prefab, (parentOverride == null ? singleton.transform : parentOverride)).transform;
        Transform newWindow = spawned.GetChild(0);
        if (toCopy == null)
        {
            newWindow.localPosition = offset;
        }
        else
        {
            toCopy = toCopy.GetChild(0);
            offset = toCopy.right * offset.x + toCopy.up * offset.y + toCopy.forward * offset.z;
            newWindow.localPosition = toCopy.localPosition + offset;
            newWindow.localRotation = toCopy.localRotation;
        }

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
        _fileWindowFab = fileWindowFab;
        _textEntryWindowFab = textEntryWindowFab;

        GameObject workspacesWindow = spawnWindow(_workspacesWindowFab, new Vector3(0, 0, 1f)).gameObject;
        workspacesWindow.GetComponentInChildren<RadialView>().enabled = true;
    }
}
