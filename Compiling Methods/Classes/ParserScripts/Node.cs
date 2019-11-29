using System;
using System.Collections.Generic;
using CompilingMethods.Classes.Compiler;
using Microsoft.VisualBasic.CompilerServices;

namespace CompilingMethods.Classes.ParserScripts
{
    public interface INode
    {
        void PrintNode(AstPrinter p);
        void ResolveNames(Scope scope);
        TypePrim CheckTypes();
    }

    public class Root : INode
    {
        private readonly List<IDeclares> decls;

        public Root(List<IDeclares> decls)
        {
            this.decls = decls;
        }

        public void PrintNode(AstPrinter p)
        {
            p.Print("Declare", decls);
        }
        
        public void ResolveNames(Scope scope)
        {
            decls.ForEach(decl => scope.Add(decl.ReturnName(), decl));
            decls.ForEach(decl => decl.ResolveNames(scope));
        }

        public TypePrim CheckTypes()
        {
            decls.ForEach(decl => decl.CheckTypes());
            return null;
            //throw new NotImplementedException();
        }
    }


    
}