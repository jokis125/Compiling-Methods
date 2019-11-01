using System.Collections.Generic;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface IStatement : INode
    {
        
    }

    public class StmtIf : IStatement
    {
        private readonly IExpression condition;
        private readonly List<IStatement> body;

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
    
    public class StmtElse : IStatement
    {
        private readonly List<IStatement> body;

        public StmtElse( List<IStatement> body)
        {
            this.body = body;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("body", body);
        }
    }

    public class StmtRet : IStatement
    {
        private readonly IExpression value;

        public StmtRet(IExpression value)
        {
            this.value = value;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("value", value);
        }
    }
    
    public class StmtBreak : IStatement
    {
        public void PrintNode(AstPrinter p)
        {
            p.Print("break", null);
        }
    }
    
    public class StmtWhile : IStatement
    {
        private readonly IExpression condition;
        private readonly List<IStatement> body;

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
        private readonly Token type;
        private readonly Token ident;
        private readonly IExpression value;

        public StmtVar(Token type, Token ident)
        {
            this.type = type;
            this.ident = ident;
            value = null;
        }

        public StmtVar(Token type, Token ident, IExpression value)
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
            p.Print("operator", op);
            p.Print("value", value);
        }
    }
    
    public class StmtFnCall : IStatement
    {
        private readonly Token ident;
        private readonly List<IExpression> args;

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