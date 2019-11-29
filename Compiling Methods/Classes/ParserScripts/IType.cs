using System.Collections.Generic;
using CompilingMethods.Classes.Compiler;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface IType : INode
    {
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

        public Token Token => token;

        public PrimType Kind => kind;

        public bool IsArithmetic()
        {
            return kind == PrimType.Int || kind == PrimType.Float;
        }

        public bool IsComparable()
        {
            return IsArithmetic();
        }

        public bool HasValue()
        {
            return kind != PrimType.Void;
        }
        
        public void PrintNode(AstPrinter p)
        {
            p.Print("kind", kind);
        }

        public void ResolveNames(Scope scope)
        {
            throw new System.NotImplementedException();
        }

        public TypePrim CheckTypes()
        {
            throw new System.NotImplementedException();
        }
    }
}