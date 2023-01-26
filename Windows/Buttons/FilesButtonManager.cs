using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileManagement;
using ActionManagement;

public class FilesButtonManager : ButtonManager3D
{
    public override void createButtons()
    {
        string[] files = FileManager.sourceFileNames;

        foreach (string file in files)
            spawnButton(file, ActionManager.OPEN_FILE, file, "IconClass");

        spawnButton("CREATE NEW", ActionManager.CREATE_FILE, null, "IconAdd");
    }
}
