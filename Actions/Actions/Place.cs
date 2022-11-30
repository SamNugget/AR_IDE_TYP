public class Place : Mode
{
    public Place(char c) : base(c, /*multi-select:*/true) { }

    // which block variant to place
    private int blockToPlace = -1;

    public override void onCall(object data)
    {
        ActionManager.EditWindow.setCollidersEnabled(false);
        ActionManager.EditWindow.setCollidersEnabled(true, BlockManager.EMPTY);
        ActionManager.EditWindow.setCollidersEnabled(true, BlockManager.VARIABLE_NAME);
        ActionManager.EditWindow.setCollidersEnabled(true, BlockManager.NAME);
        BlockManager.spawnBlock(blockToPlace, (Block)data);
    }

    public override void onSelect(object data)
    {
        int variantIndex = (int)data;
        blockToPlace = variantIndex;
        ActionManager.EditWindow.setCollidersEnabled(false);
        ActionManager.EditWindow.setCollidersEnabled(true, BlockManager.EMPTY);
        ActionManager.EditWindow.setCollidersEnabled(true, BlockManager.VARIABLE_NAME);
        ActionManager.EditWindow.setCollidersEnabled(true, BlockManager.NAME);
    }

    public override void onDeselect()
    {
        ActionManager.EditWindow.setCollidersEnabled(true);
    }

    public override string getToolsWindowMessage()
    {
        string blockName = BlockManager.getBlockVariant(blockToPlace).getName();
        return ("Placing " + blockName + " block...");
    }
}
