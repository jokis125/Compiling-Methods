using System;
using System.Collections.Generic;
using CompilingMethods.Classes.Exceptions;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private Token currentToken;
        private int offset;
        private readonly List<string> typeNames = new List<string>();
        private readonly string scriptName;
        public Parser(List<Token> newTokens, string filename)
        {
            tokens = newTokens;
            currentToken = tokens[0];
            FillTypeNames();
            scriptName = filename;
        }

        private Token Accept(TokenType type)
        {
            currentToken = tokens[offset];
            if (currentToken.State == type)
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
            typeNames.Add("boolean");
        }

        private void ThrowError(Token badToken)
        {
            throw new BadTokenException($"Bad token {badToken.State} in {scriptName}:line {badToken.LineN}");
        }

        private Token Expect(TokenType type)
        {
            currentToken = tokens[offset];
            if (currentToken.State == type)
            {
                offset++;
                currentToken = tokens[offset];
                return tokens[offset-1];
            }
            ThrowError(currentToken);
            return null;
        }

        public List<INode> ParseProgram()
        {
            var decls = new List<INode>();
            while (tokens.Count != 0 && tokens[offset].State != TokenType.Eof)
            {
                decls.Add(ParseDecl());
            }
            if(tokens.Count != 0)
                Console.WriteLine("Yay!");
            return decls;
        }

        private ExprLit ParseLitInt()
        {
            return new ExprLit(Expect(TokenType.LitInt));
        }
        private ExprLit ParseLitFloat()
        {
            return new ExprLit(Expect(TokenType.LitFloat));
        }

        private ExprLit ParseLitStr()
        {
            return new ExprLit(Expect(TokenType.LitStr));
        }
        
        private ExprLit ParseTrue()
        {
            return new ExprLit(Expect(TokenType.True));
        }
        
        private ExprLit ParseFalse()
        {
            return new ExprLit(Expect(TokenType.False));
        }

        private IExpression ParseExprVar(Token ident)
        {
            var result = new ExprLit(Expect(TokenType.Ident));
            if (currentToken.State == TokenType.ParenOp)
                return ParseExprCall(ident);
            return result;
        }
        
        private ExprFnCall ParseExprCall(Token ident)
        {
            Expect(TokenType.ParenOp);
            var args = new List<IExpression>();
            if (currentToken.State == TokenType.ParenCl)
            {
                Expect(TokenType.ParenCl);
                return new ExprFnCall(ident, args);
            }
            args.Add(ParseExpr());
            while (Accept(TokenType.OpComma) != null)
            {
                args.Add(ParseExpr());
            }

            Expect(TokenType.ParenCl);
            return new ExprFnCall(ident, args);
        }

        private IExpression ParseExprPrimary()
        {
            var tokenType = tokens[offset].State;
            switch (tokenType)
            {
                case TokenType.LitInt:
                    return ParseLitInt();
                case TokenType.Ident:
                    return ParseExprVar(tokens[offset]);
                case TokenType.LitFloat:
                    return ParseLitFloat();
                case TokenType.LitStr:
                    return ParseLitStr();
                case TokenType.True:
                    return ParseTrue();
                case TokenType.False:
                    return ParseFalse();
                case TokenType.ParenOp:
                    return ParseExprParen();
                default:
                    ThrowError(currentToken);
                    break;
            }
            return null;
        }

        private IExpression ParseExprMul()
        {
            var result = ParseExprPrimary();
            while (Accept(TokenType.OpMul) != null || Accept(TokenType.OpDiv) != null)
            {
                result = new ExprBin(TokenType.OpMul, result, ParseExprPrimary());
            }
            while (true)
            {
                if (Accept(TokenType.OpMul) != null)
                    result = new ExprBin(TokenType.OpMul, result, ParseExprMul());
                else if (Accept(TokenType.OpDiv) != null)
                    result = new ExprBin(TokenType.OpDiv, result, ParseExprMul());
                else if (Accept(TokenType.OpAssMul) != null)
                    result = new ExprBin(TokenType.OpAssMul, result, ParseExprMul());
                else if(Accept(TokenType.OpAssDiv) != null)
                    result = new ExprBin(TokenType.OpAssDiv, result, ParseExprMul());
                else
                    break;
            }

            return result;
        }

        private IExpression ParseExprAdd()
        {
            var result = ParseExprMul();
            while (true)
            {
                if (Accept(TokenType.OpAdd) != null)
                {
                    result = new ExprBin(TokenType.OpAdd, result, ParseExprMul());
                }
                else if (Accept(TokenType.OpAssAdd) != null)
                {
                    result = new ExprBin(TokenType.OpAssAdd, result, ParseExprMul());
                }
                else if (Accept(TokenType.OpSub) != null)
                    result = new ExprBin(TokenType.OpSub, result, ParseExprMul());
                else if(Accept(TokenType.OpAssSub) != null)
                    result = new ExprBin(TokenType.OpAssSub, result, ParseExprMul());
                else
                    break;
            }
            return result;
        }
        
        private IExpression ParseExprCmpEquals()
        {
            var result = ParseExprAdd();
            while (true)
            {
                if (Accept(TokenType.OpEqual) != null)
                    result = new ExprBin(TokenType.OpEqual, result, ParseExprAdd());
                else if (Accept(TokenType.OpNotEqual) != null)
                    result = new ExprBin(TokenType.OpNotEqual,  result, ParseExprAdd());
                else
                    break;
            }

            return result;
        }
        
        private IExpression ParseExprCmp()
        {
            var result = ParseExprCmpEquals();
            while (true)
            {
                if(Accept(TokenType.OpLess) != null)
                    result = new ExprBin(TokenType.OpLess, result, ParseExprCmpEquals());
                else if (Accept(TokenType.OpMore) != null)
                    result = new ExprBin(TokenType.OpMore, result, ParseExprCmpEquals());
                else if (Accept(TokenType.OpLessEqual) != null)
                    result = new ExprBin(TokenType.OpLessEqual,  result, ParseExprCmpEquals());
                else if (Accept(TokenType.OpMoreEqual) != null)
                    result = new ExprBin(TokenType.OpMoreEqual, result, ParseExprCmpEquals());
                else
                    break;
            }
            return result;
        }
        
        private IExpression ParseExprAnd()
        {
            var result = ParseExprCmp();
            while (true)
            {
                if(Accept(TokenType.OpAnd) != null)
                    result = new ExprBin(TokenType.OpAnd,  result, ParseExprCmp());
                else
                    break;
            }

            return result;
        }
        private IExpression ParseExprOr()
        {
            var result = ParseExprAnd();
            while (true)
            {
                if(Accept(TokenType.OpOr) != null)
                    result = new ExprBin(TokenType.OpOr, result, ParseExprAnd());
                else
                    break;
            }

            return result;
        }

        private IExpression ParseExpr()
        {
            return ParseExprOr();
        }

        private IExpression ParseExprParen()
        {
            Expect(TokenType.ParenOp);
            var exp =  ParseExpr();
            Expect(TokenType.ParenCl);
            return exp;
        }

        private Token ParseType()
        {
            switch (currentToken.State)
            {
                case TokenType.Boolean:
                    //return new TypePrim(Expect(TokenType.Boolean));
                    return Expect(TokenType.Boolean);
                case TokenType.Float:
                    return Expect(TokenType.Float);
                case TokenType.Int:
                    return Expect(TokenType.Int);
                case TokenType.String:
                    return Expect(TokenType.String);
                case TokenType.Char:
                    return Expect(TokenType.Char);
                case TokenType.Void:
                    return Expect(TokenType.Void);
            }
            return null;
        }

        private List<IStatement> ParseStmtBlock()
        {
            Expect(TokenType.BracesOp);
            var statements = new List<IStatement>();
            while (Accept(TokenType.BracesCl)  == null)
            {
                statements.AddRange(ParseStatement());
            }

            return statements;
        }

        private IStatement ParseStmtIf()
        {
            Expect(TokenType.If);
            var cond = ParseExprParen();
            var body = ParseStmtBlock();
            return new StmtIf(cond, body);
        }

        private List<IStatement> ParseStmtElif()
        {
            var stmts = new List<IStatement> {ParseStmtIf()};
            while((Accept(TokenType.Else) != null))
            {
                IExpression expression;
                List<IStatement> stmtBlock;
                if (Accept(TokenType.If) != null)
                {
                    expression = ParseExprParen();
                    stmtBlock = ParseStmtBlock();
                }
                else
                {
                    stmtBlock = ParseStmtBlock();
                    stmts.Add(new StmtElse(stmtBlock));
                    break;
                }
                stmts.Add(new StmtIf(expression, stmtBlock));
            }
            return stmts;
        }

        private StmtWhile ParseStmtWhile()
        {
            Expect(TokenType.While);
            var cond = ParseExprParen();
            var body = ParseStmtBlock();
            return new StmtWhile(cond, body);
        }
        private IStatement ParseStmtBreak()
        {
            Expect(TokenType.Break);
            return new StmtBreak();
        }

        private IStatement ParseStmtReturn()
        {
            Expect(TokenType.Return);
            if (currentToken.State != TokenType.Separator)
            {
                var expr = ParseExpr();
                return new StmtRet(expr);
            }
            return new StmtRet(null);
        }
        

        private List<IStatement> ParseStatement()
        {
            var list = new List<IStatement>();
            switch (currentToken.State)
            {
                case TokenType.If:
                    list = ParseStmtElif();
                    return list;
                case TokenType.While:
                    list.Add(ParseStmtWhile());
                    return list;
                case TokenType.Break:
                    list.Add(ParseStmtBreak());
                    Expect(TokenType.Separator);
                    return list;
                case TokenType.Return:
                    list.Add(ParseStmtReturn());
                    Expect(TokenType.Separator);
                    return list;
                case TokenType type when typeNames.Contains(type.ToString().ToLower()):
                    list.Add(ParseVarDecl());
                    return list;
                case TokenType.Ident:
                    list.Add(ParseCall());
                    Expect(TokenType.Separator);
                    return list;
                default:
                    ThrowError(currentToken);
                    break;
            }

            return null;
        }

        private INode ParseDecl()
        {
            var type = ParseType();
            var name = Expect(TokenType.Ident);
            if (Accept(TokenType.Separator) != null)
                return new StmtVar(name, type);
            switch (currentToken.State)
            {
                case TokenType.ParenOp:
                    return ParseFnDecl(type, name.Value);
                case TokenType.OpAssign:
                    return new StmtVar(type, name, ParseAssign());
            }

            return null;
        }

        private IStatement ParseCall()
        {
            var ident = Expect(TokenType.Ident);
            IExpression expr;
            switch (currentToken.State)
            {
                case TokenType.ParenOp:
                    return ParseFnCall(ident);
                case TokenType.OpAssign:
                    expr = ParseAssign();
                    return new StmtVarAssign(ident, TokenType.OpAssign, expr);
                case TokenType.OpAssAdd:
                    expr = ParseAssign();
                    return new StmtVarAssign(ident, TokenType.OpAssAdd, expr);
                case TokenType.OpAssSub:
                    expr = ParseAssign();
                    return new StmtVarAssign(ident, TokenType.OpAssSub, expr);
                case TokenType.OpAssDiv:
                    expr = ParseAssign();
                    return new StmtVarAssign(ident, TokenType.OpAssDiv, expr);
                case TokenType.OpAssMul:
                    expr = ParseAssign();
                    return new StmtVarAssign(ident, TokenType.OpAssMul, expr);
            }
            return null;
        }

        private StmtFnCall ParseFnCall(Token ident)
        {
            Expect(TokenType.ParenOp);
            var args = new List<IExpression>();
            if (currentToken.State == TokenType.ParenCl)
            {
                Expect(TokenType.ParenCl);
                return new StmtFnCall(ident);
            }
            args.Add(ParseExpr());
            while (Accept(TokenType.OpComma) != null)
            {
                args.Add(ParseExpr());
            }

            Expect(TokenType.ParenCl);
            return new StmtFnCall(ident, args);
        }

        private DeclFn ParseFnDecl(Token type, string name)
        {
            var paramList = ParseParams();
            var statementList = new List<IStatement>();
            if(Accept(TokenType.Separator) == null)
                statementList = ParseStmtBlock();
            return new DeclFn(type, name, paramList, statementList);
        }

        private StmtVar ParseVarDecl()
        {
            var type = ParseType();
            var name = Expect(TokenType.Ident);
            if(Accept(TokenType.Separator) != null)
                return new StmtVar(type, name);
            var exp = ParseAssign();
            Expect(TokenType.Separator);
            return new StmtVar(type, name, exp);
        }
        
        private IExpression ParseAssign()
        {
            switch (currentToken.State)
            {
                case TokenType.OpAssign:
                    Expect(TokenType.OpAssign);
                    break;
                case TokenType.OpAssAdd:
                    Expect(TokenType.OpAssAdd);
                    break;
                case TokenType.OpAssSub:
                    Expect(TokenType.OpAssSub);
                    break;
                case TokenType.OpAssDiv:
                    Expect(TokenType.OpAssDiv);
                    break;
                case TokenType.OpAssMul:
                    Expect(TokenType.OpAssMul);
                    break;
            }
            return ParseExpr();
        }

        private Param ParseParam()
        {
            var type = ParseType();
            var name = Expect(TokenType.Ident);
            return new Param(name, type);
        }

        private List<Param> ParseParams()
        {
            var paramList = new List<Param>();
            Expect(TokenType.ParenOp);
            if (currentToken.State == TokenType.ParenCl)
            {
                Expect(TokenType.ParenCl);
                return paramList;
            }

            paramList.Add(ParseParam());
            while (Accept(TokenType.OpComma) != null)
            {
                paramList.Add(ParseParam());
            }

            Expect(TokenType.ParenCl);
            return paramList;
        }
    }
}