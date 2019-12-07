using System;
using System.Collections.Generic;
using CompilingMethods.Classes.Compiler;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public abstract class Node
    {
        public Node parent = null;
        public abstract void PrintNode(AstPrinter p);
        public abstract void ResolveNames(Scope scope);
        public abstract TypePrim CheckTypes();

        public void AddChildren(params Node[] children)
        {
            foreach (var child in children)
            {
                if (child != null)
                    child.parent = this;
            }
        }

        public Node FindAncestor(System.Type classType)
        {
            var currentNode = parent;
            while (currentNode != null)
            {
                if (currentNode.GetType() == classType)
                    return currentNode;
                currentNode = currentNode.parent;
            }

            return null;
        }
    }

    public class Root : Node
    {
        private readonly List<IDeclares> decls;

        public Root(List<IDeclares> decls)
        {
            AddChildren(decls.ToArray());
            this.decls = decls;
        }

        public override void PrintNode(AstPrinter p)
        {
            p.Print("Declare", decls);
        }
        
        public override void ResolveNames(Scope scope)
        {
            decls.ForEach(decl => scope.Add(decl.ReturnName(), decl));
            decls.ForEach(decl => decl.ResolveNames(scope));
        }

        public List<IDeclares> Decls => decls;

        public override TypePrim CheckTypes()
        {
            decls.ForEach(decl => decl.CheckTypes());
            return new TypePrim(new Token(TokenType.Void, null, 0));
            //throw new NotImplementedException();
        }
        
    }


    
}