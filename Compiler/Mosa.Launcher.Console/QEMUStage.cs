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
        private static bool RunQEMU()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.FileName = Path.Combine(AppFolder, @"\Tools\qemu\qemu.exe"); ;

            // Attach output ISO
            processStartInfo.Arguments = $"-boot d -cdrom \"{ISOFilePath}\" -m 512";
            Process p2 = Process.Start(processStartInfo);

            return true;
        }
    }
}
