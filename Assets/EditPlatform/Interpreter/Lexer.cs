using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Interpreter_Basic;

namespace Interpreter_Lexer
{
    public class Token
    {
        private Basic_Type type;
        private string value;
        private int lineNum;

        public Token(Basic_Type t, string v, int l)
        {
            type = t;
            value = v;
            lineNum = l + 1;
            Debug.Log("Token{" + type.ToString() + "," + value + "}");
        }

        public string getValue()
        {
            return value;
        }

        public Basic_Type getType()
        {
            return type;
        }

        public int getLineNum()
        {
            return lineNum;
        }

        public void setLine(int l)
        {
            lineNum = l + 1;
        }
    }

    class Lexer
    {
        private string text;
        private int pos = 0;
        private int line = 0;
        private int token_pos = 0;
        private int token_line = 0;
        private string cur_char = null;
        private string cur_line = null;
        private Dictionary<string, Token> RESERVED_KEYWORDS = new Dictionary<string, Token>();
        //private Dictionary<string, Token> BUILDINFUNCTION_KEYWORDS = new Dictionary<string, Token>();
        private List<string> textList = new List<string>();

        private void init_RESERVED_KEYWORDS(Dictionary<string, HaHaObject> dict)
        {
            RESERVED_KEYWORDS.Add("public", new Token(Basic_Type.PUBLIC, "public", 0));
            RESERVED_KEYWORDS.Add("enum", new Token(Basic_Type.ENUM, "enum", 0));
            RESERVED_KEYWORDS.Add("class", new Token(Basic_Type.CLASS, "class", 0));
            RESERVED_KEYWORDS.Add("struct", new Token(Basic_Type.STRUCT, "struct", 0));
            RESERVED_KEYWORDS.Add("int", new Token(Basic_Type.INT_TYPE, "int", 0));
            RESERVED_KEYWORDS.Add("long", new Token(Basic_Type.LONG_TYPE, "long", 0));
            RESERVED_KEYWORDS.Add("decimal", new Token(Basic_Type.DECIMAL_TYPE, "decimal", 0));
            RESERVED_KEYWORDS.Add("double", new Token(Basic_Type.DOUBLE_TYPE, "double", 0));
            RESERVED_KEYWORDS.Add("float", new Token(Basic_Type.FLOAT_TYPE, "float", 0));
            RESERVED_KEYWORDS.Add("bool", new Token(Basic_Type.BOOL_TYPE, "bool", 0));
            RESERVED_KEYWORDS.Add("void", new Token(Basic_Type.VOID_TYPE, "void", 0));
            RESERVED_KEYWORDS.Add("private", new Token(Basic_Type.PRIVATE, "private", 0));
            RESERVED_KEYWORDS.Add("if", new Token(Basic_Type.IF, "if", 0));
            RESERVED_KEYWORDS.Add("return", new Token(Basic_Type.RET, "return", 0));
            RESERVED_KEYWORDS.Add("break", new Token(Basic_Type.BREAK, "break", 0));
            RESERVED_KEYWORDS.Add("continue", new Token(Basic_Type.CONTINUE, "continue", 0));
            RESERVED_KEYWORDS.Add("true", new Token(Basic_Type.BOOL, "true", 0));
            RESERVED_KEYWORDS.Add("false", new Token(Basic_Type.BOOL, "false", 0));
            RESERVED_KEYWORDS.Add("new", new Token(Basic_Type.NEW, "new", 0));
            RESERVED_KEYWORDS.Add("Vector3", new Token(Basic_Type.BUILDIN_CLASS, "Vector3", 0));
            RESERVED_KEYWORDS.Add("Quaternion", new Token(Basic_Type.BUILDIN_CLASS, "Quaternion", 0));
            RESERVED_KEYWORDS.Add("Mathf", new Token(Basic_Type.BUILDIN_CLASS, "Mathf", 0));
            RESERVED_KEYWORDS.Add("Time", new Token(Basic_Type.BUILDIN_CLASS, "Time", 0));
            RESERVED_KEYWORDS.Add("else", new Token(Basic_Type.ELSE, "else", 0));
            RESERVED_KEYWORDS.Add("while", new Token(Basic_Type.WHILE, "while", 0));
            RESERVED_KEYWORDS.Add("for", new Token(Basic_Type.FOR, "for", 0));
            RESERVED_KEYWORDS.Add("gameObject", new Token(Basic_Type.GAMEOBJECT, "gameObject", 0));
            RESERVED_KEYWORDS.Add("transform", new Token(Basic_Type.TRANSFORM, "transform", 0));
            foreach (var v in dict)
            {
                Debug.Log(v.GetType());
                RESERVED_KEYWORDS.Add(v.Key, new Token(Basic_Type.GAMEOBJECT, v.Key, 0));
            }
        }

        public Lexer(string t, Dictionary<string, HaHaObject> dict)
        {
            if (t.Length == 0)
            {
                throw new CharException("text is empty", 1);
            }
            init_RESERVED_KEYWORDS(dict);
            text = t;
            cur_char = text[pos].ToString();
            textList = t.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
            cur_line = textList[line];
        }

        public void char_error()
        {
            throw new CharException("Invalid character at " + "pos " + pos.ToString() + " '" + cur_char + "'", line + 1);
        }

        public void newLine_error()
        {
            throw new CharException("unexpected newline", line + 1);
        }

        private string peek()
        {
            int peek_pos = pos + 1;
            if (peek_pos > cur_line.Length - 1)
            {
                return null;
            }
            return cur_line[peek_pos].ToString();
        }

        private bool isSpace(string s)
        {
            if (s.Length > 1)
            {
                char_error();
            }
            Regex regexSpace = new Regex(@"^\s$");
            return regexSpace.IsMatch(s);
        }

        private bool isLetterNumOrUnderline(string s)
        {
            if (s.Length > 1)
            {
                char_error();
            }
            Regex regexLetterNumOrUnderline = new Regex(@"^\w$");
            return regexLetterNumOrUnderline.IsMatch(s);
        }

        private bool isLetterOrUnderline(string s)
        {
            if (s.Length > 1)
            {
                char_error();
            }
            Regex regexLetter = new Regex(@"^[a-zA-Z_]$");
            return regexLetter.IsMatch(s);
        }

        private bool isNum(string s)
        {
            if (s.Length > 1)
            {
                char_error();
            }
            Regex regexNum = new Regex(@"^\d$"); //interger
            return regexNum.IsMatch(s);
        }

        private void advance()
        {
            pos++;
            if (pos > cur_line.Length - 1)
            {
                line++;
                while (line < textList.Count && string.IsNullOrEmpty(textList[line]))
                {
                    line++;
                }
                if (line > textList.Count - 1)
                {
                    cur_char = null;
                    return;
                }
                cur_line = textList[line];
                pos = 0;
                cur_char = cur_line[pos].ToString();
            }
            else
            {
                cur_char = cur_line[pos].ToString();
            }
        }

        private void advanceInOneLineForAlphanumeric()
        {
            pos++;
            if (pos > cur_line.Length - 1)
            {
                line++;
                while (line < textList.Count && string.IsNullOrEmpty(textList[line]))
                {
                    line++;
                }
                if (line > textList.Count - 1)
                {
                    cur_char = null;
                    return;
                }
                cur_line = textList[line];
                pos = 0;
                cur_char = cur_line[pos].ToString();
                if (cur_char == null || isLetterNumOrUnderline(cur_char))
                {
                    newLine_error();
                }
            }
            else
            {
                cur_char = cur_line[pos].ToString();
            }
        }

        private void advanceInOneLine()
        {
            pos++;
            if (pos > cur_line.Length - 1)
            {
                newLine_error();
            }
            else
            {
                cur_char = cur_line[pos].ToString();
            }
        }

        private void skip_whitespace()
        {
            while (cur_char != null && isSpace(cur_char))
            {
                advance();
            }
        }

        private void skip_comment()
        {
            line++;
            while (line < textList.Count && string.IsNullOrEmpty(textList[line]))
            {
                line++;
            }
            if (line > textList.Count - 1)
            {
                cur_char = null;
                return;
            }
            cur_line = textList[line];
            pos = 0;
            cur_char = cur_line[pos].ToString();
        }

        private Token numberToken()
        {
            int tempLine = line;
            string result = null;
            while (cur_char != null && isNum(cur_char))
            {
                result += cur_char;
                advanceInOneLineForAlphanumeric();
            }
            if (cur_char == "f" || cur_char == "F")
            {
                advanceInOneLineForAlphanumeric();
                return new Token(Basic_Type.FLOAT, result, line);
            }
            else if (cur_char == "m" || cur_char == "M")
            {
                advanceInOneLineForAlphanumeric();
                return new Token(Basic_Type.DECIMAL, result, line);
            }
            else if (cur_char == "L")
            {
                advanceInOneLineForAlphanumeric();
                return new Token(Basic_Type.LONG, result, line);
            }
            else if (cur_char == "d" || cur_char == "D")
            {
                advanceInOneLineForAlphanumeric();
                return new Token(Basic_Type.DOUBLE, result, line);
            }
            else if (cur_char == ".")
            {
                if (tempLine != line)
                {
                    newLine_error();
                }
                result += ".";
                advanceInOneLineForAlphanumeric();
                while (cur_char != null && isNum(cur_char))
                {
                    result += cur_char;
                    advanceInOneLineForAlphanumeric();
                }
                if (cur_char == "f" || cur_char == "F")
                {
                    advanceInOneLineForAlphanumeric();
                    return new Token(Basic_Type.FLOAT, result, line);
                }
                else if (cur_char == "m" || cur_char == "M")
                {
                    advanceInOneLineForAlphanumeric();
                    return new Token(Basic_Type.DECIMAL, result, line);
                }
                else if (cur_char == "d" || cur_char == "D")
                {
                    advanceInOneLineForAlphanumeric();
                    return new Token(Basic_Type.DOUBLE, result, line);
                }
                else if (cur_char == "L")
                {
                    throw new VarException("Can't convert double to long", line);
                }
                else
                {
                    return new Token(Basic_Type.DOUBLE, result, line);
                }
            }
            else
            {
                return new Token(Basic_Type.INT, result, line);
            }
        }

        public Token get_variableOrKeyWords()
        {
            string result = "";
            while (cur_char != null && (isLetterNumOrUnderline(cur_char)))
            {
                result += cur_char;
                advanceInOneLineForAlphanumeric();
            }
            if (RESERVED_KEYWORDS.ContainsKey(result))
            {
                Token RESERVED_token = RESERVED_KEYWORDS[result];
                RESERVED_token.setLine(line);
                return RESERVED_token;
            }
            else
            {
                return new Token(Basic_Type.VARMETHODORCLASS, result, line);
            }
        }

        public Token get_next_token()
        {
            while (cur_char != null)
            {
                token_pos = pos;
                token_line = line;
                if (isSpace(cur_char))
                {
                    skip_whitespace();
                    continue;
                }
                if (isNum(cur_char))
                {
                    return numberToken();
                }
                if (cur_char == "+")
                {
                    if (peek() == "+")
                    {
                        advanceInOneLine();
                        advance();
                        return new Token(Basic_Type.PLUSPLUS, "++", token_line);
                    }
                    advance();
                    return new Token(Basic_Type.PLUS, "+", token_line);
                }
                if (cur_char == "-")
                {
                    if (peek() == "-")
                    {
                        advanceInOneLine();
                        advance();
                        return new Token(Basic_Type.MINUSMINUS, "--", token_line);
                    }
                    advance();
                    return new Token(Basic_Type.MINUS, "-", token_line);
                }
                if (cur_char == "*")
                {
                    advance();
                    return new Token(Basic_Type.MUL, "*", token_line);
                }
                if (cur_char == "%")
                {
                    advance();
                    return new Token(Basic_Type.REMAINDER, "%", token_line);
                }
                if (cur_char == "/")
                {
                    if (peek() == "/")
                    {
                        skip_comment();
                        continue;
                    }
                    advance();
                    return new Token(Basic_Type.DIV, "/", token_line);
                }
                if (cur_char == "(")
                {
                    advance();
                    return new Token(Basic_Type.LEFT_PAREN, "(", token_line);
                }
                if (cur_char == ")")
                {
                    advance();
                    return new Token(Basic_Type.RIGHT_PAREN, ")", token_line);
                }
                if (isLetterOrUnderline(cur_char))
                {
                    return get_variableOrKeyWords();
                }
                if (cur_char == ";")
                {
                    advance();
                    return new Token(Basic_Type.SEMI, ";", token_line);
                }
                if (cur_char == ":")
                {
                    advance();
                    return new Token(Basic_Type.COLON, ":", token_line);
                }
                if (cur_char == ".")
                {
                    advance();
                    return new Token(Basic_Type.DOT, ".", token_line);
                }
                if (cur_char == ",")
                {
                    advance();
                    return new Token(Basic_Type.COMMA, ",", token_line);
                }
                if (cur_char == "&")
                {
                    if (peek() == "&")
                    {
                        advanceInOneLine();
                        advance();
                        return new Token(Basic_Type.ANDAND, "&&", token_line);
                    }
                    advance();
                    return new Token(Basic_Type.AND, "&", token_line);
                }
                if (cur_char == "|")
                {
                    if (peek() == "|")
                    {
                        advanceInOneLine();
                        advance();
                        return new Token(Basic_Type.OROR, "||", token_line);
                    }
                    advance();
                    return new Token(Basic_Type.OR, "|", token_line);
                }
                if (cur_char == "!")
                {
                    if (peek() == "=")
                    {
                        advanceInOneLine();
                        advance();
                        return new Token(Basic_Type.NEQUAL, "!=", token_line);
                    }
                    advance();
                    return new Token(Basic_Type.NOT, "!", token_line);
                }
                if (cur_char == "~")
                {
                    advance();
                    return new Token(Basic_Type.BITNOT, "~", token_line);
                }
                if (cur_char == "^")
                {
                    advance();
                    return new Token(Basic_Type.XOR, "^", token_line);
                }
                if (cur_char == ">")
                {
                    if (peek() == "=")
                    {
                        advanceInOneLine();
                        advance();
                        return new Token(Basic_Type.GEQUAL, ">=", token_line);
                    }
                    else if (peek() == ">")
                    {
                        advanceInOneLine();
                        advance();
                        return new Token(Basic_Type.RIGHT_SHIFT, ">>", token_line);
                    }
                    advance();
                    return new Token(Basic_Type.GREATER, ">", token_line);
                }
                if (cur_char == "<")
                {
                    if (peek() == "=")
                    {
                        advanceInOneLine();
                        advance();
                        return new Token(Basic_Type.LEQUAL, "<=", token_line);
                    }
                    if (peek() == "<")
                    {
                        advanceInOneLine();
                        advance();
                        return new Token(Basic_Type.LEFT_SHIFT, "<<", token_line);
                    }
                    advance();
                    return new Token(Basic_Type.LESS, "<", token_line);
                }
                if (cur_char == "=")
                {
                    if (peek() == "=")
                    {
                        advanceInOneLine();
                        advance();
                        return new Token(Basic_Type.EQUAL, "==", token_line);
                    }
                    advance();
                    return new Token(Basic_Type.ASSIGN, "=", token_line);
                }
                if (cur_char == "{")
                {
                    advance();
                    return new Token(Basic_Type.LEFT_BRACE, "{", token_line);
                }
                if (cur_char == "}")
                {
                    advance();
                    return new Token(Basic_Type.RIGHT_BRACE, "}", token_line);
                }
                char_error();
            }
            return new Token(Basic_Type.EOF, null, token_line);
        }

    }
}
