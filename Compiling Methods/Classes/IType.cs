using CompilingMethods.Enums;

namespace CompilingMethods.Classes
{
    public interface IType : INode
    {
        
    }

    public class TypePrim : IType
    {
        private Token kind;

        public TypePrim(Token kind)
        {
            this.kind = kind;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("kind", kind);
        }
    }
}