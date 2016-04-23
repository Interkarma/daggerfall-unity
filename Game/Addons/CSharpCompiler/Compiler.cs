using System;
using System.Reflection;
using System.Linq;
using System.Text;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CSharp;



public class Compiler
{
    public static Dictionary<string, Assembly> AssemblyResolver = new Dictionary<string, Assembly>();


    public static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        if (assembly == null) throw new ArgumentNullException("assembly");
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t != null);
        }
    }

    //public static bool LoadSource(string FilePath, out string source)
    //{
    //    if(File.Exists(FilePath))
    //    {
    //        source = File.ReadAllText(FilePath);
    //        return true;
    //    }
    //    else
    //    {
    //        source = null;
    //        return false;
    //    }

    //}

    //public static Assembly CompileSingle(string source)
    //{
    //    var provider = new CSharpCodeProvider();
    //    var param = new CompilerParameters();

    //    // Add ALL of the assembly references
    //    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
    //    {
    //        try
    //        {
    //            param.ReferencedAssemblies.Add(assembly.Location);
    //        }
    //        catch
    //        {
    //            if (assembly.FullName.Contains("dynamic"))
    //                param.ReferencedAssemblies.Add(assembly.GetName().Name);
    //        }
    //    }

    //    // Generate a dll in memory
    //    param.GenerateExecutable = false;
    //    param.GenerateInMemory = true;

    //    // Compile the source
    //    var result = provider.CompileAssemblyFromSource(param, source);

    //    if (result.Errors.Count > 0)
    //    {
    //        var msg = new StringBuilder();
    //        foreach (CompilerError error in result.Errors)
    //        {
    //            msg.AppendFormat("Error ({0}): {1}\n",
    //                error.ErrorNumber, error.ErrorText);
    //        }
    //        throw new Exception(msg.ToString());
    //    }

    //    // Return the assembly
    //    return result.CompiledAssembly;
    //}

    public static Assembly CompileFiles(string[] sources)
    {
        var options = new CompilerParameters();
        var CodeCompiler = new CSharpCompiler.CodeCompiler();
        var codeProvider = new CSharpCodeProvider();

        //add all references to assembly - need to use Assembly resolver for Dynamicly created
        //assemblies, as assembly.Location will fail for them
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                options.ReferencedAssemblies.Add(assembly.Location);
            }
            catch
            {
                if (AssemblyResolver.ContainsKey(assembly.FullName))
                    options.ReferencedAssemblies.Add(assembly.GetName().FullName);
            }
        }

        options.GenerateExecutable = false;
        options.GenerateInMemory = true;

        AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
        {
            if (AssemblyResolver.ContainsKey(e.Name))
            {
                //UnityEngine.Debug.Log("resolved assembly for:" + e.Name);
                return AssemblyResolver[e.Name];
            }
            else
                return null;
        };

        // Compile the source
        var result = CodeCompiler.CompileAssemblyFromFileBatch(options, sources);

        if(result.CompiledAssembly != null)
        {
            if(!AssemblyResolver.ContainsKey(result.CompiledAssembly.FullName))
                AssemblyResolver.Add(result.CompiledAssembly.FullName, result.CompiledAssembly);
        }

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
