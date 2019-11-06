using System;
using CompilingMethods.Classes.ParserScripts;

namespace CompilingMethods.Classes
{
    public class Compiler
    {
        private readonly Lexer.Lexer lexer = new Lexer.Lexer();
        private Parser parser;
        public void Compile()
        {
            lexer.GetText();
            try
            {
                lexer.StartLexer();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
                return;
            }
            
            parser = new Parser(lexer.GetTokens(), lexer.GetScriptName());
            Root root;
            try
            {
                root = parser.ParseProgram();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
                return;
            }
            var printer = new AstPrinter();
            printer.Print("root", root);
        }
    }
}