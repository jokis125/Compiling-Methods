using System;
using System.Collections.Generic;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int offset = 0;

        public Parser()
        {
            tokens = new List<Token>();
        }
        public Parser(List<Token> newTokens)
        {
            tokens = newTokens;
        }

        private bool Accept(TokenType type)
        {
            var currentToken = tokens[offset];
            if (currentToken.GetState() == type)
            {
                offset++;
                return true;
            }
            return false;
        }

        private void ThrowError(TokenType badToken)
        {
            Console.Write($"Bad token {badToken}");
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
            ThrowError(currentToken.GetState());
            return currentToken;
        }

        public void ParseLitInt()
        {
            Expect(TokenType.LitInt);
        }

        public void ParseExprVar()
        {
            Expect(TokenType.Ident);
        }

        public void ParseExprPrimary()
        {
            var tokenType = tokens[offset].GetState();
            switch (tokenType)
            {
                case TokenType.LitInt:
                    ParseLitInt();
                    break;
                case TokenType.Ident:
                    ParseExprVar();
                    break;
                default:
                    ThrowError(tokenType);
                    break;
            }
        }

        public void ParseExprMul()
        {
            ParseExprPrimary();
            while (Accept(TokenType.OpMul))
            {
                ParseExprPrimary();
            }
        }

        public void ParseExprAdd()
        {
            ParseExprMul();
            while (true)
            {
                if(Accept(TokenType.OpAdd))
                    ParseExprMul();
                else if (Accept(TokenType.OpSub))
                    ParseExprMul();
                else
                    break;
            }
        }
        
    }
}