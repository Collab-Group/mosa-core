using Mosa.Kernel;
using Mosa.Kernel.x86;

namespace Mosa.External.x86.Drawing
{
    public static class GraphicsSelector
    {
        static Graphics CurrentGraphics = null;
        
        public static Graphics GetGraphics(int width = 640, int height = 480)
        {
            if (CurrentGraphics != null) return CurrentGraphics;

            //VBE Should Always Be The Top
            if (VBE.IsVBEAvailable) 
            {
                CurrentGraphics =  new VBEGraphics();
                return CurrentGraphics;
            }

            if (PCI.Exists(VendorID.VMWare, DeviceID.SVGAIIAdapter)) {

                CurrentGraphics = new VMWareSVGAIIGraphics(width, height);
                return CurrentGraphics;
            }

            Panic.Error("No graphics are available for the current system.");
            return null;
        }
    }
}
