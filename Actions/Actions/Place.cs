using System.Collections.Generic;

public class Place : Mode
{
    public Place(char c) : base(c, /*multi-select:*/true) { }

    // which block variant to place
    private int blockToPlace = -1;

    public override void onCall(object data)
    {
        Block lastMaster = ((Block)data).getMasterBlock();
        BlockManager.lastMaster = lastMaster;
        lastMaster.setColliderEnabled(false);
        lastMaster.setColliderEnabled(true, new List<string>() { BlockManager.EMPTY, BlockManager.VARIABLE_NAME, BlockManager.NAME });
        
        BlockManager.spawnBlock(blockToPlace, (Block)data);
    }

    public override void onSelect(object data)
    {
        int variantIndex = (int)data;
        blockToPlace = variantIndex;

        Block lastMaster = BlockManager.lastMaster;
        lastMaster.setColliderEnabled(false);
        lastMaster.setColliderEnabled(true, new List<string>() { BlockManager.EMPTY, BlockManager.VARIABLE_NAME, BlockManager.NAME });
    }

    public override void onDeselect()
    {
        BlockManager.lastMaster.setColliderEnabled(true);
    }

    public override string getToolsWindowMessage()
    {
        string blockName = BlockManager.getBlockVariant(blockToPlace).getName();
        return ("Placing " + blockName + " block...");
    }
}
