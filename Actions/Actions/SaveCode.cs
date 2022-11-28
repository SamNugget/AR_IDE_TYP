public class SaveCode : Act
{
    public SaveCode(char c) : base(c) { }

    public override void onCall(object data)
    {
        EditWindow editWindow = ActionManager.EditWindow;
        editWindow.saveCode();
        editWindow.setTitleTextMessage("Saved");
    }
}
