using System.Collections.Generic;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface INode
    {
        void PrintNode(AstPrinter p);
    }

    public class Root : INode
    {
        private readonly List<INode> decls;

        public Root(List<INode> decls)
        {
            this.decls = decls;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("Declare", decls);
        }
    }


    
}