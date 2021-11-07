using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mosa.Launcher.Console
{
    partial class Program
    {
        private static bool RunVirtualBox()
        {
            string VirtualBoxPath = @"C:\Program Files\Oracle\VirtualBox\VBoxManage.exe";

            if (!File.Exists(VirtualBoxPath))
            {
                WriteLine("Virtual Box Not Found!");
                return false;
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.FileName = VirtualBoxPath;

            if (!File.Exists(@"C:\Users\" + Environment.UserName + @"\VirtualBox VMs\MOSA\MOSA.vbox"))
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

            return true;
        }
    }
}
