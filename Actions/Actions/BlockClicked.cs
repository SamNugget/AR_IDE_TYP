public class BlockClicked : Act
{
    public BlockClicked(char c) : base(c) { }

    public override void onCall(object data)
    {
        EditWindow editWindow = ActionManager.EditWindow;
        Mode currentMode = ActionManager.getCurrentMode();
        char modeSymbol = currentMode.getSymbol();

        // handle actions
        Block clicked = (Block)data;
        BlockManager.BlockVariant variant = clicked.getBlockVariant();
        string type = variant.getBlockType();
        
        //Debug.Log("Clicked " + type);





        bool codeModified = true;
        // check for special types first
        if (type.Equals(BlockManager.ACCESS_MODIFIER))
        {
            // TODO: tidy this up
            BlockManager.BlockVariant newVariant;
            if (variant.getName().Equals("Public"))
                newVariant = BlockManager.getBlockVariant("Private");
            else
                newVariant = BlockManager.getBlockVariant("Public");

            int nVIndex = BlockManager.getBlockVariantIndex(newVariant);
            BlockManager.spawnBlock(nVIndex, clicked, false);

            Window2D window = clicked.getWindow2D();
            if (window != null) ((EditWindow)window).drawBlocks();
        }
        else if (type.Equals(BlockManager.NAME))
        {
            int variantIndex = BlockManager.getBlockVariantIndex(variant);
            ActionManager.callAction(ActionManager.PLACE_SELECT, variantIndex);
        }



        // apply current action if not special type
        else if (type.Equals(BlockManager.INSERT_LINE))
        {
            // this must be insert line mode, so call insert line
            currentMode.onCall(data);
        }
        else if (modeSymbol == ActionManager.PLACE_SELECT || modeSymbol == ActionManager.DELETE_SELECT)
        {
            // will delete or place
            currentMode.onCall(data);
        }
        else codeModified = false; // if dropped out, no changes



        if (codeModified) editWindow.setTitleTextMessage("*");
    }
}
