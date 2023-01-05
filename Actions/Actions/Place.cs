public class Place : Mode
{
    public Place(char c) : base(c, /*multi-select:*/true) { }

    // which block variant to place
    private int blockToPlace = -1;

    public override void onCall(object data)
    {
        EditWindow editWindow = (EditWindow)WindowManager.getWindowWithComponent<EditWindow>();
        editWindow.setCollidersEnabled(false);
        editWindow.setCollidersEnabled(true, BlockManager.EMPTY);
        editWindow.setCollidersEnabled(true, BlockManager.VARIABLE_NAME);
        editWindow.setCollidersEnabled(true, BlockManager.NAME);
        BlockManager.spawnBlock(blockToPlace, (Block)data);
    }

    public override void onSelect(object data)
    {
        int variantIndex = (int)data;
        blockToPlace = variantIndex;
        EditWindow editWindow = (EditWindow)WindowManager.getWindowWithComponent<EditWindow>();
        editWindow.setCollidersEnabled(false);
        editWindow.setCollidersEnabled(true, BlockManager.EMPTY);
        editWindow.setCollidersEnabled(true, BlockManager.VARIABLE_NAME);
        editWindow.setCollidersEnabled(true, BlockManager.NAME);
    }

    public override void onDeselect()
    {
        ((EditWindow)WindowManager.getWindowWithComponent<EditWindow>()).setCollidersEnabled(true);
    }

    public override string getToolsWindowMessage()
    {
        string blockName = BlockManager.getBlockVariant(blockToPlace).getName();
        return ("Placing " + blockName + " block...");
    }
}
