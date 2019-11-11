using System.Collections.Generic;
using System.Linq;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    //NODE STATEMENTBLOCK
    public interface IStatement : INode
    {
    }

    public class StmtIf : IStatement
    {
        private readonly List<Branch> branches;
        private readonly StatementBlock elseBody;

        public StmtIf(List<Branch> branches, StatementBlock elseBody)
        {
            this.branches = branches;
            this.elseBody = elseBody;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("branch", branches);
            p.Print("else body", elseBody);
        }
    }

    public class StatementBlock : IStatement
    {
        private readonly List<IStatement> statements;

        public StatementBlock(List<IStatement> statements)
        {
            this.statements = statements;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("statement", statements);
        }
    }

    public class Branch : IStatement
    {

        public Branch(IExpression condition, StatementBlock body)
        {
            Condition = condition;
            Body = body;
        }

        public IExpression Condition { get; }
        public StatementBlock Body { get; }

        public void PrintNode(AstPrinter p)
        {
            p.Print("Cond", Condition);
            p.Print("Body", Body);
        }
    }

    public class StmtKeywordExpr : IStatement
    {
        private readonly Token kw;
        private readonly IExpression expr;

        public StmtKeywordExpr(Token kw, IExpression expr = null)
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
        //private readonly Keyword kw; //TOKEN
        private readonly Token kw;

        public StmtKeyword(Token kw)
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
        private readonly StatementBlock body;
        private readonly IExpression condition;

        public StmtWhile(IExpression condition, StatementBlock body)
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

    public class StmtFnCall : IStatement //TO-EXPRESSION
    {
        private readonly IExpression fnCall;

        public StmtFnCall(ExprFnCall fnCall)
        {
            this.fnCall = fnCall;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("call", fnCall);
        }
    }
}