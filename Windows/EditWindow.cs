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

    public string getCode()
    {
        // TODO: update so recursive
        return masterBlock.getText();
    }



    void Start()
    {
        masterBlock = Instantiate(BlockManager.blockFab, transform).transform.GetComponent<Block>();
        masterBlock.initialise(1);
        masterBlock.drawBlock();

        resizeWindow();
    }

    void Update()
    {
        // TODO: rm this
        resizeWindow();
    }
}
