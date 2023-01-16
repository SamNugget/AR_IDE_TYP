using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionManagement;

public class FilesWindow : Window3D
{
    [SerializeField] private ActionButton backButton;

    void Start()
    {
        backButton.setAction(ActionManager.BACK_TO_WORKSPACES);
    }
}
