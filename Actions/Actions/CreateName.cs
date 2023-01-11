using UnityEngine;

public class CreateName : Mode
{
    private TextEntryWindow textEntryWindow;
    private Block beingNamed;
    private Block beingReplaced;

    public CreateName(char c) : base(c, /*multi-select:*/false) { }

    public override void onCall(object data)
    {
        string parentType = beingNamed.getBlockVariant().getBlockType();
        bool isVariable = (parentType == BlockManager.FIELD || parentType == BlockManager.VARIABLE_DECLARATION);

        BlockManager.BlockVariant bV = BlockManager.createNameBlock((string)data, isVariable);

        // TODO: fix this
        //if (isVariable)
            //((EditWindow)WindowManager.getWindowWithComponent<EditWindow>()).addVariable((string)data, bV);

        BlockManager.spawnBlock(BlockManager.getBlockVariantIndex(bV), beingReplaced, false);
        beingReplaced = null;

        ActionManager.clearMode(); // this calls onDeselect
    }

    public override void onSelect(object data)
    {
        if (textEntryWindow == null)
        {
            beingNamed = ((Block[])data)[0]; // construct, method or variable block
            beingReplaced = ((Block[])data)[1]; // the empty block

            textEntryWindow = (TextEntryWindow)WindowManager.spawnTextInputWindow();
        }
    }

    public override void onDeselect()
    {
        if (textEntryWindow != null)
        {
            // if onCall() has not been successful, delete the parent of empty
            if (beingReplaced != null)
                BlockManager.spawnBlock(0, beingNamed, false);

            WindowManager.destroyWindow(textEntryWindow);
        }
    }

    public override string getToolsWindowMessage()
    {
        return ("Naming...");
    }
}
