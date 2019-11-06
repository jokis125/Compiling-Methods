using System.Collections.Generic;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface INode
    {
        void PrintNode(AstPrinter p);
    }

    public class Root : INode
    {
        private Body body;

        public Root(Body body)
        {
            this.body = body;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("Body", body);
        }
    }

    public class Body : INode
    {
        private List<INode> declares;

        public Body(List<INode> declares)
        {
            this.declares = declares;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("Declares", declares);
        }
    }
    
}