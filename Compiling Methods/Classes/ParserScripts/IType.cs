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
                TokenType.Int => PrimType.@int,
                TokenType.LitInt => PrimType.@int,
                TokenType.Char => PrimType.@char,
                TokenType.Float => PrimType.@float,
                TokenType.LitFloat => PrimType.@float,
                TokenType.String => PrimType.@string,
                TokenType.LitStr => PrimType.@string,
                TokenType.Boolean => PrimType.@bool,
                TokenType.Void => PrimType.@void,
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
            return kind == PrimType.@int || kind == PrimType.@float;
        }

        public override bool IsComparable()
        {
            return IsArithmetic();
        }

        public override bool HasValue()
        {
            return kind != PrimType.@void;
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

        public override void GenCode(CodeWriter w)
        {
            throw new System.NotImplementedException();
        }
    }
}