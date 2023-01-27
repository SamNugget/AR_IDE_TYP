using System.Collections.Generic;

[System.Serializable]
public class ReferenceTypeS
{
    // class, interface, delegate or record
    // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/reference-types

    // only class and interface in this implementation
    // NOTE: I have chosen to have interface and class together as one
    // so the user can easily switch between the two, and there is no
    // loss of fields should they revert back from interface to class
    public string path;

    public string name;
    public bool isClass;

    public List<FieldS> fields;
    public List<MethodS> methods;

    public ReferenceTypeS(string path, string name)
    {
        this.path = path;
        this.name = name;

        isClass = true;
        fields = new List<FieldS>();
        methods = new List<MethodS>();
    }

    public void save()
    {
        foreach (FieldS f in fields)
            f.save();
        foreach (MethodS m in methods)
            m.save();
    }

    public string getCode()
    {
        // first line
        string code = "public " + (isClass ? "class " : "interface ") + name;
        /*if (implemented.Length > 0)
        {
            code += " : ";
            foreach (string i in implemented)
                code += i + ", ";
            code = code.Substring(0, code.Length - 2);
        } TODO: linking classes/interfaces by drag and drop*/
        code += "\n{\n\n";

        if (isClass)
        {
            // fields
            foreach (FieldS f in fields)
                code += f.getCode() + '\n';
            code += '\n';

            // methods
            foreach (MethodS m in methods)
                code += m.getCode(false) + "\n\n";
        }
        else // interface
        {
            // methods only
            foreach (MethodS m in methods)
                code += m.getCode(false) + '\n';
            code += '\n';
        }

        code += '}';
        return code;
    }





    public void addField(Block fieldBlock)
    {
        fields.Add(new FieldS(fieldBlock));
    }

    public void removeField(Block fieldBlock)
    {
        foreach (FieldS f in fields)
        {
            if (f.fieldBlock == fieldBlock)
            {
                fields.Remove(f);
                return;
            }
        }
    }

    public void addMethod(Block methodDeclaration, Block methodBodyMaster)
    {
        methods.Add(new MethodS(methodDeclaration, methodBodyMaster));
    }

    public void removeMethod(Block methodDeclaration)
    {
        foreach (MethodS m in methods)
        {
            if (m.methodDeclaration == methodDeclaration)
            {
                methods.Remove(m);
                return;
            }
        }
    }





    public bool cycleReferenceType()
    {
        isClass = !isClass;
        return isClass;
    }
}
