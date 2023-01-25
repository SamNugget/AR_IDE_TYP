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
        masterBlock = Instantiate(BlockManager.blockFab, transform.GetChild(0)).transform.GetComponent<Block>();
        masterBlock.transform.localScale = new Vector3(WindowManager.blockScale, WindowManager.blockScale, WindowManager.blockScale);
        masterBlock.initialise(0);

        masterBlock.setColliderEnabled(true, ActionManager.blocksEnabledDefault);
    }
}
