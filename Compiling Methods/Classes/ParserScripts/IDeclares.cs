using System;
using System.Collections.Generic;
using CompilingMethods.Classes.Compiler;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;
using Microsoft.VisualBasic.CompilerServices;

namespace CompilingMethods.Classes.ParserScripts
{
    public abstract class IDeclares : Node
    {
       public abstract Token ReturnName();
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
            var bodyAndParams = new Node[body.Count + parameters.Count + 1];
            Array.Copy(parameters.ToArray(), bodyAndParams, parameters.Count);
            Array.Copy(body.ToArray(), bodyAndParams, body.Count);
            bodyAndParams[body.Count + parameters.Count] = type;
            AddChildren(bodyAndParams);
            this.type = type;
            this.name = name;
            this.parameters = parameters;
            this.body = body;
        }

        public List<Param> Parameters => parameters;

        public TypePrim Type => type;

        public override void PrintNode(AstPrinter p)
        {
            p.Print("type", type);
            p.Print("name", name);
            p.Print("params", parameters);
            //p.Print("return type", retType);
            p.Print("body", body);
        }

        public override Token ReturnName()
        {
            return name;
        }

        public override void ResolveNames(Scope parentScope)
        {
            GlobalVars.StackSlotIndex = 0;
            var scope = new Scope(parentScope);
            parameters.ForEach(param => param.ResolveNames(scope));
            body.ForEach(bod => bod.ResolveNames(scope));
        }

        public override TypePrim CheckTypes()
        {
            //parameters.ForEach(param => param.CheckTypes());
            body.ForEach(bod => bod.CheckTypes());
            return new TypePrim(null, PrimType.Void);
            //throw new System.NotImplementedException();
            
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

        public override void PrintNode(AstPrinter p)
        {
            p.Print("type", type);
            p.Print("name", ident);
            p.Print("value", value);
        }

        public override Token ReturnName()
        {
            return ident;
        }

        public override void ResolveNames(Scope scope)
        {
            stackSlot = GlobalVars.StackSlotIndex++;
            scope.Add(ident.Value, this);
            value.ResolveNames(scope);
        }

        public override TypePrim CheckTypes()
        {
            throw new System.NotImplementedException();
        }
    }
}