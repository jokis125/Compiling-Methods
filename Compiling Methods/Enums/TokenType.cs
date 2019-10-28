namespace CompilingMethods.Classes
{
    public enum TokenType
    {
        Ident,
        LitInt,
        LitStr,
        LitFloat,
        OpLess,
        OpLessEqual,
        OpMore,
        OpMoreEqual,
        OpEqual,
        OpNeg,
        OpNotEqual,
        OpAssign,
        OpAdd,
        OpAssAdd,
        OpSub,
        OpAssSub,
        OpMul,
        OpAssMul,
        OpDiv,
        OpAssDiv,
        OpInc,
        OpDec,
        OpComma,
        Separator,
        ParenOp,
        ParenCl,
        BracesOp,
        BracesCl,
        If,
        Break,
        Return,
        While,
        Continue,
        Int,
        Float,
        String,
        Char,
        Boolean,
        True,
        False,
        Eof
    }
}