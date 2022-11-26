using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsButtonManager : ButtonManager2D
{
    protected override void distributeButtons()
    {
        int noOfButtons = 1;

        string[] buttonLabels = new string[noOfButtons];
        char[] actions = new char[noOfButtons];
        object[] data = new object[noOfButtons];

        buttonLabels[0] = "Delete";
        actions[0] = ActionManager.DELETE_SELECT;
        data[0] = null;

        distributeVertically(buttonLabels, actions, data);
    }
}
