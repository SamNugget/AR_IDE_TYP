using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsButtonManager : ButtonManager2D
{
    public override void distributeButtons()
    {
        int noOfButtons = 3;

        string[] buttonLabels = new string[noOfButtons];
        char[] actions = new char[noOfButtons];
        object[] data = new object[noOfButtons];

        // delete button
        buttonLabels[0] = "Delete";
        actions[0] = ActionManager.DELETE_SELECT;
        data[0] = null;

        // link button
        buttonLabels[1] = "Insert Line";
        actions[1] = ActionManager.INSERT_LINE;
        data[1] = null;

        // visualise button
        buttonLabels[2] = "Save";
        actions[2] = ActionManager.SAVE_CODE;
        data[2] = null;

        List<Transform> buttons = distributeHorizontally(buttonLabels, actions, data, transform);

        int width = 1;
        foreach (string label in buttonLabels) width += label.Length + 1;

        Window2D toolsWindow = GetComponentInParent<Window2D>();
        toolsWindow.setWidth(width); toolsWindow.setHeight(1);
        toolsWindow.resizeWindow();
    }
}
