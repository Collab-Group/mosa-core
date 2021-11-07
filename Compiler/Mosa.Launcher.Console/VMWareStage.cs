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
        private static bool RunVMWareWorkstation()
        {
            string VMPlayerPath0 = @"\VMware\VMware Workstation\vmplayer.exe";
            string VMPlayerPath1 = @"\VMware\VMware Player\vmplayer.exe";

            VMPlayerPath0 = Environment.Is64BitOperatingSystem ? @"C:\Program Files (x86)\VMware\VMware Workstation\vmplayer.exe" : @"C:\Program Files\VMware\VMware Workstation\vmplayer.exe";
            VMPlayerPath1 = Environment.Is64BitOperatingSystem ? @"C:\Program Files (x86)\VMware\VMware Player\vmplayer.exe" : @"C:\Program Files\VMware\VMware Player\vmplayer.exe";

            if (!File.Exists(VMPlayerPath0) && !File.Exists(VMPlayerPath1))
            {
                WriteLine("VMWare Player Not Found!");
                return false;
            }

            var args = '"' + Path.Combine(AppFolder + @"\Tools\vmware", "MOSA.vmx") + '"';

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.UseShellExecute = false;
            //processStartInfo.CreateNoWindow = true;
            processStartInfo.Arguments = args;

            if (File.Exists(VMPlayerPath0))
            {
                processStartInfo.FileName = VMPlayerPath0;
            }
            if (File.Exists(VMPlayerPath1))
            {
                processStartInfo.FileName = VMPlayerPath1;
            }

            Process.Start(processStartInfo);
            return true;
        }
    }
}
