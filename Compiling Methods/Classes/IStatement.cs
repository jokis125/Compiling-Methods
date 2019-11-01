using System.Collections.Generic;

namespace CompilingMethods.Classes
{
    public interface IStatement : INode
    {
        
    }

    public class StmtIf : IStatement
    {
        private IExpression condition;
        private List<IStatement> body;

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
        private List<IStatement> body;

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
        private INode value;

        public StmtRet(INode value)
        {
            this.value = value;
        }

        public void PrintNode(AstPrinter p)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class StmtWhile : IStatement
    {
        private IExpression condition;
        private List<IStatement> body;

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
        private Token type;
        private Token ident;
        private IExpression value;

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
        private Token ident;
        private IExpression value;


        public StmtVarAssign(Token ident, IExpression value)
        {
            this.ident = ident;
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
        private Token ident;
        private List<IExpression> args;

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