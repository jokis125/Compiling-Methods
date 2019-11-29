using CompilingMethods.Classes.Compiler;
using CompilingMethods.Classes.Lexer;

namespace CompilingMethods.Classes.ParserScripts
{
    public class Param : INode
    {
        private readonly Token name;
        private readonly TypePrim type;
        private int stackSlot;

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

        public void ResolveNames(Scope scope)
        {
            stackSlot = GlobalVars.StackSlotIndex++;
            scope.Add(name, this);
        }

        public TypePrim CheckTypes()
        {
            return null;
        }
    }
}