using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.Compiler
{
    public class CodeWriter
    {
        List<int> code = new List<int>();

        private void CompleteLabel(Label label, int value)
        {
            label.Value = value;
            foreach (var offset in label.Offsets)
            {
                code[offset] = value;
            }
        }

        public void DumpCode()
        {
            var offset = 0;
            while(offset < code.Count)
            {
                var opcode = code[offset];
                Instruction instr;
                GlobalVars.InstrsByOpCode.TryGetValue(opcode, out instr);
                var op1 = code[offset + 1];
                var op2 = code[instr.NumOps];
                Console.WriteLine($"{offset}: {opcode} {instr.Name} {op1}, {op2}");
                offset += 1 + instr.NumOps;
            }
        }

        public void PlaceLabel(Label label)
        {
            CompleteLabel(label, code.Count);
        }

        public void Write(Instructions instrName, params Object[] ops)
        {
            Instruction instr;
            GlobalVars.InstrsByName.TryGetValue(instrName, out instr);
            if(ops.Length != instr.NumOps)
                throw new SystemException("invalid instruction operand count");
            
            code.Add(instr.Opcode);
            foreach (var op in ops)
            {
                if(op.GetType() != typeof(Label))
                    code.Add((int)op);
                else if (((Label)op).Value <= 0)
                {
                    ((Label)op).Offsets.Add(code.Count);
                }
                else
                {
                    code.Add(((Label)op).Value);
                }
            }
        }
    }
}