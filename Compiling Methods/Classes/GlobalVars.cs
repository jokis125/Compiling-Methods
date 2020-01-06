using System.Collections.Generic;
using CompilingMethods.Classes.Compiler;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes
{
    public class GlobalVars
    {
        public static string FileName;
        public static Dictionary<Instructions, Instruction> InstrsByName = new Dictionary<Instructions, Instruction>();
        public static Dictionary<int, Instruction> InstrsByOpCode = new Dictionary<int, Instruction>();
        public static bool running = true;
    }
}