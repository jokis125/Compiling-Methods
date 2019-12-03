using CompilingMethods.Classes.Compiler;
using CompilingMethods.Classes.Lexer;

namespace CompilingMethods.Classes.ParserScripts
{
    public class Param : Node
    {
        private readonly Token name;
        private readonly TypePrim type;
        private int stackSlot;

        public Param(Token name, TypePrim type)
        {
            AddChildren(type);
            this.name = name;
            this.type = type;
        }

        public override void PrintNode(AstPrinter p)
        {
            p.Print("type", type);
            p.Print("name", name);
        }

        public override void ResolveNames(Scope scope)
        {
            stackSlot = GlobalVars.StackSlotIndex++;
            scope.Add(name, this);
        }

        public TypePrim Type => type;

        public override TypePrim CheckTypes()
        {
            throw new System.NotImplementedException();
        }
    }
}