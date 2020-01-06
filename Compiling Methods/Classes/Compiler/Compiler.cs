using System;
using System.Linq;
using System.Runtime.CompilerServices;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Classes.ParserScripts;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.Compiler
{
    public class Compiler
    {
        private readonly Lexer.Lexer lexer = new Lexer.Lexer();
        private Parser parser;
        private bool mainFound = false;
        private Token eofToken;
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
                throw;
                return;
            }

            foreach (var decl in root.Decls.Where(decl => decl.ReturnName().Value.ToString().ToLower() == "main" && decl.getParams().Count == 0))
            {
                mainFound = true;
            }

            if (!mainFound)
            {
                Console.WriteLine($"{lexer.GetScriptName()}:{lexer.GetTokens().Last().LineN}: error: function main must exist and have zero parameters");
                return;
            }
            var printer = new AstPrinter();
            //printer.Print("root", root);

            var rootScope = new Scope(null, lexer.GetScriptName());
            try
            {
                root.ResolveNames(rootScope);
                root.CheckTypes();
            }
            
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            
            PushInstructions();
            var writer = new CodeWriter();
            try
            {
                
                root.GenCode(writer);
                writer.DumpCode();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            var interpreter = new Interpreter.Interpreter(writer.Code);
            interpreter.Exec();
            
        }

        private void PushInstructions()
        {
            //Arithmetic
            Instruction.AddInstruction(0x10, Instructions.IntAdd, 0);
            Instruction.AddInstruction(0x11, Instructions.IntSub, 0);
            Instruction.AddInstruction(0x12, Instructions.IntMul, 0);
            Instruction.AddInstruction(0x13, Instructions.IntDiv, 0);
            //Comparison
            Instruction.AddInstruction(0x20, Instructions.IntLess, 0);
            Instruction.AddInstruction(0x21, Instructions.IntLessEqual, 0);
            Instruction.AddInstruction(0x22, Instructions.IntMore, 0);
            Instruction.AddInstruction(0x23, Instructions.IntMoreEqual, 0);
            Instruction.AddInstruction(0x24, Instructions.IntEqual, 0);
            //Stack
            Instruction.AddInstruction(0x30, Instructions.GetL, 1);
            Instruction.AddInstruction(0x31, Instructions.SetL, 1);
            Instruction.AddInstruction(0x32, Instructions.Pop, 0);
            Instruction.AddInstruction(0x33, Instructions.Push, 1);
            //Control
            Instruction.AddInstruction(0x40, Instructions.Br, 1);
            Instruction.AddInstruction(0x41, Instructions.Bz, 1);
            Instruction.AddInstruction(0x42, Instructions.Ret, 0);
            Instruction.AddInstruction(0x43, Instructions.RetV, 0);
            Instruction.AddInstruction(0x44, Instructions.CallBegin, 0);
            Instruction.AddInstruction(0x45, Instructions.Call, 2);
            
            Instruction.AddInstruction(0x46, Instructions.Exit, 0);
            Instruction.AddInstruction(0x47, Instructions.Alloc, 1);
            
            Instruction.AddInstruction(0x90, Instructions.Read, 0);
            Instruction.AddInstruction(0x91, Instructions.Print, 0);
        }
    }
}