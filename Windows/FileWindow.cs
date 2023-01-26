using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionManagement;
using FileManagement;

public class FileWindow : EditWindow
{
    // public Block masterBlock; FROM EditWindow
    private ReferenceTypeS _referenceTypeSave;

    public ReferenceTypeS referenceTypeSave
    {
        get { return _referenceTypeSave; } // temp
        set
        {
            if (_referenceTypeSave == null)
            {
                _referenceTypeSave = value;

                setName(_referenceTypeSave.name);
                cIButton.setButtonIcon(_referenceTypeSave.isClass);

                initialiseBlocks();
            }
        }
    }

    private void initialiseBlocks()
    {
        masterBlock = Instantiate(BlockManager.blockFab, transform.GetChild(0)).transform.GetComponent<Block>();
        masterBlock.transform.localScale = new Vector3(WindowManager.blockScale, WindowManager.blockScale, WindowManager.blockScale);
        masterBlock.initialise(BlockManager.getBlockVariantIndex("Window Block"));

        BlockManager.spawnBlock(BlockManager.getBlockVariantIndex("Place Field"), masterBlock.getSubBlock(0));
        BlockManager.spawnBlock(BlockManager.getBlockVariantIndex("Place Method"), masterBlock.getSubBlock(1));

        masterBlock.setColliderEnabled(true, ActionManager.blocksEnabledDefault);
    }

    [SerializeField] private CIButton cIButton;
    public void cycleReferenceType()
    {
        bool isClass = _referenceTypeSave.cycleReferenceType();
        cIButton.setButtonIcon(_referenceTypeSave.isClass);

        // TODO: hide and unhide blocks/buttons as necessary
    }

    public void saveFile()
    {
        if (FileManager.saveSourceFile(name))
            setTitleTextMessage("", false);
    }
}
