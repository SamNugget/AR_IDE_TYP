using System;

[System.Serializable]
public class MethodS
{
    [NonSerialized] public Block methodDeclaration;
    public BlockSave methodDeclarationS;
    [NonSerialized] public Block methodBodyMaster;
    public BlockSave methodBodyMasterS;

    public MethodS(Block methodDeclaration, Block methodBodyMaster)
    {
        this.methodDeclaration = methodDeclaration;
        this.methodBodyMaster = methodBodyMaster;
    }

    public void save()
    {
        methodDeclarationS = methodDeclaration.saveBlock();
        if (methodBodyMaster != null)
            methodBodyMasterS = methodBodyMaster.saveBlock();
    }

    public string getCode(bool body)
    {
        if (methodDeclaration == null)
            return null;

        string result = methodDeclaration.getBlockText(true);

        if (body)
        {
            if (methodBodyMaster == null)
                return null;
            result += "\n{\n" + methodBodyMaster.getBlockText(true) + "\n}\n";
        }
        else
        {
            result += ';';
        }

        return result;
    }
}
