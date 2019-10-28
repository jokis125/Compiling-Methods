using System;

namespace CompilingMethods.Classes
{
    public class Token
    {
        private TokenType _state;
        private dynamic value;
        private int lineN;

        public Token(TokenType newState, dynamic newValue, int newLineNr)
        {
            _state = newState;
            value = newValue;
            lineN = newLineNr;
        }

        public void PrintToken(int count)
        {
            var id = (" " + count).PadRight(4);
            var ln = (" " + lineN).PadRight(5);
            var type = (" " + _state).PadRight(16);
            Console.WriteLine($"{id}|{ln}|{type}|{value.GetType().ToString()}|{value}");
            //Console.WriteLine($"{id}|{ln}|{type}|{value}");
        }
    }
}