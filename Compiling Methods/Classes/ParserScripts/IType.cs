using System.Collections.Generic;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface IType : INode
    {
    }

    public class TypePrim : IType //TOSWITCH
    {
        private Token token;
        private readonly PrimType kind;

        public TypePrim(Token token)
        {
            this.token = token;
            this.kind = token.State switch
            {
                TokenType.Int => PrimType.Int,
                TokenType.Char => PrimType.Char,
                TokenType.Float => PrimType.Float,
                TokenType.String => PrimType.String,
                TokenType.Boolean => PrimType.Bool,
                TokenType.Void => PrimType.Void,
                _ => this.kind
            };
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("kind", kind);
        }
    }
}