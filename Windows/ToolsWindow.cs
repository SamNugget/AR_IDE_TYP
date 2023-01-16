using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionManagement;

public class ToolsWindow : Window3D
{
    [SerializeField] private ActionButton placeButton;
    [SerializeField] private ActionButton deleteButton;
    [SerializeField] private ActionButton saveButton;

    void Start()
    {
        placeButton.setAction(ActionManager.PLACE_SELECT);
        deleteButton.setAction(ActionManager.DELETE_SELECT);
        saveButton.setAction(ActionManager.SAVE_CODE);
    }
}
