using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    // ACTIONS
    private static Mode currentMode = null;
    public static void clearMode()
    {
        setCurrentMode(null, null);
    }
    private static void setCurrentMode(Mode mode, object data)
    {
        Window3D toolsWindow = WindowManager.getWindowWithComponent<ToolsWindow>();

        if (mode == currentMode) // if this is an already active mode
        {
            try
            {
                if (mode != null)
                {
                    if (mode.getMultiSelect())
                    {
                        mode.onSelect(data);
                        toolsWindow.setTitleTextMessage(mode.getToolsWindowMessage());
                    }
                    else mode.onCall(data);
                }

            }
            catch (Exception e)
            {
                Debug.Log("Err calling mode.");
                return;
            }
        }
        else // if this is not active
        {
            if (currentMode != null) currentMode.onDeselect(); // deselect current mode

            currentMode = mode; // switch state

            try
            {
                if (mode != null) mode.onSelect(data);
            }
            catch (Exception e)
            {
                Debug.Log("Err selecting mode.");
                Debug.Log(e.StackTrace);

                currentMode = null;
                toolsWindow.setTitleTextMessage("ERR");
                return;
            }

            if (mode == null) toolsWindow.setTitleTextMessage("");
            else toolsWindow.setTitleTextMessage(mode.getToolsWindowMessage());
        }
    }
    public static Mode getCurrentMode() { return currentMode; }

    public readonly static char PLACE_SELECT = 'p'; // mode
    public readonly static char DELETE_SELECT = 'd'; // mode
    public readonly static char INSERT_LINE = 'i'; // mode
    public readonly static char BLOCK_CLICKED = 'C';
    public readonly static char SAVE_CODE = 'S';
    public readonly static char CREATE_NAME = 'n'; // mode

    public readonly static char OPEN_WORKSPACE = 'W';
    public readonly static char OPEN_FILE = 'F';
    public readonly static char BACK_TO_WORKSPACES = 'B';
    public readonly static char CYCLE_CONSTRUCT = 'Y';
    //public readonly static char CREATE_VARIABLE = 'v'; // mode

    private static Act[] actions;
    private static Act getAction(char symbol)
    {
        foreach (Act a in actions)
            if (a.getSymbol() == symbol) return a;
        return null;
    }



    //                  |placement  |deletion   |empty replace      |split  |no action              |place in ANY
    //ANY               |  never  instantiated  |any                |yes    |                       |NA
    //EMPTY             |yes        |           |                   |       |                       |NA
    //USING             |           |yes        |UG                 |yes    |                       |must be explicit
    //===============================================================================================
    //ACCESS_MODIFIER   |           |           |                   |       |cycles through         |NA
    //TYPE              |           |yes        |TP  create if none |no     |                       |yes?
    //BOOLEAN_EXPRESSION|           |yes        |BE                 |(?)    |                       |must be explicit
    //NAMESPACE         |           |yes        |                   |       |keyboard or list       |must be explicit
    //BODY              |           |yes        |BY                 |yes    |                       |yes



    void Awake()
    {
        // Add all action classes to list
        List<Act> actionList = new List<Act>();
        actionList.Add(new Place(PLACE_SELECT));
        actionList.Add(new Delete(DELETE_SELECT));
        actionList.Add(new InsertLine(INSERT_LINE));
        actionList.Add(new BlockClicked(BLOCK_CLICKED));
        actionList.Add(new SaveCode(SAVE_CODE));
        actionList.Add(new CreateName(CREATE_NAME));

        actionList.Add(new OpenWorkspace(OPEN_WORKSPACE));
        actionList.Add(new OpenFile(OPEN_FILE));
        actionList.Add(new BackToWorkspaces(BACK_TO_WORKSPACES));



        actions = actionList.ToArray();
    }



    public static void callAction(char action, object data)
    {
        Act newAction = getAction(action);
        if (newAction == null)
        {
            Debug.Log("Action " + action + " was not recognised.");
            return;
        }
        Debug.Log("Action " + action);



        // is this newAction a mode-selection
        if (newAction is Mode mode)
        {
            setCurrentMode(mode, data);
        }
        // or immediate action
        else
        {
            try { newAction.onCall(data); }
            catch (Exception e) {
                Debug.Log("Err calling action " + action + " with data: " + data);
                Debug.Log(e.StackTrace);
            }
        }
    }



    void Update()
    {
        // for testing
        if (Input.GetKeyDown(KeyCode.Escape))
            clearMode();
    }
}
