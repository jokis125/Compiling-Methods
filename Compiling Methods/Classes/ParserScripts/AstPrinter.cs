using System;
using System.Collections.Generic;
using System.Linq;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public class AstPrinter
    {
        private int indentLevel;

        public void Print(string title, object obj)
        {
            if (obj == null)
                PrintText(title, "null");
            else if (obj is INode node)
                PrintNode(title, node);
            else if (obj.GetType() == typeof(List<INode>))
                PrintArray(title, (List<INode>) obj);
            else if (obj.GetType() == typeof(List<IStatement>))
                PrintArray(title, (List<IStatement>) obj);
            else if (obj.GetType() == typeof(List<Param>))
                PrintArray(title, (List<Param>) obj);
            else if (obj.GetType() == typeof(List<IExpression>))
                PrintArray(title, (List<IExpression>) obj);
            else if (obj.GetType() == typeof(List<StmtIf>))
                PrintArray(title, (List<StmtIf>) obj);
            else if (obj.GetType() == typeof(List<Branch>))
                PrintArray(title, (List<Branch>)obj);
            else if (obj.GetType() == typeof(List<IDeclares>))
                PrintArray(title, (List<IDeclares>)obj);
            else if (obj.GetType() == typeof(Token))
                PrintToken(title, (Token) obj);
            else if (obj is Enum @enum)
                PrintEnum(title, @enum);
            else if (obj is string) PrintText(title, obj.ToString());
        }

        private void PrintArray<T>(string title, IReadOnlyCollection<T> array)
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
            PrintText(title, $"{node.GetType().Name}");
            indentLevel++;
            node.PrintNode(this);
            indentLevel--;
        }

        private void PrintText(string title, string text)
        {
            var prefix = string.Concat(Enumerable.Repeat("    ", indentLevel));
            Console.WriteLine($"{prefix}{title}: {text}");
        }

        private void PrintToken(string title, Token token)
        {
            PrintText(title, $"{token.State} ({token.Value})");
        }

        private void PrintEnum<T>(string title, T enumerator)
        {
            PrintText(title, $"{enumerator}");
        }
    }
}