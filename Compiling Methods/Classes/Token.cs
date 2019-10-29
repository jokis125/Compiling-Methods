using System;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes
{
    public class Token
    {
        private readonly TokenType state;
        private readonly dynamic value;
        private readonly int lineN;

        public Token(TokenType newState, dynamic newValue, int newLineNr)
        {
            state = newState;
            value = newValue;
            lineN = newLineNr;
        }

        public void PrintToken(int count)
        {
            var id = (" " + count).PadRight(4);
            var ln = (" " + lineN).PadRight(5);
            var type = (" " + state).PadRight(16);
            Console.WriteLine($"{id}|{ln}|{type}|{value}");
            //Console.WriteLine($"{id}|{ln}|{type}|{value}");
        }

        public TokenType GetState()
        {
            return state;
        }
    }
}