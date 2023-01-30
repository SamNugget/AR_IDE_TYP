using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionManagement;

public class MethodWindow : EditWindow
{
    // public Block masterBlock; FROM EditWindow
    private MethodS _methodSave;

    public MethodS methodSave
    {
        get { return _methodSave; } // temp
        set
        {
            if (_methodSave == null)
            {
                _methodSave = value;

                setName(_methodSave.methodDeclaration.getSubBlock(3).getBlockText(false));

                initialiseBlocks();
            }
        }
    }

    private void initialiseBlocks()
    {
        // create the master block
        masterBlock = Instantiate(BlockManager.blockFab, transform).transform.GetComponent<Block>();
        float s = WindowManager.blockScale;
        masterBlock.transform.localScale = new Vector3(s, s, s);

        // try find a block save, otherwise empty block
        if (_methodSave.methodBodyMasterS != null)
            masterBlock.initialise(_methodSave.methodBodyMasterS);
        else if (_methodSave.bodySaveBuffer != null)
            masterBlock.initialise(_methodSave.bodySaveBuffer);
        else masterBlock.initialise(BlockManager.getBlockVariantIndex("Method Block"));

        masterBlock.drawBlock(false); // TODO: true so scale window
        masterBlock.setColliderEnabled(true, ActionManager.blocksEnabledDefault);
    }

    public override void close()
    {
        _methodSave.saveBodyToBuffer();
        base.close();
    }
}
