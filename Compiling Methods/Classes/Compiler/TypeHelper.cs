using System;
using System.Runtime.InteropServices.ComTypes;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Classes.ParserScripts;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.Compiler
{
    public class TypeHelper
    {
        public static void SemanticError(Token token, string message)
        {
            GlobalVars.running = false;
            var lineN = token.LineN;
            Console.WriteLine($"{GlobalVars.FileName}:{lineN}: error: {message}");
        }
        
        public static void TypesNotComparableError(PrimType type0, PrimType type1)
        {
            Console.WriteLine($"{type0} is not comparable with {type1}");
        }

        public static void UnifyTypes(TypePrim type0, TypePrim type1, Token token = null)
        {
            if (type0 == null || type1 == null)
                return;
            if (type0.GetType() != type1.GetType())
                SemanticError(type0.Token.LineN > type1.Token.LineN ? type0.Token : type1.Token,
                    $"type kind mismatch: expected {type0}, got {type1}");
            if (type0.GetType() == typeof(TypePrim) || type1.GetType() == typeof(TypePrim))
            {
                var prim0 = type0 as TypePrim;
                var prim1 = type1 as TypePrim;
                
                if(prim0.Kind != prim1.Kind)
                    SemanticError(type0.Token == null ? type1.Token : type0.Token,
                        $"type mismatch: expected {type0.Kind}, got {type1.Kind}");
                return;

            }

            throw new Exception("End should be unreachable");
        }
        
        public static TypePrim TYPE_INT(Token token) => new TypePrim(token);
    }
}