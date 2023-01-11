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
    [SerializeField] private GameObject filesWindowFab;
    [SerializeField] private GameObject fileWindowFab;
    [SerializeField] private GameObject toolsWindowFab;
    [SerializeField] private GameObject textEntryWindowFab;



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

    private static Window3D swapWindows(Window3D existingWindow, GameObject newWindowFab)
    {
        // find the workspaces window and get attributes
        bool isFollowing = existingWindow.GetComponentInChildren<RadialView>().enabled;

        // spawn new window with attributes of old window
        Window3D newWindow = spawnWindow(newWindowFab, Vector3.zero, existingWindow.transform);
        newWindow.GetComponentInChildren<RadialView>().enabled = isFollowing;

        destroyWindow(existingWindow);

        return newWindow;
    }

    public static void spawnWorkspacesWindow()
    {
        // replace the files window with a workspace window
        Window3D filesWindow = getWindowWithComponent<FilesButtonManager>();
        swapWindows(filesWindow, singleton.workspacesWindowFab);

        singleton.fileWindowParent.gameObject.SetActive(false);
    }

    public static Window3D spawnFilesWindow()
    {
        // replace the workspace window with a files window
        Window3D workspacesWindow = getWindowWithComponent<WorkspacesButtonManager>();
        Window3D filesWindow = swapWindows(workspacesWindow, singleton.filesWindowFab);

        singleton.fileWindowParent.gameObject.SetActive(true);

        return filesWindow;
    }

    public static Window3D spawnFileWindow()
    {
        // find the workspaces window and get attributes
        Window3D filesWindow = getWindowWithComponent<FilesButtonManager>();

        // spawn new window with attributes of old window
        return spawnWindow(singleton.fileWindowFab, new Vector3(0f, 0f, -0.1f), filesWindow.transform, singleton.fileWindowParent);
    }

    public static void moveToolsWindow()
    {
        // find the tools window
        Window3D tW = getWindowWithComponent<ToolsWindow>();
        if (tW == null)
            tW = spawnWindow(singleton.toolsWindowFab, Vector3.zero);
        tW.gameObject.SetActive(true);

        Window3D eW = BlockManager.getLastEditWindow();
        if (eW == null) return;

        Transform beingEdited = eW.transform.GetChild(0);
        Transform toolsWindow = tW.transform.GetChild(0);
        toolsWindow.rotation = beingEdited.rotation;
        toolsWindow.position = beingEdited.position + (toolsWindow.forward * -0.1f);
    }

    public static Window3D spawnTextInputWindow()
    {
        Block masterBlock = BlockManager.lastMaster;

        return spawnWindow(singleton.textEntryWindowFab, new Vector3(0f, 0f, -0.1f), masterBlock.transform);
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

        GameObject workspacesWindow = spawnWindow(workspacesWindowFab, new Vector3(0, 0, 1f)).gameObject;
        workspacesWindow.GetComponentInChildren<RadialView>().enabled = true;
    }
}
