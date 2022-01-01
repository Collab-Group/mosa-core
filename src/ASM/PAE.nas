[BITS 32]
    MOV EAX, CR4
    OR EAX, 1 << 5
    MOV CR4, EAX
    RET