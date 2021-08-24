[BITS 32]
    ;public static extern void INSD(uint PORT, uint DATA, uint COUNT);
    MOV DX,[ESP+4]
    MOV EDI,[ESP+8]
    MOV ECX,[ESP+12]
    REP INSD
    RET