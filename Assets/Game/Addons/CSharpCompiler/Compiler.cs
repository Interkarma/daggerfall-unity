// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
                if (!assembly.IsDynamic)
                {
                    // Precompiled assemblies loaded from bytes aren't dynamic but don't have a location
                    // Prevents "assemblyString cannot have zero length"
                    if (!string.IsNullOrWhiteSpace(assembly.Location))
                        compilerparams.ReferencedAssemblies.Add(assembly.Location);
                }
                else
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
                    string errorCodeText = !string.IsNullOrEmpty(error.ErrorNumber) ? $"CS{error.ErrorNumber}" : string.Empty;
                    string errorText = error.ErrorText;
                    string numLineText = $"line#{error.Line}";
                    string numColumnText = $"column#{error.Column}";
                    string lineContentText = string.Empty;

                    if (int.TryParse(error.FileName, out int fileIndex) && GetSpecificLine(sources[fileIndex], error.Line, out lineContentText))
                        lineContentText = $"\"{lineContentText}\"";

                    msg.AppendLine($"<b>Compilation Error {errorCodeText}</b>: {errorText}");
                    msg.AppendLine($"\tat {numLineText} {numColumnText} {lineContentText}");
                }

                throw new Exception(msg.ToString());
            }

            // Return the assembly
            return result.CompiledAssembly;
        }

        static bool GetSpecificLine(string text, int lineNumber, out string line)
        {
            bool success;
            var reader = new System.IO.StringReader(text);
            {
                int i = 0;
                do line = reader.ReadLine();
                while (line != null && ++i < lineNumber);
                success = i == lineNumber;
            }
            reader.Close();
            return success;
        }

    }
}
