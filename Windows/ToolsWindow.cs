using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionManagement;

public class ToolsWindow : Window
{
    [SerializeField] private ActionButton placeButton;
    [SerializeField] private ActionButton deleteButton;
    [SerializeField] private ActionButton saveButton;

    void Start()
    {
        placeButton.setAction(ActionManager.INSERT_LINE);
        deleteButton.setAction(ActionManager.DELETE_SELECT);
        saveButton.setAction(ActionManager.SAVE_CODE);
    }
}
