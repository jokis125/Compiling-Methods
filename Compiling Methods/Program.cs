namespace CompilingMethods.Classes
{
    class Program
    {
        static void Main(string[] args)
        {
            var lexeris = new Lexer();
            lexeris.GetText();
            lexeris.StartLexer();
        }
    }
}