namespace DaggerfallWorkshop.Game.Utility.Roslyn;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

public sealed class Compiler
{
    private static Compiler _compiler;
    public static Compiler Instance => _compiler ??= new();

    private Dictionary<string, Assembly> dynamicAssemblyResolver;
    private Dictionary<string, MetadataReference> referenceCache;
    private CSharpCompilationOptions defaultCompilationOptions;

    private Assembly OnAssemblyResolve(object sender, ResolveEventArgs e) =>
        dynamicAssemblyResolver.TryGetValue(e.Name, out var assembly) ? assembly : null;

    private void OnAssemblyLoad(object sender, AssemblyLoadEventArgs e)
    {
        if (e.LoadedAssembly.IsDynamic || string.IsNullOrWhiteSpace(e.LoadedAssembly.Location)) return;
        dynamicAssemblyResolver.TryAdd(e.LoadedAssembly.GetName().Name, e.LoadedAssembly);
    }

    private Compiler()
    {
        AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;

        defaultCompilationOptions = new(
            outputKind: OutputKind.DynamicallyLinkedLibrary
        );

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        dynamicAssemblyResolver = new(assemblies.Length);
        referenceCache = new(assemblies.Length);
        foreach (var a in assemblies)
        {
            var key = a.GetName().Name;
            if (!dynamicAssemblyResolver.ContainsKey(key) && !string.IsNullOrWhiteSpace(key))
            {
                dynamicAssemblyResolver[key] = a;
            }

            if (!referenceCache.ContainsKey(key) && !string.IsNullOrWhiteSpace(a.Location) && System.IO.File.Exists(a.Location))
            {
                referenceCache[key] = MetadataReference.CreateFromFile(a.Location);
            }
        }
    }

    public bool CompileSource(string assemblyName, string[] sources, [NotNullWhen(true)] out Assembly assembly, bool isDebugBuild = false)
    {
        assembly = null;
        try
        {
            // Forcibly build in debug mode if we are in a standalone development build or the editor.
            isDebugBuild = isDebugBuild || UnityEngine.Debug.isDebugBuild || UnityEngine.Application.isEditor;


            var compilationOptions = defaultCompilationOptions
                .WithOptimizationLevel(isDebugBuild ? OptimizationLevel.Debug : OptimizationLevel.Release)
                // Default unity settings passes -determistic compiler flag
                .WithDeterministic(true);

            var syntaxTrees = sources
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => CSharpSyntaxTree.ParseText(s));

            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees,
                referenceCache.Values,
                compilationOptions
            );

            var emitOptions = new EmitOptions()
                .WithDebugInformationFormat(DebugInformationFormat.PortablePdb);

            using var peStream = new System.IO.MemoryStream();
            using var pdbStream = new System.IO.MemoryStream();
            var result = compilation.Emit(peStream, isDebugBuild ? pdbStream : null, options: emitOptions);

            if (!result.Success)
            {
                foreach (var diagnostic in result.Diagnostics)
                {
                    // TODO: check if the error is sufficiently informative
                    UnityEngine.Debug.LogFormat(UnityEngine.LogType.Error, UnityEngine.LogOption.NoStacktrace, null, "{0}", diagnostic.ToString());
                }
                return false;
            }

            if (isDebugBuild)
            {
                peStream.Seek(0, System.IO.SeekOrigin.Begin);
                pdbStream.Seek(0, System.IO.SeekOrigin.Begin);
                assembly = Assembly.Load(peStream.ToArray(), pdbStream.ToArray());
            }
            else
            {
                peStream.Seek(0, System.IO.SeekOrigin.Begin);
                assembly = Assembly.Load(peStream.ToArray());
            }

            peStream.Seek(0, System.IO.SeekOrigin.Begin);

            dynamicAssemblyResolver.TryAdd(assemblyName, assembly);
            referenceCache.TryAdd(assemblyName, MetadataReference.CreateFromStream(peStream));
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
            return false;
        }
        return assembly != null;
    }
}