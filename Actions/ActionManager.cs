using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    // ACTIONS
    private static Mode currentMode = null;
    private static void setCurrentMode(Mode mode, object data)
    {
        if (mode == currentMode) // if this is an already active mode
        {
            try
            {
                if (mode != null)
                {
                    if (mode.getMultiSelect()) mode.onSelect(data);
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

                currentMode = null;
                tools.setTitleTextMessage("ERR");

                return;
            }
        }

        tools.setTitleTextMessage(mode.getToolsWindowMessage());
    }
    public static Mode getCurrentMode() { return currentMode; }

    public readonly static char PLACE_SELECT = 'p'; // mode
    public readonly static char DELETE_SELECT = 'd'; // mode
    public readonly static char INSERT_LINE = 'i'; // mode
    public readonly static char BLOCK_CLICKED = 'B';
    public readonly static char SAVE_CODE = 'S';
    public readonly static char CREATE_VARIABLE = 'V'; // mode
    public readonly static char CREATE_NAME = 'N'; // mode

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



    // tools window for feedback
    private static Window2D tools;
    [SerializeField] private Window2D _toolsWindow;
    public static Window2D ToolsWindow
    {
        get { return tools; }
        set { tools = value; }
    }
    // edit window
    private static EditWindow edit;
    [SerializeField] private EditWindow _editWindow;
    public static EditWindow EditWindow
    {
        get { return edit; }
        set { edit = value; }
    }
    void Awake()
    {
        tools = _toolsWindow;
        edit = _editWindow;

        actions = new Act[6];
        actions[0] = new Place(PLACE_SELECT);
        actions[1] = new Delete(DELETE_SELECT);
        actions[2] = new InsertLine(INSERT_LINE);
        actions[3] = new BlockClicked(BLOCK_CLICKED);
        actions[4] = new SaveCode(SAVE_CODE);
        actions[5] = new CreateName(CREATE_NAME);
    }



    public static void callAction(char action, object data)
    {
        Act newAction = getAction(action);
        if (newAction == null)
        {
            Debug.Log("Action " + action + " was not recognised.");
            return;
        }



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
            }
        }
    }
}
