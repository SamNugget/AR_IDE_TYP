using UnityEngine;

public class CreateName : Mode
{
    private TextEntryWindow textEntryWindow;
    private Block[] beingNamed;
    private bool named = false;

    public CreateName(char c) : base(c, /*multi-select:*/false) { }

    public override void onCall(object data)
    {
        bool isVariable = beingNamed[1].getBlockVariant().getBlockType().Equals(BlockManager.VARIABLE_NAME);
        BlockManager.BlockVariant bV = BlockManager.createNameBlock((string)data, isVariable);
        if (isVariable)
            ActionManager.EditWindow.addVariable((string)data, bV);

        BlockManager.spawnBlock(BlockManager.getBlockVariantIndex(bV), beingNamed[1], false);

        named = true;
        onDeselect();
    }

    public override void onSelect(object data)
    {
        if (textEntryWindow == null)
        {
            beingNamed = (Block[])data;
            // beingNamed[0] = class or method, beingNamed[1] = the name itself



            GameObject spawned = WindowManager.spawnWindow(WindowManager.textEntryFab);
            EditWindow editWindow = ActionManager.EditWindow;
            int editWindowHeight = editWindow.getHeight();
            spawned.transform.position = editWindow.transform.position - new Vector3(0f, FontManager.lineHeight * (editWindowHeight + 2), 0f);

            textEntryWindow = spawned.GetComponent<TextEntryWindow>();
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
            WindowManager.destroyWindow(textEntryWindow.gameObject);
        }
    }

    public override string getToolsWindowMessage()
    {
        return ("Naming...");
    }
}
