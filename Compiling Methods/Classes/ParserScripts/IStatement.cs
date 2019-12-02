using System;
using System.Collections.Generic;
using System.Linq;
using CompilingMethods.Classes.Compiler;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    //NODE STATEMENTBLOCK
    public abstract class IStatement : Node
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

        public override void PrintNode(AstPrinter p)
        {
            p.Print("branch", branches);
            p.Print("else body", elseBody);
        }

        public override void ResolveNames(Scope scope)
        {
            branches.ForEach(branch => branch.ResolveNames(scope));
            elseBody?.ResolveNames(scope);
        }

        public override TypePrim CheckTypes()
        {
            branches.ForEach(branch => branch.CheckTypes());
            elseBody?.CheckTypes();
            return new TypePrim(null, PrimType.Void);
        }
    }

    public class StmtBlock : IStatement
    {
        private readonly List<IStatement> statements;

        public StmtBlock(List<IStatement> statements)
        {
            this.statements = statements;
        }

        public override void PrintNode(AstPrinter p)
        {
            p.Print("statement", statements);
        }

        public override void ResolveNames(Scope parentScope)
        {
            var scope = new Scope(parentScope);
            statements.ForEach(statement => statement.ResolveNames(scope));
        }

        public override TypePrim CheckTypes()
        {
            statements.ForEach(statement => statement.CheckTypes());
            return new TypePrim(new Token(TokenType.Void, null, 0));
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

        public override void PrintNode(AstPrinter p)
        {
            p.Print("Cond", Condition);
            p.Print("Body", Body);
        }

        public override void ResolveNames(Scope scope)
        {
            Condition.ResolveNames(scope);
            Body.ResolveNames(scope);
        }

        public override TypePrim CheckTypes()
        {
            var condType = Condition.CheckTypes();
            TypeHelper.UnifyTypes(new TypePrim(null, PrimType.Bool), condType);
            Body.CheckTypes();
            return new TypePrim(null, PrimType.Void);
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

        public override void PrintNode(AstPrinter p)
        {
            p.Print("keyword", kw);
            p.Print("expr", expr);
        }

        public override void ResolveNames(Scope scope)
        {
            expr?.ResolveNames(scope);
        }

        public override TypePrim CheckTypes()
        {
            var returnType = (FindAncestor(typeof(DeclFn)) as DeclFn)?.Type;
            var valueType = expr.CheckTypes();
            TypeHelper.UnifyTypes(returnType, valueType);
            return new TypePrim(null, PrimType.Void);
        }
    }

    public class StmtBreak : IStatement
    {
        private readonly Token kw;
        private Node parent;

        public StmtBreak(Token kw)
        {
            this.kw = kw;
        }

        public override void PrintNode(AstPrinter p)
        {
            p.Print("keyword", kw);
        }

        public override void ResolveNames(Scope scope)
        {
            throw new System.NotImplementedException();
        }

        public override TypePrim CheckTypes()
        {
            //donothing
            return new TypePrim(null, PrimType.Void);
        }
    }
    
    public class StmtContinue : IStatement
    {
        private readonly Token kw;

        public StmtContinue(Token kw)
        {
            this.kw = kw;
        }

        public override void PrintNode(AstPrinter p)
        {
            p.Print("keyword", kw);
        }

        public override void ResolveNames(Scope scope)
        {
            throw new System.NotImplementedException();
        }

        public override TypePrim CheckTypes()
        {
            //do nothing
            return new TypePrim(null, PrimType.Void);
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

        public override void PrintNode(AstPrinter p)
        {
            p.Print("cond", condition);
            p.Print("body", body);
        }

        public override void ResolveNames(Scope scope)
        {
            condition.ResolveNames(scope);
            body.ResolveNames(scope);
        }

        public override TypePrim CheckTypes()
        {
            var condType = condition.CheckTypes();
            TypeHelper.UnifyTypes(new TypePrim(null, PrimType.Bool), condType);
            body.CheckTypes();
            return new TypePrim(null, PrimType.Void);
        }
    }

    public class StmtVar : IStatement
    {
        private readonly Token ident;
        private readonly TypePrim type;
        private readonly IExpression value;
        private int stackSlot;

        public TypePrim Type => type;

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

        public override void PrintNode(AstPrinter p)
        {
            p.Print("type", type);
            p.Print("name", ident);
            p.Print("value", value);
        }

        public override void ResolveNames(Scope scope)
        {
            stackSlot = GlobalVars.StackSlotIndex++;
            scope.Add(ident, this);
            value?.ResolveNames(scope);
        }

        public override TypePrim CheckTypes()
        {
            var exprType = value.CheckTypes();
            TypeHelper.UnifyTypes(exprType, type);
            return new TypePrim(null, PrimType.Void);
        }
    }

    public class StmtVarAssign : IStatement
    {
        private readonly Token ident;
        private readonly TokenType op;
        private readonly IExpression value;
        public Node TargetNode { get; set; }

        
        public StmtVarAssign(Token ident, TokenType op, IExpression value)
        {
            this.ident = ident;
            this.op = op;
            this.value = value;
        }

        public override void PrintNode(AstPrinter p)
        {
            p.Print("name", ident);
            p.Print("value", value);
        }

        public override void ResolveNames(Scope scope)
        {
            TargetNode = scope.ResolveName(ident);
            value.ResolveNames(scope);
        }
        
        public override TypePrim CheckTypes()
        {
            throw new System.NotImplementedException();
        }
    }

    public class StmtFnCall : IStatement //TO-EXPRESSION
    {
        private readonly IExpression fnCall;
        public Node TargetNode { get; set; }

        public StmtFnCall(ExprFnCall fnCall)
        {
            this.fnCall = fnCall;
        }

        public override void PrintNode(AstPrinter p)
        {
            p.Print("call", fnCall);
        }

        public override void ResolveNames(Scope scope)
        {
            TargetNode = scope.ResolveName(fnCall.GetToken());
            fnCall.ResolveNames(scope);
        }

        public override TypePrim CheckTypes()
        {
            return fnCall.CheckTypes();
        }
    }
}