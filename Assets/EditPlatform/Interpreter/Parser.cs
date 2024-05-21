using System.Collections.Generic;
using UnityEngine;
using Interpreter_Lexer;
using Interpreter_Basic;
using System;

namespace Interpreter_Parser
{
    public class AST
    {
        private AST_Type ast_type;
        protected int line;

        public void setAST_Type(AST_Type a)
        {
            ast_type = a;
        }

        public AST_Type getAST_Type()
        {
            return ast_type;
        }

        public int getLine()
        {
            return line;
        }
    }

    public class UnaryOP : AST
    {
        private Token op;
        private AST right;
        private bool rightOrLeft = false;

        public UnaryOP(Token o, AST r)
        {
            setAST_Type(AST_Type.UNARYOP);
            op = o;
            right = r;
            line = o.getLineNum();
            Debug.Log("UnaryOP{" + op.getValue() + "," + right.getAST_Type() + "}");
        }

        public void setRight()
        {
            rightOrLeft = true;
        }

        public bool getRightOrLeft()
        {
            return rightOrLeft;
        }

        public Token getOP()
        {
            return op;
        }

        public AST getRight()
        {
            return right;
        }
    }

    public class TypeAlterOP : AST
    {
        private TYPE type;
        private AST right;

        public TypeAlterOP(TYPE t, AST r)
        {
            setAST_Type(AST_Type.TYPEALTER);
            type = t;
            right = r;
            line = t.getLine();
        }

        public TYPE getType()
        {
            return type;
        }

        public AST getRight()
        {
            return right;
        }
    }

    public class BinOP : AST
    {
        private AST left;
        private AST right;
        private Token op;

        public BinOP(AST l, Token o, AST r)
        {
            setAST_Type(AST_Type.BINOP);
            left = l;
            op = o;
            right = r;
            line = o.getLineNum();
            Debug.Log("BinOP{" + left.getAST_Type() + "," + op.getValue() + "," + right.getAST_Type() + "}");
        }

        public Token getOp()
        {
            return op;
        }

        public AST getLeft()
        {
            return left;
        }

        public AST getRight()
        {
            return right;
        }
    }

   
    public class Num : AST
    {
        private Token token;
        private string valur_str;
        private Basic_Type numType;

        public Num(Token t)
        {
            setAST_Type(AST_Type.NUM);
            token = t;
            numType = t.getType();
            line = t.getLineNum();
            valur_str = token.getValue();
            Debug.Log("NUM{" + token.getValue() + "}");
        }

        public Basic_Type getNumType()
        {
            return numType;
        }

        public string getValueStr()
        {
            return valur_str;
        }
    }

    public class InitMethodDec : AST
    {
        private AST method_block;
        private List<Param> paramList;
        private ACM acm;

        public InitMethodDec(VarMethodOrClass v, AST b, List<Param> p, ACM a)
        {
            line = v.getLine();
            setAST_Type(AST_Type.INIT_METHOD_DEC);
            method_block = b;
            paramList = p;
            acm = a;
            Debug.Log("INIT_METHOD_DEC{name:" + v.getName()  + ", " + acm + "}");
            if (paramList != null && paramList.Count <= 0)
            {
                Debug.Log("there are no params");
            }
            else
            {
                foreach (Param pa in paramList)
                {
                    Debug.Log("INIT_METHOD_DEC_PARAM{type:" + pa.getTYPE().getName() + ", name:" + pa.getVar().getName() + "}");
                }
            }
        }

        public AST getBlock()
        {
            return method_block;
        }

        public List<Param> getParams()
        {
            return paramList;
        }

        public ACM getACM()
        {
            return acm;
        }
    }


    public class MemberMethodDec : AST
    {
        private string name;
        private TYPE return_type;
        private AST method_block;
        private List<Param> paramList;
        private ACM acm;

        public MemberMethodDec(VarMethodOrClass v, TYPE t, AST b, List<Param> p, ACM a)
        {
            line = v.getLine();
            setAST_Type(AST_Type.MEMBER_METHOD_DEC);
            name = v.getName();
            return_type = t;
            method_block = b;
            paramList = p;
            acm = a;
            Debug.Log("MEMBER_METHOD_DEC{name:" + name + ", return_type:" + return_type.getName() + ", " + acm + "}");
            if (paramList != null && paramList.Count <= 0)
            {
                Debug.Log("there are no params");
            }
            else
            {
                foreach (Param pa in paramList)
                {
                    Debug.Log("MEMBER_METHOD_PARAM{type:" + pa.getTYPE().getName() + ", name:" + pa.getVar().getName() + "}");
                }
            }
        }

        public string getName()
        {
            return name;
        }

        public TYPE getReturnType()
        {
            return return_type;
        }

        public AST getBlock()
        {
            return method_block;
        }

        public List<Param> getParams()
        {
            return paramList;
        }

        public ACM getACM()
        {
            return acm;
        }
    }

    public class Param : AST
    {
        private TYPE type_node;
        private VarMethodOrClass var_node;

        public Param(TYPE t, VarMethodOrClass v)
        {
            line = t.getLine();
            setAST_Type(AST_Type.PARAM);
            type_node = t;
            var_node = v;
            Debug.Log("Param:{" + type_node.getName() + ", " + var_node.getName() + "}");
        }

        public TYPE getTYPE()
        {
            return type_node;
        }

        public VarMethodOrClass getVar()
        {
            return var_node;
        }

    }

    public class ClassDec : AST
    {
        private VarMethodOrClass className;
        private List<MemberMethodDec> member_methods;
        private List<MemberVariableDec> member_variables;
        private List<InitMethodDec> init_methods;
        private VarMethodOrClass Father = null;

        public ClassDec(VarMethodOrClass cn, List<MemberMethodDec> mm, List<MemberVariableDec> mv, List<InitMethodDec> im, VarMethodOrClass f)
        {
            setAST_Type(AST_Type.CLASS_DEC);
            line = cn.getLine();
            className = cn;
            member_methods = mm;
            member_variables = mv;
            init_methods = im;
            Father = f;
            Debug.Log("ClassDec");
        }

        public ClassDec(VarMethodOrClass cn, List<MemberMethodDec> mm, List<MemberVariableDec> mv, List<InitMethodDec> im)
        {
            setAST_Type(AST_Type.CLASS_DEC);
            line = cn.getLine();
            className = cn;
            member_methods = mm;
            member_variables = mv;
            init_methods = im;
            Debug.Log("ClassDec");
        }

        public VarMethodOrClass getClassName()
        {
            return className;
        }

        public List<MemberVariableDec> getMemberVariables()
        {
            return member_variables;
        }

        public List<MemberMethodDec> getMemberMethods()
        {
            return member_methods;
        }

        public List<InitMethodDec> getInitMethods()
        {
            return init_methods;
        }

        public VarMethodOrClass getFather()
        {
            return Father;
        }
    }

    public class VariableDec : AST
    {
        private VarMethodOrClass var;
        private TYPE type;
        private Assign assign;

        public VariableDec(VarMethodOrClass v, TYPE t)
        {
            line = v.getLine();
            setAST_Type(AST_Type.VARIABLE_DEC);
            var = v;
            type = t;
            assign = null;
            Debug.Log("DEC{" + var.getName() + ", " + t.getName() + "}");
        }

        public VariableDec(VarMethodOrClass v, TYPE t, Assign a)
        {
            setAST_Type(AST_Type.VARIABLE_DEC);
            var = v;
            type = t;
            assign = a;
            Debug.Log("DEC{" + var.getName() + ", " + t.getName() + "}");
        }

        public VarMethodOrClass getVar()
        {
            return var;
        }

        public TYPE getType()
        {
            return type;
        }

        public bool haveAssignment()
        {
            if (assign == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Assign getAssign()
        {
            return assign;
        }
    }

    public class InitMethodCall : AST
    {
        private VarMethodOrClass initMethod;
        private List<ParamGiven> paramGivens;

        public InitMethodCall(VarMethodOrClass im, List<ParamGiven> p)
        {
            line = im.getLine();
            setAST_Type(AST_Type.INIT_METHOD_CALL);
            paramGivens = p;
            initMethod = im;
            Debug.Log("InitMethodCall");
        }

        public VarMethodOrClass getInitMethod()
        {
            return initMethod;
        }

        public List<ParamGiven> getParamsGiven()
        {
            return paramGivens;
        }
    }

    public class BuildInInitMethodCall : AST
    {
        private VarMethodOrClass initMethod;
        private List<ParamGiven> paramGivens;

        public BuildInInitMethodCall(VarMethodOrClass im, List<ParamGiven> p)
        {
            line = im.getLine();
            setAST_Type(AST_Type.BUILDIN_INIT_METHOD_CALL);
            paramGivens = p;
            initMethod = im;
            Debug.Log("BuildInitMethodCall");
        }

        public VarMethodOrClass getInitMethod()
        {
            return initMethod;
        }

        public List<ParamGiven> getParamsGiven()
        {
            return paramGivens;
        }
    }

    public class MemberVariableDec : AST
    {
        private VarMethodOrClass var;
        private TYPE type;
        private Assign assign;
        private ACM acm;

        public MemberVariableDec(VarMethodOrClass v, TYPE t, ACM ac)
        {
            line = v.getLine();
            setAST_Type(AST_Type.MEMBER_VARIABLE_DEC);
            var = v;
            type = t;
            acm = ac;
            assign = null;
            Debug.Log("MEMBER_VARIABLE_DEC{" + var.getName() + ", " + t.getName() + ", " + acm + "}");
        }

        public MemberVariableDec(VarMethodOrClass v, TYPE t, Assign a, ACM ac)
        {
            setAST_Type(AST_Type.MEMBER_VARIABLE_DEC);
            var = v;
            type = t;
            assign = a;
            acm = ac;
            Debug.Log("MEMBER_VARIABLE_DEC{" + var.getName() + ", " + t.getName() +  ", " + acm + "}");
        }

        public VarMethodOrClass getVar()
        {
            return var;
        }

        public TYPE getType()
        {
            return type;
        }

        public bool haveAssignment()
        {
            if (assign == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Assign getAssign()
        {
            return assign;
        }

        public ACM getACM()
        {
            return acm;
        }
    }

    public class TYPE : AST
    {
        private Token type_token;
        private VarMethodOrClass classType;
        private string type_name;

        public TYPE(Token t)
        {
            line = t.getLineNum();
            setAST_Type(AST_Type.TYPE);
            type_token = t;
            type_name = t.getValue();
            Debug.Log("DEC_TYPE{type name:" + type_name + "}");
        }

        public TYPE(VarMethodOrClass c)
        {
            line = c.getLine();
            setAST_Type(AST_Type.TYPE);
            classType = c;
            type_name = c.getName();
            Debug.Log("DEC_TYPE{type name:" + type_name + "}");
        }

        public string getName()
        {
            return type_name;
        }

    }


    public class Compound : AST
    {
        private List<AST> children = new List<AST>();

        public Compound(List<AST> c)
        {
            line = c[0].getLine();
            setAST_Type(AST_Type.COMPOUND);
            children = c;
            Debug.Log("COMPOUND");
        }

        public List<AST> getList()
        {
            return children;
        }
    }

    public class Assign : AST
    {
        private AST left;
        private AST right;

        public Assign(AST l, AST r)
        {
            line = l.getLine();
            setAST_Type(AST_Type.ASSIGN);
            left = l;
            right = r;
            Debug.Log("Assign");
        }

        public AST getLeft()
        {
            return left;
        }

        public AST getRight()
        {
            return right;
        }
    }

    public class VarMethodOrClass : AST
    {
        private Token token;
        private string name;

        public VarMethodOrClass(Token t)
        {
            line = t.getLineNum();
            setAST_Type(AST_Type.VARMETHODORCLASS);
            token = t;
            name = token.getValue();
            Debug.Log("VarMethodOrClass{" + name + "}");
        }

        public Token getToken()
        {
            return token;
        }

        public string getName()
        {
            return name;
        }

        public bool ifBuildInClass()
        {
            return token.getType() == Basic_Type.BUILDIN_CLASS;
        }

        public bool ifGameObject()
        {
            return token.getType() == Basic_Type.GAMEOBJECT;
        }

        public bool ifTransform()
        {
            return token.getType() == Basic_Type.TRANSFORM;
        }
    }

    public class ClassCall : AST
    {
        private AST left = null;
        private ClassCall right;
        private bool ifMethod;

        public ClassCall(AST l, ClassCall r, bool i)
        {
            setAST_Type(AST_Type.CLASS_CALL);
            left = l;
            line = l.getLine();
            right = r;
            ifMethod = i;
            Debug.Log("CLASS CALL");
        }

        public AST getLeft()
        {
            return left;
        }

        public ClassCall getRight()
        {
            return right;
        }

        public bool IfMethod()
        {
            return ifMethod;
        }
    }

    public class MethodCall : AST
    {
        private VarMethodOrClass method;
        private List<ParamGiven> paramsGiven;

        public MethodCall(VarMethodOrClass m, List<ParamGiven> p)
        {
            line = m.getLine();
            setAST_Type(AST_Type.METHOD_CALL);
            method = m;
            paramsGiven = p;
            Debug.Log("METHOD CALL{" + method.getName() + "}");
        }

        public string getMethodName()
        {
            return method.getName();
        }

        public List<ParamGiven> getParamsGiven()
        {
            return paramsGiven;
        }
    }

    public class ParamGiven : AST
    {
        private AST node;

        public ParamGiven(AST n)
        {
            line = n.getLine();
            setAST_Type(AST_Type.PARAM_GIVEN);
            node = n;
            Debug.Log("ParamGiven");
        }

        public AST getNode()
        {
            return node;
        }
    }

    public class BREAK : AST
    {
        private string scope;

        public BREAK(int l)
        {
            line = l;
            setAST_Type(AST_Type.BREAK);
        }

        public void setScope(string s)
        {
            scope = s;
        }

        public string getScope()
        {
            return scope;
        }
    }

    public class CONTINUE : AST
    {
        private string scope;

        public CONTINUE(int l)
        {
            line = l;
            setAST_Type(AST_Type.CONTINUE);
        }

        public void setScope(string s)
        {
            scope = s;
        }

        public string getScope()
        {
            return scope;
        }
    }

    public class RET : AST
    {
        private AST expr;
        private string methodName;

        public RET(AST node)
        {
            line = node.getLine();
            setAST_Type(AST_Type.RET);
            expr = node;
            Debug.Log("RET");
        }

        public AST getExpr()
        {
            return expr;
        }

        public void setMethodName(string m)
        {
            methodName = m;
        }

        public string getMethodName()
        {
            return methodName;
        }
    }

    public class ElSE_IF_BLOCK : AST
    {
        private Compound block;
        private string scope;
        private AST expr;

        public ElSE_IF_BLOCK(Compound b, AST e)
        {
            setAST_Type(AST_Type.ELSE_IF_BLOCK);
            block = b;
            expr = e;
            line = e.getLine();
        }

        public Compound getBlock()
        {
            return block;
        }

        public AST getExpr()
        {
            return expr;
        }

        public void setScope(string s)
        {
            scope = s;
        }

        public string getScope()
        {
            return scope;
        }
    }

    public class If_Block : AST
    {
        private Compound block;
        private AST expr;
        private string ifScope;
        private string elseScope = null;
        private Compound elseBlock = null;
        private List<ElSE_IF_BLOCK> elseIfs = new List<ElSE_IF_BLOCK>();

        public If_Block(Compound b, AST e)
        {
            setAST_Type(AST_Type.IF_BLOCK);
            block = b;
            expr = e;
            line = e.getLine();
            Debug.Log("If_Block");
        }

        public Compound getBlock()
        {
            return block;
        }

        public AST getExpr()
        {
            return expr;
        }

        public void setIfScope(string s)
        {
            ifScope = s;
        }

        public string getIfScope()
        {
            return ifScope;
        }

        public void setElseScope(string s)
        {
            elseScope = s;
        }

        public string getElseScope()
        {
            return elseScope;
        }


        public Compound getElseBlock()
        {
            return elseBlock;
        }

        public List<ElSE_IF_BLOCK> getElseIfBlocks()
        {
            return elseIfs;
        }

        public void setElseBlock(Compound e)
        {
            elseBlock = e;
        }

        public void setElseIfs(List<ElSE_IF_BLOCK> l)
        {
            elseIfs = l;
        }
    }

    public class For_Block : AST
    {
        private Compound block;
        private AST statement1;
        private AST statement2;
        private AST statement3;
        private string forScope;

        public For_Block(Compound b, AST s1, AST s2, AST s3)
        {
            setAST_Type(AST_Type.FOR_BLOCK);
            block = b;
            statement1 = s1;
            statement2 = s2;
            statement3 = s3;
            line = s1.getLine();
        }

        public void setForScope(string s)
        {
            forScope = s;
        }

        public string getForScope()
        {
            return forScope;
        }

        public Compound getBlock()
        {
            return block;
        }

        public AST getStatement1()
        {
            return statement1;
        }

        public AST getStatement2()
        {
            return statement2;
        }

        public AST getStatement3()
        {
            return statement3;
        }
    }

    public class While_Block : AST
    {
        private Compound block;
        private AST expr;
        private string whileScope;

        public While_Block(Compound b, AST e)
        {
            setAST_Type(AST_Type.WHILE_BLOCK);
            block = b;
            expr = e;
            line = e.getLine();
            Debug.Log("While_Block");
        }

        public void setWhileScope(string s)
        {
            whileScope = s;
        }

        public string getWhileScope()
        {
            return whileScope;
        }

        public AST getExpr()
        {
            return expr;
        }

        public Compound getBlock()
        {
            return block;
        }
    }


    public class Empty : AST
    {
        public Empty()
        {
            setAST_Type(AST_Type.EMPTY);
            Debug.Log("Empty");
        }
    }

    class Parser
    {
        private Lexer lexer;
        private Token cur_token;
        private bool getOutOfBlockInMethod = false;

        public Parser(string text, Dictionary<string, HaHaObject> dict)
        {
            lexer = new Lexer(text, dict);
            cur_token = lexer.get_next_token();
        }

        public void token_error()
        {
            throw new TokenException("unexpected token " + cur_token.getValue(), cur_token.getLineNum());
        }

        private void eat(Basic_Type t)
        {
            if (cur_token.getType() == t)
            {
                //Debug.Log(t);
                cur_token = lexer.get_next_token();
            }
            else
            {
                Debug.Log("eat:" + t);
                Debug.Log("cur_token " + cur_token.getValue());
                token_error();
            }
        }

        private bool isType(Token t)
        {
            return (t.getType() == Basic_Type.INT_TYPE || t.getType() == Basic_Type.DOUBLE_TYPE || t.getType() == Basic_Type.FLOAT_TYPE
                || t.getType() == Basic_Type.BOOL_TYPE || t.getType() == Basic_Type.VOID_TYPE || t.getType() == Basic_Type.DECIMAL_TYPE
                || t.getType() == Basic_Type.LONG_TYPE);
        }

        //private void eatType()
        //{
        //    if (isType(cur_token))
        //    {
        //        cur_token = lexer.get_next_token();
        //    }
        //    else
        //    {
        //        token_error();
        //    }
        //}

        private bool isACM(Token t)
        {
            return (t.getType() == Basic_Type.PUBLIC || t.getType() == Basic_Type.PRIVATE);
        }

        private void eatACM()
        {
            if (isACM(cur_token))
            {
                cur_token = lexer.get_next_token();
            }
            else
            {
                token_error();
            }
        }

        //private void eatRightBraceInMethod()
        //{
        //    if (cur_token.getType() == Basic_Type.RIGHT_BRACE)
        //    {
        //        cur_token = new Token(Basic_Type.SEMI, ";", cur_token.getLineNum());
        //    }
        //    else
        //    {
        //        token_error();
        //    }
        //}

        private Compound method_block()
        {
            eat(Basic_Type.LEFT_BRACE);
            Compound node = method_compound_statement();
            eat(Basic_Type.RIGHT_BRACE);
            return node;
        }

        //private Compound block_in_method()
        //{
        //    eat(Basic_Type.LEFT_BRACE);
        //    Compound node = method_compound_statement();
        //    eatRightBraceInMethod();
        //    return node;
        //}

        private AST program_block()
        {
            List<AST> class_dec_list = new List<AST>();

            while (cur_token.getType() == Basic_Type.CLASS)
            {
                class_dec_list.Add(declarationClass_statement());
            }
            if(cur_token.getType() != Basic_Type.EOF)
            {
                throw new ClassException("Namespace cannot directly define members such as variable and methods", cur_token.getLineNum());
            }

            AST node = new Compound(class_dec_list);
            return node;
        }

        private Compound method_compound_statement()
        {
            List<AST> nodes = method_statement_list();

            Compound root = new Compound(nodes);
            return root;
        }

        private List<AST> method_statement_list()
        {
            List<AST> theList = new List<AST>();
            AST node = method_statement();
            //theList.Add(node);
            if (node.getAST_Type() == AST_Type.EMPTY)
            {
                theList.Add(node);
                return theList;
            }
            while (node.getAST_Type() != AST_Type.EMPTY)
            {
                if (getOutOfBlockInMethod && cur_token.getType() != Basic_Type.SEMI)
                {
                    getOutOfBlockInMethod = false;
                }
                else
                {
                    eat(Basic_Type.SEMI);
                }
                theList.Add(node);
                node = method_statement();
                //AST theStatement = method_statement();
                //theList.Add(theStatement);
            }
            return theList;
        }


        private AST method_statement()                            //simple statement，没有条件和循环语句。
        {
            AST node;
            if (cur_token.getType() == Basic_Type.VARMETHODORCLASS)
            {
                node = handleVariableMethodOrClass();
            }
            else if (isType(cur_token))
            {
                node = declaration_in_method_statement();
            }
            else if (cur_token.getType() == Basic_Type.RET)
            {
                node = return_statement();
            }
            else if (cur_token.getType() == Basic_Type.BREAK)
            {
                int line = cur_token.getLineNum();
                eat(Basic_Type.BREAK);
                return new BREAK(line);
            }
            else if (cur_token.getType() == Basic_Type.CONTINUE)
            {
                int line = cur_token.getLineNum();
                eat(Basic_Type.CONTINUE);
                return new CONTINUE(line);
            }
            else if (cur_token.getType() == Basic_Type.IF)
            {
                node = if_statement();
            }
            else if (cur_token.getType() == Basic_Type.WHILE)
            {
                node = while_statement();
            }
            else if (cur_token.getType() == Basic_Type.FOR)
            {
                node = for_statement();
            }
            else if (cur_token.getType() == Basic_Type.GAMEOBJECT)
            {
                node = handleBuildInInstance();
            }
            else if (cur_token.getType() == Basic_Type.TRANSFORM)
            {
                node = handleBuildInInstance();
            }
            else if (cur_token.getType() == Basic_Type.BUILDIN_CLASS)
            {
                node = handleBuildInClass();
            }
            else if (cur_token.getType() == Basic_Type.LEFT_PAREN)
            {
                node = handleLeftParen();
            }
            else
            {
                node = new Empty();
            }
            return node;
        }


        private Param parameter()
        {
            if (cur_token.getType() == Basic_Type.RIGHT_PAREN)
            {
                return null;
            }
            TYPE type = declaration_type();
            VarMethodOrClass var = (VarMethodOrClass)variableMethodOrClass(false);
            return new Param(type, var);
        }

        private List<Param> getListParam()
        {
            List<Param> list = new List<Param>();
            Param p = parameter();
            if (p == null)
            {
                return list;
            }
            list.Add(p);
            while (cur_token.getType() == Basic_Type.COMMA)
            {
                eat(Basic_Type.COMMA);
                list.Add(parameter());
            }
            return list;
        }

        private AST declaration_in_method_statement()                   //maybe includs assignment(without type check)
        {
            TYPE type = declaration_type();
            VarMethodOrClass var = (VarMethodOrClass)variableMethodOrClass(true);
            if (cur_token.getValue() == "=")
            {
                Token op = cur_token;
                eat(Basic_Type.ASSIGN);
                AST right = expr();  //for number operations only
                Assign assign = new Assign(var, right);
                VariableDec node = new VariableDec(var, type, assign);
                return node;
            }
            else if (cur_token.getValue() == "(")
            {
                throw new MethodException("don't declare a method in a member method!", cur_token.getLineNum());
            }
            else
            {
                VariableDec node = new VariableDec(var, type);
                return node;
            }
        }


        private ClassDec declarationClass_statement()
        {
            eat(Basic_Type.CLASS);
            VarMethodOrClass className = new VarMethodOrClass(cur_token);
            eat(Basic_Type.VARMETHODORCLASS);
            List<MemberMethodDec> methods = new List<MemberMethodDec>();
            List<MemberVariableDec> variables = new List<MemberVariableDec>();
            List<InitMethodDec> init_methods = new List<InitMethodDec>();
            if (cur_token.getType() == Basic_Type.COLON)
            {
                eat(Basic_Type.COLON);
                VarMethodOrClass FatherName = new VarMethodOrClass(cur_token);
                eat(Basic_Type.VARMETHODORCLASS);
                eat(Basic_Type.LEFT_BRACE);
                class_block(className.getName(), variables, methods, init_methods);
                eat(Basic_Type.RIGHT_BRACE);
                ClassDec nodeWithFather = new ClassDec(className, methods, variables, init_methods, FatherName);
                return nodeWithFather;
            }
            eat(Basic_Type.LEFT_BRACE);
            class_block(className.getName(), variables, methods, init_methods);
            eat(Basic_Type.RIGHT_BRACE);
            ClassDec node = new ClassDec(className, methods, variables, init_methods);
            return node;
        }

        private AST declaration_in_class_statement(string class_name, ACM acm)
        {
            TYPE type;
            if (cur_token.getType() == Basic_Type.VARMETHODORCLASS)
            {
                VarMethodOrClass node_name = (VarMethodOrClass)variableMethodOrClass(true);
                if (cur_token.getValue() == "(")
                {
                    if (node_name.getName() != class_name)
                    {
                        throw new ClassException("A return type is required", node_name.getLine());
                    }
                    eat(Basic_Type.LEFT_PAREN);
                    List<Param> paramsList = getListParam();
                    eat(Basic_Type.RIGHT_PAREN);
                    AST block = method_block();
                    InitMethodDec node = new InitMethodDec(node_name, block, paramsList, acm);
                    return node;
                }
                else
                {
                    type = declaration_class_type(node_name);
                }
            }
            else if (cur_token.getType() == Basic_Type.BUILDIN_CLASS)
            {
                type = new TYPE(cur_token);
                eat(Basic_Type.BUILDIN_CLASS);
            }
            else
            {
                type = declaration_type();
            }
            VarMethodOrClass var = (VarMethodOrClass)variableMethodOrClass(true);
            if (cur_token.getValue() == "=")
            {
                Token op = cur_token;
                eat(Basic_Type.ASSIGN);
                AST right = expr();  //for number operations only
                Assign assign = new Assign(var, right);
                MemberVariableDec node = new MemberVariableDec(var, type, assign, acm);
                eat(Basic_Type.SEMI);
                return node;
            }
            else if (cur_token.getValue() == "(")
            {
                eat(Basic_Type.LEFT_PAREN);
                List<Param> paramsList = getListParam();
                eat(Basic_Type.RIGHT_PAREN);
                AST block = method_block();
                if (var.getName() == "FixedUpdate" || var.getName() == "Start")
                {
                    acm = ACM.Public;
                }
                MemberMethodDec node = new MemberMethodDec(var, type, block, paramsList, acm);
                return node;
            }
            else
            {
                MemberVariableDec node = new MemberVariableDec(var, type, acm);
                eat(Basic_Type.SEMI);
                return node;
            }
        }

        private void class_block(string className, List<MemberVariableDec> variables, List<MemberMethodDec> methods, List<InitMethodDec> init_methods)
        {
            while (isACM(cur_token) || isType(cur_token) || cur_token.getType() == Basic_Type.VARMETHODORCLASS || cur_token.getType() == Basic_Type.BUILDIN_CLASS)
            {
                AST dec_node;
                if (cur_token.getType() == Basic_Type.PUBLIC)
                {
                    eat(Basic_Type.PUBLIC);
                    dec_node = declaration_in_class_statement(className, ACM.Public);
                }
                else if (cur_token.getType() == Basic_Type.PRIVATE)
                {
                    eat(Basic_Type.PRIVATE);
                    dec_node = declaration_in_class_statement(className, ACM.Private);
                }
                else
                {
                    dec_node = declaration_in_class_statement(className, ACM.Private);
                }


                if (dec_node.getAST_Type() == AST_Type.MEMBER_METHOD_DEC)
                {
                    methods.Add((MemberMethodDec)dec_node);
                }
                else if (dec_node.getAST_Type() == AST_Type.INIT_METHOD_DEC)
                {
                    init_methods.Add((InitMethodDec)dec_node);
                }
                else
                {
                    variables.Add((MemberVariableDec)dec_node);
                }
            }
            if (cur_token.getType() == Basic_Type.CLASS)
            {
                Debug.Log(cur_token.getValue());
                throw new ClassException("don't declare a class in a class", cur_token.getLineNum());
            }
        }



        private TYPE declaration_type()
        {
            TYPE type = new TYPE(cur_token);
            eat(cur_token.getType());
            return type;
        }

        private TYPE declaration_class_type(VarMethodOrClass name)
        {
            TYPE class_type = new TYPE(name);
            return class_type;
        }

        private AST handleLeftParen()
        {
            eat(Basic_Type.LEFT_PAREN);
            AST node = expr();
            eat(Basic_Type.RIGHT_PAREN);
            if (cur_token.getType() == Basic_Type.DOT)
            {
                eat(Basic_Type.DOT);
                ClassCall nextCall = classMemberCall();
                ClassCall classCall = new ClassCall(node, nextCall, nextCall.IfMethod());
                if (nextCall.IfMethod())
                {
                    if (cur_token.getType() == Basic_Type.ASSIGN)
                    {
                        throw new AssignException("unexpected method at the left side of assignment", cur_token.getLineNum());
                    }
                    return classCall;
                }
                else
                {
                    eat(Basic_Type.ASSIGN);
                    AST right = expr();
                    Assign assign_node = new Assign(classCall, right);
                    return assign_node;
                }
            }
            else
            {
                token_error();
                return null;
            }
        }

        private AST handleBuildInInstance()
        {
            VarMethodOrClass node = new VarMethodOrClass(cur_token);
            if (cur_token.getType() == Basic_Type.GAMEOBJECT)
            {
                eat(Basic_Type.GAMEOBJECT);
            }
            else
            {
                eat(Basic_Type.TRANSFORM);
            }
            if (cur_token.getType() == Basic_Type.DOT)
            {
                eat(Basic_Type.DOT);
                ClassCall nextCall = classMemberCall();
                ClassCall classCall = new ClassCall(node, nextCall, nextCall.IfMethod());
                if (nextCall.IfMethod())
                {
                    if (cur_token.getType() == Basic_Type.ASSIGN)
                    {
                        throw new AssignException("unexpected method at the left side of assignment", cur_token.getLineNum());
                    }
                    return classCall;
                }
                else
                {
                    eat(Basic_Type.ASSIGN);
                    AST right = expr();
                    Assign assign_node = new Assign(classCall, right);
                    return assign_node;
                }
            }
            else
            {
                eat(Basic_Type.ASSIGN);
                AST right = expr();
                Assign assign_node = new Assign(node, right);
                return assign_node;
            }
        }

        private AST handleBuildInClass()
        {
            TYPE buildInClass = new TYPE(cur_token);
            eat(Basic_Type.BUILDIN_CLASS);
            if (cur_token.getType() == Basic_Type.VARMETHODORCLASS)
            {
                VarMethodOrClass instanceVar = new VarMethodOrClass(cur_token);
                eat(Basic_Type.VARMETHODORCLASS);
                if (cur_token.getType() == Basic_Type.ASSIGN)
                {
                    eat(Basic_Type.ASSIGN);
                    AST right = expr();
                    Assign assign_node = new Assign(instanceVar, right);
                    return new VariableDec(instanceVar, buildInClass, assign_node);
                }
                else
                {
                    return new VariableDec(instanceVar, buildInClass);
                }
            }
            else if (cur_token.getType() == Basic_Type.DOT)
            {
                eat(Basic_Type.DOT);
                ClassCall nextCall = classMemberCall();
                ClassCall classCall = new ClassCall(buildInClass, nextCall, nextCall.IfMethod());
                if (nextCall.IfMethod())
                {
                    if (cur_token.getType() == Basic_Type.ASSIGN)
                    {
                        throw new AssignException("unexpected method at the left side of assignment", cur_token.getLineNum());
                    }
                    return classCall;
                }
                else
                {
                    eat(Basic_Type.ASSIGN);
                    AST right = expr();
                    Assign assign_node = new Assign(classCall, right);
                    return assign_node;
                }
            }
            else
            {
                if (cur_token.getType() == Basic_Type.GAMEOBJECT)
                {
                    throw new VarException("Variable name conflicts with object name", buildInClass.getLine());
                }
                throw new ClassException("not valid type name " + buildInClass.getName() + " in context", buildInClass.getLine());
            }
        }


        private AST handleVariableMethodOrClass()
        {
            AST node = variableMethodOrClass(false);
            if (node.getAST_Type() == AST_Type.VARMETHODORCLASS)
            {
                if (cur_token.getType() == Basic_Type.VARMETHODORCLASS)
                {
                    TYPE classType = new TYPE((VarMethodOrClass)node);
                    VarMethodOrClass instanceVar = new VarMethodOrClass(cur_token);
                    eat(Basic_Type.VARMETHODORCLASS);
                    if (cur_token.getType() == Basic_Type.ASSIGN)
                    {
                        eat(Basic_Type.ASSIGN);
                        AST right = expr();
                        Assign assign_node = new Assign(instanceVar, right);
                        return new VariableDec(instanceVar, classType, assign_node);
                    }
                    else
                    {
                        return new VariableDec(instanceVar, classType);
                    }
                }
                else if (cur_token.getType() == Basic_Type.PLUSPLUS)
                {
                    Token t = cur_token;
                    eat(Basic_Type.PLUSPLUS);
                    return new UnaryOP(t, node);
                }
                else if (cur_token.getType() == Basic_Type.MINUSMINUS)
                {
                    Token t = cur_token;
                    eat(Basic_Type.MINUSMINUS);
                    return new UnaryOP(t, node);
                }
                else
                {
                    eat(Basic_Type.ASSIGN);
                    AST right = expr();  //for number operations only
                    Assign assign_node = new Assign(node, right);
                    return assign_node;
                }
            }
            else if (node.getAST_Type() == AST_Type.METHOD_CALL)
            {
                if (cur_token.getType() == Basic_Type.ASSIGN)
                {
                    throw new AssignException("unexpected method at the left side of assignment", cur_token.getLineNum());
                }
                return node;
            }
            else if (node.getAST_Type() == AST_Type.CLASS_CALL)
            {
                ClassCall classCall = (ClassCall)node;
                if (classCall.IfMethod())
                {
                    if (cur_token.getType() == Basic_Type.ASSIGN)
                    {
                        throw new AssignException("unexpected method at the left side of assignment", cur_token.getLineNum());
                    }
                    return node;
                }
                else
                {
                    eat(Basic_Type.ASSIGN);
                    AST right = expr();  //for number operations only
                    Assign assign_node = new Assign(node, right);
                    return assign_node;
                }
            }
            else
            {
                throw new ASTException("unexpected AST node " + node.getAST_Type(), node.getLine());
            }
        }

        private AST variableMethodOrClass(bool ifDecMethod)
        {
            VarMethodOrClass node = new VarMethodOrClass(cur_token);
            eat(Basic_Type.VARMETHODORCLASS);
            if (ifDecMethod)
            {
                return node;
            }
            if (cur_token.getType() == Basic_Type.LEFT_PAREN)
            {
                eat(Basic_Type.LEFT_PAREN);
                List<ParamGiven> paramGivens = getParamGivens();
                eat(Basic_Type.RIGHT_PAREN);
                AST methodCall = new MethodCall(node, paramGivens);
                return methodCall;
            }
            if (cur_token.getType() == Basic_Type.DOT)
            {
                eat(Basic_Type.DOT);
                ClassCall nextCall = classMemberCall();
                ClassCall classCall = new ClassCall(node, nextCall, nextCall.IfMethod());
                return classCall;
            }
            return node;
        }

        private AST buildInClassCall()
        {
            TYPE buildInClass = new TYPE(cur_token);
            eat(Basic_Type.BUILDIN_CLASS);
            if (cur_token.getType() == Basic_Type.DOT)
            {
                eat(Basic_Type.DOT);
                ClassCall nextCall = classMemberCall();
                ClassCall classCall = new ClassCall(buildInClass, nextCall, nextCall.IfMethod());
                return classCall;
            }
            else
            {
                throw new ClassException("not valid type name " + buildInClass.getName() + " in context", buildInClass.getLine());
            }
        }

        private AST buildInInstanceClassCall()
        {
            VarMethodOrClass node = new VarMethodOrClass(cur_token);
            if (cur_token.getType() == Basic_Type.GAMEOBJECT)
            {
                eat(Basic_Type.GAMEOBJECT);
            }
            else
            {
                eat(Basic_Type.TRANSFORM);
            }
            if (cur_token.getType() == Basic_Type.DOT)
            {
                eat(Basic_Type.DOT);
                ClassCall nextCall = classMemberCall();
                ClassCall classCall = new ClassCall(node, nextCall, nextCall.IfMethod());
                return classCall;
            }
            else
            {
                return node;
            }
        }

        private AST class_instance_init()
        {
            if (cur_token.getType() == Basic_Type.BUILDIN_CLASS)
            {
                VarMethodOrClass node = new VarMethodOrClass(cur_token);
                eat(Basic_Type.BUILDIN_CLASS);
                eat(Basic_Type.LEFT_PAREN);
                List<ParamGiven> paramGivens = getParamGivens();
                eat(Basic_Type.RIGHT_PAREN);
                BuildInInitMethodCall buildInitMethodCall = new BuildInInitMethodCall(node, paramGivens);
                if (cur_token.getType() == Basic_Type.DOT)
                {
                    eat(Basic_Type.DOT);
                    ClassCall nextCall = classMemberCall();
                    ClassCall classCall = new ClassCall(buildInitMethodCall, nextCall, nextCall.IfMethod());
                    return classCall;
                }
                return buildInitMethodCall;
            }
            else
            {
                VarMethodOrClass node = new VarMethodOrClass(cur_token);
                eat(Basic_Type.VARMETHODORCLASS);
                eat(Basic_Type.LEFT_PAREN);
                List<ParamGiven> paramGivens = getParamGivens();
                eat(Basic_Type.RIGHT_PAREN);
                InitMethodCall initMethodCall = new InitMethodCall(node, paramGivens);
                if (cur_token.getType() == Basic_Type.DOT)
                {
                    eat(Basic_Type.DOT);
                    ClassCall nextCall = classMemberCall();
                    ClassCall classCall = new ClassCall(initMethodCall, nextCall, nextCall.IfMethod());
                    return classCall;
                }
                return initMethodCall;
            }
        }

        private ClassCall classMemberCall()
        {
            VarMethodOrClass node = new VarMethodOrClass(cur_token);
            if (cur_token.getType() == Basic_Type.TRANSFORM)
            {
                eat(Basic_Type.TRANSFORM);
            }
            else
            {
                eat(Basic_Type.VARMETHODORCLASS);
            }
            if (cur_token.getType() == Basic_Type.LEFT_PAREN)
            {
                eat(Basic_Type.LEFT_PAREN);
                List<ParamGiven> paramGivens = getParamGivens();
                eat(Basic_Type.RIGHT_PAREN);
                MethodCall methodCall = new MethodCall(node, paramGivens);
                if (cur_token.getType() == Basic_Type.DOT)
                {
                    eat(Basic_Type.DOT);
                    ClassCall next_call = classMemberCall();
                    return new ClassCall(methodCall, next_call, next_call.IfMethod());
                }
                else
                {
                    return new ClassCall(methodCall, null, true);
                }
            }

            if (cur_token.getType() == Basic_Type.DOT)
            {
                eat(Basic_Type.DOT);
                ClassCall next_call = classMemberCall();
                return new ClassCall(node, next_call, next_call.IfMethod());
            }
            else
            {
                return new ClassCall(node, null, false);
            }
        }

        private List<ParamGiven> getParamGivens()
        {
            List<ParamGiven> list = new List<ParamGiven>();
            ParamGiven pg = paramGiven();
            if (pg == null)
            {
                return list;
            }
            list.Add(pg);
            while (cur_token.getType() == Basic_Type.COMMA)
            {
                eat(Basic_Type.COMMA);
                list.Add(paramGiven());
            }
            return list;
        }

        private ParamGiven paramGiven()
        {
            if (cur_token.getType() == Basic_Type.RIGHT_PAREN)
            {
                return null;
            }
            else
            {
                return new ParamGiven(expr());
            }
        }

        private AST return_statement()
        {
            eat(Basic_Type.RET);
            AST node = expr();
            RET ret_node = new RET(node);
            return ret_node;
        }

        private AST for_statement()
        {
            eat(Basic_Type.FOR);
            eat(Basic_Type.LEFT_PAREN);
            AST statement1 = method_statement();
            eat(Basic_Type.SEMI);
            AST statement2 = expr();
            eat(Basic_Type.SEMI);
            AST statement3 = method_statement();
            eat(Basic_Type.RIGHT_PAREN);
            Compound block_node = method_block();
            getOutOfBlockInMethod = true;
            For_Block forBlock = new For_Block(block_node, statement1, statement2, statement3);
            return forBlock;
        }

        private AST while_statement()
        {
            eat(Basic_Type.WHILE);
            eat(Basic_Type.LEFT_PAREN);
            AST cond_node = expr();
            eat(Basic_Type.RIGHT_PAREN);
            Compound block_node = method_block();
            getOutOfBlockInMethod = true;
            While_Block whileBlock = new While_Block(block_node, cond_node);
            return whileBlock;
        }

        private AST if_statement()
        {
            eat(Basic_Type.IF);
            eat(Basic_Type.LEFT_PAREN);
            AST cond_node = expr();
            eat(Basic_Type.RIGHT_PAREN);
            Compound block_node = method_block();
            //eat(Basic_Type.SEMI);
            If_Block ifBlock = new If_Block(block_node, cond_node);
            List<ElSE_IF_BLOCK> elseIfs = new List<ElSE_IF_BLOCK>();
            while (cur_token.getType() == Basic_Type.ELSE)
            {
                eat(Basic_Type.ELSE);
                if (cur_token.getType() == Basic_Type.IF)
                {
                    eat(Basic_Type.IF);
                    eat(Basic_Type.LEFT_PAREN);
                    AST cond_node_elseIf = expr();
                    eat(Basic_Type.RIGHT_PAREN);
                    Compound block_node_elseIf = method_block();
                    //eat(Basic_Type.SEMI);
                    elseIfs.Add(new ElSE_IF_BLOCK(block_node_elseIf, cond_node_elseIf));
                }
                else
                {
                    Compound block_node_else = method_block();
                    ifBlock.setElseBlock(block_node_else);
                    break;
                }
            }
            if (cur_token.getType() == Basic_Type.ELSE)
            {
                token_error();
            }
            //cur_token = new Token(Basic_Type.SEMI, ";", cur_token.getLineNum());
            ifBlock.setElseIfs(elseIfs);
            getOutOfBlockInMethod = true;
            return ifBlock;
        }

        private AST empty()
        {
            return new Empty();
        }

        private AST factor()
        {
            Token token = cur_token;
            if (token.getType() == Basic_Type.INT)
            {
                eat(Basic_Type.INT);
                return new Num(token);
            }
            else if (token.getType() == Basic_Type.DOUBLE)
            {
                eat(Basic_Type.DOUBLE);
                return new Num(token);
            }
            else if (token.getType() == Basic_Type.FLOAT)
            {
                eat(Basic_Type.FLOAT);
                return new Num(token);
            }
            else if (token.getType() == Basic_Type.BOOL)
            {
                eat(Basic_Type.BOOL);
                return new Num(token);
            }
            else if (token.getType() == Basic_Type.LONG)
            {
                eat(Basic_Type.LONG);
                return new Num(token);
            }
            else if (token.getType() == Basic_Type.DECIMAL)
            {
                eat(Basic_Type.DECIMAL);
                return new Num(token);
            }
            else if (token.getType() == Basic_Type.LEFT_PAREN)
            {
                eat(Basic_Type.LEFT_PAREN);
                AST node;
                if (isType(cur_token))
                {
                    TYPE type = declaration_type();
                    eat(Basic_Type.RIGHT_PAREN);
                    AST right = expr();
                    node = new TypeAlterOP(type, right);
                }
                else
                {
                    node = expr();
                    eat(Basic_Type.RIGHT_PAREN);
                    if (cur_token.getType() == Basic_Type.DOT)
                    {
                        eat(Basic_Type.DOT);
                        ClassCall next_call = classMemberCall();
                        return new ClassCall(node, next_call, next_call.IfMethod());
                    }
                }
                return node;
            }
            else if (token.getType() == Basic_Type.PLUS)
            {
                eat(Basic_Type.PLUS);
                return new UnaryOP(token, factor());
            }
            else if (token.getType() == Basic_Type.MINUS)
            {
                eat(Basic_Type.MINUS);
                return new UnaryOP(token, factor());
            }
            else if (token.getType() == Basic_Type.PLUSPLUS)
            {
                eat(Basic_Type.PLUSPLUS);
                AST node = variableMethodOrClass(false);
                if (node.getAST_Type() == AST_Type.METHOD_CALL)
                {
                    throw new TokenException("++ can only act on variables", node.getLine());
                }
                else if (node.getAST_Type() == AST_Type.CLASS_CALL)
                {
                    ClassCall temp = (ClassCall)node;
                    if (temp.IfMethod())
                    {
                        throw new TokenException("++ can only act on variables", node.getLine());
                    }
                }
                return new UnaryOP(token, node);
            }
            else if (token.getType() == Basic_Type.MINUSMINUS)
            {
                eat(Basic_Type.MINUSMINUS);
                AST node = variableMethodOrClass(false);
                if (node.getAST_Type() == AST_Type.METHOD_CALL)
                {
                    throw new TokenException("-- can only act on variables", node.getLine());
                }
                else if (node.getAST_Type() == AST_Type.CLASS_CALL)
                {
                    ClassCall temp = (ClassCall)node;
                    if (temp.IfMethod())
                    {
                        throw new TokenException("-- can only act on variables", node.getLine());
                    }
                }
                return new UnaryOP(token, node);
            }
            else if (token.getType() == Basic_Type.BITNOT)
            {
                eat(Basic_Type.BITNOT);
                return new UnaryOP(token, factor());
            }
            else if (token.getType() == Basic_Type.NOT)
            {
                eat(Basic_Type.NOT);
                return new UnaryOP(token, factor());
            }
            else if (cur_token.getType() == Basic_Type.NEW)
            {
                eat(Basic_Type.NEW);
                return class_instance_init();
            }
            else if (token.getType() == Basic_Type.VARMETHODORCLASS)
            {
                AST node = variableMethodOrClass(false);
                if (cur_token.getType() == Basic_Type.PLUSPLUS)
                {
                    if (node.getAST_Type() == AST_Type.METHOD_CALL)
                    {
                        throw new TokenException("++ can only act on variables", node.getLine());
                    }
                    else if (node.getAST_Type() == AST_Type.CLASS_CALL)
                    {
                        ClassCall temp = (ClassCall)node;
                        if (temp.IfMethod())
                        {
                            throw new TokenException("++ can only act on variables", node.getLine());
                        }
                    }
                    Token t = cur_token;
                    eat(Basic_Type.PLUSPLUS);
                    UnaryOP uOP = new UnaryOP(t, node);
                    uOP.setRight();
                    return uOP;
                }
                else if (cur_token.getType() == Basic_Type.MINUSMINUS)
                {
                    if (node.getAST_Type() == AST_Type.METHOD_CALL)
                    {
                        throw new TokenException("-- can only act on variables", node.getLine());
                    }
                    else if (node.getAST_Type() == AST_Type.CLASS_CALL)
                    {
                        ClassCall temp = (ClassCall)node;
                        if (temp.IfMethod())
                        {
                            throw new TokenException("-- can only act on variables", node.getLine());
                        }
                    }
                    Token t = cur_token;
                    eat(Basic_Type.MINUSMINUS);
                    UnaryOP uOP = new UnaryOP(t, node);
                    uOP.setRight();
                    return uOP;
                }
                return node;
            }
            else if (token.getType() == Basic_Type.BUILDIN_CLASS)
            {
                return buildInClassCall();
            }
            else if (token.getType() == Basic_Type.GAMEOBJECT)
            {
                return buildInInstanceClassCall();
            }
            else if (token.getType() == Basic_Type.TRANSFORM)
            {
                return buildInInstanceClassCall();
            }
            else
            {
                token_error();
                return null;
            }
        }

        private AST term()
        {
            AST node = factor();
            while (cur_token.getType() == Basic_Type.MUL || cur_token.getType() == Basic_Type.DIV || cur_token.getType() == Basic_Type.REMAINDER)
            {
                Token token = cur_token;
                if (token.getType() == Basic_Type.MUL)
                {
                    eat(Basic_Type.MUL);
                }
                else if (token.getType() == Basic_Type.DIV)
                {
                    eat(Basic_Type.DIV);
                }
                else if (token.getType() == Basic_Type.REMAINDER)
                {
                    eat(Basic_Type.REMAINDER);
                }
                node = new BinOP(node, token, factor());
            }
            return node;
        }

        private AST exprPulsAndMinus()
        {
            AST node = term();
            while (cur_token.getType() == Basic_Type.PLUS || cur_token.getType() == Basic_Type.MINUS)
            {
                Token token = cur_token;
                if (token.getType() == Basic_Type.PLUS)
                {
                    eat(Basic_Type.PLUS);
                }
                else if (token.getType() == Basic_Type.MINUS)
                {
                    eat(Basic_Type.MINUS);
                }
                node = new BinOP(node, token, term());
            }
            return node;
        }

        private AST exprShift()
        {
            AST node = exprPulsAndMinus();
            while (cur_token.getType() == Basic_Type.RIGHT_SHIFT || cur_token.getType() == Basic_Type.LEFT_SHIFT)
            {
                Token token = cur_token;
                if (token.getType() == Basic_Type.RIGHT_SHIFT)
                {
                    eat(Basic_Type.RIGHT_SHIFT);
                }
                else if (token.getType() == Basic_Type.LEFT_SHIFT)
                {
                    eat(Basic_Type.LEFT_SHIFT);
                }
                node = new BinOP(node, token, exprPulsAndMinus());
            }
            return node;
        }

        private AST exprGreaterAndLess()
        {
            AST node = exprShift();
            while (cur_token.getType() == Basic_Type.GREATER || cur_token.getType() == Basic_Type.LESS || cur_token.getType() == Basic_Type.GEQUAL || cur_token.getType() == Basic_Type.LEQUAL)
            {
                Token token = cur_token;
                if (token.getType() == Basic_Type.GREATER)
                {
                    eat(Basic_Type.GREATER);
                }
                else if (token.getType() == Basic_Type.LESS)
                {
                    eat(Basic_Type.LESS);
                }
                else if (token.getType() == Basic_Type.LEQUAL)
                {
                    eat(Basic_Type.LEQUAL);
                }
                else if (token.getType() == Basic_Type.GEQUAL)
                {
                    eat(Basic_Type.GEQUAL);
                }
                node = new BinOP(node, token, exprShift());
            }
            return node;
        }

        private AST exprEqualAndNotEqual()
        {
            AST node = exprGreaterAndLess();
            while (cur_token.getType() == Basic_Type.EQUAL || cur_token.getType() == Basic_Type.NEQUAL)
            {
                Token token = cur_token;
                if (token.getType() == Basic_Type.EQUAL)
                {
                    eat(Basic_Type.EQUAL);
                }
                if (token.getType() == Basic_Type.NEQUAL)
                {
                    eat(Basic_Type.NEQUAL);
                }
                node = new BinOP(node, token, exprGreaterAndLess());
            }
            return node;
        }

        private AST exprAND()
        {
            AST node = exprEqualAndNotEqual();
            while (cur_token.getType() == Basic_Type.AND)
            {
                Token token = cur_token;
                eat(Basic_Type.AND);
                node = new BinOP(node, token, exprEqualAndNotEqual());
            }
            return node;
        }

        private AST exprXOR()
        {
            AST node = exprAND();
            while (cur_token.getType() == Basic_Type.XOR)
            {
                Token token = cur_token;
                eat(Basic_Type.XOR);
                node = new BinOP(node, token, exprAND());
            }
            return node;
        }

        private AST exprOR()
        {
            AST node = exprXOR();
            while (cur_token.getType() == Basic_Type.OR) 
            {
                Token token = cur_token;
                eat(Basic_Type.OR);
                node = new BinOP(node, token, exprXOR());
            }
            return node;
        }

        private AST exprANDAND()
        {
            AST node = exprOR();
            while (cur_token.getType() == Basic_Type.ANDAND)
            {
                Token token = cur_token;
                eat(Basic_Type.ANDAND);
                node = new BinOP(node, token, exprOR());
            }
            return node;
        }

        private AST exprOROR()
        {
            AST node = exprANDAND();
            while (cur_token.getType() == Basic_Type.OROR)
            {
                Token token = cur_token;
                eat(Basic_Type.OROR);
                node = new BinOP(node, token, exprANDAND());
            }
            return node;
        }

        private AST expr()
        {
            return exprOROR();
        }

        public AST getAST()
        {
            //return expr();
            AST node = program_block();
            if (cur_token.getType() != Basic_Type.EOF)
            {
                token_error();
            }
            return node;
        }

    }
}
