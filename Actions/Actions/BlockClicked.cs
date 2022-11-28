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
        else if (type.Equals(BlockManager.NAMESPACE))
        {
            // keyboard or special list
            // namespaces clutter block list
        }
        else if (type.Equals(BlockManager.INSERT_LINE))
        {
            currentMode.onCall(data);
        }



        // apply current action if not special type
        else if (modeSymbol == ActionManager.PLACE_SELECT || modeSymbol == ActionManager.DELETE_SELECT)
        {
            currentMode.onCall(data);
        }
        else codeModified = false; // if dropped out, no changes



        if (codeModified) editWindow.setTitleTextMessage("*");
    }
}
