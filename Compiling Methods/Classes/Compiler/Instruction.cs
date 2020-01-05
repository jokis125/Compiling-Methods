using CompilingMethods.Enums;

namespace CompilingMethods.Classes.Compiler
{
    public class Instruction
    {
        private int opcode;
        private Instructions name;
        private int numOps; 

        public Instruction(int opcode, Instructions name, int numOps)
        {
            this.opcode = opcode;
            this.name = name;
            this.numOps = numOps;
        }

        public int Opcode
        {
            get => opcode;
            set => opcode = value;
        }

        public Instructions Name
        {
            get => name;
            set => name = value;
        }

        public int NumOps
        {
            get => numOps;
            set => numOps = value;
        }

        public static void AddInstruction(int opcode, Instructions name, int numOps)
        {
            var instr = new Instruction(opcode, name, numOps);
            GlobalVars.InstrsByName.Add(name, instr);
            GlobalVars.InstrsByOpCode.Add(opcode, instr);
        }
    }
}