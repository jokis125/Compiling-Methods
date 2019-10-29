using System;
using System.Collections.Generic;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes
{
    public class Parser
    {
        private List<Token> tokens;
        private int offset = 0;

        public Parser()
        {
            tokens = new List<Token>();
        }
        public Parser(List<Token> newTokens)
        {
            tokens = newTokens;
        }

        private Token Accept(TokenType type)
        {
            var currentToken = tokens[offset];
            if (currentToken.GetState() == type)
            {
                offset++;
                return currentToken;
            }
            return currentToken;
        }

        private void ThrowError(TokenType badToken, TokenType expectedToken)
        {
            Console.Write($"Bad token {badToken}, Expected token {expectedToken}");
        }

        private Token Expect(TokenType type)
        {
            var currentToken = tokens[offset];
            if (currentToken.GetState() == type)
            {
                offset++;
                Console.WriteLine("yay");
                return currentToken;
            }
            ThrowError(currentToken.GetState(), type);
            return currentToken;
        }

        public void ParseLitInt()
        {
            Expect(TokenType.LitInt);
        }
        
    }
}