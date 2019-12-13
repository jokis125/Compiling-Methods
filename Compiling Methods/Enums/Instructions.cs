namespace CompilingMethods.Enums
{
    public enum Instructions
    {
        //arithmetic
        IntAdd,
        IntSub,
        IntMul,
        IntDiv,
        //comparison
        IntLess,
        IntMore,
        IntLessEqual,
        IntMoreEqual,
        IntEqual,
        //stack instructions
        GetL,
        SetL,
        GetG,
        SetG,
        Pop,
        IntPush,
        //Control
        Br,
        Bz,
        Cn,
        Ret,
        RetV,
        CallBegin,
        Call
    }
}