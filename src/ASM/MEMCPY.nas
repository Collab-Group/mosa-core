;nasm -fbin MEMCPY.nas -o MEMCPY.o
[BITS 32]
    ;public static extern void MEMCPY(uint DEST, uint SOURCE, uint LENGTH);
    MOV ECX,[ESP+12]
    MOV ESI,[ESP+8]
    MOV EDI,[ESP+4]
    REP MOVSB
    RET