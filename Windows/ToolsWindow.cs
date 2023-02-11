using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionManagement;

public class ToolsWindow : Window
{
    [SerializeField] private ActionButton insertButton;
    [SerializeField] private ActionButton deleteButton;
    [SerializeField] private ActionButton saveButton;
    [SerializeField] private ActionButton compileButton;
    [SerializeField] private ActionButton codeButton;

    void Start()
    {
        insertButton.setAction(ActionManager.INSERT_LINE);
        deleteButton.setAction(ActionManager.DELETE_SELECT);
        saveButton.setAction(ActionManager.SAVE_CODE);
        compileButton.setAction(ActionManager.COMPILE);
        codeButton.setAction(ActionManager.CODE);
    }
}
