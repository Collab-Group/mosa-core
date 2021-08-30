using Mosa.Compiler.Common.Configuration;
using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Linker;
using Mosa.Compiler.Framework.Trace;
using Mosa.Compiler.MosaTypeSystem;
using System;
using System.Diagnostics;
using System.IO;

namespace Mosa.Launcher.Console
{
    class Program
    {
        private static Settings Settings = new Settings();
        private static string[] Arguments;

        private static CompilerHooks CompilerHooks;
        private static MosaLinker Linker;
        private static TypeSystem TypeSystem;

        private static string OutputName
        {
            get
            {
                return Arguments[1];
            }
        }
        private static string SourceName
        {
            get
            {
                return Arguments[0];
            }
        }
        private static string OutputFolder
        {
            get
            {
                return Path.GetDirectoryName(Arguments[1]);
            }
        }
        private static string SourceFolder
        {
            get
            {
                return Path.GetDirectoryName(Arguments[0]);
            }
        }
        private static bool VBEEnable
        {
            get
            {
                return Convert.ToBoolean(Arguments[2]);
            }
        }

        public static string AppFolder = @"C:\Program Files (x86)\MOSA-Project";
        public static string VirtualBoxPath = @"C:\Program Files\Oracle\VirtualBox\VBoxManage.exe";

        public static string ISOFilePath
        {
            get
            {
                return Path.Combine(OutputFolder, "MOSA.iso");
            }
        }

        private static DateTime StartTime;

        public static bool RunAfterBuild = true;

        public static bool x64 = false;

        static void Main(string[] args)
        {
            if (!Environment.Is64BitOperatingSystem)
            {
                System.Console.WriteLine("Fatal! 32-Bit Operating System Is Not Supported");
                System.Console.WriteLine("Press Any Key To Continue...");
                System.Console.ReadKey();
                return;
            }

            //Arguments 1: Source Name
            //Arguments 2: Output Name
            //Arguments 3: VBE Enable

            //Arguments: JustBuild
            //Arguments: x64

            //If you want to change "main.exe" to other name you have to modify the syslinux.cfg
            Arguments = new string[] { args[0], AppFolder + @"\output\main.exe", args[2] };

            System.Console.WriteLine($"VBE Status: {VBEEnable}");
            System.Console.WriteLine($"Output ISO Path:{ISOFilePath}");

            foreach (var v in args)
            {
                if (v == "JustBuild")
                {
                    RunAfterBuild = false;
                }
                if (v == "x64")
                {
                    x64 = false;
                }
            }

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

            MakeISO();

            if (RunAfterBuild)
            {
                RunVirtualBox();
            }

            Environment.Exit(0);

            return;
        }

        private static void Compile()
        {
            try
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

                    if (platform == "armv8a32")
                    {
                        platform = "ARMv8A32";
                    }

                    var fileKorlibPlatform = Path.Combine(SourceFolder, $"Mosa.Plug.Korlib.{platform}.dll");

                    if (fileKorlibPlatform != null)
                    {
                        Settings.AddPropertyListValue("Compiler.SourceFiles", fileKorlibPlatform);
                    }
                }

                var compiler = new MosaCompiler(Settings, CompilerHooks);

                compiler.Load();
                compiler.Compile();

                Linker = compiler.Linker;
                TypeSystem = compiler.TypeSystem;

                GC.Collect();
            }
            catch (Exception E)
            {
                System.Console.WriteLine("Exception Thrown While Compiling");
                System.Console.WriteLine(E.Message);
                System.Console.WriteLine(E.StackTrace);
                System.Console.WriteLine("Please Report This Problem");
                System.Console.WriteLine("Press Any Key To Continue...");
                System.Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private static void MakeISO()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(AppFolder + @"\Tools\syslinux");
            foreach (var v in directoryInfo.GetFiles())
                v.CopyTo(Path.Combine(OutputFolder, v.Name), true);

            var args = $"-relaxed-filenames -J -R -o \"{ISOFilePath}\" -b isolinux.bin -no-emul-boot -boot-load-size 4 -boot-info-table \"{OutputFolder}\"";
            Process process = Process.Start(AppFolder + @"\Tools\mkisofs\mkisofs.exe", args);

            while (!process.HasExited) ;
        }

        private static void RunVirtualBox()
        {
            if (!File.Exists(VirtualBoxPath))
            {
                System.Console.WriteLine("VirtualBox not found!");
                return;
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.UseShellExecute = false;
            processStartInfo.FileName = VirtualBoxPath;

            string path = @"C:\Users\" + Environment.UserName + @"\VirtualBox VMs";
            if (!File.Exists(path + @"\MOSA\MOSA.vbox"))
            {
                //Import VM
                processStartInfo.Arguments = $"import \"{AppFolder + @"\Tools\virtualbox\MOSA.ova"}\"";
                Process p1 = Process.Start(processStartInfo);
                p1.WaitForExit();

                // Attach output ISO
                processStartInfo.Arguments = $"storageattach \"MOSA\" --storagectl IDE --port 1 --device 0 --type dvddrive --medium \"{ISOFilePath}\"";
                Process p2 = Process.Start(processStartInfo);
                p2.WaitForExit();
            }

            ConsoleColor color = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("Warning: If this is your first time using this version(Last modify time: 5th Augest 2021 23:56). Make sure there's no virtual machine which is named \"MOSA\". If so please delete it");
            System.Console.WriteLine("If It Asks You Select Boot Disk");
            System.Console.WriteLine("Please Press Cancel");
            System.Console.ForegroundColor = color;
            // Launch VM
            processStartInfo.Arguments = "startvm MOSA";
            Process.Start(processStartInfo);
        }

        private static void NotifyEvent(CompilerEvent compilerEvent, string message, int threadID)
        {
            switch (compilerEvent)
            {
                case CompilerEvent.CompileStart:
                    StartTime = DateTime.Now;
                    System.Console.WriteLine($"Compile Start");
                    break;
                case CompilerEvent.CompilingMethods:
                    System.Console.WriteLine($"Compiling Methods");
                    break;
                case CompilerEvent.CompilingMethodsCompleted:
                    System.Console.WriteLine($"Compiling Methods Completed");
                    break;
                case CompilerEvent.CompileEnd:
                    TimeSpan timeSpan = DateTime.Now.Subtract(StartTime);
                    System.Console.WriteLine($"Compile End {timeSpan}");
                    break;
                case CompilerEvent.Exception:
                    System.Console.WriteLine("Exception Thrown:");
                    System.Console.WriteLine(message);
                    System.Console.ReadKey();
                    Environment.Exit(0);
                    break;
                case CompilerEvent.Error:
                    System.Console.WriteLine("Compiler Error:");
                    System.Console.WriteLine(message);
                    System.Console.ReadKey();
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
            Settings.SetValue("Compiler.Platform", x64 ? "x64" : "x86");
            Settings.SetValue("Compiler.BaseAddress", 0x00400000);
            Settings.SetValue("Compiler.Binary", true);
            Settings.SetValue("Compiler.MethodScanner", false);
            Settings.SetValue("Compiler.TraceLevel", 0);
            Settings.SetValue("Compiler.Multithreading", true);
            Settings.SetValue("CompilerDebug.DebugFile", string.Empty);
            Settings.SetValue("CompilerDebug.AsmFile", string.Empty);
            Settings.SetValue("CompilerDebug.MapFile", string.Empty);
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
            Settings.SetValue("Image.BootLoader", "Syslinux3.72");
            //Settings.SetValue("Image.Folder", Path.Combine(Environment.CurrentDirectory, "MOSA"));
            Settings.SetValue("Image.Format", "ISO");
            Settings.SetValue("Image.FileSystem", "FAT16");
            Settings.SetValue("Image.ImageFile", "%DEFAULT%");
            Settings.SetValue("Multiboot.Version", "v1");
            Settings.SetValue("Multiboot.Video", VBEEnable);
            Settings.SetValue("Multiboot.Video.Width", 640);
            Settings.SetValue("Multiboot.Video.Height", 480);
            Settings.SetValue("Multiboot.Video.Depth", 32);
            Settings.SetValue("Emulator", "VirtualBox");
            Settings.SetValue("Emulator.Memory", 128);
            Settings.SetValue("Emulator.Serial", "none");
            Settings.SetValue("Emulator.Serial.Host", "127.0.0.1");
            Settings.SetValue("Emulator.Serial.Port", new Random().Next(11111, 22222));
            Settings.SetValue("Emulator.Serial.Pipe", "MOSA");
            Settings.SetValue("Emulator.Display", true);
            Settings.SetValue("Launcher.Start", true);
            Settings.SetValue("Launcher.Launch", true);
            Settings.SetValue("Launcher.Exit", true);
            Settings.SetValue("Launcher.PlugKorlib", true);
            Settings.SetValue("Launcher.HuntForCorLib", true);
            Settings.SetValue("Linker.Drawf", false);
            Settings.SetValue("OS.Name", "MOSA");
            Settings.SetValue("Compiler.OutputFile", OutputName);
        }
    }
}
