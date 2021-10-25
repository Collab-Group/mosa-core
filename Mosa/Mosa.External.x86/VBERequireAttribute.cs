// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System;

namespace Mosa.External.x86
{
    //Check Out Compiler.Mosa.Compiler.Framework.MethodCompiler.VBERequireAttribute
    public class VBERequireAttribute : Attribute
    {
        private int xres;
        private int yres;


        //Tell Compiler To Enable VBE With A Specific Resolution If GPU Supported
        public VBERequireAttribute(int xres, int yres)
        {
            this.xres = xres;
            this.yres = yres;
        }
    }
}