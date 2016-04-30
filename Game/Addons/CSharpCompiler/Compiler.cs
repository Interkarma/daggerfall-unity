// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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

        /// <summary>
        /// Helper for compiling single files.  Preferable to Compile everything at once whenever possible.
        /// Avoid compiling same files repeatedly as it will lead to memory leaks.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Assembly CompileFiles(string source, bool GenerateInMemory = true)
        {
            return CompileFiles(new string[] { source }, GenerateInMemory);
        }


        /// <summary>
        /// Compiles array of files by file paths
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static Assembly CompileFiles(string[] sources, bool GenerateInMemory = true)
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

            //if (string.IsNullOrEmpty(assemblyName)) //uses /out?
            //    compilerparams.CompilerOptions = assemblyName;

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
            var result = CodeCompiler.CompileAssemblyFromFileBatch(compilerparams, sources);

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
