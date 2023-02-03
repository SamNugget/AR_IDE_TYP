using ActionManagement;

public class WorkspaceWindow : Window
{
    public void back()
    {
        ActionManager.callAction(ActionManager.BACK_TO_WORKSPACES, null);
    }
}