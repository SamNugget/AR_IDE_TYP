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
        string[] types = FileManager.sourceFileTypes;

        for (int i = 0; i < files.Length; i++)
            spawnButton(files[i], ActionManager.OPEN_FILE, files[i], "Icon" + types[i]);

        spawnButton("CREATE NEW", ActionManager.CREATE_FILE, null, "IconAdd");
    }
}
