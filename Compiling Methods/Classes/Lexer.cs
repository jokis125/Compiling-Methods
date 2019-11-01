using System;
using System.Collections.Generic;
using System.IO;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes
{
    public class Lexer
    {
        private string buffer;
        private char currentChar;
        private readonly string file;
        private readonly List<string> keywords = new List<string>();
        private readonly List<char> acceptableCharsAfterInt = new List<char>();
        private string allString;
        private State currentState = State.Start;
        private int line = 1;
        private int offset = 0;
        private int strAndComStart = 0;
        private bool printTokens = true;
        private readonly List<Token> tokens = new List<Token>();
        private int tokenStart = 0;
        private bool running = true;

        public Lexer(string input = "program.txt")
        {
            file = input;
            AddKeywords();
            AddChars();
        }

        public void GetText()
        {
            allString = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\"+file));
        }

        public void StartLexer()
        {
            LexAll();
        }

        public List<Token> GetTokens()
        {
            return tokens;
        }
        
        private void LexAll()
        {
            while (running && offset < allString.Length)
            {
                currentChar = allString[offset];
                LexChar();
                offset += 1;
            }

            if (running)
            {
                currentChar = ' ';
                LexChar();
                switch (currentState)
                {
                    case State.Start:
                        CompleteToken(TokenType.Eof);
                        break;
                    case State.LitStr:
                        PrintError(strAndComStart,$"Unterminated string");
                        printTokens = false;
                        break;
                    case State t when (t == State.CommentMlExit || t == State.CommentMl):
                        PrintError(strAndComStart,$"Unterminated comment");
                        printTokens = false;
                        break;
                }

                if (printTokens)
                {
                    for (var i = 0; i < tokens.Count; i++)
                    {
                        tokens[i].PrintToken(i);
                    }
                }
            }
            else
            {
                tokens.Clear();
            }
        }

        private void PrintError(int badLine, string errorMsg)
        {
            Console.WriteLine($"{file}: {badLine}: error: {errorMsg}");
        }

        private void AddKeywords()
        {
            keywords.Add("if");
            keywords.Add("else");
            keywords.Add("return");
            keywords.Add("break");
            keywords.Add("continue");
            keywords.Add("while");
            keywords.Add("int");
            keywords.Add("float");
            keywords.Add("string");
            keywords.Add("char");
            keywords.Add("void");
            keywords.Add("bool");
            keywords.Add("true");
            keywords.Add("false");
        }

        private void AddChars()
        {
            /*case char c when (!char.IsDigit(c) && c != ';' && c != ' ' && c != '\r' && c != '\n' && c != ')' && c != '}'
                              && c != ',' &&):*/
            acceptableCharsAfterInt.Add(';');
            acceptableCharsAfterInt.Add(' ');
            acceptableCharsAfterInt.Add('\r');
            acceptableCharsAfterInt.Add('\n');
            acceptableCharsAfterInt.Add(')');
            acceptableCharsAfterInt.Add('}');
            acceptableCharsAfterInt.Add(',');
            acceptableCharsAfterInt.Add('+');
            acceptableCharsAfterInt.Add('-');
            acceptableCharsAfterInt.Add('*');
            acceptableCharsAfterInt.Add('/');
            acceptableCharsAfterInt.Add('|');
            acceptableCharsAfterInt.Add('&');
            acceptableCharsAfterInt.Add('%');
        }

        private void BeginToken(State newState)
        {
            tokenStart = line;
            currentState = newState;
        }

        private void AddToBuffer(char c)
        {
            buffer += c;
        }


        private void LexStart()
        {
            switch (currentChar)
            {
                case char c when (Char.IsLetter(c)):
                    BeginToken(State.Ident);
                    AddToBuffer(currentChar);
                    break;
                case char c when (c == '_'):
                    BeginToken(State.Ident);
                    AddToBuffer(currentChar);
                    break;
                case char c when (Char.IsDigit(c)):
                    BeginToken(State.LitInt);
                    LexLitInt();
                    break;
                case char c when (c == '"'):
                    BeginToken(State.LitStr);
                    strAndComStart = line;
                    break;
                case char c when (c == '.'):
                    BeginToken(State.LitFloat);
                    AddToBuffer('0');
                    AddToBuffer('.');
                    break;
                case char c when (c == '\n'):
                    line++;
                    break;
                case char c when (c == ' '):
                    break;
                case char c when c == '\r':
                    break;
                case char c when c == '\t':
                    break;
                case '\\':
                    BeginToken(State.LitEsc);
                    break;
                case char c when c == '<' :
                    BeginToken(State.OpLess);
                    break;
                case char c when c == '>':
                    BeginToken(State.OpMore);
                    break;
                case char c when c == '=':
                    BeginToken(State.OpAssign);
                    break;
                case char c when c == '!':
                    BeginToken(State.OpNeg);
                    break;
                case char c  when c == '&':
                    BeginToken(State.OpBinAnd);
                    break;
                case char c when c == '|':
                    BeginToken(State.OpBinOr);
                    break;
                case char c when c == '(':
                    BeginToken(State.ParenOp);
                    CompleteToken(TokenType.ParenOp);
                    break;
                case char c when c == ')':
                    BeginToken(State.ParenCl);
                    CompleteToken(TokenType.ParenCl);
                    break;
                case char c when c == '{':
                    BeginToken(State.BracesOp);
                    CompleteToken(TokenType.BracesOp);
                    break;
                case char c when c == '}':
                    BeginToken(State.BracesCl);
                    CompleteToken(TokenType.BracesCl);
                    break;
                case char c when c == ',':
                    BeginToken(State.OpComma);
                    CompleteToken(TokenType.OpComma);
                    break;
                case char c when c == '+':
                    BeginToken(State.OpAdd);
                    break;
                case char c when c == '-':
                    BeginToken(State.OpSub);
                    break;
                case char c when c == '/':
                    BeginToken(State.OpDiv);
                    break;
                case char c when c == '*':
                    BeginToken(State.OpMul);
                    break;
                case char c when c == ';':
                    BeginToken(State.Separator);
                    CompleteToken(TokenType.Separator);
                    break;
                default:
                    running = false;
                    currentState = State.Unknown;
                    PrintError(line,$"Unexpected char {currentChar}");
                    break;
                    
            }
        }

        private void LexLitInt()
        {
            switch (currentChar)
            {
                case char c when (Char.IsDigit(c)):
                    AddToBuffer(currentChar);
                    break;
                case char c when (c == '.' || c == 'e'):
                    AddToBuffer(currentChar);
                    currentState = State.LitFloat;
                    break;
                case char c when (!char.IsDigit(c) && !acceptableCharsAfterInt.Contains(c)):
                    running = false;
                    AddToBuffer(currentChar);
                    PrintError(line,$"Bad int {buffer}");
                    break;
                default:
                    CompleteToken(TokenType.LitInt, false);
                    break;
            }
        }

        private void LexLitFloat()
        {
            switch (currentChar)
            {
                case char c when Char.IsDigit(c):
                    AddToBuffer(currentChar);
                    break;
                case char c when (c == 'e' && currentState != State.LitFloatExp):
                    AddToBuffer(currentChar);
                    currentState = State.LitFloatExp;
                    break;
                default:
                    CompleteToken(TokenType.LitFloat, false);
                    break;
                
            }
        }

        private void LexLitIdent()
        {
            switch (currentChar)
            {
                case char c when (Char.IsLetter(c)):
                    AddToBuffer(currentChar);
                    break;
                case char c when (c == '_'):
                    AddToBuffer(currentChar);
                    break;
                case char c when (Char.IsDigit(c)):
                    AddToBuffer(currentChar);
                    break;
                default:
                    //CompleteToken(State.Ident, false);
                    CompleteIdent();
                    break;
            }
        }

        private void CompleteIdent()
        {
            if (keywords.Contains(buffer))
            {
                switch (buffer)
                {
                    case string s when s == "if":
                        buffer = "";
                        CompleteToken(TokenType.If, false);
                        break;
                    case string s when s == "else":
                        buffer = "";
                        CompleteToken(TokenType.Else, false);
                        break;
                    case string s when s == "return":
                        buffer = "";
                        CompleteToken(TokenType.Return, false);
                        break;
                    case string s when s == "continue":
                        buffer = "";
                        CompleteToken(TokenType.Continue, false);
                        break;
                    case string s when s == "break":
                        buffer = "";
                        CompleteToken(TokenType.Break, false);
                        break;
                    case string s when s == "while":
                        buffer = "";
                        CompleteToken(TokenType.While, false);
                        break;
                    case "int":
                        buffer = "";
                        CompleteToken(TokenType.Int, false);
                        break;
                    case "float":
                        buffer = "";
                        CompleteToken(TokenType.Float, false);
                        break;
                    case "string":
                        buffer = "";
                        CompleteToken(TokenType.String, false);
                        break;
                    case "char":
                        buffer = "";
                        CompleteToken(TokenType.Char, false);
                        break;
                    case "bool":
                        buffer = "";
                        CompleteToken(TokenType.Boolean, false);
                        break;
                    case "void":
                        buffer = "";
                        CompleteToken(TokenType.Void, false);
                        break;
                    case "true":
                        buffer = "";
                        CompleteToken(TokenType.True, false);
                        break;
                    case "false":
                        buffer = "";
                        CompleteToken(TokenType.False, false);
                        break;
                }
                buffer = "";
            }
            else
                CompleteToken(TokenType.Ident, false);
        }

        private void LexLitStr()
        {
            switch (currentChar)
            {
                case '"':
                    CompleteToken(TokenType.LitStr);
                    break;
                case '\\':
                    currentState = State.LitEsc;
                    break;
                case '\n':
                    AddToBuffer('\n');
                    line++;
                    break;
                default:
                    AddToBuffer(currentChar);
                    break;
            }
        }

        private void LexLitStrEsc()
        {
            switch (currentChar)
            {
                case '"':
                    AddToBuffer('\"');
                    break;
                case 't':
                    AddToBuffer('\t');
                    break;
                case 'n':
                    AddToBuffer('\n');
                    break;
                default:
                    PrintError(line, $"Invalid escape sequence {currentChar}");
                    running = false;
                    break;
            }
            currentState = State.LitStr;
        }

        private void LexLitOpLess()
        {
            switch (currentChar)
            {
                case char c when c == '=':
                    CompleteToken(TokenType.OpLessEqual);
                    break;
                default:
                    CompleteToken(TokenType.OpLess, false);
                    break;
            }
        }
        
        private void LexLitOpMore()
        {
            switch (currentChar)
            {
                case char c when c == '=':
                    CompleteToken(TokenType.OpMoreEqual);
                    break;
                default:
                    CompleteToken(TokenType.OpMore, false);
                    break;
            }
        }
        
        private void LexLitOpAssign()
        {
            switch (currentChar)
            {
                case char c when c == '=':
                    CompleteToken(TokenType.OpEqual);
                    break;
                default:
                    CompleteToken(TokenType.OpAssign, false);
                    break;
            }
        }
        
        private void LexLitOpNotEqual()
        {
            switch (currentChar)
            {
                case char c when c == '=':
                    CompleteToken(TokenType.OpNotEqual);
                    break;
                default:
                    CompleteToken(TokenType.OpNeg, false);
                    break;
            }
        }

        private void CommentState()
        {
            switch (currentChar)
            {
                case '\n':
                    line++;
                    currentState = State.Start;
                    break;
            }
        }
        private void CommentStateMl()
        {
            switch (currentChar)
            {
                case '\n':
                    line++;
                    break;
                case '*':
                    currentState = State.CommentMlExit;
                    break;
                case char c when (c == '/' && currentState == State.CommentMlExit):
                    currentState = State.Start;
                    break;
                default:
                    currentState = State.CommentMl;
                    break;
            }
        }

        private void LexLitOpDiv()
        {
            switch (currentChar)
            {
                case '/':
                    currentState = State.Comment;
                    break;
                case '*':
                    currentState = State.CommentMl;
                    strAndComStart = line;
                    break;
                case '=':
                    CompleteToken(TokenType.OpAssDiv);
                    break;
                default:
                    CompleteToken(TokenType.OpDiv);
                    break;
            }
        }

        private void LexLitOpAdd()
        {
            switch (currentChar)
            {
                case '+':
                    CompleteToken(TokenType.OpInc);
                    break;
                case '=':
                    CompleteToken(TokenType.OpAssAdd);
                    break;
                default:
                    CompleteToken(TokenType.OpAdd, false);
                    break;
            }
        }
        
        private void LexLitOpSub()
        {
            switch (currentChar)
            {
                case '-':
                    CompleteToken(TokenType.OpDec);
                    break;
                case '=':
                    CompleteToken(TokenType.OpAssSub);
                    break;
                default:
                    CompleteToken(TokenType.OpSub, false);
                    break;
            }
        }
        
        private void LexLitOpMult()
        {
            switch (currentChar)
            {
                case '=':
                    CompleteToken(TokenType.OpAssMul);
                    break;
                default:
                    CompleteToken(TokenType.OpMul, false);
                    break;
            }
        }

        private void LexOpOr()
        {
            switch (currentChar)
            {
                case '|':
                    CompleteToken(TokenType.OpOr);
                    break;
                default:
                    CompleteToken(TokenType.OpBinOr, false);
                    break;
            }
        }

        private void LexOpAnd()
        {
            switch (currentChar)
            {
                case '&':
                    CompleteToken(TokenType.OpAnd);
                    break;
                default:
                    CompleteToken(TokenType.OpBinAnd, false);
                    break;
            }
        }


        private void LexChar()
        {
            switch (currentState)
            {
                case State.Start:
                    LexStart();
                    break;
                case State.LitInt:
                    LexLitInt();
                    break;
                case State.LitFloat:
                    LexLitFloat();
                    break;
                case State.LitFloatExp:
                    LexLitFloat();
                    break;
                case State.Ident:
                    LexLitIdent();
                    break;
                case State.LitStr:
                    LexLitStr();
                    break;
                case State.LitEsc:
                    LexLitStrEsc();
                    break;
                case State.Comment:
                    CommentState();
                    break;
                case State.CommentMl:
                    CommentStateMl();
                    break;
                case State.CommentMlExit:
                    CommentStateMl();
                    break;
                case State.OpDiv:
                    LexLitOpDiv();
                    break;
                case State.OpMul:
                    LexLitOpMult();
                    break;
                case State.OpAdd:
                    LexLitOpAdd();
                    break;
                case State.OpSub:
                    LexLitOpSub();
                    break;
                case State.OpLess:
                    LexLitOpLess();
                    break;
                case State.OpMore:
                    LexLitOpMore();
                    break;
                case State.OpAssign:
                    LexLitOpAssign();
                    break;
                case State.OpNeg:
                    LexLitOpNotEqual();
                    break;
                case State.OpBinAnd:
                    LexOpAnd();
                    break;
                case State.OpBinOr:
                    LexOpOr();
                    break;
                case State.Unknown:
                    running = false;
                    break;
            }
        }


        private void CompleteToken(TokenType tokenState, bool advance = true)
        {
            dynamic data = 0;
            if (int.TryParse(buffer, out int x))
                data = x;
            else if (float.TryParse(buffer, out float c))
                data = c;
            else
                data = buffer;
            tokens.Add(new Token(tokenState, data, tokenStart));
            buffer = "";
            currentState = State.Start;
            if (!advance)
                offset--;
        }

    
    }
}