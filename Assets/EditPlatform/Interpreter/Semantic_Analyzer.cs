using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interpreter_Basic;
using Interpreter_Lexer;
using Interpreter_Parser;

namespace Interpreter_Semantic_Analyzer
{
    public class Value
    {
        protected string type;
        protected bool ifClassType;
        protected bool ifCanFree;
        private int index;

        public Value(string t)
        {
            type = t;
            ifClassType = false;
            ifCanFree = true;
        }

        public string getType()
        {
            return type;
        }

        public int getIndex()
        {
            if (!ifClassType)
            {
                return index;
            }
            else
            {
                return 0;
            }
        }

        public void setIndex(int i)
        {
            index = i;
        }

        public bool getIfClassType()
        {
            return ifClassType;
        }

        public void setCanNotFree()
        {
            ifCanFree = false;
        }

        public void setCanFree()
        {
            ifCanFree = true;
        }

        public bool getIfCanFree()
        {
            return ifCanFree;
        }

    }

    public class QuaternionIntanceValue : Value
    {
        private Quaternion quaternion;
        public QuaternionIntanceValue(Quaternion q) : base("Quaternion")
        {
            type = "Quaternion";
            ifClassType = true;
            ifCanFree = false;
            quaternion = q;
        }

        public Quaternion getQuaternion()
        {
            return quaternion;
        }

        public void setQuaternion(Quaternion q)
        {
            quaternion = q;
        }
    }

    public class Vector3IntanceValue : Value
    {
        private Vector3 vector3;
        public Vector3IntanceValue(Vector3 v) : base("Vector3")
        {
            type = "Vector3";
            ifClassType = true;
            ifCanFree = false;
            vector3 = v;
        }

        public Vector3 getVector3()
        {
            return vector3;
        }

        public void  setVector3(Vector3 v)
        {
            vector3 = v;
        }
    }

    public class TransformInstanceValue : Value
    {
        private Transform transform;

        public TransformInstanceValue(Transform t) : base("Transform")
        {
            type = "Transform";
            ifClassType = true;
            ifCanFree = false;
            transform = t;
        }

        public Transform getTransform()
        {
            return transform;
        }

        public bool ifNull()
        {
            return (transform == null);
        }
    }


    public class GameObjectInstanceValue : Value
    {
        private GameObject gameObject;
        public GameObjectInstanceValue(GameObject g) : base("GameObject")
        {
            type = "GameObject";
            ifClassType = true;
            ifCanFree = false;
            gameObject = g;
        }

        public GameObject getGameObject()
        {
            return gameObject;
        }

        public bool ifNull()
        {
            return (gameObject == null);
        }
    }

    public class ClassInstanceValue : Value
    {
        private Dictionary<string, Value> ClassValueDic = new Dictionary<string, Value>();

        public ClassInstanceValue(string t, Dictionary<string, SymbolTable> symbolTables) : base(t)
        {
            type = t;
            ifClassType = true;
            ifCanFree = false;
            foreach (var table_v in symbolTables[t].getSymbolTable())
            {
                if (table_v.Value.getST() == Symbol_Type.MemberVarSymbol)
                {
                    MemberVarSymbol mv = (MemberVarSymbol)table_v.Value;
                    ClassValueDic.Add(mv.getName(), null);
                }
            }
        }

        public void setMemberValue(string memberVar, Value v, int l)
        {
            if (!ClassValueDic.ContainsKey(memberVar))
            {
                throw new VarException("unexpected member name " + memberVar, l);
            }
            ClassValueDic[memberVar] = v;
        }

        public Value getMemberValue(string memberVar, int l)
        {
            if (!ClassValueDic.ContainsKey(memberVar))
            {
                throw new VarException("unexpected member name " + memberVar, l);
            }
            return ClassValueDic[memberVar];
        }

        public bool containsMember(string memberVar)
        {
            return ClassValueDic.ContainsKey(memberVar);
        }

        public Dictionary<string, Value> forDebug()
        {
            return ClassValueDic;
        }
    }

    public class Symbol
    {
        protected string name;
        protected Symbol_Type symbol_type;

        public Symbol(string n)
        {
            name = n;
        }

        public void setSymbol_Type(Symbol_Type st)
        {
            symbol_type = st;
        }

        public string getName()
        {
            return name;
        }

        public Symbol_Type getST()
        {
            return symbol_type;
        }
    }

    public class TypeSymbol : Symbol
    {
        public TypeSymbol(string n) : base(n)
        {
            setSymbol_Type(Symbol_Type.TypeSymbol);
            Debug.Log("typeSymbol{" + name + ", " + symbol_type + "}");
        }
    }

    public class VarSymbol : Symbol
    {
        private TypeSymbol typeSymbol;
        private Stack<bool> defineFlag_stack = new Stack<bool>();
        private Stack<Value> value_stack = new Stack<Value>();

        public VarSymbol(string n, TypeSymbol type_symbol) : base(n)
        {
            setSymbol_Type(Symbol_Type.VarSymbol);
            typeSymbol = type_symbol;
            Debug.Log("varSymbol{" + name + ", " + symbol_type + "}");
        }

        public TypeSymbol getTypeSymbol()
        {
            return typeSymbol;
        }

        public void pushDefineFlag(bool b)
        {
            defineFlag_stack.Push(b);
        }

        public bool peekDefineFlag()
        {
            return defineFlag_stack.Peek();
        }

        public bool popDefineFlag()
        {
            return defineFlag_stack.Pop();
        }

        public Value popValue()
        {
            return value_stack.Pop();
        }

        public void pushValue(Value value)
        {
            value_stack.Push(value);
        }

        public Value peekValue()
        {
            return value_stack.Peek();
        }

        public bool ifValueStackEmpty()
        {
            return value_stack.Count == 0;
        }

        public bool ifFlagStackEmpty()
        {
            return defineFlag_stack.Count == 0;
        }
    }

    public class MemberVarSymbol : Symbol
    {
        private TypeSymbol typeSymbol;
        private ACM acm;

        public MemberVarSymbol(string n, TypeSymbol type_symbol, ACM a) : base(n)
        {
            setSymbol_Type(Symbol_Type.MemberVarSymbol);
            typeSymbol = type_symbol;
            acm = a;
            Debug.Log("varSymbol{" + name + ", " + symbol_type + "}");
        }

        public TypeSymbol getTypeSymbol()
        {
            return typeSymbol;
        }

        public ACM getACM()
        {
            return acm;
        }
    }

    public class MemberMethodSymbol : Symbol
    {
        private TypeSymbol ret_type;
        private List<string> parameters = new List<string>();
        private AST blockNode;
        private ACM acm;

        public MemberMethodSymbol(string n, TypeSymbol rt, List<string> p, AST b, ACM a) : base(n)
        {
            setSymbol_Type(Symbol_Type.MemberMethodSymbol);
            ret_type = rt;
            parameters = p;
            blockNode = b;
            acm = a;
            name += "(";
            Debug.Log("methodSymbol{" + name + ", " + ret_type.getName() + "}");
        }

        public TypeSymbol getRetType()
        {
            return ret_type;
        }

        public List<string> getParams()
        {
            return parameters;
        }

        public AST getBlockNode()
        {
            return blockNode;
        }

        public ACM getACM()
        {
            return acm;
        }
    }

    public class InitMethodSymbol : Symbol
    {
        private List<string> parameters = new List<string>();
        private AST blockNode;
        private ACM acm;

        public InitMethodSymbol(string n,List<string> p, AST b, ACM a) : base(n)
        {
            setSymbol_Type(Symbol_Type.InitMethodSymbol);
            parameters = p;
            blockNode = b;
            acm = a;
            name += "(";
            Debug.Log("methodSymbol{" + name + "}");
        }

        public List<string> getParams()
        {
            return parameters;
        }

        public AST getBlockNode()
        {
            return blockNode;
        }

        public ACM getACM()
        {
            return acm;
        }
    }

    public class ParamSymbol : Symbol
    {
        private TypeSymbol typeSymbol;
        private Stack<Value> value_stack = new Stack<Value>();

        public ParamSymbol(string n, TypeSymbol type_symbol) : base(n)
        {
            setSymbol_Type(Symbol_Type.ParamSymbol);
            typeSymbol = type_symbol;
            Debug.Log("paramSymbol{" + name + ", " + symbol_type + "}");
        }

        public Value popValue()
        {
            return value_stack.Pop();
        }

        public void pushValue(Value value)
        {
            value_stack.Push(value);
        }

        public Value peekValue()
        {
            return value_stack.Peek();
        }

        public bool ifStackEmpty()
        {
            return value_stack.Count == 0;
        }

        public TypeSymbol getTypeSymbol()
        {
            return typeSymbol;
        }
    }

    public class SymbolTable
    {
        private Dictionary<string, Symbol> symbolTable = new Dictionary<string, Symbol>();
        private int scope_level;
        private List<Symbol> decOwnVars = new List<Symbol>();

        public SymbolTable(int l)
        {
            scope_level = l;
        }

        public void init_buildIn()
        {
            TypeSymbol int_symbol = new TypeSymbol("int");
            TypeSymbol double_symbol = new TypeSymbol("double");
            TypeSymbol float_symbol = new TypeSymbol("float");
            TypeSymbol bool_symbol = new TypeSymbol("bool");
            TypeSymbol void_symbol = new TypeSymbol("void");
            TypeSymbol decimal_symbol = new TypeSymbol("decimal");
            TypeSymbol long_symbol = new TypeSymbol("long");
            TypeSymbol Vector3_symbol = new TypeSymbol("Vector3");
            TypeSymbol Quaternion_symbol = new TypeSymbol("Quaternion");
            TypeSymbol Transform_symbol = new TypeSymbol("Transform");
            TypeSymbol GameObject_symbol = new TypeSymbol("GameObject");
            /////////////////////////////////
            //List<string> param1 = new List<string>();
            //param1.Add("sqrtVal");
            //ParamSymbol sqrtParam = new ParamSymbol("sqrtVal", double_symbol);
            //Token sqrt_token = new Token(Basic_Type.SQRT, "sqrt", 0);
            //Token sqrt_var = new Token(Basic_Type.VARMETHODORCLASS, "sqrtVal", 0);
            //VarMethodOrClass sqrt_varAST = new VarMethodOrClass(sqrt_var);
            //UnaryOP sqrt_uop = new UnaryOP(sqrt_token, sqrt_varAST);
            //RET retAST = new RET(sqrt_uop);
            //MethodSymbol SqrtSymbol = new MethodSymbol("Sqrt", double_symbol, param1, retAST);
            //symbolTable.Add(sqrtParam.getName(), sqrtParam);
            //symbolTable.Add(SqrtSymbol.getName(), SqrtSymbol);
            //////////////////////////////////
            symbolTable.Add("int", int_symbol);
            symbolTable.Add("double", double_symbol);
            symbolTable.Add("float", float_symbol);
            symbolTable.Add("bool", bool_symbol);
            symbolTable.Add("void", void_symbol);
            symbolTable.Add("decimal", decimal_symbol);
            symbolTable.Add("long", long_symbol);
            symbolTable.Add("Vector3", Vector3_symbol);
            symbolTable.Add("Quaternion", Quaternion_symbol);
            symbolTable.Add("Transform", Transform_symbol);
            symbolTable.Add("GameObject", GameObject_symbol);
        }

        public void decOwnVar(VarSymbol v)
        {
            decOwnVars.Add(v);
        }

        public void define(Symbol s)
        {
            symbolTable[s.getName()] = s;
        }

        public TypeSymbol getTypeSymbol(string name, int l)
        {
            if (!symbolTable.ContainsKey(name))
            {
                throw new TypeException("unexpected type name " + name, l);
            }
            Symbol aSymbol = symbolTable[name];
            if (aSymbol.getST() != Symbol_Type.TypeSymbol)
            {
                throw new TypeException("unexpected type name " + name, l);
            }
            TypeSymbol theSymbol = (TypeSymbol)aSymbol;
            return theSymbol;
        }

        public void setVarSymbolDefineFlag(string name, int line)
        {
            if (!symbolTable.ContainsKey(name))
            {
                throw new VarException("undefined variable name " + name, 0);
            }
            Symbol aSymbol = symbolTable[name];
            if (aSymbol.getST() == Symbol_Type.VarSymbol)
            {
                VarSymbol theSymbol = (VarSymbol)aSymbol;
                if (theSymbol.ifFlagStackEmpty())
                {
                    theSymbol.pushDefineFlag(true);
                }
                else
                {
                    theSymbol.popDefineFlag();
                    theSymbol.pushDefineFlag(true);
                }
            }
            else
            {
                throw new VarException("undefined variable name " + name, 0);
            }
        }

        
        public Value getVarOrParamSymbleValueDefineFlagAndDefineType(string name, out string t, out bool f, out bool a, int l, ClassInstanceValue env)
        {
            if (!symbolTable.ContainsKey(name))
            {
                throw new VarException("undefined variable name " + name, l);
            }
            Symbol aSymbol = symbolTable[name];
            if (aSymbol.getST() == Symbol_Type.VarSymbol)
            {
                VarSymbol theSymbol = (VarSymbol)aSymbol;
                Value value;
                if (theSymbol.ifValueStackEmpty())
                {
                    value = null;
                }
                else
                {
                    value = theSymbol.peekValue();
                }
                t = theSymbol.getTypeSymbol().getName();
                if (theSymbol.ifFlagStackEmpty())
                {
                    f = false;
                }
                else
                {
                    f = theSymbol.peekDefineFlag();
                }
                a = true;
                return value;

            }
            else if (aSymbol.getST() == Symbol_Type.ParamSymbol)
            {
                ParamSymbol theSymbol = (ParamSymbol)aSymbol;
                Value value;
                if (theSymbol.ifStackEmpty())
                {
                    value = null;
                }
                else
                {
                    value = theSymbol.peekValue();
                }
                t = theSymbol.getTypeSymbol().getName();
                f = true;
                a = true;
                return value;
            }
            else if (aSymbol.getST() == Symbol_Type.MemberVarSymbol)
            {
                MemberVarSymbol theSymbol = (MemberVarSymbol)aSymbol;
                string varName = theSymbol.getName();
                Value value = env.getMemberValue(varName, l);
                t = theSymbol.getTypeSymbol().getName();
                f = true;
                if (theSymbol.getACM() == ACM.Public)
                {
                    a = true;
                }
                else
                {
                    a = false;
                }
                return value;
            }
            else
            {
                throw new VarException("undefined variable name " + name, l);
            }
        }

        public List<Value> popOwnVarSymbolValue()
        {
            List<Value> lv = new List<Value>();
            foreach (VarSymbol vs in decOwnVars)
            {
                Value value = vs.popValue();
                vs.popDefineFlag();
                lv.Add(value);
            }
            return lv;
        }

        public Value popParamSymbleValue(string name, int l)
        {
            if (!symbolTable.ContainsKey(name))
            {
                throw new ParamException("undefined variable name " + name, l);
            }
            Symbol aSymbol = symbolTable[name];
            if (aSymbol.getST() == Symbol_Type.ParamSymbol)
            {
                ParamSymbol theSymbol = (ParamSymbol)aSymbol;
                Value v = theSymbol.popValue();
                return v;
            }
            else
            {
                throw new ParamException("undefined variable name " + name, l);
            }
        }

        public string getMemberVarType(string name, int l)
        {
            if (!symbolTable.ContainsKey(name))
            {
                throw new VarException("undefined variable name " + name, l);
            }
            Symbol aSymbol = symbolTable[name];
            if (aSymbol.getST() == Symbol_Type.MemberVarSymbol)
            {
                MemberVarSymbol theSymbol = (MemberVarSymbol)aSymbol;
                return theSymbol.getTypeSymbol().getName();
            }
            else
            {
                throw new VarException("undefined variable name " + name, l);
            }
        }

        public string getParamsType(string name, int l)
        {
            if (!symbolTable.ContainsKey(name))
            {
                throw new VarException("undefined paramter name " + name, l);
            }
            Symbol aSymbol = symbolTable[name];
            if (aSymbol.getST() == Symbol_Type.ParamSymbol)
            {
                ParamSymbol theSymbol = (ParamSymbol)aSymbol;
                return theSymbol.getTypeSymbol().getName();
            }
            else
            {
                throw new VarException("undefined paramter name " + name, l);
            }
        }

        public void setVarOrParamSymbolValue(Value v, string name, int l, ClassInstanceValue env)
        {
            if (!symbolTable.ContainsKey(name))
            {
                throw new VarException("undefined variable name " + name, l);
            }
            Symbol aSymbol = symbolTable[name];
            if (aSymbol.getST() == Symbol_Type.VarSymbol)
            {
                VarSymbol theSymbol = (VarSymbol)aSymbol;
                if (theSymbol.ifValueStackEmpty())
                {
                    theSymbol.pushValue(v);
                }
                else
                {
                    theSymbol.popValue();
                    theSymbol.pushValue(v);
                }
            }
            else if (aSymbol.getST() == Symbol_Type.ParamSymbol)
            {
                ParamSymbol theSymbol = (ParamSymbol)aSymbol;
                if (theSymbol.ifStackEmpty())
                {
                    theSymbol.pushValue(v);
                }
                else
                {
                    theSymbol.popValue();
                    theSymbol.pushValue(v);
                }
            }
            else if (aSymbol.getST() == Symbol_Type.MemberVarSymbol)
            {
                MemberVarSymbol theSymbol = (MemberVarSymbol)aSymbol;
                string varName = theSymbol.getName();
                env.setMemberValue(varName, v, l);
            }
            else
            {
                throw new VarException("undefined variable name " + name, l);
            }
        }

        public void pushOwnVarSymbolValue()
        {
            foreach (VarSymbol vs in decOwnVars)
            {
                vs.pushValue(null);
                vs.pushDefineFlag(false);
            }
        }

        public void pushParamSymbolValue(Value v, string name, int l)
        {
            if (!symbolTable.ContainsKey(name))
            {
                throw new ParamException("undefined variable name " + name, l);
            }
            Symbol aSymbol = symbolTable[name];
            if (aSymbol.getST() == Symbol_Type.ParamSymbol)
            {
                ParamSymbol theSymbol = (ParamSymbol)aSymbol;
                theSymbol.pushValue(v);
            }
            else
            {
                throw new ParamException("undefined variable name " + name, l);
            }
        }

        public List<string> getParamsRetTypeAndBlockNode(string name, out string t, out AST node, out bool a, int l)
        {
            name += "(";
            if (!symbolTable.ContainsKey(name))
            {
                throw new MethodException("undefined method name " + name, l);
            }
            Symbol aSymbol = symbolTable[name];
            if (aSymbol.getST() == Symbol_Type.MemberMethodSymbol)
            {
                MemberMethodSymbol theSymbol = (MemberMethodSymbol)aSymbol;
                t = theSymbol.getRetType().getName();
                node = theSymbol.getBlockNode();
                if (theSymbol.getACM() == ACM.Public)
                {
                    a = true;
                }
                else
                {
                    a = false;
                }
                return theSymbol.getParams();
            }
            else if (aSymbol.getST() == Symbol_Type.InitMethodSymbol)
            {
                InitMethodSymbol theSymbol = (InitMethodSymbol)aSymbol;
                t = "void";
                node = theSymbol.getBlockNode();
                if (theSymbol.getACM() == ACM.Public)
                {
                    a = true;
                }
                else
                {
                    a = false;
                }
                return theSymbol.getParams();
            }
            else
            {
                throw new MethodException("undefined method name " + name, l);
            }
        }

        public Dictionary<string, Symbol> getSymbolTable()
        {
            return symbolTable;
        }

        public string getRetType(string name, int l)
        {
            name += "(";
            if (!symbolTable.ContainsKey(name))
            {
                throw new MethodException("undefined method name " + name, l);
            }
            Symbol aSymbol = symbolTable[name];
            if (aSymbol.getST() == Symbol_Type.MemberMethodSymbol)
            {
                MemberMethodSymbol theSymbol = (MemberMethodSymbol)aSymbol;
                return theSymbol.getRetType().getName();
            }
            else if (aSymbol.getST() == Symbol_Type.InitMethodSymbol)
            {
                return "void";
            }
            else
            {
                throw new MethodException("undefined method name " + name, l);
            }
        }

        public bool containsKey(string k)
        {
            return symbolTable.ContainsKey(k);
        }

        public int getScopeLevel()
        {
            return scope_level;
        }

        public void mergeTo(SymbolTable st)
        {
            foreach (var v in symbolTable)
            {
                st.define(v.Value);
            }
        }

        public void debugSymbolTable(string name)
        {
            Debug.Log("-------start-------symbolTable_level:" + scope_level + "   symbolTable_Name:" + name + "-------start-------");
            foreach (var v in symbolTable)
            {
                if (v.Value.getST() == Symbol_Type.TypeSymbol)
                {
                    TypeSymbol temp = (TypeSymbol)v.Value;
                    Debug.Log("              TypeSymbol{" + v.Key + ", " + temp.getName() + "}");
                }
                else if (v.Value.getST() == Symbol_Type.VarSymbol)
                {
                    VarSymbol temp = (VarSymbol)v.Value;
                    Debug.Log("              VarSymbol{" + v.Key + ", " + temp.getTypeSymbol().getName() + "}");
                }
                else if (v.Value.getST() == Symbol_Type.MemberVarSymbol)
                {
                    MemberVarSymbol temp = (MemberVarSymbol)v.Value;
                    Debug.Log("              MemberVarSymbol{" + v.Key + ", " + temp.getTypeSymbol().getName() + ", " + temp.getACM() + "}");
                }
                else if (v.Value.getST() == Symbol_Type.MemberMethodSymbol)
                {
                    MemberMethodSymbol temp = (MemberMethodSymbol)v.Value;
                    Debug.Log("              -------start-------MemberMethodSymbol{" + v.Key + ", " + temp.getRetType().getName() + ", " + temp.getACM() + "}-------start-------");
                    foreach (string p in temp.getParams())
                    {
                        Debug.Log("              MethodParam:" + p);
                    }
                    Debug.Log("              --------end--------MemberMethodSymbol{" + v.Key + ", " + temp.getRetType().getName() + ", " + temp.getACM() + "}--------end--------");
                }
                else if (v.Value.getST() == Symbol_Type.InitMethodSymbol)
                {
                    InitMethodSymbol temp = (InitMethodSymbol)v.Value;
                    Debug.Log("              -------start-------InitMethodSymbol{" + v.Key + ", " + temp.getACM() + "}-------start-------");
                    foreach (string p in temp.getParams())
                    {
                        Debug.Log("              MethodParam:" + p);
                    }
                    Debug.Log("              --------end--------InitMethodSymbol{" + v.Key + ", " + temp.getACM() + "}--------end--------");
                }
            }
            Debug.Log("--------end--------symbolTable_level:" + scope_level + "   symbolTable_Name:" + name + "--------end--------");
        }
    }


    public class SemanticAnlysis
    {
        //private Parser parser;

        private AST the_node;

        private string class_MonoBehaviour = null;

        private Dictionary<string, SymbolTable> symbolTables = new Dictionary<string, SymbolTable>();

        private int ifBlockScopeIndexTemp = 1;

        private int elseBlockScopeIndexTemp = 1;

        private int elseIfBlockScopeIndexTemp = 1;

        private int whileBlockScopeIndexTemp = 1;

        private int forBlockScopeIndexTemp = 1;

        private Dictionary<string, List<string>> methodSymbolTablesForMerge = new Dictionary<string, List<string>>();

        private Dictionary<string, List<MemberMethodDec>> member_method_node_dict = new Dictionary<string, List<MemberMethodDec>>();

        private Dictionary<string, List<InitMethodDec>> init_method_node_dict = new Dictionary<string, List<InitMethodDec>>();

        private Dictionary<string, List<MemberVariableDec>> member_variable_node_dict = new Dictionary<string, List<MemberVariableDec>>();

        private Dictionary<string, List<If_Block>> scope_ifBlock_dict = new Dictionary<string, List<If_Block>>();

        private Dictionary<string, List<While_Block>> scope_whileBlock_dict = new Dictionary<string, List<While_Block>>();

        private Dictionary<string, List<For_Block>> scope_forBlock_dict = new Dictionary<string, List<For_Block>>();

        public SemanticAnlysis(AST node)
        {
            the_node = node;
            SymbolTable globalSymbolTable = new SymbolTable(1);
            globalSymbolTable.init_buildIn();
            symbolTables.Add("global", globalSymbolTable);


            ////////////////////////////////////////
            //SymbolTable symbolTable_forSqrt = new SymbolTable(2);
            //globalSymbolTable.mergeTo(symbolTable_forSqrt);
            //symbolTables.Add("Sqrt", symbolTable_forSqrt);
        }

        private void updateSymbolTables(string n, Symbol s)
        {
            symbolTables[n].define(s);
        }

        private void AST_error(AST node)
        {
            throw new ASTException("unexpected AST node " + node.getAST_Type(), node.getLine());
        }

        private void track_programe(Compound root, string scope)
        {
            foreach (AST node in root.getList())
            {
                if (node.getAST_Type() != AST_Type.CLASS_DEC)
                {
                    throw new ClassException("Namespace cannot directly define members such as variable and methods", node.getLine());
                }
                ClassDec class_node = (ClassDec)node;
                VarMethodOrClass father = class_node.getFather();
                if (father != null)
                {
                    if (father.getName() == "MonoBehaviour")
                    {
                        if (class_MonoBehaviour == null)
                        {
                            class_MonoBehaviour = class_node.getClassName().getName();
                            track_class_MonoBehavior_dec((ClassDec)node, scope);
                            continue;
                        }
                        else
                        {
                            throw new ASTException("Only accept one MonoBehaviour class", node.getLine());
                        }
                    }
                }
                track_class_dec((ClassDec)node, scope);
            }
            foreach (var v in methodSymbolTablesForMerge)
            {
                symbolTables[scope].mergeTo(symbolTables[v.Key]);
            }
            foreach (var v in member_variable_node_dict)
            {
                foreach(MemberVariableDec mvd in v.Value)
                {
                    track_member_var_dec(mvd, v.Key);
                }
            }
            foreach (var v in member_method_node_dict)
            {
                foreach (MemberMethodDec mmd in v.Value)
                {
                    track_member_method_dec(mmd, v.Key);
                }
            }
            foreach (var v in init_method_node_dict)
            {
                foreach (InitMethodDec imd in v.Value)
                {
                    track_init_method_dec(imd, v.Key);
                }
            }
            foreach (var v in methodSymbolTablesForMerge)
            {
                symbolTables[scope].mergeTo(symbolTables[v.Key]);
                foreach (string m in methodSymbolTablesForMerge[v.Key])
                {
                    symbolTables[v.Key].mergeTo(symbolTables[m]);
                }
            }
            foreach (var v in member_method_node_dict)
            {
                foreach (MemberMethodDec mmd in v.Value)
                {
                    string method_name = v.Key + "*" + mmd.getName();
                    List<Param> ps = mmd.getParams();
                    foreach (Param p in ps)
                    {
                        track_parameters(p, method_name);
                    }
                    track_in_method(mmd.getBlock(), method_name, method_name);
                }
            }
            foreach (var v in init_method_node_dict)
            {
                foreach (InitMethodDec imd in v.Value)
                {
                    string method_name = v.Key + "*init";
                    List<Param> ps = imd.getParams();
                    foreach (Param p in ps)
                    {
                        track_parameters(p, method_name);
                    }
                    track_in_method(imd.getBlock(), method_name, method_name);
                }
            }
        }

        private void track_class_dec(ClassDec node, string scope)
        {
            SymbolTable st = symbolTables[scope];
            string class_name = node.getClassName().getName();
            if (st.containsKey(class_name))
            {
                throw new ClassException("duplicate class name" + class_name, node.getLine());
            }
            SymbolTable class_symbolTable = new SymbolTable(st.getScopeLevel() + 1);
            symbolTables.Add(class_name, class_symbolTable);
            methodSymbolTablesForMerge.Add(class_name, new List<string>());
            List<MemberVariableDec> node_list = new List<MemberVariableDec>();
            member_variable_node_dict.Add(class_name, node_list);
            member_method_node_dict.Add(class_name, new List<MemberMethodDec>());
            init_method_node_dict.Add(class_name, new List<InitMethodDec>());
            foreach (MemberVariableDec mvd in node.getMemberVariables())
            {
                member_variable_node_dict[class_name].Add(mvd);
            }
            foreach(MemberMethodDec mmd in node.getMemberMethods())
            {
                member_method_node_dict[class_name].Add(mmd);
            }
            foreach (InitMethodDec imd in node.getInitMethods())
            {
                init_method_node_dict[class_name].Add(imd);
            }
            if (symbolTables[class_name].containsKey(class_name))
            {
                throw new ClassException("class name conflict " + class_name, node.getLine());
            }
            TypeSymbol classTypeSymbol = new TypeSymbol(class_name);
            updateSymbolTables(scope, classTypeSymbol);
            updateSymbolTables(class_name, classTypeSymbol);
        }

        private void track_class_MonoBehavior_dec(ClassDec node, string scope)
        {
            SymbolTable st = symbolTables[scope];
            string class_name = node.getClassName().getName();
            if (st.containsKey(class_name))
            {
                throw new ClassException("duplicate class name" + class_name, node.getLine());
            }
            SymbolTable class_symbolTable = new SymbolTable(st.getScopeLevel() + 1);
            symbolTables.Add(class_name, class_symbolTable);
            methodSymbolTablesForMerge.Add(class_name, new List<string>());
            List<MemberVariableDec> node_list = new List<MemberVariableDec>();
            member_variable_node_dict.Add(class_name, node_list);
            member_method_node_dict.Add(class_name, new List<MemberMethodDec>());
            if (node.getInitMethods().Count != 0)
            {
                throw new ClassException("unrequired constructor for subclass of MonoBehavior", node.getLine());
            }
            foreach (MemberVariableDec mvd in node.getMemberVariables())
            {
                member_variable_node_dict[class_name].Add(mvd);
            }
            foreach (MemberMethodDec mmd in node.getMemberMethods())
            {
                member_method_node_dict[class_name].Add(mmd);
            }
            if (symbolTables[class_name].containsKey(class_name))
            {
                throw new ClassException("class name conflict " + class_name, node.getLine());
            }
            TypeSymbol classTypeSymbol = new TypeSymbol(class_name);
            updateSymbolTables(scope, classTypeSymbol);
            updateSymbolTables(class_name, classTypeSymbol);
        }

        private void track_member_var_dec(MemberVariableDec node, string scope)
        {
            string type_name = node.getType().getName();
            TypeSymbol type_symbol = symbolTables[scope].getTypeSymbol(type_name, node.getLine());
            VarMethodOrClass var = node.getVar();
            string var_name = var.getName();
            if (symbolTables[scope].containsKey(var_name))
            {
                throw new VarException("duplicate variable name " + var_name, node.getLine());
            }
            MemberVarSymbol member_var_symbol = new MemberVarSymbol(var_name, type_symbol, node.getACM());
            updateSymbolTables(scope, member_var_symbol);
        }

        private void track_in_method(AST node, string scope , string methodName)
        {
            if (node.getAST_Type() == AST_Type.COMPOUND)
            {
                track_compound_in_method((Compound)node, scope, methodName);
            }
            else if (node.getAST_Type() == AST_Type.VARIABLE_DEC)
            {
                track_dec((VariableDec)node, scope, methodName);
            }
            else if (node.getAST_Type() == AST_Type.IF_BLOCK)
            {
                track_ifBlock((If_Block)node, scope, methodName);
            }
            else if (node.getAST_Type() == AST_Type.WHILE_BLOCK)
            {
                track_whileBlock((While_Block)node, scope, methodName);
            }
            else if (node.getAST_Type() == AST_Type.FOR_BLOCK)
            {
                track_forBlock((For_Block)node, scope, methodName);
            }
            else if (node.getAST_Type() == AST_Type.EMPTY)
            {
                track_empty();
            }
            else if (node.getAST_Type() == AST_Type.RET)
            {
                track_ret((RET)node, methodName);
            }
            else if (node.getAST_Type() == AST_Type.BREAK)
            {
                track_break((BREAK)node, scope);
            }
            else if (node.getAST_Type() == AST_Type.CONTINUE)
            {
                track_continue((CONTINUE)node, scope);
            }
        }

        private void track_ret(RET node, string methodName)
        {
            node.setMethodName(methodName);
        }

        private void track_break(BREAK node, string scope)
        {
            node.setScope(scope);
        }

        private void track_continue(CONTINUE node, string scope)
        {
            node.setScope(scope);
        }

        private void track_compound_in_method(Compound root, string scope, string methodName)
        {
            foreach (AST node in root.getList())
            {
                track_in_method(node, scope, methodName);
            }
            if (scope_ifBlock_dict.ContainsKey(scope))
            {
                foreach (If_Block node in scope_ifBlock_dict[scope])
                {
                    symbolTables[scope].mergeTo(symbolTables[node.getIfScope()]);
                    track_compound_in_method(node.getBlock(), node.getIfScope(), methodName);
                    if (node.getElseScope() != null)
                    {
                        symbolTables[scope].mergeTo(symbolTables[node.getElseScope()]);
                        track_compound_in_method(node.getElseBlock(), node.getElseScope(), methodName);
                    }
                    foreach (ElSE_IF_BLOCK elseIf in node.getElseIfBlocks())
                    {
                        symbolTables[scope].mergeTo(symbolTables[elseIf.getScope()]);
                        track_compound_in_method(elseIf.getBlock(), elseIf.getScope(), methodName);
                    }
                }
            }
            if (scope_whileBlock_dict.ContainsKey(scope))
            {
                foreach (While_Block node in scope_whileBlock_dict[scope])
                {
                    symbolTables[scope].mergeTo(symbolTables[node.getWhileScope()]);
                    track_compound_in_method(node.getBlock(), node.getWhileScope(), methodName);
                }       
            }
            if (scope_forBlock_dict.ContainsKey(scope))
            {
                foreach (For_Block node in scope_forBlock_dict[scope])
                {
                    symbolTables[scope].mergeTo(symbolTables[node.getForScope()]);
                    track_in_method(node.getStatement1(), node.getForScope(), methodName);
                    track_compound_in_method(node.getBlock(), node.getForScope(), methodName);
                }
            }
        }

        private void track_empty()
        {

        }

        private void track_dec(VariableDec node, string scope, string methodName)
        {
            string type_name = node.getType().getName();
            TypeSymbol type_symbol = symbolTables[scope].getTypeSymbol(type_name, node.getLine());
            VarMethodOrClass var = node.getVar();
            string var_name = var.getName();
            if (symbolTables[scope].containsKey(var_name))
            {
                throw new VarException("duplicate variable name " + var_name, node.getLine());
            }
            VarSymbol var_symbol = new VarSymbol(var_name, type_symbol);
            symbolTables[methodName].decOwnVar(var_symbol);
            updateSymbolTables(scope, var_symbol);
        }

        private void track_init_method_dec(InitMethodDec node, string scope)
        {
            SymbolTable st = symbolTables[scope];
            string method_name = scope + "*init";
            if (st.containsKey(method_name + "("))
            {
                throw new MethodException("duplicate method name " + method_name, node.getLine());
            }
            List<Param> parameters = node.getParams();
            List<string> param_temp = new List<string>();
            SymbolTable symbolTable_forMethod = new SymbolTable(st.getScopeLevel() + 1);
            methodSymbolTablesForMerge[scope].Add(method_name);
            symbolTables.Add(method_name, symbolTable_forMethod);
            foreach (Param p in parameters)
            {
                //track_parameters(p, method_name);
                param_temp.Add(p.getVar().getName());
            }
            InitMethodSymbol initMethodSymbol = new InitMethodSymbol(method_name, param_temp, node.getBlock(), node.getACM());
            if (symbolTables[method_name].containsKey(method_name + "("))
            {
                throw new MethodException("method name conflict " + method_name, node.getLine());
            }
            updateSymbolTables(scope, initMethodSymbol);
            updateSymbolTables(method_name, initMethodSymbol);
        }

        private void track_member_method_dec(MemberMethodDec node, string scope)
        {
            SymbolTable st = symbolTables[scope];
            string method_name = scope + "*" + node.getName();
            if (st.containsKey(method_name + "("))
            {
                throw new MethodException("duplicate method name " + method_name, node.getLine());
            }
            TypeSymbol ret_typeSymbol = st.getTypeSymbol(node.getReturnType().getName(), node.getLine());
            List<Param> parameters = node.getParams();
            List<string> param_temp = new List<string>();
            SymbolTable symbolTable_forMethod = new SymbolTable(st.getScopeLevel() + 1);
            methodSymbolTablesForMerge[scope].Add(method_name);
            symbolTables.Add(method_name, symbolTable_forMethod);
            foreach (Param p in parameters)
            {
                //track_parameters(p, method_name);
                param_temp.Add(p.getVar().getName());
            }
            MemberMethodSymbol memberMethodSymbol = new MemberMethodSymbol(method_name, ret_typeSymbol, param_temp, node.getBlock(), node.getACM());
            if (symbolTables[method_name].containsKey(method_name + "("))
            {
                throw new MethodException("method name conflict "+ method_name, node.getLine());
            }
            updateSymbolTables(scope, memberMethodSymbol);
            updateSymbolTables(method_name, memberMethodSymbol);
        }

        private void track_parameters(Param node, string scope)
        {
            string type_name = node.getTYPE().getName();
            TypeSymbol type_symbol = symbolTables[scope].getTypeSymbol(type_name, node.getLine());
            VarMethodOrClass var = node.getVar();
            string param_name = var.getName();
            if (symbolTables[scope].containsKey(param_name))
            {
                throw new VarException("parameter name conflict '" + param_name + "'", node.getLine());
            }
            ParamSymbol param_symbol = new ParamSymbol(param_name, type_symbol);
            updateSymbolTables(scope, param_symbol);
        }

        private void track_forBlock(For_Block node, string scope, string methodName)
        {
            if (scope_forBlock_dict.ContainsKey(scope))
            {
                scope_forBlock_dict[scope].Add(node);
            }
            else
            {
                List<For_Block> forList = new List<For_Block>();
                forList.Add(node);
                scope_forBlock_dict[scope] = forList;
            }
            SymbolTable forBlockSymbolTable = new SymbolTable(symbolTables[scope].getScopeLevel() + 1);
            string forScopeName = scope + "*FOR*" + forBlockScopeIndexTemp;
            forBlockScopeIndexTemp++;
            node.setForScope(forScopeName);
            symbolTables.Add(forScopeName, forBlockSymbolTable);
        }

        private void track_whileBlock(While_Block node, string scope, string methodName)
        {
            if (scope_whileBlock_dict.ContainsKey(scope))
            {
                scope_whileBlock_dict[scope].Add(node);
            }
            else
            {
                List<While_Block> whileList = new List<While_Block>();
                whileList.Add(node);
                scope_whileBlock_dict[scope] = whileList;
            }
            SymbolTable whileBlockSymbolTable = new SymbolTable(symbolTables[scope].getScopeLevel() + 1);
            string whileScopeName = scope + "*WHILE*" + whileBlockScopeIndexTemp;
            whileBlockScopeIndexTemp++;
            node.setWhileScope(whileScopeName);
            symbolTables.Add(whileScopeName, whileBlockSymbolTable);
        }

        private void track_ifBlock(If_Block node, string scope, string methodName)
        {
            if (scope_ifBlock_dict.ContainsKey(scope))
            {
                scope_ifBlock_dict[scope].Add(node);
            }
            else
            {
                List<If_Block> ifList = new List<If_Block>();
                ifList.Add(node);
                scope_ifBlock_dict[scope] = ifList;
            }
            SymbolTable ifBlockSymbolTable = new SymbolTable(symbolTables[scope].getScopeLevel() + 1);
            string ifScopeName = scope + "*IF*" + ifBlockScopeIndexTemp;
            ifBlockScopeIndexTemp++;
            node.setIfScope(ifScopeName);
            symbolTables.Add(ifScopeName, ifBlockSymbolTable);
            Compound elseBlock = node.getElseBlock();
            if (elseBlock != null)
            {
                string elseScopeName = scope + "*ELSE*" + elseBlockScopeIndexTemp;
                elseBlockScopeIndexTemp++;
                node.setElseScope(elseScopeName);
                symbolTables.Add(elseScopeName, new SymbolTable(symbolTables[scope].getScopeLevel() + 1));
            }
            List<ElSE_IF_BLOCK> elseIfs = node.getElseIfBlocks();
            foreach (ElSE_IF_BLOCK elseIf in elseIfs)
            {
                string elseIfScopeName = scope + "*ELSEIF*" + elseIfBlockScopeIndexTemp;
                elseIfBlockScopeIndexTemp++;
                elseIf.setScope(elseIfScopeName);
                symbolTables.Add(elseIfScopeName, new SymbolTable(symbolTables[scope].getScopeLevel() + 1));
            }
        }

        public void lets_anlysis()
        {
            //track_programe((Compound)parser.getAST(), "global");
            track_programe((Compound)the_node, "global");
            //foreach (var v in symbolTables)
            //{
            //    v.Value.debugSymbolTable(v.Key);
            //}
        }

        public Dictionary<string, SymbolTable> getSymbolTables()
        {
            return symbolTables;
        }

        public string getClassMonoBehaviour()
        {
            return class_MonoBehaviour;
        }

        public Dictionary<string, List<MemberVariableDec>> getMemberVar_decNode()
        {
            return member_variable_node_dict;
        }
    }
}

