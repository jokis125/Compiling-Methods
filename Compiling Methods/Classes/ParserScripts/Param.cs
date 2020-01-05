using CompilingMethods.Classes.Compiler;
using CompilingMethods.Classes.Lexer;

namespace CompilingMethods.Classes.ParserScripts
{
    public class Param : Node, IStackSlot
    {
        private readonly Token name;
        private readonly TypePrim type;
        public int StackSlot { get; set; }

        public Param(Token name, TypePrim type)
        {
            AddChildren(type);
            this.name = name;
            this.type = type;
            locationToken = name;
        }

        public override void PrintNode(AstPrinter p)
        {
            p.Print("type", type);
            p.Print("name", name);
        }

        public override void ResolveNames(Scope scope)
        {
            StackSlot = Scope.StackSlotIndex;
            Scope.StackSlotIndex++;
            scope.Add(name, this);
        }

        public TypePrim Type => type;

        public override TypePrim CheckTypes()
        {
            throw new System.NotImplementedException();
        }

        public override void GenCode(CodeWriter w)
        {
            throw new System.NotImplementedException();
        }

        
    }
}