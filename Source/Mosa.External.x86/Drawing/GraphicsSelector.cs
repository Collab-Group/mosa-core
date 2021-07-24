using Mosa.Kernel;
using Mosa.Kernel.x86;

namespace Mosa.External.x86.Drawing
{
    public static class GraphicsSelector
    {
        public static Graphics GetGraphics(int width = 640, int height = 480)
        {
            // Even if VBE is enabled, choose VMWare SVGA II driver first for best performance in VM
            if (PCI.Exists(VendorID.VMWare, DeviceID.SVGAIIAdapter))
            {
                return new VMWareSVGAIIGraphics(width, height);
            }
            else if (VBE.IsVBEAvailable)
            {
                return new VBEGraphics();
            }
            else
            {
                throw new System.Exception("No graphics are available for the current system.");
            }
        }
    }
}
