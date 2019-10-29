using System.Reflection.Metadata.Ecma335;
using CompilingMethods.Classes;

namespace CompilingMethods
{
    class Program
    {
        static void Main(string[] args)
        {
            var lexeris = new Lexer();
            lexeris.GetText();
            lexeris.StartLexer();
            var parser = new Parser(lexeris.GetTokens());
            parser.ParseProgram();
        }
    }
}