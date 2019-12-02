using System.Collections.Generic;
using CompilingMethods.Classes.Compiler;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public abstract class IType : Node
    {
        public abstract bool IsArithmetic();
        public abstract bool IsComparable();
        public abstract bool HasValue();
    }

    public class TypePrim : IType 
    {
        private Token token;
        private readonly PrimType kind;

        public TypePrim(Token token)
        {
            this.token = token;
            this.kind = token.State switch
            {
                TokenType.Int => PrimType.Int,
                TokenType.LitInt => PrimType.Int,
                TokenType.Char => PrimType.Char,
                TokenType.Float => PrimType.Float,
                TokenType.LitFloat => PrimType.Float,
                TokenType.String => PrimType.String,
                TokenType.LitStr => PrimType.String,
                TokenType.Boolean => PrimType.Bool,
                TokenType.Void => PrimType.Void,
                _ => this.kind
            };
        }

        public TypePrim(Token token, PrimType type)
        {
            this.token = token;
            kind = type;
        }

        public Token Token => token;

        public PrimType Kind => kind;

        public override bool IsArithmetic()
        {
            return kind == PrimType.Int || kind == PrimType.Float;
        }

        public override bool IsComparable()
        {
            return IsArithmetic();
        }

        public override bool HasValue()
        {
            return kind != PrimType.Void;
        }
        
        public override void PrintNode(AstPrinter p)
        {
            p.Print("kind", kind);
        }

        public override void ResolveNames(Scope scope)
        {
            throw new System.NotImplementedException();
        }

        public override TypePrim CheckTypes()
        {
            throw new System.NotImplementedException();
        }
    }
}