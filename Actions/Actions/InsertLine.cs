public class InsertLine : Mode
{
    public InsertLine(char c) : base(c, /*multi-select:*/false) { }

    public override void onCall(object data)
    {
        if (!(data is Block)) return;

        Block parent = ((Block)data).getParent();
        BlockManager.splitBlock(parent);

        Block lastMaster = parent.getMasterBlock();
        lastMaster.setColliderEnabled(false);
        lastMaster.setSpecialChildBlock(BlockManager.getBlockVariantIndex("Insert Line"), false);
        lastMaster.setSpecialChildBlock(BlockManager.getBlockVariantIndex("Insert Line"), true);
    }

    public override void onSelect(object data)
    {
        Block lastMaster = BlockManager.lastMaster;
        lastMaster.setColliderEnabled(false);
        lastMaster.setSpecialChildBlock(BlockManager.getBlockVariantIndex("Insert Line"), true);
    }

    public override void onDeselect()
    {
        Block lastMaster = BlockManager.lastMaster;
        lastMaster.setColliderEnabled(true);
        lastMaster.setSpecialChildBlock(BlockManager.getBlockVariantIndex("Insert Line"), false);
    }

    public override string getToolsWindowMessage()
    {
        return ("Inserting lines...");
    }
}
