using System.Collections.Generic;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface INode
    {
        void PrintNode(AstPrinter p);
    }

    public class Root : INode
    {
        private List<INode> declares;

        public Root(List<INode> declares)
        {
            this.declares = declares;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("Root", declares);
        }
    }
}