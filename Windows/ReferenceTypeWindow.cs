using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileManagement;
using ActionManagement;

public class ReferenceTypeWindow : Window3D
{
    private Block masterBlock;

    private ReferenceTypeS _referenceTypeSave;
    public ReferenceTypeS referenceTypeSave
    {
        set
        {
            if (_referenceTypeSave == null)
            {
                _referenceTypeSave = value;

                masterBlock = Instantiate(BlockManager.blockFab, transform.GetChild(0)).transform.GetComponent<Block>();
                masterBlock.transform.localScale = new Vector3(WindowManager.blockScale, WindowManager.blockScale, WindowManager.blockScale);
                masterBlock.initialise(BlockManager.getBlockVariantIndex("Window Block"));

                BlockManager.spawnBlock(BlockManager.getBlockVariantIndex("Place Field"), masterBlock.getSubBlock(0));
                BlockManager.spawnBlock(BlockManager.getBlockVariantIndex("Place Method"), masterBlock.getSubBlock(1));

                masterBlock.setColliderEnabled(true, ActionManager.blocksEnabledDefault);
            }
        }
    }
}
