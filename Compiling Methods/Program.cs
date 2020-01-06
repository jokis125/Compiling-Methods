using System;
using CompilingMethods.Classes;
using CompilingMethods.Classes.Compiler;

namespace CompilingMethods
{
    internal class Program
    {
        public static int SingleToInt32Bits(float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        public static float Int32BitsToSingle(int value)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        }
        private static void Main(string[] args)
        {
            var compiler = new Compiler();
            compiler.Compile();
        }
    }
}