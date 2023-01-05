using UnityEngine;

public class CreateName : Mode
{
    private TextEntryWindow textEntryWindow;
    private Block[] beingNamed;
    private bool named = false;

    public CreateName(char c) : base(c, /*multi-select:*/false) { }

    public override void onCall(object data)
    {
        string parentType = beingNamed[0].getBlockVariant().getBlockType();
        bool isVariable = (parentType == BlockManager.FIELD || parentType == BlockManager.VARIABLE_DECLARATION);
        BlockManager.BlockVariant bV = BlockManager.createNameBlock((string)data, isVariable);
        if (isVariable)
            ((EditWindow)WindowManager.getWindowWithComponent<EditWindow>()).addVariable((string)data, bV);

        BlockManager.spawnBlock(BlockManager.getBlockVariantIndex(bV), beingNamed[1], false);

        named = true;
        ActionManager.clearMode(); // this calls onDeselect
    }

    public override void onSelect(object data)
    {
        if (textEntryWindow == null)
        {
            beingNamed = (Block[])data;
            // beingNamed[0] = class, method or variable block, beingNamed[1] = the empty block



            Window3D spawned = WindowManager.spawnTextInputWindow();
            EditWindow editWindow = (EditWindow)WindowManager.getWindowWithComponent<EditWindow>();
            spawned.transform.position = editWindow.transform.position - editWindow.transform.forward;

            textEntryWindow = (TextEntryWindow)spawned;
            textEntryWindow.setAction(getSymbol());
        }
    }

    public override void onDeselect()
    {
        if (textEntryWindow != null)
        {
            if (named) named = false;
            else
            {
                // delete the block being named
                BlockManager.spawnBlock(0, beingNamed[0], false);
            }
            WindowManager.destroyWindow(textEntryWindow);
        }
    }

    public override string getToolsWindowMessage()
    {
        return ("Naming...");
    }
}
