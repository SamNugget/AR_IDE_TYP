using UnityEngine;

public class Delete : Mode
{
    public Delete(char c) : base(c, /*multi-select:*/true) { }

    public override void onCall(object data)
    {
        string type = ((Block)data).getBlockVariant().getBlockType();
        if (type.Equals(BlockManager.EMPTY) || type.Equals(BlockManager.ACCESS_MODIFIER))
        {
            Debug.Log("Can't delete blocks of this type.");
            return;
        }

        BlockManager.spawnBlock(0, (Block)data, false);
        EditWindow editWindow = ActionManager.EditWindow;
        editWindow.setCollidersEnabled(false, 0);
    }

    public override void onSelect(object data)
    {
        EditWindow editWindow = ActionManager.EditWindow;
        editWindow.setCollidersEnabled(false, 0);
    }

    public override void onDeselect()
    {
        EditWindow editWindow = ActionManager.EditWindow;
        editWindow.setCollidersEnabled(true);
    }

    public override string getToolsWindowMessage()
    {
        return ("Deleting...");
    }
}
