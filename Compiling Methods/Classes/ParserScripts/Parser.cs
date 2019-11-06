using System;
using System.Collections.Generic;
using CompilingMethods.Classes.Exceptions;
using CompilingMethods.Classes.Lexer;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.ParserScripts
{
    public class Parser
    {
        private readonly string scriptName;
        private readonly List<Token> tokens;
        private readonly List<string> typeNames = new List<string>();
        private Token currentToken;
        private int offset;

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
            if (currentToken.State != type) return null;
            offset++;
            currentToken = tokens[offset];
            return tokens[offset - 1];

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
                return tokens[offset - 1];
            }

            ThrowError(currentToken);
            return null;
        }

        public Root ParseProgram()
        {
            var decls = new List<INode>();
            while (tokens.Count != 0 && tokens[offset].State != TokenType.Eof)
            {
                decls.Add(ParseDecl());
                if (currentToken.State == TokenType.Separator)
                    Expect(TokenType.Separator);
            }
            var body = new Body(decls);
            return new Root(body);
        }

        private ExprConst ParseLitInt()
        {
            return new ExprConst(Expect(TokenType.LitInt));
        }

        private ExprConst ParseLitFloat()
        {
            return new ExprConst(Expect(TokenType.LitFloat));
        }

        private ExprConst ParseLitStr()
        {
            return new ExprConst(Expect(TokenType.LitStr));
        }

        private ExprConst ParseTrue()
        {
            return new ExprConst(Expect(TokenType.True));
        }

        private ExprConst ParseFalse()
        {
            return new ExprConst(Expect(TokenType.False));
        }

        private IExpression ParseExprVar(Token ident)
        {
            var result = new ExprVar(Expect(TokenType.Ident));
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
            while (Accept(TokenType.OpComma) != null) args.Add(ParseExpr());

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
                result = new ExprBin(BinExpression.Mul, result, ParseExprPrimary());
            while (true)
                if (Accept(TokenType.OpMul) != null)
                    result = new ExprBin(BinExpression.Mul, result, ParseExprMul());
                else if (Accept(TokenType.OpDiv) != null)
                    result = new ExprBin(BinExpression.Div, result, ParseExprMul());
                else if (Accept(TokenType.OpAssMul) != null)
                    result = new ExprBin(BinExpression.AssMul, result, ParseExprMul());
                else if (Accept(TokenType.OpAssDiv) != null)
                    result = new ExprBin(BinExpression.AssDiv, result, ParseExprMul());
                else
                    break;

            return result;
        }

        private IExpression ParseExprAdd()
        {
            var result = ParseExprMul();
            while (true)
                if (Accept(TokenType.OpAdd) != null)
                    result = new ExprBin(BinExpression.Add, result, ParseExprMul());
                else if (Accept(TokenType.OpAssAdd) != null)
                    result = new ExprBin(BinExpression.AssAdd, result, ParseExprMul());
                else if (Accept(TokenType.OpSub) != null)
                    result = new ExprBin(BinExpression.Sub, result, ParseExprMul());
                else if (Accept(TokenType.OpAssSub) != null)
                    result = new ExprBin(BinExpression.AssSub, result, ParseExprMul());
                else
                    break;
            return result;
        }

        private IExpression ParseExprCmpEquals()
        {
            var result = ParseExprAdd();
            while (true)
                if (Accept(TokenType.OpEqual) != null)
                    result = new ExprBin(BinExpression.Equal, result, ParseExprAdd());
                else if (Accept(TokenType.OpNotEqual) != null)
                    result = new ExprBin(BinExpression.NotEqual, result, ParseExprAdd());
                else
                    break;

            return result;
        }

        private IExpression ParseExprCmp()
        {
            var result = ParseExprCmpEquals();
            while (true)
                if (Accept(TokenType.OpLess) != null)
                    result = new ExprBin(BinExpression.Less, result, ParseExprCmpEquals());
                else if (Accept(TokenType.OpMore) != null)
                    result = new ExprBin(BinExpression.More, result, ParseExprCmpEquals());
                else if (Accept(TokenType.OpLessEqual) != null)
                    result = new ExprBin(BinExpression.LessEqual, result, ParseExprCmpEquals());
                else if (Accept(TokenType.OpMoreEqual) != null)
                    result = new ExprBin(BinExpression.MoreEqual, result, ParseExprCmpEquals());
                else
                    break;
            return result;
        }

        private IExpression ParseExprAnd()
        {
            var result = ParseExprCmp();
            while (true)
                if (Accept(TokenType.OpAnd) != null)
                    result = new ExprBin(BinExpression.And, result, ParseExprCmp());
                else
                    break;

            return result;
        }

        private IExpression ParseExprOr()
        {
            var result = ParseExprAnd();
            while (true)
                if (Accept(TokenType.OpOr) != null)
                    result = new ExprBin(BinExpression.Or, result, ParseExprAnd());
                else
                    break;

            return result;
        }

        private IExpression ParseExpr()
        {
            return ParseExprOr();
        }

        private IExpression ParseExprParen()
        {
            Expect(TokenType.ParenOp);
            var exp = ParseExpr();
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
            while (Accept(TokenType.BracesCl) == null) statements.AddRange(ParseStatement());

            return statements;
        }

        private StmtIf ParseStmtIf()
        {
            Expect(TokenType.If);
            var cond = ParseExprParen();
            var body = ParseStmtBlock();
            return new StmtIf(cond, body);
        }

        private StmtElif ParseStmtElif()
        {
            var ifStmt = ParseStmtIf();
            var elifs = new List<StmtIf>();
            var elseObj = new List<IStatement>();
            while (Accept(TokenType.Else) != null)
            {
                List<IStatement> stmtBlock;
                if (Accept(TokenType.If) != null)
                {
                    var expression = ParseExprParen();
                    stmtBlock = ParseStmtBlock();
                    elifs.Add(new StmtIf(expression, stmtBlock));
                }
                else
                {
                    stmtBlock = ParseStmtBlock();
                    elseObj = stmtBlock;
                    //stmts.Add(new StmtElse(stmtBlock));

                    break;
                }

                //stmts.Add(new StmtIf(expression, stmtBlock));
            }

            return new StmtElif(ifStmt, elifs, elseObj);
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
            return new StmtKeyword(Keyword.Break);
        }
        
        private IStatement ParseStmtContinue()
        {
            Expect(TokenType.Continue);
            return new StmtKeyword(Keyword.Continue);
        }

        private IStatement ParseStmtReturn()
        {
            Expect(TokenType.Return);
            if (currentToken.State == TokenType.Separator) return new StmtKeywordExpr(Keyword.Return, null);
            var expr = ParseExpr();
            return new StmtKeywordExpr(Keyword.Return, expr);
        }


        private IEnumerable<IStatement> ParseStatement()
        {
            var list = new List<IStatement>();
            switch (currentToken.State)
            {
                case TokenType.If:
                    list.Add(ParseStmtElif());
                    return list;
                case TokenType.While:
                    list.Add(ParseStmtWhile());
                    return list;
                case TokenType.Break:
                    list.Add(ParseStmtBreak());
                    Expect(TokenType.Separator);
                    return list;
                case TokenType.Continue:
                    list.Add(ParseStmtContinue());
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
                return new StmtVar(new TypePrim(type.State), name);
            return currentToken.State switch
            {
                TokenType.ParenOp => ParseFnDecl(type, name.Value),
                TokenType.OpAssign => new StmtVar(new TypePrim(type.State), name, ParseAssign()),
                _ => null
            };
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
            while (Accept(TokenType.OpComma) != null) args.Add(ParseExpr());

            Expect(TokenType.ParenCl);
            return new StmtFnCall(ident, args);
        }

        private DeclFn ParseFnDecl(Token type, string name)
        {
            var paramList = ParseParams();
            var statementList = new List<IStatement>();
            if (Accept(TokenType.Separator) == null)
                statementList = ParseStmtBlock();
            return new DeclFn(new TypePrim(type.State), name, paramList, statementList);
        }

        private StmtVar ParseVarDecl()
        {
            var type = ParseType();
            var name = Expect(TokenType.Ident);
            if (Accept(TokenType.Separator) != null)
                return new StmtVar(new TypePrim(type.State), name);
            var exp = ParseAssign();
            Expect(TokenType.Separator);
            return new StmtVar(new TypePrim(type.State), name, exp);
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
            return new Param(name, new TypePrim(type.State));
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
            while (Accept(TokenType.OpComma) != null) paramList.Add(ParseParam());

            Expect(TokenType.ParenCl);
            return paramList;
        }
    }
}