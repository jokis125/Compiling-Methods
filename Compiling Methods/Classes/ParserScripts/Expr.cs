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
        private Token lit;

        public ExprLit(Token lit)
        {
            this.lit = lit;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("lit", lit);
        }

        public Token Lit
        {
            get => lit;
            set => lit = value;
        }
    }

    public class ExprBin : IExpression
    {
        private TokenType op;
        private IExpression left;
        private IExpression right;

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
        private Token ident;
        private List<IExpression> args;

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