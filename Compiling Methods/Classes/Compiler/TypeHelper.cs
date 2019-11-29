using System;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Classes.ParserScripts;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.Compiler
{
    public class TypeHelper
    {
        public static void SemanticError(Token token, string message)
        {
            var lineN = token.LineN;
            Console.WriteLine($"{GlobalVars.FileName}:{lineN}: semantic error: {message}");
        }

        public static void UnifyTypes(TypePrim type0, TypePrim type1, Token token = null)
        {
            if (type0 == null || type1 == null)
            {
                //do nothing
            }
            else if (type0.GetType() != type1.GetType())
                SemanticError(type0.Token, $"type kind mismatch: expected {type0}, got {type1}");
            else if (type0.Kind != type1.Kind)
                SemanticError(type0.Token, $"type mismatch: expected {type0.Kind}, got {type1.Kind}");
        }
    }
}