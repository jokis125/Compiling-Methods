using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private Token currentToken;
        private int offset = 0;
        private bool running = true;
        private List<String> typeNames = new List<string>();

        public Parser()
        {
            tokens = new List<Token>();
            currentToken = tokens[0];
            FillTypeNames();
        }
        public Parser(List<Token> newTokens)
        {
            tokens = newTokens;
            currentToken = tokens[0];
            FillTypeNames();
        }

        private Token Accept(TokenType type)
        {
            currentToken = tokens[offset];
            if (currentToken.GetState() == type)
            {
                offset++;
                currentToken = tokens[offset];
                return tokens[offset-1];
            }
            return null;
        }

        private void FillTypeNames()
        {
            typeNames.Add("int");
            typeNames.Add("float");
            typeNames.Add("string");
            typeNames.Add("char");
            typeNames.Add("void");
        }

        private void ThrowError(TokenType badToken)
        {
            //Console.WriteLine($"Bad token {badToken}");
            throw new InvalidOperationException($"Bad token {badToken}");
            running = false;
        }

        private Token Expect(TokenType type)
        {
            currentToken = tokens[offset];
            if (currentToken.GetState() == type)
            {
                offset++;
                currentToken = tokens[offset];
                return tokens[offset-1];
            }
            ThrowError(currentToken.GetState());
            return null;
        }

        public void ParseProgram()
        {
            while (tokens.Count != 0 && tokens[offset].GetState() != TokenType.Eof && running)
            {
                ParseDecl();
                //ParseDecl();
            }
            if(running && tokens.Count != 0)
                Console.WriteLine("Yay!");
        }

        private void ParseLitInt()
        {
            Expect(TokenType.LitInt);
        }
        private void ParseLitFloat()
        {
            Expect(TokenType.LitFloat);
        }

        private void ParseLitStr()
        {
            Expect(TokenType.LitStr);
        }

        private void ParseExprVar()
        {
            Expect(TokenType.Ident);
            if (currentToken.GetState() == TokenType.ParenOp)
                ParseFnCall();
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
                case TokenType.LitFloat:
                    ParseLitFloat();
                    break;
                case TokenType.LitStr:
                    ParseLitStr();
                    break;
                case TokenType.True:
                    break;
                case TokenType.False:
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
            while (Accept(TokenType.OpMul) != null)
            {
                ParseExprPrimary();
            }
        }

        private void ParseExprAdd()
        {
            ParseExprMul();
            while (true)
            {
                if(Accept(TokenType.OpAdd) != null)
                    ParseExprMul();
                else if (Accept(TokenType.OpSub) != null)
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
            switch (currentToken.GetState())
            {
                case TokenType.Boolean:
                    Expect(TokenType.Boolean);
                    break;
                case TokenType.Float:
                    Expect(TokenType.Float);
                    break;
                case TokenType.Int:
                    Expect(TokenType.Int);
                    break;
                case TokenType.String:
                    Expect(TokenType.String);
                    break;
                case TokenType.Char:
                    Expect(TokenType.Char);
                    break;
                case TokenType.Void:
                    Expect(TokenType.Void);
                    break;
            }
        }

        private void ParseStmtBlock()
        {
            Expect(TokenType.BracesOp);
            while (Accept(TokenType.BracesCl)  == null)
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

        private void ParseStmtElif()
        {
            ParseStmtIf();
            while((Accept(TokenType.Else) != null))
            {
                if (Accept(TokenType.If) != null)
                {
                    ParseExprParen();
                    ParseStmtBlock();
                }
                else
                {
                    ParseStmtBlock();
                    break;
                }
            }
        }

        private void ParseStmtReturn()
        {
            var tokenType = tokens[offset].GetState();
            Expect(TokenType.Return);
            if(tokenType != TokenType.Separator)
                ParseExpr();
            Expect(TokenType.Separator);
        }
        

        private void ParseStatement()
        {
            switch (currentToken.GetState())
            {
                case TokenType.If:
                    ParseStmtElif();
                    break;
                case TokenType.Return:
                    ParseStmtReturn();
                    break;
                case TokenType type when typeNames.Contains(type.ToString().ToLower()):
                    ParseVarDecl();
                    Expect(TokenType.Separator);
                    break;
                case TokenType.Ident:
                    ParseCall();
                    Expect(TokenType.Separator);
                    break;
                default:
                    ThrowError(currentToken.GetState());
                    break;
            }
        }

        private void ParseDecl()
        {
            ParseType();
            Expect(TokenType.Ident);
            if (Accept(TokenType.Separator) != null)
                return;
            switch (currentToken.GetState())
            {
                case TokenType.ParenOp:
                    ParseFnDecl();
                    break;
                case TokenType.OpAssign:
                    ParseAssign();
                    Expect(TokenType.Separator);
                    break;
            }
        }

        private void ParseCall()
        {
            Expect(TokenType.Ident);
            switch (currentToken.GetState())
            {
                case TokenType.ParenOp:
                    ParseFnCall();
                    break;
                case TokenType.OpAssign:
                    ParseAssign();
                    break;
            }
        }

        private void ParseFnCall()
        {
            Expect(TokenType.ParenOp);
            if (currentToken.GetState() == TokenType.ParenCl)
            {
                Expect(TokenType.ParenCl);
                return;
            }
            ParseExpr();
            while (Accept(TokenType.OpComma) != null)
            {
                ParseExpr();
            }

            Expect(TokenType.ParenCl);
        }

        private void ParseArguments()
        {
            ParseExpr();
        }

        private void ParseFnDecl()
        {
            ParseParams();
            if(Accept(TokenType.Separator) == null)
                ParseStmtBlock();
        }

        private void ParseVarDecl()
        {
            ParseType();
            Expect(TokenType.Ident);
            if(Accept(TokenType.Separator) != null)
                return;
            ParseAssign();
        }
        
        private void ParseAssign()
        {
            Expect(TokenType.OpAssign);
            ParseExpr();
        }

        private void ParseParam()
        {
            ParseType();
            Expect(TokenType.Ident);
        }

        private void ParseParams()
        {
            //var tokenType = tokens[offset].GetState();
            Expect(TokenType.ParenOp);
            if (currentToken.GetState() == TokenType.ParenCl)
            {
                Expect(TokenType.ParenCl);
                return;
            }

            ParseParam();
            while (Accept(TokenType.OpComma) != null)
            {
                ParseParam();
            }

            Expect(TokenType.ParenCl);
        }
    }
}