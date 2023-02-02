using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionManagement;

public class ClassWindow : FileWindow
{
    protected override void initialiseBlocks()
    {
        // create the master block
        masterBlock = Instantiate(BlockManager.blockFab, contentParent).transform.GetComponent<Block>();
        float s = WindowManager.blockScale;
        masterBlock.transform.localScale = new Vector3(s, s, s);
        masterBlock.initialise(BlockManager.getBlockVariantIndex("Window Block"));


        // load the fields. TODO: make this more elegant
        Block topBlock = masterBlock.getSubBlock(0);
        if (_referenceTypeSave.fields.Count == 0)
            BlockManager.spawnBlock(BlockManager.getBlockVariantIndex("Place Field"), topBlock);
        else
        {
            // spawn first block
            topBlock = BlockManager.spawnBlock(0, topBlock, true, _referenceTypeSave.fields[0].fieldBlockS);
            // save reference to spawned block
            _referenceTypeSave.fields[0].fieldBlock = topBlock;

            for (int i = 1; i < _referenceTypeSave.fields.Count; i++)
            {
                // split orig block
                Block splitter = BlockManager.splitBlock(topBlock);
                // spawn new block
                topBlock = BlockManager.spawnBlock(0, topBlock, true, _referenceTypeSave.fields[i].fieldBlockS);
                // save reference to spawned block
                _referenceTypeSave.fields[i].fieldBlock = topBlock;
                // put new block in splitter
                splitter.replaceSubBlock(topBlock, 1);
            }
        }

        // load the method declarations
        Block botBlock = masterBlock.getSubBlock(1);
        if (_referenceTypeSave.methods.Count == 0)
            BlockManager.spawnBlock(BlockManager.getBlockVariantIndex("Place Method"), botBlock);
        else
        {
            botBlock = BlockManager.spawnBlock(0, botBlock, true, _referenceTypeSave.methods[0].methodDeclarationS);
            _referenceTypeSave.methods[0].methodDeclaration = botBlock;

            for (int i = 1; i < _referenceTypeSave.methods.Count; i++)
            {
                Block splitter = BlockManager.splitBlock(botBlock);
                botBlock = BlockManager.spawnBlock(0, botBlock, true, _referenceTypeSave.methods[i].methodDeclarationS);
                _referenceTypeSave.methods[i].methodDeclaration = botBlock;
                splitter.replaceSubBlock(botBlock, 1);
            }
        }

        masterBlock.setColliderEnabled(true, ActionManager.blocksEnabledDefault);
    }
}
