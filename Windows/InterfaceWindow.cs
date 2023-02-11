using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionManagement;

public class InterfaceWindow : FileWindow
{
    protected override void initialiseBlocks()
    {
        // create the master block
        masterBlock = Instantiate(BlockManager.blockFab, contentParent).transform.GetComponent<Block>();
        float s = WindowManager.blockScale;
        masterBlock.transform.localScale = new Vector3(s, s, s);
        masterBlock.initialise(BlockManager.getBlockVariantIndex("Method Block"));

        // load the method declarations
        Block methodBlock = masterBlock.getSubBlock(0);
        if (_referenceTypeSave.methods.Count == 0)
            BlockManager.spawnBlock(BlockManager.getBlockVariantIndex("Place Method"), methodBlock);
        else
        {
            methodBlock = BlockManager.spawnBlock(0, methodBlock, true, _referenceTypeSave.methods[0].methodDeclarationS);
            _referenceTypeSave.methods[0].methodDeclaration = methodBlock;

            for (int i = 1; i < _referenceTypeSave.methods.Count; i++)
            {
                Block splitter = BlockManager.splitBlock(methodBlock);
                methodBlock = BlockManager.spawnBlock(0, methodBlock, true, _referenceTypeSave.methods[i].methodDeclarationS);
                _referenceTypeSave.methods[i].methodDeclaration = methodBlock;
                splitter.replaceSubBlock(methodBlock, 1);
            }
        }

        masterBlock.setColliderEnabled(true, ActionManager.blocksEnabledDefault);
    }
}
