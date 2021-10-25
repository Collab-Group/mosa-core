// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System;

namespace Mosa.External.x86
{
    public class ResourceAttribute : Attribute
    {
        string[] files;

        public ResourceAttribute(params string[] files)
        {
            this.files = files;
        }
    }
}