using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Xsl;
using CompilingMethods.Classes.Compiler;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public abstract class IExpression : Node
    {
        public abstract Token GetToken();
        public abstract TokenType GetTokenType();
    }

    public class ExprVar : IExpression, ITargetNode
    {
        public ExprVar(Token lit)
        {
            Lit = lit;
        }

        private Token Lit { get; }
        public Node TargetNode { get; set; }

        public override void PrintNode(AstPrinter p)
        {
            p.Print("Var", Lit);
        }

        public override void ResolveNames(Scope scope)
        {
            TargetNode = scope.ResolveName(Lit);
        }

        public override TypePrim CheckTypes()
        {
            TypePrim type = null;
            if (TargetNode?.GetType() == typeof(Param))
                type = (TargetNode as Param).Type;
            else if (TargetNode?.GetType() == typeof(StmtVar))
                type = (TargetNode as StmtVar).Type;
            return type;
        }

        public override TokenType GetTokenType()
        {
            return Lit.State;
        }

        public override Token GetToken()
        {
            return Lit;
        }

        public override void GenCode(CodeWriter w)
        {
            if(TargetNode.GetType().GetField("stackSlot") != null)
                w.Write(Instructions.GetL); //TODO @target_node.stack_slot
            else if (TargetNode.GetType().GetField("globalSlot") != null)
                w.Write(Instructions.GetG); //TODO @target_node.global_slot;
            else
                throw new SystemException("bad variable");
        }
    }

    public class ExprConst : IExpression
    {
        private Token constant;

        public ExprConst(Token constant)
        {
            this.constant = constant;
        }

        public override void PrintNode(AstPrinter p)
        {
            p.Print("const", constant.Value.ToString());
        }

        public override void ResolveNames(Scope scope)
        {
            //do nothing
        }

        public override TypePrim CheckTypes()
        {
            switch (constant.State)
            {
                case TokenType.LitInt:
                    return new TypePrim(constant, PrimType.@int);
                case TokenType.LitFloat:
                    return new TypePrim(constant, PrimType.@float);
                case TokenType.LitStr:
                    return new TypePrim(constant, PrimType.@string);
                case var state when state == TokenType.False || state == TokenType.True:
                    return new TypePrim(constant, PrimType.@bool);
                default:
                    throw new NotImplementedException($"check types {constant.State}");
            }
            //throw new Exception();
        }

        public override Token GetToken()
        {
            return constant;
        }

        public override TokenType GetTokenType()
        {
            return constant.State;
        }

        public override void GenCode(CodeWriter w)
        {
            switch (constant.State)
            {
                case TokenType.LitInt:
                    w.Write(Instructions.IntPush, constant.Value);
                    break;
                default: 
                    throw new SystemException("bad literal");
            }
        }
    }

    public class ExprBin : IExpression
    {
        protected readonly IExpression left;
        private readonly ExprBinKind op;
        protected readonly IExpression right;

        public ExprBin(ExprBinKind op, IExpression left, IExpression right)
        {
            AddChildren(left, right);
            this.left = left;
            this.op = op;
            this.right = right;
        }

        public override void PrintNode(AstPrinter p)
        {
            p.Print("op", op);
            p.Print("left", left);
            p.Print("right", right);
        }

        public override void ResolveNames(Scope scope)
        {
            left.ResolveNames(scope);
            right.ResolveNames(scope);
        }

        public override Token GetToken()
        {
            return left.GetToken();
        }

        public override TypePrim CheckTypes()
        {
            throw new InvalidOperationException("This should not be created");
        }

        public override TokenType GetTokenType()
        {
            return left.GetTokenType();
        }

        public override void GenCode(CodeWriter w)
        {
            left.GenCode(w);
            right.GenCode(w);

            switch (op)
            {
                case ExprBinKind.Add:
                    w.Write(Instructions.IntAdd);
                    break;
                case ExprBinKind.Sub:
                    w.Write(Instructions.IntSub);
                    break;
                case ExprBinKind.Mul:
                    w.Write(Instructions.IntMul);
                    break;
                case ExprBinKind.Div:
                    w.Write(Instructions.IntDiv);
                    break;
                case ExprBinKind.Less:
                    w.Write(Instructions.IntLess);
                    break;
                case ExprBinKind.Equal:
                    w.Write(Instructions.IntEqual);
                    break;
                default:
                    throw new NotImplementedException($"{op} not implemented");
            }
        }
    }

    class ExprBinaryArithmetic : ExprBin
    {
        public ExprBinaryArithmetic(ExprBinKind op, IExpression left, IExpression right) : base(op, left, right)
        {
        }

        public override TypePrim CheckTypes()
        {
            var leftType = left.CheckTypes();
            var rightType = right.CheckTypes();

            if (!leftType.IsArithmetic())
            {
                
                TypeHelper.SemanticError(left.GetToken(), $"{leftType.Kind} is not arithmetic");
                return rightType;
            }
            if (!rightType.IsArithmetic())
            {
                
                TypeHelper.SemanticError(right.GetToken(),$"{rightType.Kind} is not arithmetic");
                return leftType;
            }
                
            TypeHelper.UnifyTypes(leftType, rightType);
            return leftType;
        }
    }
    
    class ExprBinaryComparison : ExprBin
    {
        public ExprBinaryComparison(ExprBinKind op, IExpression left, IExpression right) : base(op, left, right)
        {
        }
        
        public override TypePrim CheckTypes()
        {
            var leftType = left.CheckTypes();
            var rightType = right.CheckTypes();
            
            if(!leftType.IsComparable() || !rightType.IsComparable())
                TypeHelper.SemanticError(left.GetToken().LineN > right.GetToken().LineN ? left.GetToken() : right.GetToken(), $"{leftType.Kind} is not comparable with {rightType.Kind}");
            TypeHelper.UnifyTypes(leftType, rightType);
            return new TypePrim(new Token(TokenType.Boolean, null, left.GetToken().LineN));
        }
    }
    
    class ExprBinaryEquality : ExprBin
    {
        public ExprBinaryEquality(ExprBinKind op, IExpression left, IExpression right) : base(op, left, right)
        {
        }
        
        public override TypePrim CheckTypes()
        {
            var leftType = left.CheckTypes();
            var rightType = right.CheckTypes();
            
            if(!leftType.HasValue() || !rightType.HasValue())
                TypeHelper.SemanticError(left.GetToken().LineN > right.GetToken().LineN ? left.GetToken() : right.GetToken(), $"{leftType.Kind} or {rightType.Kind} do not have value");
            TypeHelper.UnifyTypes(leftType, rightType);
            return new TypePrim(new Token(TokenType.Boolean, null, left.GetToken().LineN));
        }
    }
    
    class ExprBinaryLogic : ExprBin
    {
        public ExprBinaryLogic(ExprBinKind op, IExpression left, IExpression right) : base(op, left, right)
        {
        }

        public override TypePrim CheckTypes()
        {
            var leftType = left.CheckTypes();
            var rightType = right.CheckTypes();
            
            TypeHelper.UnifyTypes(leftType, new TypePrim(new Token(TokenType.Boolean, null, left.GetToken().LineN)));
            TypeHelper.UnifyTypes(rightType, new TypePrim(new Token(TokenType.Boolean, null, left.GetToken().LineN)));

            return new TypePrim(new Token(TokenType.Boolean, null, left.GetToken().LineN));
        }
    }
    

    public class ExprFnCall : IExpression, ITargetNode
    {
        private readonly List<IExpression> args;
        private readonly Token ident;

        public ExprFnCall(Token ident, List<IExpression> args)
        {
            AddChildren(args.ToArray());
            this.ident = ident;
            this.args = args;
        }

        public Node TargetNode { get; set; }

        public override void PrintNode(AstPrinter p)
        {
            p.Print("ident", ident);
            p.Print("args", args);
        }

        public override Token GetToken()
        {
            return ident;
        }

        public override void ResolveNames(Scope scope)
        {
            TargetNode = scope.ResolveName(ident);
            foreach (var arg in args)
            {
                arg.ResolveNames(scope);
            }
        }

        public override TokenType GetTokenType()
        {
            return ident.State;
        }

        public override TypePrim CheckTypes()
        {
            var argTypes = args.Select(x => x.CheckTypes()).ToList();

            if (TargetNode == null)
                return null;
            else if (TargetNode.GetType() != typeof(DeclFn))
            {
                TypeHelper.SemanticError(ident, "Not a function");
                return null;
            }

            var paramTypes = (TargetNode as DeclFn).Parameters.Select(x => x.Type).ToList();
            if (argTypes.Count != paramTypes.Count)
            {
                TypeHelper.SemanticError(ident, "Invalid function argument count");
                return null;
            }

            if (argTypes.Any(x => x == null) || paramTypes.Any(x => x == null))
            {
                TypeHelper.SemanticError(ident, $"Argument {ident.Value} is not assignable to parameter");
            }

            var count = argTypes.Count < paramTypes.Count ? argTypes.Count : paramTypes.Count;

            for (var i = 0; i < count; i++)
            {
                var paramType = paramTypes[i];
                var argType = argTypes[i];
                TypeHelper.UnifyTypes(paramType, argType);
            }

            return (TargetNode as DeclFn).Type;
        }

        public override void GenCode(CodeWriter w)
        {
            w.Write(Instructions.CallBegin);
            foreach (var expression in args)
            {
                expression.GenCode(w);
            }
            w.Write(Instructions.Call, args.Count); //@targetnode,startlabel??????????
        }
    }
}