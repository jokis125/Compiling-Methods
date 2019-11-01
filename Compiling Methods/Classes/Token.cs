using System;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes
{
    public class Token
    {
        public Token(TokenType newState, dynamic newValue, int newLineNr)
        {
            State = newState;
            Value = newValue;
            LineN = newLineNr;
        }

        public void PrintToken(int count)
        {
            var id = (" " + count).PadRight(4);
            var ln = (" " + LineN).PadRight(5);
            var type = (" " + State).PadRight(16);
            Console.WriteLine($"{id}|{ln}|{type}|{Value}");
            //Console.WriteLine($"{id}|{ln}|{type}|{value}");
        }

        public TokenType State { get; }

        public dynamic Value { get; }

        public int LineN { get; }
    }
}