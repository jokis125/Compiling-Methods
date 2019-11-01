using System.Collections.Generic;
using CompilingMethods.Classes.Lexer;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface IDeclares : INode
    {
        
    }

    public class DeclFn : IDeclares
    {
        private Token type;
        private string name;
        private List<Param> parameters;
        //private TokenType retType;
        private List<IStatement> body;

        public DeclFn(Token type, string name, List<Param> parameters, List<IStatement> body)
        {
            this.type = type;
            this.name = name;
            this.parameters = parameters;
            this.body = body;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("type",type);
            p.Print("name", name);
            p.Print("params", parameters);
            //p.Print("return type", retType);
            p.Print("body", body);
        }
    }
}