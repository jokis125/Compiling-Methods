using System;
using CompilingMethods.Classes.ParserScripts;

namespace CompilingMethods.Classes.Compiler
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
                //throw;
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
                //throw;
                return;
            }

            if (root.Decls.Count == 0)
            {
                Console.Write($"{lexer.GetScriptName()} is empty");
                return;
            }
            var printer = new AstPrinter();
            printer.Print("root", root);

            var rootScope = new Scope(null, lexer.GetScriptName());
            try
            {
                root.ResolveNames(rootScope);
                root.CheckTypes();
            }
            
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw;
            }
            
            
        }
    }
}