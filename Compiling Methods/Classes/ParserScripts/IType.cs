using CompilingMethods.Classes.Lexer;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface IType : INode
    {
    }

    public class TypePrim : IType
    {
        private readonly Token kind;

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