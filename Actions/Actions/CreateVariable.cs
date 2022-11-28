using UnityEngine;

public class CreateVariable : Mode
{
    private TextEntryWindow textEntryWindow;

    public CreateVariable(char c) : base(c, /*multi-select:*/false) { }

    public override void onCall(object data)
    {
        EditWindow editWindow = ActionManager.EditWindow;
        editWindow.addVariable((string)data);
    }

    public override void onSelect(object data)
    {
        if (textEntryWindow == null)
        {
            GameObject spawned = WindowManager.spawnWindow(WindowManager.textEntryFab);
            EditWindow editWindow = ActionManager.EditWindow;
            int editWindowHeight = editWindow.getHeight();
            spawned.transform.position = editWindow.transform.position - new Vector3(0f, FontManager.lineHeight * (editWindowHeight + 2), 0f);

            textEntryWindow = spawned.GetComponent<TextEntryWindow>();
        }
    }

    public override void onDeselect()
    {
        WindowManager.destroyWindow(textEntryWindow.gameObject);
    }

    public override string getToolsWindowMessage()
    {
        return ("Creating variable...");
    }
}
