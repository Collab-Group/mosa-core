﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System;

namespace Mosa.External.x86
{
    public class VBERequireAttribute : Attribute
    {
        private int XRes;
        private int YRes;

        public VBERequireAttribute(int xres, int yres)
        {
            this.XRes = xres;
            this.YRes = yres;
        }
    }
}