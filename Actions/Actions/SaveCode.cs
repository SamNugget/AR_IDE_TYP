using FileManagement;

public class SaveCode : Act
{
    public SaveCode(char c) : base(c) { }

    public override void onCall(object data)
    {
        //Window3D editWindow = BlockManager.getLastEditWindow();

        Window3D toolsWindow = (ToolsWindow)WindowManager.getWindowWithComponent<ToolsWindow>();

        if (FileManager.saveAllFiles())
            toolsWindow.setTitleTextMessage("Saved");
        else toolsWindow.setTitleTextMessage("Err saving");
    }
}
