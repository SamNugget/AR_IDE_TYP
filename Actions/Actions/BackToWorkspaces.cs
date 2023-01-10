public class BackToWorkspaces : Act
{
    public BackToWorkspaces(char c) : base(c) { }

    public override void onCall(object data)
    {
        WindowManager.spawnWorkspacesWindow();

        // TODO: make user choose whether to save
    }
}
