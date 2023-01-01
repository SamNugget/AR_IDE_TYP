using System.Collections.Generic;

public class ReferenceTypeS : IConvertToCode
{
    // class, interface, delegate or record
    // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/reference-types

    // only class and interface in this implementation
    // NOTE: I have chosen to have interface and class together as one
    // so the user can easily switch between the two, and there is no
    // loss of fields should they revert back from interface to class
    public bool isClass;
    public string name;
    public List<ReferenceTypeS> implemented;
    public List<FieldS> fields;
    public List<MethodS> methods;

    public string getCode()
    {
        // first line
        string code = "public " + (isClass ? "class " : "interface ") + name;
        if (implemented.Count > 0)
        {
            code += " : ";
            foreach (IConvertToCode b in implemented)
                code += b.getCode() + ", ";
            code = code.Substring(0, code.Length - 2);
        }
        code += "\n{\n";

        // fields (only if class)
        if (isClass)
        {
            foreach (IConvertToCode f in fields)
                code += f.getCode() + '\n';
            code += '\n';
        }

        // methods
        foreach (IConvertToCode m in methods)
            code += m.getCode() + "\n\n";

        return code;
    }

    public void cycleReferenceType()
    {
        isClass = !isClass;
    }
}
