// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Utility
{
    internal static class ModAssemblyBuilder
    {
        /// <summary>
        /// Compiles C# files using AssemblyBuilder API.
        /// </summary>
        /// <param name="assemblyPath">Path to .dll to be created (directory must exist and be writable).</param>
        /// <param name="scriptPaths">Paths to C# files.</param>
        /// <returns>Value indicating if operation was succesful.</returns>
        internal static bool Compile(string assemblyPath, params string[] scriptPaths)
        {
            var assemblyBuilder = new AssemblyBuilder(assemblyPath, scriptPaths)
            {
                referencesOptions = ReferencesOptions.UseEngineModules,
                buildTargetGroup = BuildTargetGroup.Standalone,
                additionalReferences = GetAdditionalReferences()
            };

            assemblyBuilder.buildFinished += AssemblyBuilder_buildFinished;

            try
            {
                if (!assemblyBuilder.Build())
                {
                    Debug.LogError($"Failed to start build of assembly {assemblyPath}.");
                    return false;
                }

                while (assemblyBuilder.status != AssemblyBuilderStatus.Finished)
                    System.Threading.Thread.Sleep(10);
            }
            finally
            {
                assemblyBuilder.buildFinished -= AssemblyBuilder_buildFinished;
            }

            if (!File.Exists(assemblyPath))
            {
                Debug.LogError($"Failed to build {assemblyPath}.");
                return false;
            }

            return true;
        }

        private static string[] GetAdditionalReferences()
        {
            var references = new List<string>();

            foreach (Assembly assembly in CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies))
                references.Add(assembly.outputPath);

            return references.ToArray();
        }

        private static void AssemblyBuilder_buildFinished(string assemblyPath, CompilerMessage[] compilerMessages)
        {
            foreach (CompilerMessage compilerMessage in compilerMessages)
            {
                if (compilerMessage.type == CompilerMessageType.Error)
                {
                    Debug.LogError(compilerMessage.message);
                }
                else
                {
                    // Ignore warning for type already defined in 'Assembly-CSharp' where mod source code is compiled in editor.
                    if (compilerMessage.message.IndexOf("CS0436", StringComparison.Ordinal) != -1)
                        continue;

                    Debug.LogWarning(compilerMessage.message);
                }
            }
        }
    }
}
