using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileManagement;

public class FilesButtonManager : ButtonManager2D
{
    public override void distributeButtons()
    {
        deleteButtons();



        string[] files = FileManager.fileNames;

        int noOfButtons = files.Length;

        string[] buttonLabels = new string[noOfButtons];
        char[] actions = new char[noOfButtons];
        object[] data = new object[noOfButtons];

        for (int i = 0; i < noOfButtons; i++)
        {
            buttonLabels[i] = files[i];
            actions[i] = '\0';
            data[i] = null;
        }

        distributeVertically(buttonLabels, actions, data);
    }
}
