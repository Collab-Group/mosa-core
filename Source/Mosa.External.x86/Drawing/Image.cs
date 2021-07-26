﻿using Mosa.External.x86;

namespace System.Drawing
{
    public unsafe class Image
    {
        public MemoryBlock RawData;
        public int Length;
        public int Bpp;
        public int Width;
        public int Height;
    }
}