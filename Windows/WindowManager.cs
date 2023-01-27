using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using ActionManagement;

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

    public static List<Window3D> getWindowsWithComponent<T>()
    {
        List<Window3D> found = new List<Window3D>();

        foreach (Window3D window in windows)
        {
            // check at top level
            Transform w = window.transform;
            if (w.GetComponent<T>() != null)
            {
                found.Add(window);
                continue;
            }
            // check children
            foreach (Transform child in w)
            {
                if (child.GetComponent<T>() != null)
                    found.Add(window);
            }
        }
        return found;
    }





    public static void updateEditWindowColliders(bool enabled, List<string> mask = null, bool invert = false)
    {
        List<Window3D> blockWindows = getWindowsWithComponent<EditWindow>();
        foreach (Window3D w in blockWindows)
            ((EditWindow)w).masterBlock.setColliderEnabled(enabled, mask, invert);
    }

    public static void updateEditWindowSpecialBlocks(int variantIndex, bool enabled) // e.g., for insert line
    {
        List<Window3D> blockWindows = getWindowsWithComponent<EditWindow>();
        foreach (Window3D w in blockWindows)
            ((EditWindow)w).masterBlock.setSpecialChildBlock(variantIndex, enabled);
    }

    public static void enableEditWindowLeafBlocks() // for deletion of blocks
    {
        List<Window3D> blockWindows = getWindowsWithComponent<EditWindow>();
        foreach (Window3D w in blockWindows)
            ((EditWindow)w).masterBlock.enableLeafBlocks();
    }





    // TODO: these should be saved
    public class WindowSettings
    {
        public Vector3 position;
        public Quaternion rotation;

        public WindowSettings()
        {
            position = new Vector3(0f, 0f, -0.1f);
            rotation = Quaternion.identity;
        }

        public WindowSettings(Vector3 position)
        {
            this.position = position;
            rotation = Quaternion.identity;
        }

        public WindowSettings(Window3D window)
        {
            Transform w = window.transform;
            position = w.localPosition;
            rotation = w.localRotation;
        }

        public void copyTo(Window3D window)
        {
            if (window == null) return;
            window.transform.localPosition = position;
            window.transform.localRotation = rotation;
        }
    }





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
        ActionManager.clearMode();

        // find the workspaces window and get attributes
        Window3D filesWindow = getWindowWithComponent<FilesWindow>();

        // spawn new window with attributes of old window
        return spawnWindow(singleton.fileWindowFab, new Vector3(0f, 0f, -0.1f), filesWindow.transform.GetChild(0), singleton.fileWindowParent);
    }


    [SerializeField] private GameObject methodWindowFab;
    private static WindowSettings methodWS = new WindowSettings();
    public static void moveMethodWindow()
    {
        ActionManager.clearMode();
        makeWindowChildOfWindow<MethodWindow>(singleton.methodWindowFab, ref methodWS);
    }


    [SerializeField] private GameObject toolsWindowFab;
    private static WindowSettings toolsWS = new WindowSettings(new Vector3(0f, -0.12f, 0f));
    [SerializeField] private GameObject blockSelectWindowFab;
    private static WindowSettings blockWS = new WindowSettings(new Vector3(-0.15f, 0f, -0f));
    public static void moveEditToolWindows()
    {
        makeWindowChildOfWindow<ToolsWindow>(singleton.toolsWindowFab, ref toolsWS);
        makeWindowChildOfWindow<EditButtonManager>(singleton.blockSelectWindowFab, ref blockWS);
    }


    [SerializeField] private GameObject textEntryWindowFab;
    private static WindowSettings textEntryWS = new WindowSettings();
    public static Window3D spawnTextInputWindow(Window3D parent = null)
    {
        return makeWindowChildOfWindow<TextEntryWindow>(singleton.textEntryWindowFab, ref textEntryWS, parent);
    }





    // old method to be superseded
    private static Window3D spawnWindow(GameObject prefab, Vector3 offset, Transform toCopy = null, Transform parentOverride = null)
    {
        if (parentOverride == null)
            parentOverride = singleton.transform;
        Transform newWindow = Instantiate(prefab, parentOverride).transform.GetChild(0);

        if (toCopy == null)
            newWindow.localPosition = offset;
        else
        {
            offset = toCopy.right * offset.x + toCopy.up * offset.y + toCopy.forward * offset.z;
            newWindow.localPosition = toCopy.localPosition + offset;
            newWindow.localRotation = toCopy.localRotation;
        }

        Window3D w = newWindow.parent.GetComponent<Window3D>();
        if (w == null) Debug.Log("No Window3D component on window.");
        else windows.Add(w);

        return w;
    }

    private static Window3D spawnWindow(GameObject prefab, WindowSettings settings, Transform parent)
    {
        Transform spawned = Instantiate(prefab, parent).transform;
        spawned.localPosition = settings.position;
        spawned.localRotation = settings.rotation;

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
        Window3D newWindow = spawnWindow(newWindowFab, Vector3.zero, existingWindow.transform.GetChild(0));
        newWindow.GetComponentInChildren<RadialView>().enabled = isFollowing;

        destroyWindow(existingWindow);

        return newWindow;
    }

    // finds the sole window with component T and changes the parent
    private static Window3D makeWindowChildOfWindow<T>(GameObject windowFab, ref WindowSettings settings, Window3D parent = null)
    {
        // get the last window being edited
        if (parent == null)
        {
            parent = BlockManager.getLastWindow();
            if (parent == null) return null;
        }


        // find the window
        Window3D window = getWindowWithComponent<T>();


        Transform lET = parent.transform.GetChild(0);
        // if it doesn't exist, spawn
        if (window == null)
            window = spawnWindow(windowFab, settings, lET);
        else // otherwise change parent
        {
            Transform wT = window.transform;
            if (wT.parent == lET) return null;

            settings = new WindowSettings(window);

            wT.parent = lET;
            wT.localPosition = settings.position;
            wT.localRotation = settings.rotation;
        }

        return window;
    }

    public static void destroyWindow(Window3D window)
    {
        // this is not very elegant
        if (window is ToolsWindow)
            toolsWS = new WindowSettings(window);
        else if (window.GetComponentInChildren<EditButtonManager>() != null)
            blockWS = new WindowSettings(window);
        else if (window is TextEntryWindow)
            textEntryWS = new WindowSettings(window);
        else if (window is MethodWindow)
            methodWS = new WindowSettings(window);



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

        GameObject workspacesWindow = spawnWindow(workspacesWindowFab, new Vector3(0f, 0f, 1f)).gameObject;
        workspacesWindow.GetComponentInChildren<RadialView>().enabled = true;
    }
}
