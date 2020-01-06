using System;
using System.Collections.Generic;
using System.Linq;
using CompilingMethods.Classes.Compiler;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public abstract class Node
    {
        public Node parent = null;
        public Token locationToken;
        public abstract void PrintNode(AstPrinter p);
        public abstract void ResolveNames(Scope scope);
        public abstract TypePrim CheckTypes();
        public abstract void GenCode(CodeWriter w);
        
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

        public Label MainLabel { get; set; }

        public Root(List<IDeclares> decls)
        {
            AddChildren(decls.ToArray());
            this.decls = decls;
            if (decls.Any())
                locationToken = decls[0].locationToken;
            this.MainLabel = new Label();
        }

        public override void PrintNode(AstPrinter p)
        {
            p.Print("Declare", decls);
        }
        
        public override void ResolveNames(Scope scope)
        {
            var printToken = new Token(TokenType.Print, "printInt", 0);
            var readToken = new Token(TokenType.Read, "readInt", 0);
            var printStringToken = new Token(TokenType.PrintString, "printString", 0);
            var printFloatToken = new Token(TokenType.PrintFloat, "printFloat", 0);
            var printType = new TypePrim(printToken);
            var readType = new TypePrim(readToken);
            var printStringType = new TypePrim(printStringToken);
            var printFloatType = new TypePrim(printFloatToken);
            
            scope.Add(printToken, new DeclFn(printType, printToken, new List<Param> { new Param(printToken, new TypePrim(new Token(TokenType.Int, null, 0)))}, null));
            scope.Add(printStringToken, new DeclFn(printStringType, printStringToken, new List<Param> { new Param(printStringToken, new TypePrim(new Token(TokenType.String, null, 0)))}, null));
            scope.Add(printFloatToken, new DeclFn(printFloatType, printFloatToken, new List<Param> { new Param(printFloatToken, new TypePrim(new Token(TokenType.Float, null, 0)))}, null));
            scope.Add(readToken, new DeclFn(readType, readToken, new List<Param>(),null));

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

        public override void GenCode(CodeWriter w)
        {
            w.Write(Instructions.CallBegin);
            w.Write(Instructions.Call, MainLabel, 0);
            w.Write(Instructions.Exit);
            foreach (var declares in decls)
            {
                declares.GenCode(w);
            }
        }
    }


    
}