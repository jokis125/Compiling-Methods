using System.Collections.Generic;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface IType : INode
    {
    }

    public class TypePrim : IType
    {
        private readonly PrimType kind;
        private readonly Dictionary<TokenType, PrimType> typeDict = new Dictionary<TokenType, PrimType>();

        public TypePrim(TokenType kind)
        {
            PopulateDict();
            this.kind = typeDict[kind];
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("kind", kind);
        }
        
        private void PopulateDict()
        {
            typeDict.Add(TokenType.Int, PrimType.Int);
            typeDict.Add(TokenType.Char, PrimType.Char);
            typeDict.Add(TokenType.Float, PrimType.Float);
            typeDict.Add(TokenType.String, PrimType.String);
            typeDict.Add(TokenType.Boolean, PrimType.Bool);
            typeDict.Add(TokenType.Void, PrimType.Void);
        }
    }
}