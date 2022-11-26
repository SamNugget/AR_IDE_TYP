using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsButtonManager : ButtonManager2D
{
    protected override void distributeButtons()
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
        buttonLabels[1] = "Link";
        actions[1] = 'Z';
        data[1] = null;

        // visualise button
        buttonLabels[2] = "Visualise";
        actions[2] = 'Z';
        data[2] = null;

        List<Transform> buttons = distributeHorizontally(buttonLabels, actions, data);

        int width = 1;
        foreach (string label in buttonLabels) width += label.Length + 1;

        Window2D toolsWindow = GetComponent<Window2D>();
        toolsWindow.setWidth(width); toolsWindow.setHeight(1);
        toolsWindow.resizeWindow();
        Debug.Log(width);
    }
}
