using System;
using System.Collections.Generic;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int offset = 0;
        private bool running = true;

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
            //Console.WriteLine($"Bad token {badToken}");
            throw new InvalidOperationException($"Bad token {badToken}");
            running = false;
        }

        private Token Expect(TokenType type)
        {
            var currentToken = tokens[offset];
            if (currentToken.GetState() == type)
            {
                offset++;
                return currentToken;
            }
            ThrowError(currentToken.GetState());
            return currentToken;
        }

        public void ParseProgram()
        {
            while (tokens.Count != 0 && tokens[offset].GetState() != TokenType.Eof && running)
            {
                ParseDecl();
            }
            if(running && tokens.Count != 0)
                Console.WriteLine("Yay!");
        }

        private void ParseLitInt()
        {
            Expect(TokenType.LitInt);
        }

        private void ParseExprVar()
        {
            Expect(TokenType.Ident);
        }

        private void ParseExprPrimary()
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
                case TokenType.ParenOp:
                    ParseExprParen();
                    break;
                default:
                    ThrowError(tokenType);
                    break;
            }
        }

        private void ParseExprMul()
        {
            ParseExprPrimary();
            while (Accept(TokenType.OpMul))
            {
                ParseExprPrimary();
            }
        }

        private void ParseExprAdd()
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

        private void ParseExpr()
        {
            ParseExprAdd();
        }

        private void ParseExprParen()
        {
            Expect(TokenType.ParenOp);
            ParseExpr();
            Expect(TokenType.ParenCl);
        }

        private void ParseType()
        {
            var tokenType = tokens[offset].GetState();
            switch (tokenType)
            {
                case TokenType.Boolean:
                    Expect(TokenType.Boolean);
                    break;
                case TokenType.Float:
                    Expect(TokenType.Float);
                    break;
                case TokenType.LitInt:
                    Expect(TokenType.LitInt);
                    break;
                case TokenType.Void:
                    Expect(TokenType.Void);
                    break;
            }
        }

        private void ParseStmtBlock()
        {
            Expect(TokenType.BracesOp);
            while (!Accept(TokenType.BracesCl))
            {
                ParseStatement();
            }
        }

        private void ParseStmtIf()
        {
            Expect(TokenType.If);
            ParseExprParen();
            ParseStmtBlock();
        }
        
        private void ParseStmtReturn()
        {
            var tokenType = tokens[offset].GetState();
            Expect(TokenType.Return);
            if(tokenType != TokenType.Separator)
                ParseExpr();
        }
        

        private void ParseStatement()
        {
            var tokenType = tokens[offset].GetState();
            switch (tokenType)
            {
                case TokenType.If:
                    ParseStmtIf();
                    break;
                case TokenType.Return:
                    ParseStmtReturn();
                    break;
            }
        }

        private void ParseDecl()
        {
            ParseFnDecl();
        }

        private void ParseFnDecl()
        {
            ParseType();
            Expect(TokenType.Ident);
            ParseParams();
            ParseStmtBlock();
        }

        private void ParseParam()
        {
            ParseType();
            Expect(TokenType.Ident);
        }

        private void ParseParams()
        {
            var tokenType = tokens[offset].GetState();
            Expect(TokenType.ParenOp);
            if(tokenType == TokenType.ParenCl)
                return;
            ParseParam();
            while (!Accept(TokenType.OpComma))
            {
                ParseParam();
            }
            
        }
    }
}