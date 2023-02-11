using System.Collections.Generic;

[System.Serializable]
public class ReferenceTypeS
{
    // class, interface, delegate or record
    // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/reference-types

    // only class and interface in this implementation
    // saved as the same class for saving/loading simplicity

    // TODO: why am I saving path?
    public string path;

    public string name;
    public bool isClass;

    public bool locked;

    // bool opened
    // float[] position

    public List<FieldS> fields;
    public List<MethodS> methods;

    public ReferenceTypeS(string path, string name, bool isClass)
    {
        this.path = path;
        this.name = name;
        this.isClass = isClass;
        
        this.locked = false;

        methods = new List<MethodS>();
        if (!isClass) return;

        fields = new List<FieldS>();
    }

    public void save()
    {
        foreach (MethodS m in methods)
            m.save();
        if (!isClass) return;

        foreach (FieldS f in fields)
            f.save();
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
                code += f.getCode();
            code += '\n';

            // methods
            foreach (MethodS m in methods)
                code += m.getCode(true) + '\n';
        }
        else // interface
        {
            // methods only
            foreach (MethodS m in methods)
                code += m.getCode(false) + '\n';
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

    public void addMethod(Block methodDeclaration)
    {
        methods.Add(new MethodS(methodDeclaration));
    }

    public void removeMethod(Block methodDeclaration)
    {
        MethodS m = findMethodSave(methodDeclaration);
        methods.Remove(m);
    }

    public MethodS findMethodSave(Block b, bool dec = true)
    {
        foreach (MethodS m in methods)
        {
            if (dec && m.methodDeclaration == b)
                return m;
            else if (!dec && m.methodBodyMaster == b)
                return m;
        }
        return null;
    }





    /*public bool cycleReferenceType()
    {
        isClass = !isClass;
        return isClass;
    }*/
}
