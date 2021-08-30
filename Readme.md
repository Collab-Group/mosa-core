# MOSA-Core
A fork of the MOSA (Managed Operating System Alliance) project, whose goals are to add useful features into MOSA, while keeping it organized and easy to use.<br/>
MOSA is a powerful and relatively stable way of creating an operating system in C# **(.NET Core 5.0)**.

# Informations
**Important**: Your project name **cannot** contain spaces (or SYSLINUX, the bootloader that we use, won't be able to load your OS)
ARMv8 is currently **not** available in MOSA-Core for stability reasons. We will add them if those get more stable in MOSA.

**Wiki: https://github.com/nifanfa/MOSA-Core/wiki<br/>**
**Discord server: https://discord.gg/WrNEsmUmKa<br/>**
[IntelliSense completion for unimported types and extension methods](https://docs.microsoft.com/en-us/visualstudio/ide/reference/intellisense-completion-unimported-types-extension-methods?view=vs-2019)<br/>

MOSA's official Gitter chat:<br/>
[![Gitter Chat][gitter-image]][gitter-chat]

[gitter-image]: https://img.shields.io/badge/gitter-join%20chat%20-blue.svg
[gitter2-image]: https://badges.gitter.im/Join%20Chat.svg
[gitter-chat]: https://gitter.im/mosa/MOSA-Project

# Dependencies
**Windows**<br/>
Visual Studio 2019 (recommended version: 16.10.x )<br/>
Inno Setup Compiler (recommended version: 6.2.x )<br/>

**Cross-platform**<br/>
JetBrains Rider (recommended IDE if you're on a Unix-based system)<br/>
Oracle VM VirtualBox (recommended version: 6.1.x )<br/>

# Usage
Install all required dependencies, then double click ``CreateInstaller.bat`` if you're on Windows, or build manually the solution using **MSBuild** if you're on macOS or Linux.<br/>
Typically, on macOS and Linux, you'll have to use **Mono**. MOSA-Core uses .NET 5.0, but the templates (and projects that are using MOSA-Core) are using .NET Framework 4.8. So Mono is required if you're on Unix-based systems.

# Demo
A GUI-based demo of MOSA-Core is available [here](https://github.com/nifanfa/MOSA-GUI-Sample).

# Contributors
<a href = "https://github.com/nifanfa/MOSA-Core/graphs/contributors">
  <img src = "https://contrib.rocks/image?repo=nifanfa/MOSA-Core"/>
</a>

# License

MOSA is licensed under the [New BSD License](http://en.wikipedia.org/wiki/New_BSD).

Copyright (c) 2008, MOSA-Project
All rights reserved.

Redistribution and use in source and binary forms, 
with or without modification, are permitted provided 
that the following conditions are met:

* Redistributions of source code must retain the
  above copyright notice, this list of conditions 
  and the following disclaimer.

* Redistributions in binary form must reproduce the 
  above copyright notice, this list of conditions and 
  the following disclaimer in the documentation and/or 
  other materials provided with the distribution.

* Neither the name of Managed Operating System Alliance (MOSA) 
  nor the names of its contributors may be used to endorse 
  or promote products derived from this software without 
  specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS
BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY 
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF
THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
OF SUCH DAMAGE.
