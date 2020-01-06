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
        public List<Branch> Branches { get; }

        public StmtBlock ElseBody { get; }

        public Label ElseLabel;
        public Label AfterElseLabel;

        public StmtIf(List<Branch> branches, StmtBlock elseBody)
        {
            var temp = new Node[branches.Count + 1];
            temp[branches.Count] = elseBody;
            
            AddChildren(branches.ToArray());
            AddChildren(elseBody);
            this.Branches = branches;
            this.ElseBody = elseBody;
            
            ElseLabel = new Label();
            AfterElseLabel = new Label();
            
        }

        public override void PrintNode(AstPrinter p)
        {
            p.Print("branch", Branches);
            p.Print("else body", ElseBody);
        }

        public override void ResolveNames(Scope scope)
        {
            Branches.ForEach(branch => branch.ResolveNames(scope));
            ElseBody?.ResolveNames(scope);
        }

        public override TypePrim CheckTypes()
        {
            Branches.ForEach(branch => branch.CheckTypes());
            ElseBody?.CheckTypes();
            return new TypePrim(null, PrimType.@void);
        }

        public override void GenCode(CodeWriter w)
        {
            Branches.ForEach(branch => branch.GenCode(w));
            ElseBody?.GenCode(w);
        }
    }

    public class StmtBlock : IStatement
    {
        private readonly List<IStatement> statements;

        public StmtBlock(List<IStatement> statements)
        {
            AddChildren(statements.ToArray());
            this.statements = statements;
            if (statements.Any())
                locationToken = statements[0].locationToken;
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

        public override void GenCode(CodeWriter w)
        {
            if(parent is StmtIf par)
                w.PlaceLabel(par.ElseLabel);
            statements.ForEach(s => s.GenCode(w));
            if(parent is StmtIf par2)
                w.PlaceLabel(par2.AfterElseLabel);
        }
    }

    public class Branch : Node
    {

        public IExpression Condition { get; }
        public StmtBlock Body { get; }

        public Label EndLabel;
        public Branch(IExpression condition, StmtBlock body)
        {
            Condition = condition;
            Body = body;
            
            EndLabel = new Label();
        }


        public override void PrintNode(AstPrinter p)
        {
            
            AddChildren(Condition);
            AddChildren(Body);
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
            TypeHelper.UnifyTypes(new TypePrim(null, PrimType.@bool), condType);
            Body.CheckTypes();
            return new TypePrim(null, PrimType.@void);
        }

        public override void GenCode(CodeWriter w)
        {
            Condition.GenCode(w);
            if (parent is StmtIf ifNode)
            {
                if (ifNode.ElseBody != null && ifNode.Branches[^1] == this)
                {
                    w.Write(Instructions.Bz, ifNode.ElseLabel);
                }
                else if (ifNode.ElseBody == null && ifNode.Branches[^1] == this)
                {
                    w.Write(Instructions.Bz, ifNode.AfterElseLabel);
                }
                else
                {
                    w.Write(Instructions.Bz, EndLabel);
                }
            }
            
            Body.GenCode(w);

            if (parent is StmtIf ifNode2)
            {
                if (ifNode2.ElseBody != null && ifNode2.Branches[^1] == this)
                {
                    w.Write(Instructions.Br, ifNode2.AfterElseLabel);
                }
                else if (ifNode2.ElseBody == null && ifNode2.Branches[^1] == this)
                {
                    w.PlaceLabel(ifNode2.AfterElseLabel);
                }
                else
                {
                    w.Write(Instructions.Br, ifNode2.AfterElseLabel);
                    w.PlaceLabel(EndLabel);
                }
            }

        }
        
    }

    public class StmtReturn : IStatement
    {
        private readonly Token kw;
        private readonly IExpression expr;

        public StmtReturn(Token kw, IExpression expr = null)
        {
            AddChildren(expr);
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
            var valueType = expr != null ? expr.CheckTypes() : new TypePrim(new Token(TokenType.Void, kw.Value, kw.LineN));
            TypeHelper.UnifyTypes(returnType, valueType);
            return new TypePrim(null, PrimType.@void);
        }

        public override void GenCode(CodeWriter w)
        {
            if (expr == null)
            {
                w.Write(Instructions.Ret);
                return;
            }
            expr.GenCode(w);
            w.Write(Instructions.RetV);
        }
    }

    public class StmtBreak : IStatement
    {
        private readonly Token kw;
        private Node TargetNode { get; set; }

        public Label endLabel;

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
            Node targetNode = null;
            var currNode = parent;
            while (currNode != null)
            {
                if (currNode.GetType() == typeof(StmtWhile))
                {
                    targetNode = currNode;
                    break;
                }
                currNode = currNode.parent;

            }

            if (targetNode == null)
            {
                Console.WriteLine($"{GlobalVars.FileName}:{kw.LineN}: error: Break not inside a loop");
            }

            var program = FindAncestor(typeof(StmtWhile));
            if (program != null)
                endLabel = (program as StmtWhile)?.endLabel;
        }
        

        public override TypePrim CheckTypes()
        {
            //donothing
            return new TypePrim(null, PrimType.@void);
        }

        public override void GenCode(CodeWriter w)
        {
            w.Write(Instructions.Br, endLabel);
        }
    }
    
    public class StmtContinue : IStatement
    {
        private readonly Token kw;
        private Node TargetNode { get; set; }
        public Label startLabel;

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
            var currNode = parent;
            Node targetNode = null;
            while (currNode != null)
            {
                if (currNode.GetType() == typeof(StmtWhile))
                {
                    targetNode = currNode;
                    break;
                }
                currNode = currNode.parent;
            }

            if (targetNode == null)
            {
                Console.WriteLine($"{GlobalVars.FileName}:{kw.LineN}: error: Continue not inside a loop");
            }
            
            var program = FindAncestor(typeof(StmtWhile));
            if (program != null)
                startLabel = (program as StmtWhile)?.startLabel;
        }

        public override TypePrim CheckTypes()
        {
            //do nothing
            return new TypePrim(null, PrimType.@void);
        }
        
        public override void GenCode(CodeWriter w)
        {
            w.Write(Instructions.Br, startLabel); //TODO :LOOP_END
        }
    }
    
    

    public class StmtWhile : IStatement
    {
        private readonly StmtBlock body;
        private readonly IExpression condition;

        public Label startLabel;
        public Label endLabel;

        public StmtWhile(IExpression condition, StmtBlock body)
        {
            AddChildren(condition, body);
            this.condition = condition;
            this.body = body;
            
            startLabel = new Label();
            endLabel = new Label();
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
            TypeHelper.UnifyTypes(new TypePrim(null, PrimType.@bool), condType);
            body.CheckTypes();
            return new TypePrim(null, PrimType.@void);
        }
        
        public override void GenCode(CodeWriter w)
        {
            //var startL = new Label();
            //var endL = new Label();
            w.PlaceLabel(startLabel);
            condition.GenCode(w);
            w.Write(Instructions.Bz, endLabel);
            body.GenCode(w);
            w.Write(Instructions.Br, startLabel);
            w.PlaceLabel(endLabel);
        }
    }

    public class StmtVar : IStatement, IStackSlot
    {
        private readonly Token ident;
        private readonly TypePrim type;
        private readonly IExpression value;
        public int StackSlot { get; set; }

        public TypePrim Type => type;

        public StmtVar(TypePrim type, Token ident)
        {
            AddChildren(type);
            this.type = type;
            this.ident = ident;
            value = null;
        }

        public StmtVar(TypePrim type, Token ident, IExpression value)
        {
            AddChildren(type, value);
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
            StackSlot = Scope.StackSlotIndex;
            Scope.StackSlotIndex++;
            value?.ResolveNames(scope);
            scope.Add(ident, this);
        }

        public override TypePrim CheckTypes()
        {
            var exprType = value?.CheckTypes();
            TypeHelper.UnifyTypes(type, exprType);
            return new TypePrim(null, PrimType.@void);
        }

        public override void GenCode(CodeWriter w)
        {
            value?.GenCode(w);
            w.Write(Instructions.SetL, StackSlot);
        }
    }

    public class StmtVarAssign : IStatement, IStackSlot
    {
        private readonly Token ident;
        private readonly TokenType op;
        private readonly IExpression value;
        public Node TargetNode { get; set; }
        public int StackSlot { get; set; }

        
        public StmtVarAssign(Token ident, TokenType op, IExpression value)
        {
            AddChildren(value);
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
            var exprType = value.CheckTypes();
            return new TypePrim(null, PrimType.@void);
        }
        
        public override void GenCode(CodeWriter w)
        {
            value.GenCode(w);
            
            if(TargetNode is StmtVar decl)
                w.Write(Instructions.SetL, decl.StackSlot);
            else
                throw new Exception("var assign bad pls send help");
            
        }
    }

    public class StmtFnCall : IStatement //TO-EXPRESSION
    {
        private readonly IExpression fnCall;
        public Node TargetNode { get; set; }

        public StmtFnCall(ExprFnCall fnCall)
        {
            AddChildren(fnCall);
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

        public override void GenCode(CodeWriter w)
        {
            if (fnCall.GetToken().Value is string name)
            {
                if (name == "printInt")
                {
                    (fnCall as ExprFnCall)?.Args.ForEach(arg => arg.GenCode(w));
                    w.Write(Instructions.Print);
                    return;
                }
                else if (name == "readInt")
                {
                    (fnCall as ExprFnCall)?.Args.ForEach(arg => arg.GenCode(w));
                    w.Write(Instructions.Read);
                    return;
                }
                if (name == "printString")
                {
                    (fnCall as ExprFnCall)?.Args.ForEach(arg => arg.GenCode(w));
                    w.Write(Instructions.PrintString);
                    return;
                }
                else if (name == "printFloat")
                {
                    (fnCall as ExprFnCall)?.Args.ForEach(arg => arg.GenCode(w));
                    w.Write(Instructions.PrintFloat);
                    return;
                }
            }
            w.Write(Instructions.CallBegin);
            fnCall.GenCode(w);
            w.Write(Instructions.Pop);
        }
    }
}