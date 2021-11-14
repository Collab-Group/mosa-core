using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Mosa.Launcher.Console
{
    partial class Program
    {
        private static void MakeISO_Grub2()
        {
            ZipFile.ExtractToDirectory(AppFolder+ @"Tools\grub2\grub2.zip", AppFolder+ @"output\");

            File.Copy(AppFolder+ @"output\main.exe", AppFolder+ @"output\boot\main.exe", true);
            File.Delete(AppFolder+ @"output\main.exe");

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
    }
}
