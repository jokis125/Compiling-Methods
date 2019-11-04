using System.Collections.Generic;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface IExpression : INode
    {
    }

    public class ExprLit : IExpression
    {
        public ExprLit(Token lit)
        {
            Lit = lit;
        }

        public Token Lit { get; set; }

        public void PrintNode(AstPrinter p)
        {
            p.Print("lit", Lit);
        }
    }

    public class ExprBin : IExpression
    {
        private readonly IExpression left;
        private readonly TokenType op;
        private readonly IExpression right;

        public ExprBin(TokenType op, IExpression left, IExpression right)
        {
            this.op = op;
            this.left = left;
            this.right = right;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("op", new Token(op, "", 1));
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