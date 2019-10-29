using System.Reflection.Metadata.Ecma335;
using CompilingMethods.Classes;

namespace CompilingMethods
{
    class Program
    {
        static void Main(string[] args)
        {
            var lexer = new Lexer();
            lexer.GetText();
            lexer.StartLexer();
            var parser = new Parser(lexer.GetTokens());
            parser.ParseProgram();
        }
    }
}