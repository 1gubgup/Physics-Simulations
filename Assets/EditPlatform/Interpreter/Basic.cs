using System;

namespace Interpreter_Basic
{
    public class ExceptionWithOutLine : ApplicationException
    {
        public ExceptionWithOutLine(string message) : base(message)
        {

        }

        public string getInformation()
        {
            return "Exception:" + Message;
        }
    }

    public class MyException : ApplicationException
    {
        protected string type;
        protected int line;
        public MyException(string message) : base(message)
        {

        }

        public string getInformation()
        {
            return "at line " + line + " Exception:" + Message;
        }

        public string getInformationWithoutLine()
        {
            return "Exception:" + Message;
        }
    }
    public class TokenException : MyException
    {
        public TokenException(string message, int l) : base(message)
        {
            type = "TokenException";
            line = l;
        }
    }

    public class CharException : MyException
    {
        public CharException(string message, int l) : base(message)
        {
            type = "CharException";
            line = l;
        }
    }

    public class ASTException : MyException
    {
        public ASTException(string message, int l) : base(message)
        {
            type = "ASTException";
            line = l;
        }
    }

    public class VarException : MyException
    {
        public VarException(string message, int l) : base(message)
        {
            type = "VarException";
            line = l;
        }
    }

    public class TypeException : MyException
    {
        public TypeException(string message, int l) : base(message)
        {
            type = "TypeException";
            line = l;
        }
    }

    public class MethodException : MyException
    {
        public MethodException(string message, int l) : base(message)
        {
            type = "MethodException";
            line = l;
        }
    }

    public class SymbleException : MyException
    {
        public SymbleException(string message, int l) : base(message)
        {
            type = "SymbleException";
            line = l;
        }
    }

    public class ParamException : MyException
    {
        public ParamException(string message, int l) : base(message)
        {
            type = "ParamException";
            line = l;
        }
    }

    public class ClassException : MyException
    {
        public ClassException(string message, int l) : base(message)
        {
            type = "ClassException";
            line = l;
        }
    }

    public class AssignException : MyException
    {
        public AssignException(string message, int l) : base(message)
        {
            type = "AssignException";
            line = l;
        }
    }

    public class OperatorException : MyException
    {
        public OperatorException(string message, int l) : base(message)
        {
            type = "OperatorException";
            line = l;
        }
    }

    public class ExcuteException : MyException
    {
        public ExcuteException(string message, int l) : base(message)
        {
            type = "ExcuteException";
            line = l;
        }
    }

    public class Warning
    {
        private int line;
        private string words;
        public Warning(int l, string w)
        {
            line = l;
            words = w;
        }
    }

    public enum Basic_Type
    {
        PLUS,
        MINUS,
        MUL,
        DIV,
        REMAINDER,
        LEFT_PAREN,
        RIGHT_PAREN,
        PUBLIC,
        PRIVATE,
        STRUCT,
        ENUM,
        CLASS,
        DOUBLE_TYPE,
        FLOAT_TYPE,
        INT_TYPE,
        VOID_TYPE,
        LONG_TYPE,
        DECIMAL_TYPE,
        BOOL_TYPE,
        INT,
        FLOAT,
        DOUBLE,
        LONG,
        DECIMAL,
        BOOL,
        VOID,
        SEMI,
        COMMA,
        AND,
        OR,
        ANDAND,
        OROR,
        NOT,
        DOT,
        VARMETHODORCLASS,
        GREATER,
        LESS,
        EQUAL,
        NEQUAL,
        GEQUAL,
        LEQUAL,
        ASSIGN,
        IF,
        ELSE,
        WHILE,
        FOR,
        LEFT_BRACE,
        RIGHT_BRACE,
        RET,
        EOF,
        SQRT,
        BUILDIN_CLASS,
        GAMEOBJECT,
        TRANSFORM,
        NEW,
        COLON,
        BITNOT,
        XOR,
        LEFT_SHIFT,
        RIGHT_SHIFT,
        PLUSPLUS,
        MINUSMINUS,
        BREAK,
        CONTINUE
    }

    public enum AST_Type
    {
        UNARYOP,
        BINOP,
        NUM,
        COMPOUND,
        ASSIGN,
        VARMETHODORCLASS,
        CLASS_DEC,
        CLASS_CALL,
        PARAM,
        TYPE,
        EMPTY,
        METHOD_CALL,
        PARAM_GIVEN,
        RET,
        IF_BLOCK,
        BUILDIN_INIT_METHOD_CALL,
        MEMBER_METHOD_DEC,
        INIT_METHOD_DEC,
        INIT_METHOD_CALL,
        VARIABLE_DEC,
        MEMBER_VARIABLE_DEC,
        INSTANCEDEC,
        TYPEALTER,
        ELSE_IF_BLOCK,
        WHILE_BLOCK,
        FOR_BLOCK,
        BREAK,
        CONTINUE
    }

    public enum Symbol_Type
    {
        TypeSymbol,
        VarSymbol,
        MemberVarSymbol,
        MemberMethodSymbol,
        InitMethodSymbol,
        BuildInClassSymbol,
        ParamSymbol
    }

    public enum ACM
    {
        Public,
        Private,
        Protect
    }
}
