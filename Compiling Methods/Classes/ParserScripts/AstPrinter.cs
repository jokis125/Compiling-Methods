using System;
using System.Collections.Generic;
using System.Linq;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public class AstPrinter
    {
        private int indentLevel = 0;

        public void Print(string title, Object obj)
        {
            if(obj == null)
                PrintText(title, "null");
            else if (obj is INode)
            {
                PrintNode(title, (INode)obj);
            }
            else if (obj.GetType() == typeof(List<INode>))
            {
                PrintArray(title, (List<INode>)obj);
            }
            else if (obj.GetType() == typeof(List<IStatement>))
            {
                PrintArray<IStatement>(title, (List<IStatement>)obj);
            }
            else if (obj.GetType() == typeof(List<Param>))
            {
                PrintArray(title, (List<Param>)obj);
            }
            else if (obj.GetType() == typeof(List<IExpression>))
            {
                PrintArray(title, (List<IExpression>)obj);
            }
            else if (obj.GetType() == typeof(List<StmtIf>))
            {
                PrintArray(title, (List<StmtIf>)obj);
            }
            else if (obj.GetType() == typeof(Token))
            {
                PrintToken(title, (Token)obj);
            }
            else if (obj.GetType() == typeof(TokenType))
            {
                PrintTokenType(title, (TokenType)obj);
            }
            else if (obj is string)
            {
                PrintText(title, obj.ToString());
            }
        }

        private void PrintArray<T>(string title, List<T> array)
        {
            if (array.Count == 0)
            {
                PrintText(title, "[]");
                return;
            }

            var index = 0;
            foreach (var elem in array)
            {
                var elemTitle = $"{title}[{index++}]";
                Print(elemTitle, elem);
            }
        }

        private void PrintNode(string title, INode node)
        {
            PrintText(title, $"{node.GetType()}");
            indentLevel++;
            node.PrintNode(this);
            indentLevel--;
        }

        void PrintText(string title, string text)
        {
            string prefix = String.Concat(Enumerable.Repeat("    ", indentLevel));
            Console.WriteLine($"{prefix}{title}: {text}");
        }

        void PrintToken(string title, Token token)
        {
            PrintText(title, $"{token.State} ({token.Value})");
        }
        
        void PrintTokenType(string title, TokenType token)
        {
            PrintText(title, $"{token} ()");
        }
    }
}