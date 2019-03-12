// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Reflection;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;


namespace DaggerfallWorkshop.Game.Utility
{
    public class Compiler
    {
        private static Dictionary<string, Assembly> DynamicAssemblyResolver = new Dictionary<string, Assembly>();
        private static CSharpCompiler.CodeCompiler CodeCompiler;


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


        public static Assembly CompileSource(string[] sources, bool isSource, bool GenerateInMemory = true)
        {
            if (CodeCompiler == null)
                CodeCompiler = new CSharpCompiler.CodeCompiler();
            var compilerparams = new CompilerParameters();

            //add all references to assembly - need to use Assembly resolver for Dynamicly created
            //assemblies, as assembly.Location will fail for them
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    compilerparams.ReferencedAssemblies.Add(assembly.Location);
                }
                catch
                {
                    if (DynamicAssemblyResolver.ContainsKey(assembly.FullName))
                        compilerparams.ReferencedAssemblies.Add(assembly.GetName().FullName);
                }
            }

            compilerparams.GenerateExecutable = false;
            compilerparams.GenerateInMemory = GenerateInMemory;

            AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
            {
                if (DynamicAssemblyResolver.ContainsKey(e.Name))
                {
                    //UnityEngine.Debug.Log("resolved assembly for:" + e.Name);
                    return DynamicAssemblyResolver[e.Name];
                }
                else
                    return null;
            };

            // Compile the source
            CompilerResults result;
            if (isSource)
                result = CodeCompiler.CompileAssemblyFromSourceBatch(compilerparams, sources);
            else
                result = CodeCompiler.CompileAssemblyFromFileBatch(compilerparams, sources);


            if (result.CompiledAssembly != null)
            {
                if (!DynamicAssemblyResolver.ContainsKey(result.CompiledAssembly.FullName))
                    DynamicAssemblyResolver.Add(result.CompiledAssembly.FullName, result.CompiledAssembly);
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
}
