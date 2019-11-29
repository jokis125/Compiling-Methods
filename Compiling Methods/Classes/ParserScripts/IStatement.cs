using System;
using System.Collections.Generic;
using System.Linq;
using CompilingMethods.Classes.Compiler;
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
        private readonly StmtBlock elseBody;

        public StmtIf(List<Branch> branches, StmtBlock elseBody)
        {
            this.branches = branches;
            this.elseBody = elseBody;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("branch", branches);
            p.Print("else body", elseBody);
        }

        public void ResolveNames(Scope scope)
        {
            branches.ForEach(branch => branch.ResolveNames(scope));
        }

        public TypePrim CheckTypes()
        {
            branches.ForEach(branch => branch.CheckTypes());
            if (elseBody != null) 
                return elseBody.CheckTypes();
            return null;
        }
    }

    public class StmtBlock : IStatement
    {
        private readonly List<IStatement> statements;

        public StmtBlock(List<IStatement> statements)
        {
            this.statements = statements;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("statement", statements);
        }

        public void ResolveNames(Scope parentScope)
        {
            var scope = new Scope(parentScope);
            statements.ForEach(statement => statement.ResolveNames(scope));
        }

        public TypePrim CheckTypes()
        {
            statements.ForEach(statement => statement.CheckTypes());
            return null;
        }
    }

    public class Branch : IStatement
    {

        public Branch(IExpression condition, StmtBlock body)
        {
            Condition = condition;
            Body = body;
        }

        public IExpression Condition { get; }
        public StmtBlock Body { get; }

        public void PrintNode(AstPrinter p)
        {
            p.Print("Cond", Condition);
            p.Print("Body", Body);
        }

        public void ResolveNames(Scope scope)
        {
            Condition.ResolveNames(scope);
            Body.ResolveNames(scope);
        }

        public TypePrim CheckTypes()
        {
            var condType = Condition.CheckTypes();
            //TypeHelper.UnifyTypes(new TypePrim(new Token(Condition.GetTokenType(), 0, 0)), new TypePrim(new Token(TokenType.Boolean, false, 0)));
            return null;
        }
    }

    public class StmtReturn : IStatement
    {
        private readonly Token kw;
        private readonly IExpression expr;

        public StmtReturn(Token kw, IExpression expr = null)
        {
            this.kw = kw;
            this.expr = expr;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("keyword", kw);
            p.Print("expr", expr);
        }

        public void ResolveNames(Scope scope)
        {
            expr.ResolveNames(scope);
        }

        public TypePrim CheckTypes()
        {
            throw new NotImplementedException();
        }
    }

    public class StmtBreak : IStatement
    {
        private readonly Token kw;
        private INode parent;

        public StmtBreak(Token kw)
        {
            this.kw = kw;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("keyword", kw);
        }

        public void ResolveNames(Scope scope)
        {
            throw new System.NotImplementedException();
        }

        public TypePrim CheckTypes()
        {
            //donothing
            return null;
        }
    }
    
    public class StmtContinue : IStatement
    {
        private readonly Token kw;

        public StmtContinue(Token kw)
        {
            this.kw = kw;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("keyword", kw);
        }

        public void ResolveNames(Scope scope)
        {
            throw new System.NotImplementedException();
        }

        public TypePrim CheckTypes()
        {
            //do nothing
            return null;
        }
    }
    
    

    public class StmtWhile : IStatement
    {
        private readonly StmtBlock body;
        private readonly IExpression condition;

        public StmtWhile(IExpression condition, StmtBlock body)
        {
            this.condition = condition;
            this.body = body;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("cond", condition);
            p.Print("body", body);
        }

        public void ResolveNames(Scope scope)
        {
            condition.ResolveNames(scope);
            body.ResolveNames(scope);
        }

        public TypePrim CheckTypes()
        {
            var condType = condition.CheckTypes();
            TypeHelper.UnifyTypes(condType, new TypePrim(new Token(TokenType.Boolean, false, 0)));
            return body.CheckTypes();
        }
    }

    public class StmtVar : IStatement
    {
        private readonly Token ident;
        private readonly TypePrim type;
        private readonly IExpression value;
        private int stackSlot;
        

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

        public void ResolveNames(Scope scope)
        {
            stackSlot = GlobalVars.StackSlotIndex++;
            scope.Add(ident, this);
        }

        public TypePrim CheckTypes()
        {
            if (value == null) return null;
            var valueType = value.CheckTypes();
            TypeHelper.UnifyTypes(type, valueType, ident);
            
            return null;
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

        public void ResolveNames(Scope scope)
        {
            var targetNode = scope.ResolveName(ident);
            value.ResolveNames(scope);
        }
        
        public TypePrim CheckTypes()
        {
            return null;
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

        public void ResolveNames(Scope scope)
        {
            fnCall.ResolveNames(scope);
        }

        public TypePrim CheckTypes()
        {
            return fnCall.CheckTypes();
        }
    }
}