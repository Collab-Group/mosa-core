using Mosa.Kernel;
using Mosa.Kernel.x86;

namespace Mosa.External.x86.Drawing
{
    public static class GraphicsSelector
    {
        public static Graphics GetGraphics(int width = 640, int height = 480)
        {
            // BGA first, then VBE (so that graphics work in Bochs for example)
            PCIDevice bga = PCI.GetDevice(VendorID.Bochs, DeviceID.BGA);

            if (bga != null)
                return new BGAGraphics(bga, width, height);

            if (VBE.IsVBEAvailable)
                return new VBEGraphics();

            if (PCI.Exists(VendorID.VMWare, DeviceID.SVGAIIAdapter))
                return new VMWareSVGAIIGraphics(width, height);

            Panic.Error("No graphics are available for the current system.");
            return null;
        }
    }
}
