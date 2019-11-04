using CompilingMethods.Classes.Lexer;
using CompilingMethods.Classes.ParserScripts;

namespace CompilingMethods
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var lexer = new Lexer();
            lexer.GetText();
            lexer.StartLexer();
            var parser = new Parser(lexer.GetTokens(), lexer.GetScriptName());
            var root = parser.ParseProgram();
            var printer = new AstPrinter();
            printer.Print("root", root);
        }
    }
}