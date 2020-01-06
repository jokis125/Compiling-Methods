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
        IntNotEqual,
        //stack instructions
        GetL,
        SetL,
        GetG,
        SetG,
        Pop,
        Push,
        //Control
        Br,
        Bz,
        Cn,
        Ret,
        RetV,
        CallBegin,
        Call,
        //
        Exit,
        Alloc,
        Read,
        Print
        //Print
        
    }
}