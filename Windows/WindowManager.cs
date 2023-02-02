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





    private static List<Window> windows = new List<Window>();

    public static Window getWindowWithComponent<T>()
    {
        foreach (Window window in windows)
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

    public static List<Window> getWindowsWithComponent<T>()
    {
        List<Window> found = new List<Window>();

        foreach (Window window in windows)
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
        List<Window> blockWindows = getWindowsWithComponent<EditWindow>();
        foreach (Window w in blockWindows)
            ((EditWindow)w).masterBlock.setColliderEnabled(enabled, mask, invert);
    }

    public static void updateEditWindowSpecialBlocks(int variantIndex, bool enabled) // e.g., for insert line
    {
        List<Window> blockWindows = getWindowsWithComponent<EditWindow>();
        foreach (Window w in blockWindows)
            ((EditWindow)w).masterBlock.setSpecialChildBlock(variantIndex, enabled);
    }

    public static void enableEditWindowLeafBlocks() // for deletion of blocks
    {
        List<Window> blockWindows = getWindowsWithComponent<EditWindow>();
        foreach (Window w in blockWindows)
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

        public WindowSettings(Window window)
        {
            Transform w = window.transform;
            position = w.localPosition;
            rotation = w.localRotation;
        }

        public void copyTo(Window window)
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
        Window workspaceWindow = getWindowWithComponent<WorkspaceWindow>();
        swapWindows(workspaceWindow, singleton.workspacesWindowFab);

        singleton.fileWindowParent.gameObject.SetActive(false);
    }


    [SerializeField] private GameObject workspaceWindowFab;
    public static Window spawnWorkspaceWindow()
    {
        // replace the workspace window with a files window
        Window workspacesWindow = getWindowWithComponent<WorkspacesButtonManager>();
        Window workspaceWindow = swapWindows(workspacesWindow, singleton.workspaceWindowFab);

        singleton.fileWindowParent.gameObject.SetActive(true);

        return workspaceWindow;
    }


    [SerializeField] private GameObject classWindowFab;
    [SerializeField] private GameObject interfaceWindowFab;
    public static Window spawnFileWindow(bool isClass)
    {
        ActionManager.clearMode();

        // find the workspaces window and get attributes
        Window filesWindow = getWindowWithComponent<WorkspaceWindow>();

        // spawn new window with attributes of old window
        GameObject fab = isClass ? singleton.classWindowFab : singleton.interfaceWindowFab;
        return spawnWindow(fab, new Vector3(0f, 0f, -0.1f), filesWindow.transform.GetChild(0), singleton.fileWindowParent);
    }


    [SerializeField] private GameObject methodWindowFab;
    private static WindowSettings methodWS = new WindowSettings();
    public static void moveMethodWindow(FileWindow fileWindow, Block declarationBlock)
    {
        ActionManager.clearMode();

        // find method save
        MethodS mS = fileWindow.referenceTypeSave.findMethodSave(declarationBlock);
        if (mS == null)
        {
            Debug.Log("Err, could not find method save.");
            return;
        }

        // create method window
        MethodWindow mW = (MethodWindow)replaceSoleChildWindow<MethodWindow>(singleton.methodWindowFab, ref methodWS, (Window)fileWindow);

        // pass method save into window
        mW.methodSave = mS;
        // pass new method body block into file window
        mS.methodBodyMaster = mW.masterBlock;
    }


    [SerializeField] private GameObject toolsWindowFab;
    private static WindowSettings toolsWS = new WindowSettings(new Vector3(0f, -0.15f, -0.01f));
    [SerializeField] private GameObject blockSelectWindowFab;
    private static WindowSettings blockWS = new WindowSettings(new Vector3(-0.15f, 0f, 0f));
    public static void moveEditToolWindows()
    {
        replaceChildSoleWindow<ToolsWindow>(singleton.toolsWindowFab, ref toolsWS);
        replaceChildSoleWindow<EditButtonManager>(singleton.blockSelectWindowFab, ref blockWS);
    }


    [SerializeField] private GameObject textEntryWindowFab;
    private static WindowSettings textEntryWS = new WindowSettings();
    public static Window spawnTextInputWindow(Window parent = null)
    {
        return replaceChildSoleWindow<TextEntryWindow>(singleton.textEntryWindowFab, ref textEntryWS, parent);
    }





    // old method to be superseded
    private static Window spawnWindow(GameObject prefab, Vector3 offset, Transform toCopy = null, Transform parentOverride = null)
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

        Window w = newWindow.parent.GetComponent<Window>();
        if (w == null) Debug.Log("No Window component on window.");
        else windows.Add(w);

        return w;
    }

    private static Window spawnWindow(GameObject prefab, WindowSettings settings, Transform parent)
    {
        Transform spawned = Instantiate(prefab, parent).transform;
        spawned.localPosition = settings.position;
        spawned.localRotation = settings.rotation;

        Window w = spawned.GetComponent<Window>();
        if (w == null) Debug.Log("No Window component on window.");
        else windows.Add(w);

        return w;
    }

    private static Window swapWindows(Window existingWindow, GameObject newWindowFab)
    {
        // find the workspaces window and get attributes
        bool isFollowing = existingWindow.GetComponentInChildren<RadialView>().enabled;

        // spawn new window with attributes of old window
        Window newWindow = spawnWindow(newWindowFab, Vector3.zero, existingWindow.transform.GetChild(0));
        newWindow.GetComponentInChildren<RadialView>().enabled = isFollowing;

        existingWindow.close();

        return newWindow;
    }

    // finds the sole window with component T and changes the parent
    private static Window replaceChildSoleWindow<T>(GameObject windowFab, ref WindowSettings settings, Window parent = null)
    {
        // get the last window being edited
        if (parent == null)
        {
            parent = (Window)BlockManager.lastFileWindow;
            if (parent == null) return null;
        }


        // find the window
        Window window = getWindowWithComponent<T>();


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

    // finds the sole window (if exists) attached to parent, and moves it/spawns one
    private static Window replaceSoleChildWindow<T>(GameObject windowFab, ref WindowSettings settings, Window parent) where T : Window
    {
        Transform p = parent.transform.GetChild(0);

        // find the window
        T w = p.GetComponentInChildren<T>();

        // if window exists, close
        if (w != null)
            ((Window)w).close();

        // spawn a new child window
        return spawnWindow(windowFab, settings, p);
    }

    public static void destroyWindow(Window window)
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

    /*public static void stopAllFollowing(Window caller)
    {
        foreach (Window w in windows)
            if (w != caller) w.stopFollowing();
    }*/





    void Awake()
    {
        singleton = this;

        GameObject workspacesWindow = spawnWindow(workspacesWindowFab, new Vector3(0f, 0f, 1f)).gameObject;
        workspacesWindow.GetComponentInChildren<RadialView>().enabled = true;
    }
}
