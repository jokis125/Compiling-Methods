using CompilingMethods.Classes;

namespace CompilingMethods
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var compiler = new Compiler();
            compiler.Compile();
        }
    }
}