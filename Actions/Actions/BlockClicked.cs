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

            Window3D window = clicked.getWindow3D();
            if (window != null) ((EditWindow)window).drawBlocks();
        }
        else if (type == BlockManager.INSERT_LINE)
        {
            // this must be insert line mode, so call insert line
            currentMode.onCall(data);
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
            ((EditWindow)WindowManager.getWindowWithComponent<EditWindow>()).setTitleTextMessage("*");
    }
}
