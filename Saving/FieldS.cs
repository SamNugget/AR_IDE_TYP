public class FieldS : VariableS
{
    public string accessModifier;

    public override string getCode()
    {
        return accessModifier + " " + base.getCode() + ";";
    }
}
