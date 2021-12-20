using Mosa.External.x86.Driver;

namespace Mosa.External.x86.FileSystem
{
    public unsafe class IDEDisk : IDisk
    {
        private readonly IDE IDE;

        public IDEDisk(IDE.ControllerIndex controllerIndex)
        {
            IDE = new IDE(controllerIndex);
            IDE.Initialize();
        }

        public bool ReadBlock(uint sector, uint count, byte[] data)
        {
            fixed (byte* p = data)
            {
                for (uint i = 0; i < count; i++)
                {
                    IDE.PerformLBA28(
                        IDE.SectorOperation.Read,
                        IDE.Drive.Master,
                        sector + i,
                        p + (i * IDE.SectorSize)
                        );
                }
            }
            return true;
        }

        public bool WriteBlock(uint sector, uint count, byte[] data)
        {
            fixed (byte* p = data)
            {
                for (uint i = 0; i < count; i++)
                {
                    IDE.PerformLBA28(
                        IDE.SectorOperation.Write,
                        IDE.Drive.Master,
                        sector + i,
                        p + (i * IDE.SectorSize)
                        );
                }
            }
            return true;
        }
    }
}
