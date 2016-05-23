using Mono.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Linq;
using System.Text;

namespace CSharpCompiler
{
    //
    // Summary:
    //     Defines an interface for invoking compilation of source code or a CodeDOM tree
    //     using a specific compiler.
    public class CodeCompiler : ICodeCompiler
    {
        static long assemblyCounter = 0;

        // Summary:
        //     Compiles an assembly from the System.CodeDom tree contained in the specified
        //     System.CodeDom.CodeCompileUnit, using the specified compiler settings.
        //
        // Parameters:
        //   options:
        //     A System.CodeDom.Compiler.CompilerParameters object that indicates the settings
        //     for compilation.
        //
        //   compilationUnit:
        //     A System.CodeDom.CodeCompileUnit that indicates the code to compile.
        //
        // Returns:
        //     A System.CodeDom.Compiler.CompilerResults object that indicates the results of
        //     compilation.
        public CompilerResults CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit compilationUnit)
        {
            return CompileAssemblyFromDomBatch(options, new[] { compilationUnit });
        }

        // Summary:
        //     Compiles an assembly based on the System.CodeDom trees contained in the specified
        //     array of System.CodeDom.CodeCompileUnit objects, using the specified compiler
        //     settings.
        //
        // Parameters:
        //   options:
        //     A System.CodeDom.Compiler.CompilerParameters object that indicates the settings
        //     for compilation.
        //
        //   compilationUnits:
        //     An array of type System.CodeDom.CodeCompileUnit that indicates the code to compile.
        //
        // Returns:
        //     A System.CodeDom.Compiler.CompilerResults object that indicates the results of
        //     compilation.
        public CompilerResults CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            try
            {
                return CompileFromDomBatch(options, ea);
            }
            finally
            {
                options.TempFiles.Delete();
            }
        }

        // Summary:
        //     Compiles an assembly based on the System.CodeDom trees contained in the specified
        //     array of System.CodeDom.CodeCompileUnit objects, using the specified compiler
        //     settings.
        //
        // Parameters:
        //   options:
        //     A System.CodeDom.Compiler.CompilerParameters object that indicates the settings
        //     for compilation.
        //
        //   compilationUnits:
        //     An array of type System.CodeDom.CodeCompileUnit that indicates the code to compile.
        //
        // Returns:
        //     A System.CodeDom.Compiler.CompilerResults object that indicates the results of
        //     compilation.
        private CompilerResults CompileFromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
        {
            throw new NotImplementedException("sorry ICodeGenerator is not implemented, feel free to fix it and request merge");
            /*
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (ea == null)
            {
                throw new ArgumentNullException("ea");
            }

            string[] fileNames = new string[ea.Length];
            var assemblies = options.ReferencedAssemblies;

            for (int i = 0; i < ea.Length; i++)
            {
                CodeCompileUnit compileUnit = ea[i];
                fileNames[i] = options.TempFiles.AddExtension(i + ".cs");
                FileStream f = new FileStream(fileNames[i], FileMode.OpenOrCreate);
                StreamWriter s = new StreamWriter(f, Encoding.UTF8);
                if (compileUnit.ReferencedAssemblies != null)
                {
                    foreach (string str in compileUnit.ReferencedAssemblies)
                    {
                        if (!assemblies.Contains(str))
                            assemblies.Add(str);
                    }
                }

                ((ICodeGenerator)this).GenerateCodeFromCompileUnit(compileUnit, s, new CodeGeneratorOptions());
                s.Close();
                f.Close();
            }
            return CompileAssemblyFromFileBatch(options, fileNames);
             */ 
        }

        // Summary:
        //     Compiles an assembly from the source code contained within the specified file,
        //     using the specified compiler settings.
        //
        // Parameters:
        //   options:
        //     A System.CodeDom.Compiler.CompilerParameters object that indicates the settings
        //     for compilation.
        //
        //   fileName:
        //     The file name of the file that contains the source code to compile.
        //
        // Returns:
        //     A System.CodeDom.Compiler.CompilerResults object that indicates the results of
        //     compilation.
        public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string fileName)
        {
            return CompileAssemblyFromFileBatch(options, new[] { fileName });
        }

        // Summary:
        //     Compiles an assembly from the source code contained within the specified files,
        //     using the specified compiler settings.
        //
        // Parameters:
        //   options:
        //     A System.CodeDom.Compiler.CompilerParameters object that indicates the settings
        //     for compilation.
        //
        //   fileNames:
        //     The file names of the files to compile.
        //
        // Returns:
        //     A System.CodeDom.Compiler.CompilerResults object that indicates the results of
        //     compilation.
        public CompilerResults CompileAssemblyFromFileBatch(CompilerParameters options, string[] fileNames)
        {
            var settings = ParamsToSettings(options);

            foreach (var fileName in fileNames)
            {
                string path = Path.GetFullPath(fileName);
                var unit = new SourceFile(fileName, path, settings.SourceFiles.Count + 1);
                settings.SourceFiles.Add(unit);
            }

            return CompileFromCompilerSettings(settings, options.GenerateInMemory);
        }



        // Summary:
        //     Compiles an assembly from the specified string containing source code, using
        //     the specified compiler settings.
        //
        // Parameters:
        //   options:
        //     A System.CodeDom.Compiler.CompilerParameters object that indicates the settings
        //     for compilation.
        //
        //   source:
        //     The source code to compile.
        //
        // Returns:
        //     A System.CodeDom.Compiler.CompilerResults object that indicates the results of
        //     compilation.
        public CompilerResults CompileAssemblyFromSource(CompilerParameters options, string source)
        {
            return CompileAssemblyFromSourceBatch(options, new[] { source });
        }

        // Summary:
        //     Compiles an assembly from the specified array of strings containing source code,
        //     using the specified compiler settings.
        //
        // Parameters:
        //   options:
        //     A System.CodeDom.Compiler.CompilerParameters object that indicates the settings
        //     for compilation.
        //
        //   sources:
        //     The source code strings to compile.
        //
        // Returns:
        //     A System.CodeDom.Compiler.CompilerResults object that indicates the results of
        //     compilation.
        public CompilerResults CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources)
        {
            var settings = ParamsToSettings(options);

            int i = 0;
            foreach (var _source in sources)
            {
                var source = _source;
                Func<Stream> getStream = () => { return new MemoryStream(Encoding.UTF8.GetBytes(source ?? "")); };
                var fileName = i.ToString();
                var unit = new SourceFile(fileName, fileName, settings.SourceFiles.Count + 1, getStream);
                settings.SourceFiles.Add(unit);
                i++;
            }

            return CompileFromCompilerSettings(settings, options.GenerateInMemory);
        }


        CompilerResults CompileFromCompilerSettings(CompilerSettings settings, bool generateInMemory)
        {
            var compilerResults = new CompilerResults(new TempFileCollection(Path.GetTempPath()));
            var driver = new CustomDynamicDriver(new CompilerContext(settings, new CustomReportPrinter(compilerResults)));

            AssemblyBuilder outAssembly = null;
            try
            {
                driver.Compile(out outAssembly, AppDomain.CurrentDomain, generateInMemory);
            }
            catch (Exception e)
            {
                compilerResults.Errors.Add(new CompilerError()
                {
                    IsWarning = false,
                    ErrorText = e.Message,
                });
            }
            compilerResults.CompiledAssembly = outAssembly;

            return compilerResults;
        }


        CompilerSettings ParamsToSettings(CompilerParameters parameters)
        {
            var settings = new CompilerSettings();


            foreach (var assembly in parameters.ReferencedAssemblies) settings.AssemblyReferences.Add(assembly);

            //settings.AssemblyReferencesAliases;
            //settings.BreakOnInternalError
            //settings.Checked;
            //settings.DebugFlags;
            //settings.DocumentationFile;
            settings.Encoding = System.Text.Encoding.UTF8;
            //settings.EnhancedWarnings;
            //settings.FatalCounter;

            settings.GenerateDebugInfo = parameters.IncludeDebugInformation;
            //settings.GetResourceStrings;
            //settings.LoadDefaultReferences;

            settings.MainClass = parameters.MainClass;
            //settings.Modules;
            //settings.Optimize;


            //settings.ParseOnly;
            settings.Platform = Platform.AnyCPU;
            /*
            settings.ReferencesLookupPaths;
            settings.Resources;
            settings.RuntimeMetadataVersion;
            settings.SdkVersion;
            settings.ShowFullPaths;
            settings.Stacktrace;
            settings.StatementMode;
            settings.StdLib;
            */
            settings.StdLibRuntimeVersion = RuntimeVersion.v4;
            /*
            settings.StrongNameDelaySign;
            settings.StrongNameKeyContainer;
            settings.StrongNameKeyFile;
            settings.TabSize;
            */
            if (parameters.GenerateExecutable)
            {
                settings.Target = Target.Exe;
                settings.TargetExt = ".exe";
            }
            else
            {
                settings.Target = Target.Library;
                settings.TargetExt = ".dll";
            }
            if (parameters.GenerateInMemory) settings.Target = Target.Library;

            if (string.IsNullOrEmpty(parameters.OutputAssembly))
            {
                parameters.OutputAssembly = settings.OutputFile = "DynamicAssembly_" + assemblyCounter + settings.TargetExt;
                assemblyCounter++;
            }
            settings.OutputFile = parameters.OutputAssembly; // if it is not being outputted, we use this to set name of the dynamic assembly

            /*
            settings.Timestamps;
            settings.TokenizeOnly;
            settings.Unsafe;
            settings.VerboseParserFlag;
            settings.VerifyClsCompliance;
            */
            settings.Version = LanguageVersion.Default;
            settings.WarningLevel = parameters.WarningLevel;
            settings.WarningsAreErrors = parameters.TreatWarningsAsErrors;
            /*
            settings.Win32IconFile;
            settings.Win32ResourceFile;
            settings.WriteMetadataOnly;
            */

            return settings;
        }
    }
}
