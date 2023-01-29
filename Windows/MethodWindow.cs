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
        set
        {
            if (_methodSave == null)
            {
                _methodSave = value;

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
        else masterBlock.initialise(0);

        masterBlock.drawBlock(true);
        masterBlock.setColliderEnabled(true, ActionManager.blocksEnabledDefault);
    }

    public override void close()
    {
        _methodSave.saveBodyToBuffer();
        base.close();
    }
}
