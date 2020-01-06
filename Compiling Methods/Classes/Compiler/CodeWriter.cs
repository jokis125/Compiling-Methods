using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.Compiler
{
    public class CodeWriter
    {
        public List<int> Code { get; set; } = new List<int>();

        private void CompleteLabel(Label label, int value)
        {
            label.Value = value;
            foreach (var offset in label.Offsets)
            {
                Code[offset] = value;
            }
        }

        public void DumpCode()
        {
            var offset = 0;
            while(offset < Code.Count)
            {
                var opcode = Code[offset];
                Instruction instr;
                GlobalVars.InstrsByOpCode.TryGetValue(opcode, out instr);
                //var op1 = Code[offset + 1];
                var op1 = instr.NumOps > 0 ? Code[offset + 1] : Code[offset];
                var op2 = instr.NumOps > 0 ? Code[instr.NumOps] : -1;
                //Console.WriteLine($"{offset.ToString()}: {opcode.ToString()} {instr.Name} {op1}, {op2}");
                Console.WriteLine($"{offset.ToString().PadLeft(3)}: 0x{opcode.ToString("X")}|{opcode.ToString()} - {instr.Name.ToString().PadRight(13)} {(String.Join(" ", Code.GetRange(offset + 1, instr.NumOps)))}");
                offset += 1 + instr.NumOps;
            }
            
            Console.WriteLine("Code: ");
            Console.WriteLine($"[{String.Join(", ", Code)}]");
        }

        public void PlaceLabel(Label label)
        {
            CompleteLabel(label, Code.Count);
        }

        public void Write(Instructions instrName, params Object[] ops)
        {
            Instruction instr;
            GlobalVars.InstrsByName.TryGetValue(instrName, out instr);
            if(ops.Length != instr.NumOps)
                throw new SystemException("invalid instruction operand count");
            
            Code.Add(instr.Opcode);

            foreach (var op in ops)
            {
                if (op is Label label)
                {
                    if (label.Value == -1)
                    {
                        label.Offsets.Add(Code.Count);
                        Code.Add(666);
                    }
                    else
                        Code.Add(label.Value);
                }
                else
                {
                    var aa = (int) op;
                    Code.Add(aa);
                }
            }
        }
    }
}