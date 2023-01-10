using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileManagement;

public class WorkspacesButtonManager : ButtonManager3D
{
    public override void createButtons()
    {
        string[] workspaces = FileManager.workspaceNames;

        foreach (string workspace in workspaces)
            spawnButton(workspace, ActionManager.OPEN_WORKSPACE, workspace);

        spawnButton("CREATE NEW", '\0', null, "IconAdd");
    }
}
