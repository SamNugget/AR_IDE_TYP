public class InsertLine : Mode
{
    public InsertLine(char c) : base(c, /*multi-select:*/false) { }

    public override void onCall(object data)
    {
        if (!(data is Block)) return;

        Block parent = ((Block)data).getParent();
        BlockManager.splitBlock(parent);

        EditWindow editWindow = (EditWindow)WindowManager.getWindowWithComponent<EditWindow>();
        editWindow.setCollidersEnabled(false);
        editWindow.setSpecialChildBlocks(BlockManager.getBlockVariantIndex("Insert Line"), false);
        editWindow.setSpecialChildBlocks(BlockManager.getBlockVariantIndex("Insert Line"), true);
    }

    public override void onSelect(object data) { select(false); }

    public override void onDeselect() { select(true); }

    private void select(bool de)
    {
        EditWindow editWindow = (EditWindow)WindowManager.getWindowWithComponent<EditWindow>();
        editWindow.setCollidersEnabled(de);
        editWindow.setSpecialChildBlocks(BlockManager.getBlockVariantIndex("Insert Line"), !de);
    }

    public override string getToolsWindowMessage()
    {
        return ("Inserting lines...");
    }
}
