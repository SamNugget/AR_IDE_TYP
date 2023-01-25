[System.Serializable]
public class MethodS
{
    public Block methodDeclaration;
    public Block methodBodyMaster;

    public MethodS(Block methodDeclaration, Block methodBodyMaster)
    {
        this.methodDeclaration = methodDeclaration;
        this.methodBodyMaster = methodBodyMaster;
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
