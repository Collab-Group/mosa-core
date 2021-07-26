;nasm -fbin MEMFILL.nas -o MEMFILL.o
[BITS 32]
    ;public static extern void MEMFILL(uint DEST, uint LENGTH, uint VALUE);
    MOV ECX,[ESP+4]
    MOV EDX,[ESP+12]
    MOV EAX,0
CONTINUE:
    MOV [ECX],EDX
    ADD ECX,4
    ADD EAX,4
    CMP EAX,[ESP+8]
    JB CONTINUE
    RET