using CompilingMethods.Classes.Lexer;

namespace CompilingMethods.Classes.ParserScripts
{
    public class Param : INode
    {
        private readonly Token name;
        private readonly TypePrim type;

        public Param(Token name, TypePrim type)
        {
            this.name = name;
            this.type = type;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("type", type);
            p.Print("name", name);
        }
    }
}