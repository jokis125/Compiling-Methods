using System.Security.Cryptography;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes
{
    public class Param : INode
    {
        private Token name;
        private Token type;

        public Param(Token name, Token type)
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