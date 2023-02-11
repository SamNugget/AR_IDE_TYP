using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileManagement;
using TMPro;

public abstract class FileWindow : EditWindow
{
    protected ReferenceTypeS _referenceTypeSave;

    public ReferenceTypeS referenceTypeSave
    {
        get { return _referenceTypeSave; }
        set
        {
            if (_referenceTypeSave == null)
            {
                _referenceTypeSave = value;

                setName((_referenceTypeSave.isClass ? "<u>C</u> " : "<u>I</u> ") + _referenceTypeSave.name);

                initialiseBlocks();
            }
        }
    }

    protected abstract void initialiseBlocks();





    public void saveFile()
    {
        if (FileManager.saveSourceFile(_referenceTypeSave.name))
        {
            setTitleTextMessage("", false);
            Window w = WindowManager.getWindowWithComponent<ToolsWindow>();
            w.setTitleTextMessage("Saved " + _referenceTypeSave.name, true);
        }
    }





    protected override void setSimpleText()
    {
        simpleText.GetComponent<TextMeshPro>().text = (_referenceTypeSave.isClass ? "<u>Class</u>\n" : "<u>Interface</u>\n") + _referenceTypeSave.name;
    }

    public override void setSimpleView(bool simple)
    {
        base.setSimpleView(simple);

        if (!simple) BlockManager.lastMaster = masterBlock;
    }
}
