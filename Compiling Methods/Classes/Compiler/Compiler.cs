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
        private bool running = true;
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
                //throw;
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
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                GlobalVars.running = false;
                //throw;
            }
            try
            {
                root.CheckTypes();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw;
            }
            
            if(!GlobalVars.running)
                return;
            PushInstructions();
            var writer = new CodeWriter();
            try
            {
                root.GenCode(writer);
                writer.DumpCode();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw;
                return;
                
            }

            var interpreter = new Interpreter.Interpreter(writer.Code, CodeWriter.StringStorage);
            try
            {
                interpreter.Exec();
            }
            catch (Exception e)
            {

                /*if (e is System.FormatException)
                {
                    //Console.WriteLine($"{GlobalVars.FileName}:{}:Error :Input was not an integer");
                    //throw;
                    return;
                }*/
                Console.WriteLine(e.Message);
                //throw;
            }
            
            
        }

        private void PushInstructions()
        {
            //Arithmetic
            Instruction.AddInstruction(0x10, Instructions.IntAdd, 0);
            Instruction.AddInstruction(0x11, Instructions.IntSub, 0);
            Instruction.AddInstruction(0x12, Instructions.IntMul, 0);
            Instruction.AddInstruction(0x13, Instructions.IntDiv, 0);
            Instruction.AddInstruction(0x14, Instructions.IntMod, 0);
            Instruction.AddInstruction(0x15, Instructions.FloatAdd, 0);
            Instruction.AddInstruction(0x16, Instructions.FloatSub, 0);
            Instruction.AddInstruction(0x17, Instructions.FloatMul, 0);
            Instruction.AddInstruction(0x18, Instructions.FloatDiv, 0);
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
            Instruction.AddInstruction(0x92, Instructions.PrintString, 0);
            Instruction.AddInstruction(0x93, Instructions.PrintFloat, 0);
        }
    }
}