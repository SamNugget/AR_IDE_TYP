using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsButtonManager : ButtonManager2D
{
    public override void distributeButtons()
    {
        transform.localPosition += (Vector3)FontManager.lettersAndLinesToVector(0, 2);

        int noOfButtons = 2;

        string[] buttonLabels = new string[noOfButtons];
        char[] actions = new char[noOfButtons];
        object[] data = new object[noOfButtons];

        // delete button
        buttonLabels[0] = "Edit/Blueprint";
        actions[0] = '\0';
        data[0] = null;

        // link button
        buttonLabels[1] = "Minimise";
        actions[1] = '\0';
        data[1] = null;

        List<Transform> buttons = distributeHorizontally(buttonLabels, actions, data, transform);
    }
}
