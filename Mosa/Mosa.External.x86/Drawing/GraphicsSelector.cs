using Mosa.Kernel;
using Mosa.Kernel.x86;

namespace Mosa.External.x86.Drawing
{
    public static class GraphicsSelector
    {
        VBEGraphics CurrentVBEGraphics = null;
        VMWareSVGAIIGraphics CurrentVMWareSVGAIIGraphics = null;
        
        public static Graphics GetGraphics(int width = 640, int height = 480)
        {
            //VBE Should Always Be The Top
            if (VBE.IsVBEAvailable) {
                
                if(CurrentVBEGraphics == null) 
                    CurrentVBEGraphics =  new VBEGraphics();
                
                return CurrentVBEGraphics;
                
            }

            if (PCI.Exists(VendorID.VMWare, DeviceID.SVGAIIAdapter)) {
                
                if(CurrentVMWareSVGAIIGraphics == null) 
                    CurrentVMWareSVGAIIGraphics =  new VMWareSVGAIIGraphics(width, height);
                
                return CurrentVMWareSVGAIIGraphics 
            }

            Panic.Error("No graphics are available for the current system.");
            return null;
        }
    }
}
