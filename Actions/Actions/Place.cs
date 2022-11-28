public class Place : Mode
{
    public Place(char c) : base(c, /*multi-select:*/true) { }

    // which block variant to place
    private int blockToPlace = -1;

    public override void onCall(object data)
    {
        BlockManager.spawnBlock(blockToPlace, (Block)data);
    }

    public override void onSelect(object data)
    {
        int variantIndex = (int)data;
        blockToPlace = variantIndex;
    }

    public override void onDeselect()
    {

    }

    public override string getToolsWindowMessage()
    {
        string blockName = BlockManager.getBlockVariant(blockToPlace).getName();
        return ("Placing " + blockName + " block...");
    }
}
