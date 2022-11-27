using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditWindow : Window2D
{
    private Block masterBlock;

    //private List<Variable> variables;



    public void drawBlocks()
    {
        masterBlock.drawBlock();

        // TODO: don't allow blocks to spill
    }

    public void setCollidersEnabled(bool enabled, int variantToMask = -1)
    {
        masterBlock.setColliderEnabled(enabled, variantToMask);
    }

    public void setSpecialChildBlocks(int variantIndex, bool enabled)
    {
        masterBlock.setSpecialChildBlock(variantIndex, enabled);
    }

    public void saveCode()
    {
        string code = masterBlock.getBlockText(true);
        SaveSystem.saveCode(name, code);
    }



    void Start()
    {
        masterBlock = Instantiate(BlockManager.blockFab, transform).transform.GetComponent<Block>();
        masterBlock.initialise(BlockManager.getBlockVariantIndex("Window Block"));
        masterBlock.drawBlock();

        base.Start();
    }
}
