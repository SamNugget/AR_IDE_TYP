using System;
using System.Collections.Generic;

[Serializable]
public class MethodS : IConvertToCode
{
    public string accessModifier;
    public string returnType;
    public string name;
    public VariableS[] parameters;

    public MethodS(string name)
    {
        accessModifier = "public";
        returnType = "void";
        this.name = name;
        parameters = new VariableS[0];
    }

    public void addParameter(VariableS toAdd)
    {
        parameters = ReferenceTypeS.appendToArray<VariableS>(parameters, toAdd);
    }

    public void removeParameter(VariableS toRemove)
    {
        int i = Array.IndexOf(parameters, toRemove);
        parameters = ReferenceTypeS.removeFromArray<VariableS>(parameters, i);
    }

    public virtual string getCode()
    {
        string code = accessModifier + " " + returnType + " " + name + "(";
        foreach (IConvertToCode v in parameters)
            code += v.getCode() + ", ";

        if (parameters.Length > 0) // if there's ", " on the end, remove it
            code = code.Substring(0, code.Length - 2);
        
        code += ");";

        return code;
    }
}
