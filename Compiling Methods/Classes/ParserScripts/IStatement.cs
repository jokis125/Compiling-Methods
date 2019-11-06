using System.Collections.Generic;
using System.Linq;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface IStatement : INode
    {
    }

    public class StmtIf : IStatement
    {
        private readonly List<Branch> branches;
        private readonly List<IStatement> elseBody;

        public StmtIf(List<Branch> branches, List<IStatement> elseBody)
        {
            this.branches = branches;
            this.elseBody = elseBody;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("if expr", branches[0].Condition);
            p.Print("if body", branches[0].Body);
            foreach (var branch in branches.Skip(1))
            {
                p.Print("elif expr", branch.Condition);
                p.Print("elif body", branch.Body);
            }
            p.Print("else body", elseBody);
        }
    }

    public class Branch
    {

        public Branch(IExpression condition, List<IStatement> body)
        {
            Condition = condition;
            Body = body;
        }

        public IExpression Condition { get; }
        public List<IStatement> Body { get; }

    }

    public class StmtKeywordExpr : IStatement
    {
        private readonly Keyword kw;
        private readonly IExpression expr;
        
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
        private readonly Keyword kw;

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