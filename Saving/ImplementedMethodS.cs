using System;

[Serializable]
public class ImplementedMethodS : MethodS
{
    [NonSerialized] public Block masterBlock;
    public BlockSave blockSave;

    public ImplementedMethodS(string name, BlockSave blockSave) : base(name)
    {

    }

    public override string getCode()
    {
        string code = base.getCode();
        code = code.Substring(0, code.Length - 1) + "{\n"; // replace the ';'

        code += masterBlock.getBlockText(true) + "\n}"; // add the method body

        return code;
    }
}
