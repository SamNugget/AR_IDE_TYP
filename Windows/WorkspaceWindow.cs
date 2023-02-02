using UnityEngine;
using ActionManagement;

public class WorkspaceWindow : Window
{
    [SerializeField] private ActionButton backAButton;

    void Start()
    {
        // TODO: move to Window class
        backAButton.setAction(ActionManager.BACK_TO_WORKSPACES);
    }
}