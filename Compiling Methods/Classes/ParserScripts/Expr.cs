using System.Collections.Generic;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface IExpression : INode
    {
    }

    public class ExprVar : IExpression
    {
        public ExprVar(Token lit)
        {
            Lit = lit;
        }

        private Token Lit { get; }

        public void PrintNode(AstPrinter p)
        {
            p.Print("Var", Lit);
        }
    }

    public class ExprConst : IExpression
    {
        private Token constant;

        public ExprConst(Token constant)
        {
            this.constant = constant;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("const", constant.Value.ToString());
        }
    }

    public class ExprBin : IExpression
    {
        private readonly IExpression left;
        private readonly BinExpression op;
        private readonly IExpression right;

        public ExprBin(BinExpression op, IExpression left, IExpression right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("op", op);
            p.Print("left", left);
            p.Print("right", right);
        }
    }

    public class ExprFnCall : IExpression
    {
        private readonly List<IExpression> args;
        private readonly Token ident;

        public ExprFnCall(Token ident, List<IExpression> args)
        {
            this.ident = ident;
            this.args = args;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("ident", ident);
            p.Print("args", args);
        }
    }
}