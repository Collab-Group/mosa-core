//!Install before use!

using Mosa.Compiler.Common.Configuration;
using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Trace;
using System;
using System.Diagnostics;
using System.IO;

namespace Mosa.Launcher.Console
{
    partial class Program
    {
        private static Settings Settings = new Settings();

        private static CompilerHooks CompilerHooks;

        private static string OutputName;
        private static string SourceName;
        private static string OutputFolder
        {
            get
            {
                return Path.GetDirectoryName(OutputName);
            }
        }
        private static string SourceFolder
        {
            get
            {
                if (SourceName.IndexOf("\\") == -1)
                {
                    return Environment.CurrentDirectory;
                }
                else
                {
                    return Path.GetDirectoryName(SourceName);
                }
            }
        }

        public static string AppFolder = @"C:\Program Files\MOSA-Core";

        public static string ISOFilePath
        {
            get
            {
                return OutputFolder + "MOSA.iso";
            }
        }

        private static DateTime StartTime;

        //For Arguments
        public static bool JustBuild = false;
        public static bool IA64 = false;

        //Arguments:
        //Arguments 1 Is The Input File
        //-JUSTBUILD (Tell Compiler Do Not Launch VirtualBox After Compiling)
        //-THREAD{ThreadCount} (Compile With {{ThreadCount}} Threads)
        //-IA64 (Intel x64 Architecture)

        static void Main(string[] args)
        {
            try
            {
                //For me to debug
                if (args.Length == 0)
                {
                    args = new string[]
                    {
                        //@"C:\Users\nifan\Documents\GitHub\MOSA-GUI-Sample\MOSA1\bin\MOSA1.dll",
                        @"C:\Users\Administrator\source\Repos\MOSA1\MOSA1\bin\MOSA1.dll",
                        //"-THREAD128"
                        //"-JUSTBUILD"
                    };
                }

                SourceName = args[0];
                OutputName = AppFolder + @"\output\main.exe";

                string s;
                foreach (var v in args)
                {
                    //Uppered
                    s = v.ToUpper();

                    if (s.IndexOf("-JUSTBUILD") == 0)
                    {
                        JustBuild = true;
                    }
                    else if (s.IndexOf("-THREAD") == 0)
                    {
                        s = s.Replace("-THREAD", "");
                        Settings.SetValue("Compiler.Multithreading.MaxThreads", Convert.ToInt32(s));
                    }
                    else if (s.IndexOf("-IA64") == 0)
                    {
                        IA64 = true;
                    }
                }

                WriteLine($"JUSTBUILD Enabled: {JustBuild}");
                WriteLine($"Output ISO Path: {ISOFilePath}");

                DefaultSettings();
                RegisterPlatforms();
                SetFile();

                CompilerHooks = new CompilerHooks();
                CompilerHooks.NotifyEvent += NotifyEvent;

                if (Directory.Exists(OutputFolder))
                {
                    Directory.Delete(OutputFolder, true);
                }

                Directory.CreateDirectory(OutputFolder);

                Compile();

                MakeISO_Grub2();

                if (!JustBuild)
                {
                    if (!RunVMWareWorkstation())
                    {
                        if (!RunVirtualBox())
                        {
                            if (!RunQEMU())
                            {
                                WriteLine("No Virtual Machine Software Found!");
                            }
                        }
                    }
                }

                WriteLine("Please Right Click Visual Studio Output Window And Uncheck \"Module Load Message\" For Better Use!");

                Environment.Exit(0);
            }
            catch (Exception E)
            {
                WriteLine("Exception Thrown While Compiling");
                WriteLine(E.Message);
                //WriteLine(E.StackTrace);
                WriteLine("Please Report This Problem");
                WriteLine("Press Any Key To Continue...");
                Environment.Exit(0);
            }

            return;
        }

        public static void WriteLine(string s)
        {
            Debug.WriteLine($"\t{s}");
        }

        private static void Compile()
        {
            if (Settings.GetValue("Launcher.HuntForCorLib", false))
            {
                var fileCorlib = Path.Combine(SourceFolder, "mscorlib.dll");

                if (fileCorlib != null)
                {
                    Settings.AddPropertyListValue("Compiler.SourceFiles", fileCorlib);
                }
            }

            if (Settings.GetValue("Launcher.PlugKorlib", false))
            {
                var fileKorlib = Path.Combine(SourceFolder, "Mosa.Plug.Korlib.dll");

                if (fileKorlib != null)
                {
                    Settings.AddPropertyListValue("Compiler.SourceFiles", fileKorlib);
                }

                var platform = Settings.GetValue("Compiler.Platform", "x86");

                WriteLine($"Target Architecture: {platform}");

                var fileKorlibPlatform = Path.Combine(SourceFolder, $"Mosa.Plug.Korlib.{platform}.dll");

                if (fileKorlibPlatform != null)
                {
                    Settings.AddPropertyListValue("Compiler.SourceFiles", fileKorlibPlatform);
                }
            }

            var compiler = new MosaCompiler(Settings, CompilerHooks);

            compiler.Load();
            compiler.Compile();

            GC.Collect();
        }

        private static void NotifyEvent(CompilerEvent compilerEvent, string message, int threadID)
        {
            switch (compilerEvent)
            {
                case CompilerEvent.CompileStart:
                    StartTime = DateTime.Now;
                    WriteLine($"Starting to compile...");
                    break;
                case CompilerEvent.CompilingMethods:
                    WriteLine($"Compiling methods...");
                    break;
                case CompilerEvent.CompilingMethodsCompleted:
                    WriteLine($"Finished compiling methods!");
                    break;
                case CompilerEvent.CompileEnd:
                    TimeSpan timeSpan = DateTime.Now.Subtract(StartTime);
                    WriteLine($"Ended compilation {timeSpan}");
                    break;
                case CompilerEvent.Exception:
                    WriteLine("Exception Thrown:");
                    WriteLine(message);
                    Environment.Exit(0);
                    break;
                case CompilerEvent.Error:
                    WriteLine("Compiler Error:");
                    WriteLine(message);
                    Environment.Exit(0);
                    break;
            }
        }

        private static void SetFile()
        {
            Settings.AddPropertyListValue("Compiler.SourceFiles", SourceName);
            Settings.AddPropertyListValue("SearchPaths", SourceFolder);
            Settings.AddPropertyListValue("SearchPaths", Path.Combine(AppFolder, "asm"));
        }

        private static void RegisterPlatforms()
        {
            PlatformRegistry.Add(new Mosa.Platform.x86.Architecture());
            PlatformRegistry.Add(new Mosa.Platform.x64.Architecture());
        }

        private static void DefaultSettings()
        {
            Settings.SetValue("Compiler.Platform", IA64 ? "x64" : "x86");
            Settings.SetValue("Compiler.BaseAddress", 0x00400000);
            Settings.SetValue("Compiler.Binary", true);
            Settings.SetValue("Compiler.MethodScanner", false);
            Settings.SetValue("Compiler.TraceLevel", 0);
            Settings.SetValue("Compiler.Multithreading", true);
            Settings.SetValue("CompilerDebug.DebugFile", string.Empty);
            Settings.SetValue("CompilerDebug.AsmFile", string.Empty);
            Settings.SetValue("CompilerDebug.MapFile", AppFolder+ @"\output\map.txt");
            Settings.SetValue("CompilerDebug.NasmFile", string.Empty);
            Settings.SetValue("CompilerDebug.InlineFile", string.Empty);
            Settings.SetValue("Optimizations.Basic", true);
            Settings.SetValue("Optimizations.BitTracker", true);
            Settings.SetValue("Optimizations.Inline", true);
            Settings.SetValue("Optimizations.Inline.AggressiveMaximum", 24);
            Settings.SetValue("Optimizations.Inline.ExplicitOnly", false);
            Settings.SetValue("Optimizations.Inline.Maximum", 12);
            Settings.SetValue("Optimizations.LongExpansion", true);
            Settings.SetValue("Optimizations.LoopInvariantCodeMotion", true);
            Settings.SetValue("Optimizations.Platform", true);
            Settings.SetValue("Optimizations.SCCP", true);
            Settings.SetValue("Optimizations.Devirtualization", true);
            Settings.SetValue("Optimizations.SSA", true);
            Settings.SetValue("Optimizations.TwoPass", true);
            Settings.SetValue("Optimizations.ValueNumbering", true);
            Settings.SetValue("Multiboot.Version", "v1");
            Settings.SetValue("Multiboot.Video", false);
            Settings.SetValue("Launcher.PlugKorlib", true);
            Settings.SetValue("Launcher.HuntForCorLib", true);
            Settings.SetValue("Linker.Drawf", false);
            Settings.SetValue("Compiler.OutputFile", OutputName);
        }
    }
}
