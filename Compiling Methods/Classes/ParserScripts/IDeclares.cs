using System.Collections.Generic;
using CompilingMethods.Classes.Lexer;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface IDeclares : INode
    {
    }

    public class DeclFn : IDeclares
    {
        //private TokenType retType;
        private readonly List<IStatement> body;
        private readonly string name;
        private readonly List<Param> parameters;
        private readonly TypePrim type;

        public DeclFn(TypePrim type, string name, List<Param> parameters, List<IStatement> body)
        {
            this.type = type;
            this.name = name;
            this.parameters = parameters;
            this.body = body;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("type", type);
            p.Print("name", name);
            p.Print("params", parameters);
            //p.Print("return type", retType);
            p.Print("body", body);
        }
    }
}