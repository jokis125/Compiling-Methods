using System;
using System.Collections.Generic;
using CompilingMethods.Classes.Exceptions;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Classes.ParserScripts;

namespace CompilingMethods.Classes.Compiler
{
    public class Scope
    {
        private Scope parentScope;
        private Dictionary<String, INode> members = new Dictionary<String, INode>();

        public Scope(Scope parentScope, string fileName)
        {
            this.parentScope = parentScope;
            GlobalVars.FileName = fileName;
        }
        public Scope(Scope parentScope)
        {
            this.parentScope = parentScope;
        }

        public void Add(Token nameToken, INode node)
        {
            var name = nameToken.Value;
            if (members.ContainsKey(name))
            {
                throw new NameExistsException($"{GlobalVars.FileName}: {nameToken.LineN}: error: duplicate variable name: {name}");
            }
            members.Add(name, node);
        }

        public INode ResolveName(Token nameToken)
        {
            var name = nameToken.Value;
            if (members.ContainsKey(name))
                return members[name];

            if (parentScope != null)
                return parentScope.ResolveName(nameToken);
            throw new UndeclaredVariableException($"{GlobalVars.FileName}: {nameToken.LineN}: error: undeclared variable: '{name}'");
        }
    }
}