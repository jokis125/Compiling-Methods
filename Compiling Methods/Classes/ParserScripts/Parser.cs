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
        private readonly Dictionary<TokenType, string> tokenToString = new Dictionary<TokenType, string>();

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

        private void PopulateDict()
        {
            tokenToString.Add(TokenType.OpAdd, "+");
            tokenToString.Add(TokenType.OpAssAdd, "+=");
            tokenToString.Add(TokenType.OpSub, "-");
            tokenToString.Add(TokenType.OpAssSub, "-=");
            tokenToString.Add(TokenType.Separator, ";");
            tokenToString.Add(TokenType.OpDiv, "/");
            tokenToString.Add(TokenType.OpMul, "*");
            tokenToString.Add(TokenType.OpBinAnd, "&");
            tokenToString.Add(TokenType.OpBinOr, "|");
            tokenToString.Add(TokenType.OpComma, ",");
            tokenToString.Add(TokenType.ParenOp, "(");
            tokenToString.Add(TokenType.ParenCl, ")");
            tokenToString.Add(TokenType.BracesCl, "}");
            tokenToString.Add(TokenType.BracesOp, "{");
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
            PopulateDict();
            if (tokenToString.TryGetValue(badToken.State, out var result))
                throw new BadTokenException($"{scriptName}:{badToken.LineN} Error: Unexpected {result}");
            if(badToken.State == TokenType.Ident)
                throw new BadTokenException($" in {scriptName}:{badToken.LineN} Error: Unexpected " +
                                            $"identifier \"{badToken.Value}\"");
            throw new BadTokenException($"in {scriptName}:{badToken.LineN} Error: Unexpected Token {badToken.State}");
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
            var decls = new List<IDeclares>();
            while (tokens.Count != 0 && tokens[offset].State != TokenType.Eof)
            {
                decls.Add(ParseDecl());
                if (currentToken.State == TokenType.Separator)
                    Expect(TokenType.Separator);
            }
            return new Root(decls);
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
                result = new ExprBinaryArithmetic(ExprBinKind.Mul, result, ParseExprPrimary());
            while (true)
                if (Accept(TokenType.OpMul) != null)
                    result = new ExprBinaryArithmetic(ExprBinKind.Mul, result, ParseExprMul());
                else if (Accept(TokenType.OpDiv) != null)
                    result = new ExprBinaryArithmetic(ExprBinKind.Div, result, ParseExprMul());
                else if (Accept(TokenType.OpAssMul) != null)
                    result = new ExprBinaryArithmetic(ExprBinKind.AssMul, result, ParseExprMul());
                else if (Accept(TokenType.OpAssDiv) != null)
                    result = new ExprBinaryArithmetic(ExprBinKind.AssDiv, result, ParseExprMul());
                else
                    break;

            return result;
        }

        private IExpression ParseExprAdd()
        {
            var result = ParseExprMul();
            while (true)
                if (Accept(TokenType.OpAdd) != null)
                    result = new ExprBinaryArithmetic(ExprBinKind.Add, result, ParseExprMul());
                else if (Accept(TokenType.OpAssAdd) != null)
                    result = new ExprBinaryArithmetic(ExprBinKind.AssAdd, result, ParseExprMul());
                else if (Accept(TokenType.OpSub) != null)
                    result = new ExprBinaryArithmetic(ExprBinKind.Sub, result, ParseExprMul());
                else if (Accept(TokenType.OpAssSub) != null)
                    result = new ExprBinaryArithmetic(ExprBinKind.AssSub, result, ParseExprMul());
                else
                    break;
            return result;
        }

        private IExpression ParseExprCmpEquals()
        {
            var result = ParseExprAdd();
            while (true)
                if (Accept(TokenType.OpEqual) != null)
                    result = new ExprBinaryEquality(ExprBinKind.Equal, result, ParseExprAdd());
                else if (Accept(TokenType.OpNotEqual) != null)
                    result = new ExprBinaryEquality(ExprBinKind.NotEqual, result, ParseExprAdd());
                else
                    break;

            return result;
        }

        private IExpression ParseExprCmp()
        {
            var result = ParseExprCmpEquals();
            while (true)
                if (Accept(TokenType.OpLess) != null)
                    result = new ExprBinaryComparison(ExprBinKind.Less, result, ParseExprCmpEquals());
                else if (Accept(TokenType.OpMore) != null)
                    result = new ExprBinaryComparison(ExprBinKind.More, result, ParseExprCmpEquals());
                else if (Accept(TokenType.OpLessEqual) != null)
                    result = new ExprBinaryComparison(ExprBinKind.LessEqual, result, ParseExprCmpEquals());
                else if (Accept(TokenType.OpMoreEqual) != null)
                    result = new ExprBinaryComparison(ExprBinKind.MoreEqual, result, ParseExprCmpEquals());
                else
                    break;
            return result;
        }

        private IExpression ParseExprAnd()
        {
            var result = ParseExprCmp();
            while (true)
                if (Accept(TokenType.OpAnd) != null)
                    result = new ExprBinaryLogic(ExprBinKind.And, result, ParseExprCmp());
                else
                    break;

            return result;
        }

        private IExpression ParseExprOr()
        {
            var result = ParseExprAnd();
            while (true)
                if (Accept(TokenType.OpOr) != null)
                    result = new ExprBinaryLogic(ExprBinKind.Or, result, ParseExprAnd());
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

        private Branch ParseStmtIf()
        {
            Expect(TokenType.If);
            var cond = ParseExprParen();
            var body = ParseStmtBlock();
            return new Branch(cond, new StmtBlock(body));
        }

        private StmtIf ParseStmtElif()
        {
            StmtBlock elseObj = null;
            var branches = new List<Branch>();
            branches.Add(ParseStmtIf());
            while (Accept(TokenType.Else) != null)
            {
                List<IStatement> stmtBlock;
                if (Accept(TokenType.If) != null)
                {
                    var expression = ParseExprParen();
                    stmtBlock = ParseStmtBlock();
                    branches.Add(new Branch(expression, new StmtBlock(stmtBlock)));
                }
                else
                {
                    stmtBlock = ParseStmtBlock();
                    elseObj = new StmtBlock(stmtBlock);

                    break;
                }
            }

            return new StmtIf(branches, elseObj);
        }

        private StmtWhile ParseStmtWhile()
        {
            Expect(TokenType.While);
            var cond = ParseExprParen();
            var body = ParseStmtBlock();
            return new StmtWhile(cond, new StmtBlock(body));
        }

        private IStatement ParseStmtBreak()
        {
            return new StmtBreak(Expect(TokenType.Break));
        }
        
        private IStatement ParseStmtContinue()
        {
            return new StmtContinue(Expect(TokenType.Continue));
        }

        private IStatement ParseStmtReturn()
        {
            var ret = Expect(TokenType.Return);
            if (currentToken.State == TokenType.Separator) return new StmtReturn(ret);
            var expr = ParseExpr();
            return new StmtReturn(ret, expr);
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

        private IDeclares ParseDecl()
        {
            var type = ParseType();
            var name = Expect(TokenType.Ident);
            if (Accept(TokenType.Separator) != null)
                return new DeclVar(new TypePrim(type), name);
            return currentToken.State switch
            {
                TokenType.ParenOp => (IDeclares) ParseFnDecl(type, name),
                TokenType.OpAssign => new DeclVar(new TypePrim(type), name, ParseAssign()),
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
                return new StmtFnCall(new ExprFnCall(ident, args));
            }

            args.Add(ParseExpr());
            while (Accept(TokenType.OpComma) != null) args.Add(ParseExpr());

            Expect(TokenType.ParenCl);
            return new StmtFnCall(new ExprFnCall(ident, args));
        }

        private DeclFn ParseFnDecl(Token type, Token name)
        {
            var paramList = ParseParams();
            var statementList = new List<IStatement>();
            if (Accept(TokenType.Separator) == null)
                statementList = ParseStmtBlock();
            return new DeclFn(new TypePrim(type), name, paramList, statementList);
        }

        private StmtVar ParseVarDecl()
        {
            var type = ParseType();
            var name = Expect(TokenType.Ident);
            if (Accept(TokenType.Separator) != null)
                return new StmtVar(new TypePrim(type), name);
            var exp = ParseAssign();
            Expect(TokenType.Separator);
            return new StmtVar(new TypePrim(type), name, exp);
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
            return new Param(name, new TypePrim(type));
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