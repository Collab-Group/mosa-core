[BITS 32]
    ;public static extern void MEMCLR(uint DEST, uint LENGTH);
    MOV EAX,[ESP+4]
    MOV ECX,0
CONTINUE:
    MOV BYTE [EAX],0
    ADD ECX,1
    ADD EAX,1
    CMP ECX,[ESP+8]
    JB CONTINUE
    
    RET