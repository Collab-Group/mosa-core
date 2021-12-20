[BITS 32]
    ;public static extern void OUTSD(ushort PORT, uint DATA, uint COUNT);
    MOV DX,[ESP+4]
    MOV ESI,[ESP+8]
    MOV ECX,[ESP+12]
    REP OUTSD ;REP表示循环ECX次，每次循环ECX-1
    RET