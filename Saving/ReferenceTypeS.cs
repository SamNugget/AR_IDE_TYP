using System;
using System.Collections.Generic;

public class ReferenceTypeS : IConvertToCode
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
    public string[] implemented;
    public FieldS[] fields;
    public MethodS[] methods;

    public ReferenceTypeS(string path, string name)
    {
        this.path = path;
        this.name = name;
        isClass = true;
        implemented = new string[0];
        fields = new FieldS[0];
        methods = new MethodS[0];
    }

    public static T[] appendToArray<T>(T[] arr, T toAdd)
    {
        Array.Resize<T>(ref arr, arr.Length + 1);
        arr[arr.Length - 1] = toAdd;
        return arr;
    }

    public static T[] removeFromArray<T>(T[] arr, int i)
    {
        for (; i < arr.Length - 1; i++) arr[i] = arr[i + 1]; // shift left
        Array.Resize<T>(ref arr, arr.Length - 1);
        return arr;
    }

    public void addImplemented(string toAdd)
    {
        implemented = appendToArray<string>(implemented, toAdd);
    }

    public void removeImplemented(string toRemove)
    {
        int i = Array.FindIndex<string>(implemented, x => x == toRemove);
        implemented = removeFromArray<string>(implemented, i);
    }

    public void addField(FieldS toAdd)
    {
        fields = appendToArray<FieldS>(fields, toAdd);
    }

    public void removeField(FieldS toRemove)
    {
        int i = Array.IndexOf(fields, toRemove);
        fields = removeFromArray<FieldS>(fields, i);
    }

    public void addMethod(MethodS toAdd)
    {
        methods = appendToArray<MethodS>(methods, toAdd);
    }

    public void removeMethod(MethodS toRemove)
    {
        int i = Array.IndexOf(methods, toRemove);
        methods = removeFromArray<MethodS>(methods, i);
    }

    public string getCode()
    {
        // first line
        string code = "public " + (isClass ? "class " : "interface ") + name;
        if (implemented.Length > 0)
        {
            code += " : ";
            foreach (string i in implemented)
                code += i + ", ";
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
