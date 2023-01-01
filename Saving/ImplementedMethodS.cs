public class ImplementedMethodS : MethodS
{
    public Block masterBlock;

    public override string getCode()
    {
        string code = base.getCode();
        code = code.Substring(0, code.Length - 1) + "{\n"; // replace the ';'

        code += masterBlock.getBlockText(true) + "\n}"; // add the method body

        return code;
    }
}
