// modified version of Mono.CSharp.Driver

// driver.cs: The compiler command line driver.
//
// Authors:
//   Miguel de Icaza (miguel@gnu.org)
//   Marek Safar (marek.safar@gmail.com)
//
// Dual licensed under the terms of the MIT X11 or GNU GPL
//
// Copyright 2001, 2002, 2003 Ximian, Inc (http://www.ximian.com)
// Copyright 2004, 2005, 2006, 2007, 2008 Novell, Inc
// Copyright 2011 Xamarin Inc
//

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Threading;

using Mono.CSharp;

namespace CSharpCompiler
{
    /// <summary>
    ///    The compiler driver.
    /// </summary>
    public class CustomDynamicDriver
    {
        readonly CompilerContext ctx;

        public CustomDynamicDriver(CompilerContext ctx)
        {
            this.ctx = ctx;
        }

        public Report Report
        {
            get
            {
                return ctx.Report;
            }
        }

        void tokenize_file(SourceFile sourceFile, ModuleContainer module, ParserSession session)
        {
            Stream input;

            try
            {
                input = sourceFile.GetDataStream();
            }
            catch
            {
                Report.Error(2001, "Source file `" + sourceFile.Name + "' could not be found");
                return;
            }

            using (input)
            {
                SeekableStreamReader reader = new SeekableStreamReader(input, ctx.Settings.Encoding);
                var file = new CompilationSourceFile(module, sourceFile);

                Tokenizer lexer = new Tokenizer(reader, file, session, ctx.Report);
                int token, tokens = 0, errors = 0;

                while ((token = lexer.token()) != Token.EOF)
                {
                    tokens++;
                    if (token == Token.ERROR)
                        errors++;
                }
                Console.WriteLine("Tokenized: " + tokens + " found " + errors + " errors");
            }

            return;
        }

        public void Parse(ModuleContainer module)
        {
            bool tokenize_only = module.Compiler.Settings.TokenizeOnly;
            var sources = module.Compiler.SourceFiles;

            Location.Initialize(sources);

            var session = new ParserSession
            {
                UseJayGlobalArrays = true,
                LocatedTokens = new LocatedToken[15000]
            };

            for (int i = 0; i < sources.Count; ++i)
            {
                if (tokenize_only)
                {
                    tokenize_file(sources[i], module, session);
                }
                else
                {
                    Parse(sources[i], module, session, Report);
                }
            }
        }


        public void Parse(SourceFile file, ModuleContainer module, ParserSession session, Report report)
        {
            Stream input;

            try
            {
                input = file.GetDataStream();
            }
            catch
            {
                report.Error(2001, "Source file `{0}' could not be found", file.Name);
                return;
            }

            // Check 'MZ' header
            if (input.ReadByte() == 77 && input.ReadByte() == 90)
            {

                report.Error(2015, "Source file `{0}' is a binary file and not a text file", file.Name);
                input.Close();
                return;
            }

            input.Position = 0;
            SeekableStreamReader reader = new SeekableStreamReader(input, ctx.Settings.Encoding, session.StreamReaderBuffer);

            Parse(reader, file, module, session, report);

            if (ctx.Settings.GenerateDebugInfo && report.Errors == 0 && !file.HasChecksum)
            {
                input.Position = 0;
                var checksum = session.GetChecksumAlgorithm();
                file.SetChecksum(checksum.ComputeHash(input));
            }

            reader.Dispose();
            input.Close();
        }

        public static void Parse(SeekableStreamReader reader, SourceFile sourceFile, ModuleContainer module, ParserSession session, Report report)
        {
            var file = new CompilationSourceFile(module, sourceFile);
            module.AddTypeContainer(file);

            CSharpParser parser = new CSharpParser(reader, file, report, session);
            parser.parse();
        }



        //
        // Main compilation method
        //
        public bool Compile(out AssemblyBuilder outAssembly, AppDomain domain, bool generateInMemory)
        {
            var settings = ctx.Settings;

            outAssembly = null;
            //
            // If we are an exe, require a source file for the entry point or
            // if there is nothing to put in the assembly, and we are not a library
            //
            if (settings.FirstSourceFile == null &&
                ((settings.Target == Target.Exe || settings.Target == Target.WinExe || settings.Target == Target.Module) ||
                settings.Resources == null))
            {
                Report.Error(2008, "No files to compile were specified");
                return false;
            }

            if (settings.Platform == Platform.AnyCPU32Preferred && (settings.Target == Target.Library || settings.Target == Target.Module))
            {
                Report.Error(4023, "Platform option `anycpu32bitpreferred' is valid only for executables");
                return false;
            }

            TimeReporter tr = new TimeReporter(settings.Timestamps);
            ctx.TimeReporter = tr;
            tr.StartTotal();

            var module = new ModuleContainer(ctx);
            RootContext.ToplevelTypes = module;

            tr.Start(TimeReporter.TimerType.ParseTotal);
            Parse(module);
            tr.Stop(TimeReporter.TimerType.ParseTotal);

            if (Report.Errors > 0)
                return false;

            if (settings.TokenizeOnly || settings.ParseOnly)
            {
                tr.StopTotal();
                tr.ShowStats();
                return true;
            }

            var output_file = settings.OutputFile;
            string output_file_name;
           /* if (output_file == null)
            {
                var source_file = settings.FirstSourceFile;

                if (source_file == null)
                {
                    Report.Error(1562, "If no source files are specified you must specify the output file with -out:");
                    return false;
                }

                output_file_name = source_file.Name;
                int pos = output_file_name.LastIndexOf('.');

                if (pos > 0)
                    output_file_name = output_file_name.Substring(0, pos);

                output_file_name += settings.TargetExt;
                output_file = output_file_name;
            }
            else
            {*/
                output_file_name = Path.GetFileName(output_file);

              /*  if (string.IsNullOrEmpty(Path.GetFileNameWithoutExtension(output_file_name)) ||
                    output_file_name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    Report.Error(2021, "Output file name is not valid");
                    return false;
                }
            }*/


            var assembly = new AssemblyDefinitionDynamic(module, output_file_name, output_file);
            module.SetDeclaringAssembly(assembly);

            var importer = new ReflectionImporter(module, ctx.BuiltinTypes);
            assembly.Importer = importer;

            var loader = new DynamicLoader(importer, ctx);
            loader.LoadReferences(module);

            if (!ctx.BuiltinTypes.CheckDefinitions(module))
                return false;

            if (!assembly.Create(domain, AssemblyBuilderAccess.RunAndSave))
                return false;

            module.CreateContainer();

            loader.LoadModules(assembly, module.GlobalRootNamespace);

            module.InitializePredefinedTypes();

            if (settings.GetResourceStrings != null)
                module.LoadGetResourceStrings(settings.GetResourceStrings);

            tr.Start(TimeReporter.TimerType.ModuleDefinitionTotal);
            module.Define();
            tr.Stop(TimeReporter.TimerType.ModuleDefinitionTotal);

            if (Report.Errors > 0)
                return false;

            if (settings.DocumentationFile != null)
            {
                var doc = new DocumentationBuilder(module);
                doc.OutputDocComment(output_file, settings.DocumentationFile);
            }

            assembly.Resolve();

            if (Report.Errors > 0)
                return false;


            tr.Start(TimeReporter.TimerType.EmitTotal);
            assembly.Emit();
            tr.Stop(TimeReporter.TimerType.EmitTotal);

            if (Report.Errors > 0)
            {
                return false;
            }

            tr.Start(TimeReporter.TimerType.CloseTypes);
            module.CloseContainer();
            tr.Stop(TimeReporter.TimerType.CloseTypes);

            tr.Start(TimeReporter.TimerType.Resouces);
            if (!settings.WriteMetadataOnly)
                assembly.EmbedResources();
            tr.Stop(TimeReporter.TimerType.Resouces);

            if (Report.Errors > 0)
                return false;


            if(!generateInMemory) assembly.Save();
            outAssembly = assembly.Builder;


            tr.StopTotal();
            tr.ShowStats();

            return Report.Errors == 0;
        }
    }

}
