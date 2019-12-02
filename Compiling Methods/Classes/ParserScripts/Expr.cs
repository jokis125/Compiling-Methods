using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CompilingMethods.Classes.Compiler;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface IExpression : INode
    {
        Token GetToken();
        TokenType GetTokenType();
    }

    public class ExprVar : IExpression
    {
        public ExprVar(Token lit)
        {
            Lit = lit;
        }

        private Token Lit { get; }

        public void PrintNode(AstPrinter p)
        {
            p.Print("Var", Lit);
        }

        public void ResolveNames(Scope scope)
        {
            var targetNode = scope.ResolveName(Lit);
        }

        public TypePrim CheckTypes()
        {
            return new TypePrim(Lit);
        }

        public TokenType GetTokenType()
        {
            return Lit.State;
        }

        public Token GetToken()
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

        public void PrintNode(AstPrinter p)
        {
            p.Print("const", constant.Value.ToString());
        }

        public void ResolveNames(Scope scope)
        {
            //do nothing
        }

        public TypePrim CheckTypes()
        {
            switch (constant.State)
            {
                case TokenType.LitInt:
                    return new TypePrim(constant);
                case TokenType.LitFloat:
                    return new TypePrim(constant);
                case TokenType.LitStr:
                    return new TypePrim(constant);
                default:
                    throw new NotImplementedException();
            }
            //throw new Exception();
        }

        public Token GetToken()
        {
            return constant;
        }

        public TokenType GetTokenType()
        {
            return constant.State;
        }
    }

    public class ExprBin : IExpression
    {
        private readonly IExpression left;
        private readonly ExprBinKind op;
        private readonly IExpression right;

        public ExprBin(ExprBinKind op, IExpression left, IExpression right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("op", op);
            p.Print("left", left);
            p.Print("right", right);
        }

        public void ResolveNames(Scope scope)
        {
            left.ResolveNames(scope);
            right.ResolveNames(scope);
        }

        public Token GetToken()
        {
            return left.GetToken();
        }

        public TypePrim CheckTypes()
        {
            if (OperatorType.ReturnOperatorKind(op) == OpTypes.Arithmetic)
            {
                var leftType = left.CheckTypes();
                var rightType = right.CheckTypes();

                if (leftType.IsArithmetic())
                    TypeHelper.UnifyTypes(leftType, rightType, left.GetToken());
                else
                    TypeHelper.SemanticError(left.GetToken(), $"cannot perform arithmetic with this type: {leftType}");
                return leftType;
            }

            if (OperatorType.ReturnOperatorKind(op) == OpTypes.Comparison)
            {
                var leftType = left.CheckTypes();
                var rightType = right.CheckTypes();

                if (leftType.IsComparable())
                    TypeHelper.UnifyTypes(leftType, rightType, left.GetToken());
                else
                    TypeHelper.SemanticError(left.GetToken(), $"cannot compare of this type: {leftType}");
                return new TypePrim(new Token(TokenType.Boolean, false, left.GetToken().LineN));
            }

            if (OperatorType.ReturnOperatorKind(op) == OpTypes.Equality)
            {
                var leftType = left.CheckTypes();
                var rightType = right.CheckTypes();

                if (leftType.HasValue())
                    TypeHelper.UnifyTypes(leftType, rightType, left.GetToken());
                else
                    TypeHelper.SemanticError(left.GetToken(), $"cannot compare void values");
                return new TypePrim(new Token(TokenType.Boolean, false, 0));
            }

            if (OperatorType.ReturnOperatorKind(op) == OpTypes.Logic)
            {
                var boolType = new TypePrim(new Token(TokenType.Boolean, false, 0));
                var leftType = left.CheckTypes();
                var rightType = right.CheckTypes();
                TypeHelper.UnifyTypes(leftType, boolType, left.GetToken());
                TypeHelper.UnifyTypes(rightType, boolType, left.GetToken());
                return boolType;
            }

            throw new SystemException();
        }

        public TokenType GetTokenType()
        {
            return left.GetTokenType();
        }
    }

    class ExprBinaryArithmetic : ExprBin
    {
        public ExprBinaryArithmetic(ExprBinKind op, IExpression left, IExpression right) : base(op, left, right)
        {
        }
    }
    
    class ExprBinaryComparison : ExprBin
    {
        public ExprBinaryComparison(ExprBinKind op, IExpression left, IExpression right) : base(op, left, right)
        {
        }
    }
    
    class ExprBinaryEquality : ExprBin
    {
        public ExprBinaryEquality(ExprBinKind op, IExpression left, IExpression right) : base(op, left, right)
        {
        }
    }
    
    class ExprBinaryLogic : ExprBin
    {
        public ExprBinaryLogic(ExprBinKind op, IExpression left, IExpression right) : base(op, left, right)
        {
        }
    }
    

    public class ExprFnCall : IExpression
    {
        private readonly List<IExpression> args;
        private readonly Token ident;

        public ExprFnCall(Token ident, List<IExpression> args)
        {
            this.ident = ident;
            this.args = args;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("ident", ident);
            p.Print("args", args);
        }

        public Token GetToken()
        {
            return ident;
        }

        public void ResolveNames(Scope scope)
        {
            var targetNode = scope.ResolveName(ident);
            foreach (var arg in args)
            {
                arg.ResolveNames(scope);
            }
        }

        public TokenType GetTokenType()
        {
            return ident.State;
        }

        public TypePrim CheckTypes()
        {
            throw new NotImplementedException();
        }
    }
}