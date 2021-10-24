using Mosa.Compiler.Common.Configuration;
using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Linker;
using Mosa.Compiler.Framework.Trace;
using Mosa.Compiler.MosaTypeSystem;
using System;
using System.Diagnostics;
using System.IO;
using Mosa.Platform.Intel.CompilerStages;

namespace Mosa.Launcher.Console
{
    class Program
    {
        private static Settings Settings = MultibootV1Stage.Settings;

        private static CompilerHooks CompilerHooks;
        private static MosaLinker Linker;
        private static TypeSystem TypeSystem;

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

        public static string AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MOSA-Core");
        public static string VirtualBoxPath = @"C:\Program Files\Oracle\VirtualBox\VBoxManage.exe";

        public static string ISOFilePath
        {
            get
            {
                return Path.Combine(OutputFolder, "MOSA.iso");
            }
        }

        private static DateTime StartTime;

        public static bool JustBuild = false;
        private static bool VBEEnable = false;
        private static bool GrubEnable = false;
        public static int Width = 640;
        public static int Height = 480;

        //Arguments:
        //Arguments 1 Is The Input File
        //-VBE (Enable VBE)
        //-JUSTBUILD (Tell Compiler Do Not Launch VirtualBox After Compiling)
        //-GRUB (Use GRUB2 As Bootloader Instead Of Syslinux)

        //!Install before use!

        static void Main(string[] args)
        {
            try
            {
                /*
                if (!Environment.Is64BitOperatingSystem)
                {
                    WriteLine("Fatal! 32-Bit Operating System Is Not Supported");
                    WriteLine("Press Any Key To Continue...");
                    return;
                }
                */

                //For me to debug
                if (args.Length == 0)
                {
                    args = new string[]
                    {
                        @"C:\Users\nifan\source\repos\MOSA1\MOSA1\bin\MOSA1.dll"
                    };
                }

                //If you want to change "main.exe" to other name you have to modify the syslinux.cfg

                SourceName = args[0];
                OutputName = AppFolder + @"\output\main.exe";

                foreach (var v in args)
                {
                    switch (v.ToUpper())
                    {
                        case "-VBE":
                            VBEEnable = true;
                            break;
                        case "-JUSTBUILD":
                            JustBuild = true;
                            break;
                        case "-GRUB":
                            GrubEnable = true;
                            break;
                        case "-320X200":
                            SetVBEResolution(320, 200);
                            break;
                        case "-640X400":
                            SetVBEResolution(640, 400);
                            break;
                        case "-640X480":
                            SetVBEResolution(640, 480);
                            break;
                        case "-800X500":
                            SetVBEResolution(800, 500);
                            break;
                        case "-800X600":
                            SetVBEResolution(800, 600);
                            break;
                        case "-896X672":
                            SetVBEResolution(896, 672);
                            break;
                        case "-1024X640":
                            SetVBEResolution(1024, 640);
                            break;
                        case "-1024X768":
                            SetVBEResolution(1024, 768);
                            break;
                        case "-1152X720":
                            SetVBEResolution(1152, 720);
                            break;
                        case "-1280X1024":
                            SetVBEResolution(1280, 1024);
                            break;
                        case "-1360X768":
                            SetVBEResolution(1360, 768);
                            break;
                        case "-1440X900":
                            SetVBEResolution(1440, 900);
                            break;
                        case "-1600X1200":
                            SetVBEResolution(1600, 1200);
                            break;
                        case "-1920X1080":
                            SetVBEResolution(1920, 1080);
                            break;
                    }
                }

                WriteLine($"VBE Status: {VBEEnable}");
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

                if (GrubEnable)
                {
                    MakeISO_Grub2();
                }
                else 
                {
                    MakeISO_Syslinux();
                }

                if (!JustBuild)
                {
                    RunVirtualBox();
                }

                Environment.Exit(0);
            }
            catch (Exception E)
            {
                WriteLine("Exception Thrown While Compiling");
                WriteLine(E.Message);
                WriteLine(E.StackTrace);
                WriteLine("Please Report This Problem");
                WriteLine("Press Any Key To Continue...");
                Environment.Exit(0);
            }

            return;
        }

        private static void SetVBEResolution(int nWidth, int nHeight)
        {
            Width = nWidth;
            Height = nHeight;
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

        private static void MakeISO_Syslinux()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(AppFolder + @"\Tools\syslinux");
            foreach (var v in directoryInfo.GetFiles())
                v.CopyTo(Path.Combine(OutputFolder, v.Name), true);

            var args = $"-relaxed-filenames -J -R -o \"{ISOFilePath}\" -b isolinux.bin -no-emul-boot -boot-load-size 4 -boot-info-table \"{OutputFolder}\"";

            Process proc = new Process();
            proc.StartInfo.FileName = AppFolder + @"\Tools\mkisofs\mkisofs.exe";
            proc.StartInfo.Arguments = args;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            while (!proc.HasExited) ;
        }

        private static void MakeISO_Grub2()
        {
            Directory.CreateDirectory(Path.Combine(OutputFolder, @"boot\grub\i386-pc"));

            DirectoryInfo directoryInfo = new DirectoryInfo(AppFolder + @"\Tools\grub2\boot\grub\i386-pc");
            foreach (var v in directoryInfo.GetFiles())
                v.CopyTo(Path.Combine(Path.Combine(OutputFolder, @"boot\grub\i386-pc"), v.Name), true);

            directoryInfo = new DirectoryInfo(AppFolder + @"\Tools\grub2\boot\grub");
            foreach (var v in directoryInfo.GetFiles())
                v.CopyTo(Path.Combine(Path.Combine(OutputFolder, @"boot\grub"), v.Name), true);

            File.Copy(Path.Combine(AppFolder, @"output\main.exe"), Path.Combine(AppFolder, @"output\boot\main.exe"), true);
            File.Delete(Path.Combine(AppFolder, @"output\main.exe"));

            //var args = $"-relaxed-filenames -J -R -o \"{ISOFilePath}\" -b isolinux.bin -no-emul-boot -boot-load-size 4 -boot-info-table \"{OutputFolder}\"";
            var args = $"-relaxed-filenames -J -R -o \"{ISOFilePath}\" -b \"{@"boot/grub/i386-pc/eltorito.img"}\" -no-emul-boot -boot-load-size 4 -boot-info-table \"{OutputFolder}\"";

            Process proc = new Process();
            proc.StartInfo.FileName = AppFolder + @"\Tools\mkisofs\mkisofs.exe";
            proc.StartInfo.Arguments = args;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            while (!proc.HasExited) ;
        }

        private static void RunVirtualBox()
        {
            if (!File.Exists(VirtualBoxPath))
            {
                throw new FileNotFoundException("VirtualBox not found! Here to get VirtualBox: https://www.virtualbox.org/");

                return;
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.FileName = VirtualBoxPath;

            string path = @"C:\Users\" + Environment.UserName + @"\VirtualBox VMs";
            if (!File.Exists(path + @"\MOSA\MOSA.vbox"))
            {
                //Import VM
                processStartInfo.Arguments = $"import \"{AppFolder + @"\Tools\virtualbox\MOSA.ova"}\"";
                Process p1 = Process.Start(processStartInfo);
                p1.WaitForExit();
            }

            // Attach output ISO
            processStartInfo.Arguments = $"storageattach \"MOSA\" --storagectl IDE --port 1 --device 0 --type dvddrive --medium \"{ISOFilePath}\"";
            Process p2 = Process.Start(processStartInfo);
            p2.WaitForExit();

            WriteLine(@"Warning: If this is your first time using this version (Last modify time: 6th October 2021). Please Delete C:\Users\Your Use Name\VirtualBox VMs\MOSA !!!");
            WriteLine("If It Asks You Select Boot Disk");
            WriteLine("Please Press Cancel");
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
        }

        private static void DefaultSettings()
        {
            Settings.SetValue("Compiler.Platform", "x86");
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
            Settings.SetValue("Multiboot.Version", "v1");
            Settings.SetValue("Multiboot.Video", VBEEnable);
            Settings.SetValue("Multiboot.Video.Width", Width);
            Settings.SetValue("Multiboot.Video.Height", Height);
            Settings.SetValue("Multiboot.Video.Depth", 32);
            Settings.SetValue("Launcher.PlugKorlib", true);
            Settings.SetValue("Launcher.HuntForCorLib", true);
            Settings.SetValue("Linker.Drawf", false);
            Settings.SetValue("Compiler.OutputFile", OutputName);
        }
    }
}
