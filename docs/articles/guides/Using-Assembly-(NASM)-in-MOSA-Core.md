### Create a new file with the extension .nas in your project  
### And Just do whatever you want!

# Example
### ⚠ I put this file in ASM folder
```assembly
;nasm -fbin MEMCPY.nas -o MEMCPY.o
[BITS 32]
    ;public static extern void MEMCPY(uint DEST, uint SOURCE, uint LENGTH);
    MOV EAX,[ESP+4]
    MOV ECX,[ESP+8]
    MOV EDX,0
CONTINUE:
    MOV EBX,[ECX]
    MOV [EAX],EBX
    ADD EAX,4
    ADD ECX,4
    ADD EDX,4
    CMP EDX,[ESP+12]
    JB CONTINUE
    RET
```

### You can find nasm.exe in C:\Program Files (x86)\MOSA-Project\Tools\nasm
### ℹ I'd recommend you copy it to your asm file directory
### To compile your Assembly file. you have to use cmd.exe
### Press Win+R to launch cmd.exe and input the following commands
```cmd
nasm -fbin NASFileName -o OutputFileName
```
### Modify NASFileName and OutputFileName for yourself

# Use your assembly method in C#
### ⚠ You have to copy the output file (with folder if you have) into the bin folder
### Example
```csharp
[DllImport("ASM/MEMCPY.o", EntryPoint = "MEMCPY")]
public static extern void MEMCPY(uint DEST, uint SOURCE, uint LENGTH);
```