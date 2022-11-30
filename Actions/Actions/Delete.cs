using UnityEngine;

public class Delete : Mode
{
    public Delete(char c) : base(c, /*multi-select:*/true) { }

    public override void onCall(object data)
    {
        Block toReplace = (Block)data;
        Block parent = toReplace.getParent();
        if (parent == null) {
            Debug.Log("Can't delete the master block."); return;
        }

        // TODO: will deleting this affect variable declaration?

        /*string actualType = ((Block)data).getBlockVariant().getBlockType();
        if (actualType.Equals(BlockManager.EMPTY) || actualType.Equals(BlockManager.ACCESS_MODIFIER))
        {
            Debug.Log("Can't delete blocks of this type.");
            return;
        }*/

        string[] subBlockTypes = parent.getBlockVariant().getSubBlockTypes();
        int subBlockIndex = parent.getSubBlockIndex(toReplace);
        string supposedType = subBlockTypes[subBlockIndex]; // not the actual type, but what should be here
        if (supposedType.Equals(BlockManager.NEW_NAME))
        {
            Debug.Log("Can't delete a name.");
            return;
        }

        BlockManager.spawnBlock(0, toReplace, false);
        onSelect(null);
    }

    public override void onSelect(object data)
    {
        // hide all blocks that can't be deleted
        ActionManager.EditWindow.setCollidersEnabled(false, BlockManager.EMPTY);
        ActionManager.EditWindow.setCollidersEnabled(false, BlockManager.CONSTRUCT);
        ActionManager.EditWindow.setCollidersEnabled(false, BlockManager.ACCESS_MODIFIER);
        ActionManager.EditWindow.setCollidersEnabled(false, BlockManager.TRUE_FALSE);
    }

    public override void onDeselect()
    {
        ActionManager.EditWindow.setCollidersEnabled(true);
    }

    public override string getToolsWindowMessage()
    {
        return ("Deleting...");
    }
}
