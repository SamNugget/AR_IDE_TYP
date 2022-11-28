using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using UnityEngine;

// https://www.arcturuscollective.com/archive/2015/12/06/compile-cs-in-unity3d.html
public class CompilerExample : MonoBehaviour
{
    void Start()
    {
        var assembly = Compile(@"
using UnityEngine;

public class Test
{
    public static void Foo()
    {
        Debug.Log(""Hello, World!"");
        GameObject.CreatePrimitive(PrimitiveType.Cube);
    }

    public static void Foo2()
    {
        Debug.Log(""Hello, World2!"");
    }

    public static void Foo3()
    {
        Debug.Log(""Hello, World3!"");
    }
}"
);

        var method = assembly.GetType("Test").GetMethod("Foo");
        var del = (System.Action)Delegate.CreateDelegate(typeof(System.Action), method);
        del.Invoke();
    }

    public static Assembly Compile(string source)
    {
        var provider = new CSharpCodeProvider();
        var param = new CompilerParameters();

        // Add ALL of the assembly references
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            string assemblyLocation = assembly.Location;
            if (!assembly.Location.Contains("PlasticSCM"))
                param.ReferencedAssemblies.Add(assemblyLocation);
            //Debug.Log(assembly.Location);
        }

        // Add specific assembly references
        //param.ReferencedAssemblies.Add("System.dll");
        //param.ReferencedAssemblies.Add("CSharp.dll");
        //param.ReferencedAssemblies.Add("UnityEngines.dll");

        // Generate a dll in memory
        param.GenerateExecutable = false;
        param.GenerateInMemory = true;

        // Compile the source
        var result = provider.CompileAssemblyFromSource(param, source);

        if (result.Errors.Count > 0)
        {
            var msg = new StringBuilder();
            foreach (CompilerError error in result.Errors)
            {
                msg.AppendFormat("Error ({0}): {1}\n",
                error.ErrorNumber, error.ErrorText);
            }
            throw new Exception(msg.ToString());
        }

        // Return the assembly
        return result.CompiledAssembly;
    }
}