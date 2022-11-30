using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditWindow : Window2D
{
    private Block masterBlock;

    [SerializeField] private EditButtonManager editButtonManager;

    private List<Variable> variables = new List<Variable>();
    public List<Variable> getVariables() { return variables; }
    public Variable getVariable(string name)
    {
        foreach (Variable v in variables)
            if (v.name.Equals(name)) return v;
        return null;
    }
    public void addVariable(string name, BlockManager.BlockVariant bV)
    {
        Variable newVariable = new Variable(name, bV);
        variables.Add(newVariable);

        editButtonManager.distributeButtons();
    }



    public void drawBlocks()
    {
        masterBlock.drawBlock();

        // TODO: don't allow blocks to spill
    }

    public void drawButtons()
    {
        editButtonManager.distributeButtons();
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
