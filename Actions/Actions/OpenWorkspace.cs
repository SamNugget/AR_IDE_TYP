using FileManagement;

public class OpenWorkspace : Act
{
    public OpenWorkspace(char c) : base(c) { }

    public override void onCall(object data)
    {
        FileManager.loadWorkspace((string)data);
        WindowManager.spawnFilesWindow();
    }
}
