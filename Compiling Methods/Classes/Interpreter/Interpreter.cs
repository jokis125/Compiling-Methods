using System;
using System.Collections.Generic;
using CompilingMethods.Classes.Compiler;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.Interpreter
{
    public class Interpreter
    {
        List<int> memory = new List<int>(4096);
        private bool running = true;
        private int ip = 0;
        private int fp = 1024;
        private int sp = 1024;

        public Interpreter(List<int> code)
        {
            for (var i = 0; i < 4096; ++i)
            {
                memory.Add(0);
            }

            for (var i = 0; i < code.Count; i++)
            {
                memory[i] = code[i];
            }
        }

        public void Exec()
        {
            while (running)
                ExecOne();
            var result = Pop();
            Console.WriteLine($"Result: {result}");
        }

        public void ExecOne()
        {
            var hexopcode = ReadImm();
            var opcode = GlobalVars.InstrsByOpCode[hexopcode].Name;
            int a, b, i;

            switch(opcode) {
                case Instructions.IntAdd:
                    b = Pop();
                    a = Pop();
                    Push(a + b);
                    break;
                case Instructions.IntSub:
                    b = Pop();
                    a = Pop();
                    Push(a - b);
                    break;
                case Instructions.IntMul:
                    b = Pop();
                    a = Pop();
                    Push(a * b);
                    break;
                case Instructions.IntDiv:
                    b = Pop();
                    a = Pop();
                    Push(a / b);
                    break;
                case Instructions.IntLess:
                    b = Pop();
                    a = Pop();
                    Push(a < b ? 1 : 0);
                    break;
                case Instructions.IntLessEqual:
                    b = Pop();
                    a = Pop();
                    Push(a <= b ? 1 : 0);
                    break;
                case Instructions.IntEqual:
                    b = Pop();
                    a = Pop();
                    Push(a == b ? 1 : 0);
                    break;
                case Instructions.GetL:
                    i = ReadImm();
                    Push(memory[fp + i]);
                    break;
                case Instructions.SetL:
                    i = ReadImm();
                    memory[fp + i] = Pop();
                    break;
                case Instructions.Pop:
                    sp--;
                    break;
                case Instructions.Push:
                    Push(ReadImm());
                    break;
                case Instructions.Alloc:
                    sp += ReadImm();
                    break;
                case Instructions.Br:
                    i = ReadImm();
                    ip = i;
                    break;
                case Instructions.Bz:
                    i = ReadImm();
                    if (Pop() == 0)
                        ip = i;
                    break;
                case Instructions.Ret:
                    ExecRet(0);
                    break;
                case Instructions.RetV:
                    ExecRet(Pop());
                    break;
                case Instructions.CallBegin:
                    Push(0);
                    Push(0);
                    Push(0);
                    break;
                case Instructions.Call:
                    ExecCall(ReadImm(), ReadImm());
                    break;
                case Instructions.Read:
                    a = Convert.ToInt32(Console.ReadLine());
                    Push(a);
                    break;
                case Instructions.Print:
                    a = Pop();
                    Console.WriteLine(a);
                    break;
                case Instructions.Exit:
                    running = false;
                    break;
                default:
                    throw new Exception("not work >:C");
            }
        }

        public void ExecRet(int value)
        {
            var oldIp = memory[fp - 3];
            var oldFp = memory[fp - 2];
            var oldSp = memory[fp - 1];
            ip = oldIp;
            fp = oldFp;
            sp = oldSp;
            Push(value);
        }

        public void ExecCall(int target, int numArgs)
        {
            var newIp = target;
            var newFp = sp - numArgs;
            var newSp = newFp;
            memory[newFp - 3] = ip;
            memory[newFp - 2] = fp;
            memory[newFp - 1] = newFp - 3;
            ip = newIp;
            fp = newFp;
            sp = newSp;
        }

        public int Pop()
        {
            sp--;
            return memory[sp];
        }

        public void Push(int value)
        {
            memory[sp] = value;
            sp++;
        }

        public int ReadImm()
        {
            var value = memory[ip];
            ip++;
            return value;
        }
        
    }
}