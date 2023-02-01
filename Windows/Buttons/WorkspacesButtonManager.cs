using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileManagement;
using ActionManagement;

public class WorkspacesButtonManager : ButtonManager3D
{
    public override void createButtons()
    {
        string[] workspaces = FileManager.workspaceNames;

        foreach (string workspace in workspaces)
            spawnButton(workspace, ActionManager.OPEN_WORKSPACE, workspace);

        spawnButton("CREATE NEW", ActionManager.CREATE_WORKSPACE, null, "IconAdd");
    }
}
