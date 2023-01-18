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




    private static List<Window3D> windows = new List<Window3D>();

    public static Window3D getWindowWithComponent<T>()
    {
        foreach (Window3D window in windows)
        {
            // check at top level
            Transform w = window.transform;
            if (w.GetComponent<T>() != null)
                return window;
            // check children
            foreach (Transform child in w)
            {
                if (child.GetComponent<T>() != null)
                    return window;
            }
        }
        return null;
    }

    // ======= TO SAVE =======
    //    lastLocalPosTools
    //    lastLocalRotTools
    // lastLocalPosBlockSelect
    // lastLocalRotBlockSelect





    [SerializeField] private GameObject workspacesWindowFab;
    public static void spawnWorkspacesWindow()
    {
        // replace the files window with a workspace window
        Window3D filesWindow = getWindowWithComponent<FilesWindow>();
        swapWindows(filesWindow, singleton.workspacesWindowFab);

        singleton.fileWindowParent.gameObject.SetActive(false);
    }

    [SerializeField] private GameObject filesWindowFab;
    public static Window3D spawnFilesWindow()
    {
        // replace the workspace window with a files window
        Window3D workspacesWindow = getWindowWithComponent<WorkspacesButtonManager>();
        Window3D filesWindow = swapWindows(workspacesWindow, singleton.filesWindowFab);

        singleton.fileWindowParent.gameObject.SetActive(true);

        return filesWindow;
    }

    [SerializeField] private GameObject fileWindowFab;
    public static Window3D spawnFileWindow()
    {
        // find the workspaces window and get attributes
        Window3D filesWindow = getWindowWithComponent<FilesWindow>();

        // spawn new window with attributes of old window
        return spawnWindow(singleton.fileWindowFab, new Vector3(0f, 0f, -0.1f), true, filesWindow.transform.GetChild(0), singleton.fileWindowParent);
    }

    [SerializeField] private GameObject toolsWindowFab;
    [SerializeField] private GameObject blockSelectWindowFab;
    public static void moveEditToolWindows()
    {
        makeWindowChildOfWindow<ToolsWindow>(singleton.toolsWindowFab);
        makeWindowChildOfWindow<EditButtonManager>(singleton.blockSelectWindowFab);
    }

    [SerializeField] private GameObject textEntryWindowFab;
    public static Window3D spawnTextInputWindow()
    {
        Window3D lEW = BlockManager.getLastWindow();

        return spawnWindow(singleton.textEntryWindowFab, new Vector3(0f, 0f, -0.1f), true, lEW.transform.GetChild(0));
    }





    private static Window3D spawnWindow(GameObject prefab, Vector3 offset, bool nestedWindow = true, Transform toCopy = null, Transform parentOverride = null)
    {
        Transform spawned = Instantiate(prefab, (parentOverride == null ? singleton.transform : parentOverride)).transform;
        Transform newWindow = (nestedWindow ? spawned.GetChild(0) : spawned);
        if (toCopy == null)
        {
            newWindow.localPosition = offset;
        }
        else
        {
            offset = toCopy.right * offset.x + toCopy.up * offset.y + toCopy.forward * offset.z;
            newWindow.localPosition = toCopy.localPosition + offset;
            newWindow.localRotation = toCopy.localRotation;
        }

        Window3D w = spawned.GetComponent<Window3D>();
        if (w == null) Debug.Log("No Window3D component on window.");
        else windows.Add(w);

        return w;
    }

    private static Window3D swapWindows(Window3D existingWindow, GameObject newWindowFab)
    {
        // find the workspaces window and get attributes
        bool isFollowing = existingWindow.GetComponentInChildren<RadialView>().enabled;

        // spawn new window with attributes of old window
        Window3D newWindow = spawnWindow(newWindowFab, Vector3.zero, true, existingWindow.transform.GetChild(0));
        newWindow.GetComponentInChildren<RadialView>().enabled = isFollowing;

        destroyWindow(existingWindow);

        return newWindow;
    }

    private static void makeWindowChildOfWindow<T>(GameObject windowFab)
    {
        // get the last window being edited
        Window3D lastEdited = BlockManager.getLastWindow();
        if (lastEdited == null) return;

        // find the window
        Window3D window = getWindowWithComponent<T>();



        Transform lET = lastEdited.transform.GetChild(0);
        // if it doesn't exist, spawn
        if (window == null)
        {
            window = spawnWindow(windowFab, new Vector3(0f, 0f, -0.1f), false, null, lET);
        }
        else // otherwise change parent
        {
            Transform wT = window.transform;
            if (wT.parent == lET) return;

            Vector3 localPos = wT.localPosition;
            Quaternion localRot = wT.localRotation;

            wT.parent = lET;
            wT.localPosition = localPos;
            wT.localRotation = localRot;
        }
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
