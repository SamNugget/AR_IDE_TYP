public class BlockClicked : Act
{
    public BlockClicked(char c) : base(c) { }

    public override void onCall(object data)
    {
        Mode currentMode = ActionManager.getCurrentMode();
        char modeSymbol = (currentMode == null ? '\0' : currentMode.getSymbol());

        Block clicked = (Block)data;
        BlockManager.BlockVariant variant = clicked.getBlockVariant();
        string type = variant.getBlockType();



        bool codeModified = true;
        // check for special types first
        if (BlockManager.isCycleable(type))
        {
            int nVIndex = BlockManager.cycleBlockVariantIndex(variant);
            BlockManager.spawnBlock(nVIndex, clicked, false);

            Block master = clicked.getMasterBlock();
            if (master != null) master.drawBlock();
        }
        else if (type == BlockManager.INSERT_LINE)
        {
            // this must be insert line mode, so call insert line
            currentMode.onCall(data);
        }
        else if (type == BlockManager.PLACE_FIELD || type == BlockManager.PLACE_METHOD)
        {
            // split clicked, and put it on the bottom
            BlockManager.splitBlock(clicked, false);

            Block splitter = clicked.getParent();

            // get object reference to top (empty) block
            int clickedIndex = splitter.getSubBlockIndex(clicked);
            Block toReplace = splitter.getSubBlock(clickedIndex == 0 ? 1 : 0);

            // get variant index of to place
            int variantIndex;
            if (type == BlockManager.PLACE_FIELD)
                variantIndex = BlockManager.getBlockVariantIndex("Field");
            else variantIndex = BlockManager.getBlockVariantIndex("Method");

            // replace top block
            BlockManager.spawnBlock(variantIndex, toReplace);
        }


        else if (modeSymbol == ActionManager.DELETE_SELECT)
        {
            // will delete or place
            currentMode.onCall(data);
        }


        // check for lower-priority special types
        else if (type == BlockManager.VARIABLE_NAME || type == BlockManager.NAME)
        {
            int variantIndex = BlockManager.getBlockVariantIndex(variant);
            ActionManager.callAction(ActionManager.PLACE_SELECT, variantIndex);
            codeModified = false; // just copying, no changes
        }


        else if (modeSymbol == ActionManager.PLACE_SELECT)
        {
            // will delete or place
            currentMode.onCall(data);
        }
        else codeModified = false; // if dropped out, no changes



        if (codeModified)
        ;//    ((EditWindow)WindowManager.getWindowWithComponent<EditWindow>()).setTitleTextMessage("*");
    }
}
