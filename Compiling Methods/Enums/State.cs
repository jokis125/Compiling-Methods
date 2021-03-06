namespace CompilingMethods.Enums
{
    public enum State
    {
        Start,
        Ident,

        //variable
        LitInt,
        LitStr,
        LitEsc,
        LitFloat,
        LitFloatExp,

        //operators
        OpLess,
        OpMore,
        OpNeg,
        OpAssign,
        OpAdd,
        OpSub,
        OpMul,
        OpDiv,
        OpMod,
        OpComma,
        OpAnd,
        OpBinAnd,
        OpOr,
        OpBinOr,

        //separator
        Separator,

        //comments
        Comment,
        CommentMl,
        CommentMlExit,

        //curlies
        ParenOp,
        ParenCl,
        BracesOp,
        BracesCl,

        //misc
        Unknown
    }
}