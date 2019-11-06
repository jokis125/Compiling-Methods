using System.Collections.Generic;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface IStatement : INode
    {
    }

    public class StmtElif : IStatement
    {
        private readonly List<StmtIf> elifs;
        private readonly List<IStatement> elseBody;
        private readonly StmtIf ifStmt;

        public StmtElif(StmtIf ifStmt, List<StmtIf> elifs, List<IStatement> elseBody)
        {
            this.ifStmt = ifStmt;
            this.elifs = elifs;
            this.elseBody = elseBody;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("if", ifStmt);
            p.Print("else if", elifs);
            p.Print("else", elseBody);
        }
    }

    public class StmtIf : IStatement
    {
        private readonly List<IStatement> body;
        private readonly IExpression condition;

        public StmtIf(IExpression condition, List<IStatement> body)
        {
            this.condition = condition;
            this.body = body;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("cond", condition);
            p.Print("body", body);
        }
    }

    public class StmtKeywordExpr : IStatement
    {
        private Keyword kw;
        private IExpression expr;
        
        public StmtKeywordExpr(Keyword kw, IExpression expr = null)
        {
            this.kw = kw;
            this.expr = expr;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("keyword", kw);
            p.Print("expr", expr);
        }
    }
    
    public class StmtKeyword : IStatement
    {
        private Keyword kw;

        public StmtKeyword(Keyword kw)
        {
            this.kw = kw;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("keyword", kw);
        }
    }

    public class StmtWhile : IStatement
    {
        private readonly List<IStatement> body;
        private readonly IExpression condition;

        public StmtWhile(IExpression condition, List<IStatement> body)
        {
            this.condition = condition;
            this.body = body;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("cond", condition);
            p.Print("body", body);
        }
    }

    public class StmtVar : IStatement
    {
        private readonly Token ident;
        private readonly TypePrim type;
        private readonly IExpression value;
        

        public StmtVar(TypePrim type, Token ident)
        {
            this.type = type;
            this.ident = ident;
            value = null;
        }

        public StmtVar(TypePrim type, Token ident, IExpression value)
        {
            this.type = type;
            this.ident = ident;
            this.value = value;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("type", type);
            p.Print("name", ident);
            p.Print("value", value);
        }
    }

    public class StmtVarAssign : IStatement
    {
        private readonly Token ident;
        private readonly TokenType op;
        private readonly IExpression value;


        public StmtVarAssign(Token ident, TokenType op, IExpression value)
        {
            this.ident = ident;
            this.op = op;
            this.value = value;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("name", ident);
            p.Print("value", value);
        }
    }

    public class StmtFnCall : IStatement
    {
        private readonly List<IExpression> args;
        private readonly Token ident;

        public StmtFnCall(Token ident, List<IExpression> args)
        {
            this.ident = ident;
            this.args = args;
        }

        public StmtFnCall(Token ident)
        {
            this.ident = ident;
            args = new List<IExpression>();
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("ident", ident);
            p.Print("args", args);
        }
    }
}