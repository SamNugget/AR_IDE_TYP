public class VariableS : IConvertToCode
{
    public string type;
    public string name;

    public virtual string getCode()
    {
        return type + " " + name;
    }
}
