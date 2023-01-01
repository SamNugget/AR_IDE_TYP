using System.Collections.Generic;

public class MethodS : IConvertToCode
{
    public string accessModifier;
    public string returnType;
    public string name;
    public List<VariableS> parameters;

    public virtual string getCode()
    {
        string code = accessModifier + " " + returnType + " " + name + "(";
        foreach (IConvertToCode v in parameters)
            code += v.getCode() + ", ";

        if (parameters.Count > 0) // if there's ", " on the end, remove it
            code = code.Substring(0, code.Length - 2);
        
        code += ");";

        return code;
    }
}
