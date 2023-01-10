using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileManagement;

public class FilesButtonManager : ButtonManager3D
{
    public override void createButtons()
    {
        string[] files = FileManager.sourceFileNames;

        foreach (string file in files)
            spawnButton(file, ActionManager.OPEN_FILE, file, "IconClass");

        spawnButton("CREATE NEW", '\0', null, "IconAdd");
    }
}
