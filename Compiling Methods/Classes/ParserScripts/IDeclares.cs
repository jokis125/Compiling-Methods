using System.Collections.Generic;
using CompilingMethods.Classes.Compiler;
using CompilingMethods.Classes.Lexer;
using Microsoft.VisualBasic.CompilerServices;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface IDeclares : INode
    {
        Token ReturnName();
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

        public Token ReturnName()
        {
            return name;
        }

        public void ResolveNames(Scope parentScope)
        {
            GlobalVars.StackSlotIndex = 0;
            var scope = new Scope(parentScope);
            parameters.ForEach(param => param.ResolveNames(scope));
            body.ForEach(bod => bod.ResolveNames(scope));
        }

        public TypePrim CheckTypes()
        {
            parameters.ForEach(param => param.CheckTypes());
            body.ForEach(bod => bod.CheckTypes());
            return type;
        }
    }
    
    public class DeclVar : IDeclares
    {
        private readonly Token ident;
        private readonly TypePrim type;
        private readonly IExpression value;
        private int stackSlot;
        

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

        public Token ReturnName()
        {
            return ident;
        }

        public void ResolveNames(Scope scope)
        {
            stackSlot = GlobalVars.StackSlotIndex++;
            scope.Add(ident.Value, this);
            value.ResolveNames(scope);
        }

        public TypePrim CheckTypes()
        {
            throw new System.NotImplementedException();
        }
    }
}