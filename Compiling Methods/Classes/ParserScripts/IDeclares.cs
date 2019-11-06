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
        private readonly Token name;
        private readonly List<Param> parameters;
        private readonly TypePrim type;

        public DeclFn(TypePrim type, Token name, List<Param> parameters, List<IStatement> body)
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
    
    public class DeclVar : IDeclares
    {
        private readonly Token ident;
        private readonly TypePrim type;
        private readonly IExpression value;
        

        public DeclVar(TypePrim type, Token ident)
        {
            this.type = type;
            this.ident = ident;
            value = null;
        }

        public DeclVar(TypePrim type, Token ident, IExpression value)
        {
            this.type = type;
            this.ident = ident;
            this.value = value;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("type", type);
            p.Print("name", ident);
            p.Print("value", value);
        }
    }
}