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
                    return new TypePrim(constant, PrimType.Int);
                case TokenType.LitFloat:
                    return new TypePrim(constant, PrimType.Float);
                case TokenType.LitStr:
                    return new TypePrim(constant, PrimType.String);
                case var state when state == TokenType.False || state == TokenType.True:
                    return new TypePrim(constant, PrimType.Bool);
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
    }

    public class ExprBin : IExpression
    {
        protected readonly IExpression left;
        private readonly ExprBinKind op;
        protected readonly IExpression right;

        public ExprBin(ExprBinKind op, IExpression left, IExpression right)
        {
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

            if (!leftType.IsArithmetic() || !rightType.IsArithmetic())
            {
                
                TypeHelper.SemanticError(left.GetToken().LineN > right.GetToken().LineN ? left.GetToken() : right.GetToken(),
                    $"{leftType.Kind} or {rightType.Kind} is not arithmetic");
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
    }
}