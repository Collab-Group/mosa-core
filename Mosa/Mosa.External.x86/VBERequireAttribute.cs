// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System;

namespace Mosa.External.x86
{
    //Check Out Compiler.Mosa.Compiler.Framework.MethodCompiler.VBERequireAttribute
    public class VBERequireAttribute : Attribute
    {
        private int XRes;
        private int YRes;


        //Tell Compiler To Enable VBE With A Specific Resolution If GPU Supported
        public VBERequireAttribute(int xres, int yres)
        {
            this.XRes = xres;
            this.YRes = yres;
        }
    }
}