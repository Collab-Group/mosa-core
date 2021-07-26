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