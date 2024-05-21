using Interpreter_Basic;
using Interpreter_Parser;
using Interpreter_Semantic_Analyzer;
using Interpreter_Lexer;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Interpreter_Execute
{
    public class Execute
    {
        string theCodeName;
        private Stack<Value> retValueStack = new Stack<Value>();

        private Dictionary<string, SymbolTable> symbolTables;

        private Dictionary<string, List<MemberVariableDec>> memberVarDic;

        private List<int> intValueList = new List<int>();

        private List<double> doubleValueList = new List<double>();

        private List<float> floatValueList = new List<float>();

        private List<bool> boolValueList = new List<bool>();

        private List<long> longValueList = new List<long>();

        private List<decimal> decimalValueList = new List<decimal>();

        private Dictionary<int, bool> intValueIfFree = new Dictionary<int, bool>();

        private Dictionary<int, bool> doubleValueIfFree = new Dictionary<int, bool>();

        private Dictionary<int, bool> floatValueIfFree = new Dictionary<int, bool>();

        private Dictionary<int, bool> boolValueIfFree = new Dictionary<int, bool>();

        private Dictionary<int, bool> longValueIfFree = new Dictionary<int, bool>();

        private Dictionary<int, bool> decimalValueIfFree = new Dictionary<int, bool>();

        private Dictionary<string, bool> scope_break_flag = new Dictionary<string, bool>();

        private Dictionary<string, bool> scope_continue_flag = new Dictionary<string, bool>();

        private AST Excute_start_AST;

        private AST Excute_update_AST;

        private bool ifHaveReturn = false;

        private string suffix_for_excute_instance = "*excute";

        private string global = "global";

        private GameObjectInstanceValue theGameObject;

        private TransformInstanceValue theTransform;

        private Dictionary<string, GameObjectInstanceValue> theGameObjects = new Dictionary<string, GameObjectInstanceValue>();

        private ClassInstanceValue monoEnv;

        public Execute(AST node, HaHaObject g, string name, Dictionary<string, HaHaObject> dict)
        {
            
            if (g != null)
            {
                theGameObject = new GameObjectInstanceValue(g.gameObject);
                theTransform = new TransformInstanceValue(theGameObject.getGameObject().transform);
            }
            foreach (var v in dict)
            {
                theGameObjects.Add(v.Key, new GameObjectInstanceValue(v.Value.gameObject));
            }
            SemanticAnlysis s = new SemanticAnlysis(node);
            s.lets_anlysis();
            theCodeName = s.getClassMonoBehaviour();
            memberVarDic = s.getMemberVar_decNode();
            symbolTables = s.getSymbolTables();
            Debug.Log(theCodeName);
            if (theCodeName != name)
            {
                throw new ExceptionWithOutLine("File name " + name + " does not match the name of MonoBehaviour class");
            }
        }

        private void operator_error(string op, string t1, string t2, int l)
        {
            throw new OperatorException("operator " + op + " can't be used between " + t1 + " and " + t2, l);
        }

        private int getFreeIndex(string t, int l)
        {
            if (t == "int")
            {
                if (intValueList.Count == 0)
                {
                    intValueList.Add(0);
                    intValueIfFree.Add(0, true);
                    return 0;
                }
                int temp = 0;
                foreach (var v in intValueIfFree)
                {
                    temp = v.Key;
                    if (v.Value == true)
                    {
                        return v.Key;
                    }
                }
                temp++;
                intValueList.Add(0);
                intValueIfFree.Add(temp, true);
                return temp;
            }
            else if (t == "float")
            {
                if (floatValueList.Count == 0)
                {
                    floatValueList.Add(0);
                    floatValueIfFree.Add(0, true);
                    return 0;
                }
                int temp = 0;
                foreach (var v in floatValueIfFree)
                {
                    temp = v.Key;
                    if (v.Value == true)
                    {
                        return v.Key;
                    }
                }
                temp++;
                floatValueList.Add(0);
                floatValueIfFree.Add(temp, true);
                return temp;
            }
            else if (t == "double")
            {
                if (doubleValueList.Count == 0)
                {
                    doubleValueList.Add(0);
                    doubleValueIfFree.Add(0, true);
                    return 0;
                }
                int temp = 0;
                foreach (var v in doubleValueIfFree)
                {
                    temp = v.Key;
                    if (v.Value == true)
                    {
                        return v.Key;
                    }
                }
                temp++;
                doubleValueList.Add(0);
                doubleValueIfFree.Add(temp, true);
                return temp;
            }
            else if (t == "bool")
            {
                if (boolValueList.Count == 0)
                {
                    boolValueList.Add(false);
                    boolValueIfFree.Add(0, true);
                    return 0;
                }
                int temp = 0;
                foreach (var v in boolValueIfFree)
                {
                    temp = v.Key;
                    if (v.Value == true)
                    {
                        return v.Key;
                    }
                }
                temp++;
                boolValueList.Add(false);
                boolValueIfFree.Add(temp, true);
                return temp;
            }
            else if (t == "decimal")
            {
                if (decimalValueList.Count == 0)
                {
                    decimalValueList.Add(0);
                    decimalValueIfFree.Add(0, true);
                    return 0;
                }
                int temp = 0;
                foreach (var v in decimalValueIfFree)
                {
                    temp = v.Key;
                    if (v.Value == true)
                    {
                        return v.Key;
                    }
                }
                temp++;
                decimalValueList.Add(0);
                decimalValueIfFree.Add(temp, true);
                return temp;
            }
            else if (t == "long")
            {
                if (longValueList.Count == 0)
                {
                    longValueList.Add(0);
                    longValueIfFree.Add(0, true);
                    return 0;
                }
                int temp = 0;
                foreach (var v in longValueIfFree)
                {
                    temp = v.Key;
                    if (v.Value == true)
                    {
                        return v.Key;
                    }
                }
                temp++;
                longValueList.Add(0);
                longValueIfFree.Add(temp, true);
                return temp;
            }
            else
            {
                return -1;
            }
        }

        private void setIndexOccupied(Value v, int l)
        {
            if (v == null)
            {
                return;
            }
            int index = v.getIndex();
            string t = v.getType();
            if (t == "int")
            {
                if (intValueIfFree.ContainsKey(index))
                {
                    intValueIfFree[index] = false;
                }
                else
                {
                    throw new TypeException("unexpected index " + index, l);
                }
            }
            else if (t == "float")
            {
                if (floatValueIfFree.ContainsKey(index))
                {
                    floatValueIfFree[index] = false;
                }
                else
                {
                    throw new TypeException("unexpected index " + index, l);
                }
            }
            else if (t == "double")
            {
                if (doubleValueIfFree.ContainsKey(index))
                {
                    doubleValueIfFree[index] = false;
                }
                else
                {
                    throw new TypeException("unexpected index " + index, l);
                }
            }
            else if (t == "bool")
            {
                if (boolValueIfFree.ContainsKey(index))
                {
                    boolValueIfFree[index] = false;
                }
                else
                {
                    throw new TypeException("unexpected index " + index, l);
                }
            }
            else if (t == "long")
            {
                if (longValueIfFree.ContainsKey(index))
                {
                    longValueIfFree[index] = false;
                }
                else
                {
                    throw new TypeException("unexpected index " + index, l);
                }
            }
            else if (t == "decimal")
            {
                if (decimalValueIfFree.ContainsKey(index))
                {
                    decimalValueIfFree[index] = false;
                }
                else
                {
                    throw new TypeException("unexpected index " + index, l);
                }
            }
            else
            {
                throw new TypeException("unexpected type " + t, l);
            }
        }

        private void setIndexFree(Value v, int l)
        {
            if (v == null)
            {
                return;
            }
            if (!v.getIfCanFree() || v.getIfClassType())
            {
                return;
            }
            int index = v.getIndex();
            string t = v.getType();
            if (t == "int")
            {
                if (intValueIfFree.ContainsKey(index))
                {
                    intValueIfFree[index] = true;
                }
                else
                {
                    throw new TypeException("unexpected index " + index, l);
                }
            }
            else if (t == "float")
            {
                if (floatValueIfFree.ContainsKey(index))
                {
                    floatValueIfFree[index] = true;
                }
                else
                {
                    throw new TypeException("unexpected index " + index, l);
                }
            }
            else if (t == "double")
            {
                if (doubleValueIfFree.ContainsKey(index))
                {
                    doubleValueIfFree[index] = true;
                }
                else
                {
                    throw new TypeException("unexpected index " + index, l);
                }
            }
            else if (t == "bool")
            {
                if (boolValueIfFree.ContainsKey(index))
                {
                    boolValueIfFree[index] = true;
                }
                else
                {
                    throw new TypeException("unexpected index " + index, l);
                }
            }
            else if (t == "decimal")
            {
                if (decimalValueIfFree.ContainsKey(index))
                {
                    decimalValueIfFree[index] = true;
                }
                else
                {
                    throw new TypeException("unexpected index " + index, l);
                }
            }
            else if (t == "long")
            {
                if (longValueIfFree.ContainsKey(index))
                {
                    longValueIfFree[index] = true;
                }
                else
                {
                    throw new TypeException("unexpected index " + index, l);
                }
            }
            else
            {
                throw new TypeException("unexpected type " + t, l);
            }
        }

        private void AST_error(AST node)
        {
            throw new ASTException("unexpected AST node " + node.getAST_Type(), node.getLine());
        }

        private void Type_error(Basic_Type t1, int line)
        {
            throw new TypeException("unexpected data of " + t1 + " type", line);
        }

        private void Type_op_error(Basic_Type t, string op, int line)
        {
            throw new TypeException("can't use " + op + " with " + t, line);
        }

        private void track_in_method(AST node, string scope, ClassInstanceValue environment, string CirScope)
        {
            //if (ifHaveReturn)
            //{
            //    return;
            //}
            if (node.getAST_Type() == AST_Type.COMPOUND)
            {
                track_compound((Compound)node, scope, environment, CirScope);
            }
            else if (node.getAST_Type() == AST_Type.ASSIGN)
            {
                track_assign((Assign)node, scope, environment, false);
            }
            else if (node.getAST_Type() == AST_Type.VARIABLE_DEC)
            {
                track_dec((VariableDec)node, scope, environment);
            }
            else if (node.getAST_Type() == AST_Type.EMPTY || node.getAST_Type() == AST_Type.MEMBER_METHOD_DEC || node.getAST_Type() == AST_Type.CLASS_DEC)
            {
                track_empty();
            }
            else if (node.getAST_Type() == AST_Type.RET)
            {
                track_ret((RET)node, scope, environment);
            }
            else if (node.getAST_Type() == AST_Type.CONTINUE)
            {
                if (CirScope != null)
                {
                    scope_continue_flag[CirScope] = true;
                }
                else
                {
                    AST_error(node);
                }
            }
            else if (node.getAST_Type() == AST_Type.BREAK)
            {
                if (CirScope != null)
                {
                    scope_break_flag[CirScope] = true;
                }
                else
                {
                    AST_error(node);
                }
            }
            else if (node.getAST_Type() == AST_Type.IF_BLOCK)
            {
                track_ifBlock((If_Block)node, scope, environment, CirScope);
            }
            else if (node.getAST_Type() == AST_Type.WHILE_BLOCK)
            {
                track_whileBlock((While_Block)node, scope, environment);
            }
            else if (node.getAST_Type() == AST_Type.FOR_BLOCK)
            {
                track_forBlock((For_Block)node, scope, environment);
            }
            else if (node.getAST_Type() == AST_Type.METHOD_CALL)
            {
                track_methodCall((MethodCall)node, scope, environment, environment, false);
            }
            else if (node.getAST_Type() == AST_Type.CLASS_CALL)
            {
                ClassCall temp = (ClassCall)node;
                if (temp.IfMethod())
                {
                    Value a;
                    string b;
                    string c;
                    track_classCall((ClassCall)node, scope, environment, environment, out a, out b, out c, scope, false);
                }
                else
                {
                    throw new ExcuteException("Only assignment and method can be used as statements", node.getLine());
                }

            }
            else if (node.getAST_Type() == AST_Type.UNARYOP)
            {
                track_UopWithAssign((UnaryOP)node, scope, environment);
            }
            else
            {
                AST_error(node);
            }
        }

        private Value track_call_staticTime(ClassCall node, string scope, ClassInstanceValue environment)
        {
            AST leftNode = node.getLeft();
            if (leftNode.getAST_Type() == AST_Type.VARMETHODORCLASS)
            {
                VarMethodOrClass leftVar = (VarMethodOrClass)leftNode;
                string name = leftVar.getName();
                if (name == "deltaTime")
                {
                    ClassCall rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Time.deltaTime;
                    if (rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(rightNode);
                        return null;
                    }
                }
                else if (name == "fixedDeltaTime")
                {
                    ClassCall rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Time.fixedDeltaTime;
                    if (rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(rightNode);
                        return null;
                    }
                }
                else if (name == "fixedTime")
                {
                    ClassCall rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Time.fixedTime;
                    if (rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(rightNode);
                        return null;
                    }
                }
                else if (name == "frameCount")
                {
                    ClassCall rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Time.frameCount;
                    if (rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(rightNode);
                        return null;
                    }
                }
                else if (name == "time")
                {
                    ClassCall rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Time.time;
                    if (rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(rightNode);
                        return null;
                    }
                }
                else
                {
                    throw new VarException("unsupported member " + name, node.getLine());
                }
            }
            else
            {
                AST_error(node);
                return null;
            }
        }

        private Value track_call_staticMathf(ClassCall node, string scope, ClassInstanceValue environment)
        {
            AST leftNode = node.getLeft();
            if (leftNode.getAST_Type() == AST_Type.VARMETHODORCLASS)
            {
                VarMethodOrClass leftVar = (VarMethodOrClass)leftNode;
                string name = leftVar.getName();
                if (name == "Deg2Rad")
                {
                    ClassCall rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Deg2Rad;
                    if (rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(rightNode);
                        return null;
                    }
                }
                else if (name == "Rad2Deg")
                {
                    ClassCall rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Rad2Deg;
                    if (rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(rightNode);
                        return null;
                    }
                }
                else if (name == "Epsilon")
                {
                    ClassCall rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Epsilon;
                    if (rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(rightNode);
                        return null;
                    }
                }
                else if (name == "PI")
                {
                    ClassCall rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.PI;
                    if (rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(rightNode);
                        return null;
                    }
                }
                else
                {
                    throw new VarException("unsupported member " + name, node.getLine());
                }
            }
            else if (leftNode.getAST_Type() == AST_Type.METHOD_CALL)
            {
                ClassCall p_rightNode = node.getRight();
                MethodCall theMethod = (MethodCall)leftNode;
                string name = theMethod.getMethodName();
                if (name == "Abs")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Abs(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Acos")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Acos(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Asin")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Asin(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Atan")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Atan(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Ceil")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Ceil(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "CeilToInt")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value iv = init_a_new_value(out index, "int", node.getLine());
                    intValueList[index] = Mathf.CeilToInt(pfv);
                    if (p_rightNode == null)
                    {
                        return iv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Approximately")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    int index;
                    Value bv = init_a_new_value(out index, "bool", node.getLine());
                    boolValueList[index] = Mathf.Approximately(pfv0, pfv1);
                    if (p_rightNode == null)
                    {
                        return bv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Atan2")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Atan2(pfv0, pfv1);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Clamp")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    setIndexFree(v2, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Clamp(pfv0, pfv1, pfv2);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Clamp01")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Clamp01(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "ClosestPowerOfTwo")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    int pfv;
                    if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "int", node.getLine());
                    intValueList[index] = Mathf.ClosestPowerOfTwo(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Cos")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Cos(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "DeltaAngle")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.DeltaAngle(pfv0, pfv1);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Exp")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Exp(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Floor")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Floor(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "FloorToInt")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value iv = init_a_new_value(out index, "int", node.getLine());
                    intValueList[index] = Mathf.FloorToInt(pfv);
                    if (p_rightNode == null)
                    {
                        return iv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "InverseLerp")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    setIndexFree(v2, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.InverseLerp(pfv0, pfv1, pfv2);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "IsPowerOfTwo")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    int pfv;
                    if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value bv = init_a_new_value(out index, "bool", node.getLine());
                    boolValueList[index] = Mathf.IsPowerOfTwo(pfv);
                    if (p_rightNode == null)
                    {
                        return bv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Lerp")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    setIndexFree(v2, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Lerp(pfv0, pfv1, pfv2);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "LerpAngle")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    setIndexFree(v2, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.LerpAngle(pfv0, pfv1, pfv2);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "LerpUnclamped")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    setIndexFree(v2, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.LerpUnclamped(pfv0, pfv1, pfv2);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Log")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Log(pfv0, pfv1);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Log10")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Log10(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Max")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Max(pfv0, pfv1);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Min")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Min(pfv0, pfv1);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "MoveTowards")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    setIndexFree(v2, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.MoveTowards(pfv0, pfv1, pfv2);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "MoveTowardsAngle")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    setIndexFree(v2, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.MoveTowardsAngle(pfv0, pfv1, pfv2);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "NextPowerOfTwo")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    int pfv;
                    if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "int", node.getLine());
                    intValueList[index] = Mathf.NextPowerOfTwo(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "PerlinNoise")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.PerlinNoise(pfv0, pfv1);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "PingPong")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.PingPong(pfv0, pfv1);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Pow")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Pow(pfv0, pfv1);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Repeat")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Repeat(pfv0, pfv1);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Round")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Round(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "RoundToInt")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "int", node.getLine());
                    intValueList[index] = Mathf.RoundToInt(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Sign")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Sign(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Sin")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Sin(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                //else if (name == "SmoothDamp")
                //{

                //}
                //else if (name == "SmoothDampAngle")
                //{

                //}
                else if (name == "SmoothStep")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    setIndexFree(v2, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.SmoothStep(pfv0, pfv1, pfv2);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Sqrt")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Sqrt(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if(name == "Tan")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Mathf.Tan(pfv);
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else
                {
                    throw new VarException("unsupported member " + name, node.getLine());
                }
            }
            else
            {
                AST_error(node);
                return null;
            }
        }

        private Value track_call_staticVector3(ClassCall node, string scope, ClassInstanceValue environment)
        {
            AST leftNode = node.getLeft();
            if (leftNode.getAST_Type() == AST_Type.VARMETHODORCLASS)
            {
                VarMethodOrClass leftVar = (VarMethodOrClass)leftNode;
                string name = leftVar.getName();
                if (name == "back")
                {
                    ClassCall rightNode = node.getRight();
                    Vector3IntanceValue v3iv = new Vector3IntanceValue(Vector3.back);
                    if (rightNode == null)
                    {
                        return v3iv;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(rightNode, v3iv, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "down")
                {
                    ClassCall rightNode = node.getRight();
                    Vector3IntanceValue v3iv = new Vector3IntanceValue(Vector3.down);
                    if (rightNode == null)
                    {
                        return v3iv;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(rightNode, v3iv, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "forward")
                {
                    ClassCall rightNode = node.getRight();
                    Vector3IntanceValue v3iv = new Vector3IntanceValue(Vector3.forward);
                    if (rightNode == null)
                    {
                        return v3iv;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(rightNode, v3iv, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "left")
                {
                    ClassCall rightNode = node.getRight();
                    Vector3IntanceValue v3iv = new Vector3IntanceValue(Vector3.left);
                    if (rightNode == null)
                    {
                        return v3iv;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(rightNode, v3iv, scope, environment, out le, out vn, out tn, false);
                    }
                }
                //else if (name == "negativeInfinity")
                //{
                //    ClassCall rightNode = node.getRight();
                //    Vector3IntanceValue v3iv = new Vector3IntanceValue(Vector3.negativeInfinity);
                //    if (rightNode == null)
                //    {
                //        return v3iv;
                //    }
                //    else
                //    {
                //        Value le;
                //        string vn;
                //        string tn;
                //        return track_call_vector3(rightNode, v3iv, scope, environment, out le, out vn, out tn, false);
                //    }
                //}
                else if (name == "one")
                {
                    ClassCall rightNode = node.getRight();
                    Vector3IntanceValue v3iv = new Vector3IntanceValue(Vector3.one);
                    if (rightNode == null)
                    {
                        return v3iv;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(rightNode, v3iv, scope, environment, out le, out vn, out tn, false);
                    }
                }
                //else if (name == "positiveInfinity")
                //{
                //    ClassCall rightNode = node.getRight();
                //    Vector3IntanceValue v3iv = new Vector3IntanceValue(Vector3.positiveInfinity);
                //    if (rightNode == null)
                //    {
                //        return v3iv;
                //    }
                //    else
                //    {
                //        Value le;
                //        string vn;
                //        string tn;
                //        return track_call_vector3(rightNode, v3iv, scope, environment, out le, out vn, out tn, false);
                //    }
                //}
                else if (name == "right")
                {
                    ClassCall rightNode = node.getRight();
                    Vector3IntanceValue v3iv = new Vector3IntanceValue(Vector3.right);
                    if (rightNode == null)
                    {
                        return v3iv;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(rightNode, v3iv, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "up")
                {
                    ClassCall rightNode = node.getRight();
                    Vector3IntanceValue v3iv = new Vector3IntanceValue(Vector3.up);
                    if (rightNode == null)
                    {
                        return v3iv;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(rightNode, v3iv, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "zero")
                {
                    ClassCall rightNode = node.getRight();
                    Vector3IntanceValue v3iv = new Vector3IntanceValue(Vector3.zero);
                    if (rightNode == null)
                    {
                        return v3iv;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(rightNode, v3iv, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else
                {
                    throw new VarException("unsupported member " + name, node.getLine());
                }
            }
            else if (leftNode.getAST_Type() == AST_Type.METHOD_CALL)
            {
                ClassCall p_rightNode = node.getRight();
                MethodCall theMethod = (MethodCall)leftNode;
                string name = theMethod.getMethodName();
                if (name == "Angle")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Vector3.Angle(v0_temp.getVector3(), v1_temp.getVector3());
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "ClampMagnitude")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv;
                    if (v1.getType() == "float")
                    {
                        pfv = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    setIndexFree(v1, node.getLine());
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.ClampMagnitude(v0_temp.getVector3(), pfv));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "Cross")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.Cross(v0_temp.getVector3(), v1_temp.getVector3()));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "Distance")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Vector3.Distance(v0_temp.getVector3(), v1_temp.getVector3());
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Dot")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Vector3.Dot(v0_temp.getVector3(), v1_temp.getVector3());
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Lerp")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv;
                    if (v2.getType() == "float")
                    {
                        pfv = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    setIndexFree(v2, node.getLine());
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.Lerp(v0_temp.getVector3(), v1_temp.getVector3(), pfv));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "LerpUnclamped")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv;
                    if (v2.getType() == "float")
                    {
                        pfv = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    setIndexFree(v2, node.getLine());
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.LerpUnclamped(v0_temp.getVector3(), v1_temp.getVector3(), pfv));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "Max")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.Max(v0_temp.getVector3(), v1_temp.getVector3()));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "Min")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.Min(v0_temp.getVector3(), v1_temp.getVector3()));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "MoveTowards")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv;
                    if (v2.getType() == "float")
                    {
                        pfv = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    setIndexFree(v2, node.getLine());
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.MoveTowards(v0_temp.getVector3(), v1_temp.getVector3(), pfv));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "Normalize")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.Normalize(v0_temp.getVector3()));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                //else if (name == "OrthoNormalize")
                //{

                //}
                else if (name == "Project")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.Project(v0_temp.getVector3(), v1_temp.getVector3()));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "ProjectOnPlane")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.ProjectOnPlane(v0_temp.getVector3(), v1_temp.getVector3()));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "Reflect")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.Reflect(v0_temp.getVector3(), v1_temp.getVector3()));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "RotateTowards")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 4)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    Value v3 = trackForNumberOP(paramGivens[3].getNode(), scope, false, environment);
                    float pfv3;
                    if (v3.getType() == "float")
                    {
                        pfv3 = floatValueList[v3.getIndex()];
                    }
                    else if (v3.getType() == "long")
                    {
                        pfv3 = longValueList[v3.getIndex()];
                    }
                    else if (v3.getType() == "int")
                    {
                        pfv3 = intValueList[v3.getIndex()];
                    }
                    else
                    {
                        pfv3 = 0;
                        throw new MethodException("can't convert " + v3.getType() + " value to float value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    setIndexFree(v2, node.getLine());
                    setIndexFree(v3, node.getLine());
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.RotateTowards(v0_temp.getVector3(), v1_temp.getVector3(), pfv2, pfv3));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "Scale")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.Scale(v0_temp.getVector3(), v1_temp.getVector3()));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "SignedAngle")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    if (v2.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v2.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    Vector3IntanceValue v2_temp = (Vector3IntanceValue)v2;
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Vector3.SignedAngle(v0_temp.getVector3(), v1_temp.getVector3(), v2_temp.getVector3());
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Slerp")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv;
                    if (v2.getType() == "float")
                    {
                        pfv = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    setIndexFree(v2, node.getLine());
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.Slerp(v0_temp.getVector3(), v1_temp.getVector3(), pfv));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "SlerpUnclamped")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv;
                    if (v2.getType() == "float")
                    {
                        pfv = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    setIndexFree(v2, node.getLine());
                    Vector3IntanceValue result = new Vector3IntanceValue(Vector3.SlerpUnclamped(v0_temp.getVector3(), v1_temp.getVector3(), pfv));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_vector3(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                //else if (name == "SmoothDamp")
                //{
                //}
                else
                {
                    throw new VarException("unsupported member " + name, node.getLine());
                }
            }
            else
            {
                AST_error(node);
                return null;
            }
        }

        private Value track_call_vector3(ClassCall node, Vector3IntanceValue v, string scope, ClassInstanceValue environment, out Value last_env, out string var_name, out string type_name, bool ifLeft)
        {
            AST leftNode = node.getLeft();
            if (leftNode.getAST_Type() == AST_Type.VARMETHODORCLASS)
            {
                VarMethodOrClass leftVar = (VarMethodOrClass)leftNode;
                string name = leftVar.getName();
                if (name == "x")
                {
                    ClassCall p_rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = v.getVector3().x;
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = name;
                        type_name = "float";
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "y")
                {
                    ClassCall p_rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = v.getVector3().y;
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = name;
                        type_name = "float";
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "z")
                {
                    ClassCall p_rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = v.getVector3().z;
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = name;
                        type_name = "float";
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "magnitude")
                {
                    ClassCall p_rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = v.getVector3().magnitude;
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = name;
                        type_name = "float";
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "sqrMagnitude")
                {
                    ClassCall p_rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = v.getVector3().sqrMagnitude;
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = name;
                        type_name = "float";
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "normalized")
                {
                    ClassCall p_rightNode = node.getRight();
                    Vector3IntanceValue v3iv = new Vector3IntanceValue(v.getVector3().normalized);
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = name;
                        type_name = "Vector3";
                        return v3iv;
                    }
                    else
                    {
                        return track_call_vector3(p_rightNode, v3iv, scope, environment, out last_env, out var_name, out type_name, ifLeft);
                    }
                }
                else
                {
                    throw new VarException("unsupported member " + name, node.getLine());
                }
            }
            else if(leftNode.getAST_Type() == AST_Type.METHOD_CALL)
            {
                if (ifLeft)
                {
                    throw new AssignException("Method cannot be placed to the left of assignment", node.getLine());
                }
                ClassCall p_rightNode = node.getRight();
                MethodCall theMethod = (MethodCall)leftNode;
                string name = theMethod.getMethodName();
                if (name == "Equals")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value vpg = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (vpg.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + vpg.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v3_vpg = (Vector3IntanceValue)vpg;
                    int index;
                    Value bv = init_a_new_value(out index, "bool", node.getLine());
                    boolValueList[index] = v.getVector3().Equals(v3_vpg.getVector3());
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = null;
                        type_name = "bool";
                        return bv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "Set")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    v.setVector3(new Vector3(pfv0, pfv1, pfv2));
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    setIndexFree(v2, node.getLine());
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else
                {
                    throw new VarException("unsupported member " + name, node.getLine());
                }
            }
            else
            {
                AST_error(node);
                last_env = null;
                var_name = null;
                type_name = null;
                return null;
            }
        }

        private Value track_call_quaternion(ClassCall node, QuaternionIntanceValue v, string scope, ClassInstanceValue environment, out Value last_env, out string var_name, out string type_name, bool ifLeft)
        {
            AST leftNode = node.getLeft();
            if (leftNode.getAST_Type() == AST_Type.VARMETHODORCLASS)
            {
                VarMethodOrClass leftVar = (VarMethodOrClass)leftNode;
                string name = leftVar.getName();
                if (name == "x")
                {
                    ClassCall p_rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = v.getQuaternion().x;
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = name;
                        type_name = "float";
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "y")
                {
                    ClassCall p_rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = v.getQuaternion().y;
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = name;
                        type_name = "float";
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "z")
                {
                    ClassCall p_rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = v.getQuaternion().z;
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = name;
                        type_name = "float";
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "w")
                {
                    ClassCall p_rightNode = node.getRight();
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = v.getQuaternion().w;
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = name;
                        type_name = "float";
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "eulerAngles")
                {
                    ClassCall p_rightNode = node.getRight();
                    Vector3IntanceValue v3iv = new Vector3IntanceValue(v.getQuaternion().eulerAngles);
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = name;
                        type_name = "Vector3";
                        return v3iv;
                    }
                    else
                    {
                        return track_call_vector3(p_rightNode, v3iv, scope, environment, out last_env, out var_name, out type_name, ifLeft);
                    }
                }
                else if (name == "normalized")
                {
                    ClassCall p_rightNode = node.getRight();
                    QuaternionIntanceValue vq = new QuaternionIntanceValue(v.getQuaternion().normalized);
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = name;
                        type_name = "Quaternion";
                        return vq;
                    }
                    else
                    {
                        return track_call_quaternion(p_rightNode, vq, scope, environment, out last_env, out var_name, out type_name, ifLeft);
                    }
                }
                else
                {
                    throw new VarException("unsupported member " + name, node.getLine());
                }
            }
            else if (leftNode.getAST_Type() == AST_Type.METHOD_CALL)
            {
                if (ifLeft)
                {
                    throw new AssignException("Method cannot be placed to the left of assignment", node.getLine());
                }
                ClassCall p_rightNode = node.getRight();
                MethodCall theMethod = (MethodCall)leftNode;
                string name = theMethod.getMethodName();
                if (name == "Set")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 4)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv0;
                    if (v0.getType() == "float")
                    {
                        pfv0 = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv0 = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv0 = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv0 = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    float pfv1;
                    if (v1.getType() == "float")
                    {
                        pfv1 = floatValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "long")
                    {
                        pfv1 = longValueList[v1.getIndex()];
                    }
                    else if (v1.getType() == "int")
                    {
                        pfv1 = intValueList[v1.getIndex()];
                    }
                    else
                    {
                        pfv1 = 0;
                        throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    Value v3 = trackForNumberOP(paramGivens[3].getNode(), scope, false, environment);
                    float pfv3;
                    if (v3.getType() == "float")
                    {
                        pfv3 = floatValueList[v3.getIndex()];
                    }
                    else if (v3.getType() == "long")
                    {
                        pfv3 = longValueList[v3.getIndex()];
                    }
                    else if (v3.getType() == "int")
                    {
                        pfv3 = intValueList[v3.getIndex()];
                    }
                    else
                    {
                        pfv3 = 0;
                        throw new MethodException("can't convert " + v3.getType() + " value to float value", node.getLine());
                    }
                    v.setQuaternion(new Quaternion(pfv0, pfv1, pfv2, pfv3));
                    setIndexFree(v0, node.getLine());
                    setIndexFree(v1, node.getLine());
                    setIndexFree(v2, node.getLine());
                    setIndexFree(v3, node.getLine());
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "SetFromToRotation")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    Quaternion q = v.getQuaternion();
                    q.SetFromToRotation(v0_temp.getVector3(), v1_temp.getVector3());
                    v.setQuaternion(q);
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "SetLookRotation")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count == 2)
                    {
                        Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                        if (v0.getType() != "Vector3")
                        {
                            throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                        }
                        Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                        if (v1.getType() != "Vector3")
                        {
                            throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                        }
                        Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                        Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                        Quaternion q = v.getQuaternion();
                        q.SetLookRotation(v0_temp.getVector3(), v1_temp.getVector3());
                        v.setQuaternion(q);
                    }
                    else if (paramGivens.Count == 1)
                    {
                        Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                        if (v0.getType() != "Vector3")
                        {
                            throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                        }
                        Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                        Quaternion q = v.getQuaternion();
                        q.SetLookRotation(v0_temp.getVector3());
                        v.setQuaternion(q);
                    }
                    else
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    if (p_rightNode == null)
                    {
                        last_env = v;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                //else if (name == "ToAngleAxis")
                //{

                //}
                else
                {
                    throw new VarException("unsupported member " + name, node.getLine());
                }
            }
            else
            {
                AST_error(node);
                last_env = null;
                var_name = null;
                type_name = null;
                return null;
            }
        }

        private Value track_call_staticQuaternion(ClassCall node, string scope, ClassInstanceValue environment)
        {
            AST leftNode = node.getLeft();
            if (leftNode.getAST_Type() == AST_Type.VARMETHODORCLASS)
            {
                VarMethodOrClass leftVar = (VarMethodOrClass)leftNode;
                string name = leftVar.getName();
                if (name == "identity")
                {
                    ClassCall rightNode = node.getRight();
                    QuaternionIntanceValue vq = new QuaternionIntanceValue(Quaternion.identity);
                    if (rightNode == null)
                    {
                        return vq;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_quaternion(rightNode, vq, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else
                {
                    throw new VarException("unsupported member " + name, node.getLine());
                }
            }
            else if (leftNode.getAST_Type() == AST_Type.METHOD_CALL)
            {
                ClassCall p_rightNode = node.getRight();
                MethodCall theMethod = (MethodCall)leftNode;
                string name = theMethod.getMethodName();
                if (name == "Angle")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Quaternion value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Quaternion value", node.getLine());
                    }
                    QuaternionIntanceValue v0_temp = (QuaternionIntanceValue)v0;
                    QuaternionIntanceValue v1_temp = (QuaternionIntanceValue)v1;
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Quaternion.Angle(v0_temp.getQuaternion(), v1_temp.getQuaternion());
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "AngleAxis")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    float pfv;
                    if (v0.getType() == "float")
                    {
                        pfv = floatValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "long")
                    {
                        pfv = longValueList[v0.getIndex()];
                    }
                    else if (v0.getType() == "int")
                    {
                        pfv = intValueList[v0.getIndex()];
                    }
                    else
                    {
                        pfv = 0;
                        throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    setIndexFree(v0, node.getLine());
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    QuaternionIntanceValue result = new QuaternionIntanceValue(Quaternion.AngleAxis(pfv, v1_temp.getVector3()));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_quaternion(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "Dot")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Quaternion value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Quaternion value", node.getLine());
                    }
                    QuaternionIntanceValue v0_temp = (QuaternionIntanceValue)v0;
                    QuaternionIntanceValue v1_temp = (QuaternionIntanceValue)v1;
                    int index;
                    Value fv = init_a_new_value(out index, "float", node.getLine());
                    floatValueList[index] = Quaternion.Dot(v0_temp.getQuaternion(), v1_temp.getQuaternion());
                    if (p_rightNode == null)
                    {
                        return fv;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        return null;
                    }
                }
                else if (name == "Euler")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    QuaternionIntanceValue result;
                    if (paramGivens.Count == 3)
                    {
                        Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                        float pfv0;
                        if (v0.getType() == "float")
                        {
                            pfv0 = floatValueList[v0.getIndex()];
                        }
                        else if (v0.getType() == "long")
                        {
                            pfv0 = longValueList[v0.getIndex()];
                        }
                        else if (v0.getType() == "int")
                        {
                            pfv0 = intValueList[v0.getIndex()];
                        }
                        else
                        {
                            pfv0 = 0;
                            throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                        }
                        Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                        float pfv1;
                        if (v1.getType() == "float")
                        {
                            pfv1 = floatValueList[v1.getIndex()];
                        }
                        else if (v1.getType() == "long")
                        {
                            pfv1 = longValueList[v1.getIndex()];
                        }
                        else if (v1.getType() == "int")
                        {
                            pfv1 = intValueList[v1.getIndex()];
                        }
                        else
                        {
                            pfv1 = 0;
                            throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                        }
                        Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                        float pfv2;
                        if (v2.getType() == "float")
                        {
                            pfv2 = floatValueList[v2.getIndex()];
                        }
                        else if (v2.getType() == "long")
                        {
                            pfv2 = longValueList[v2.getIndex()];
                        }
                        else if (v2.getType() == "int")
                        {
                            pfv2 = intValueList[v2.getIndex()];
                        }
                        else
                        {
                            pfv2 = 0;
                            throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                        }
                        setIndexFree(v0, node.getLine());
                        setIndexFree(v1, node.getLine());
                        setIndexFree(v2, node.getLine());
                        result = new QuaternionIntanceValue(Quaternion.Euler(pfv0, pfv1, pfv2));
                    }
                    else if (paramGivens.Count == 1)
                    {
                        Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                        if (v0.getType() != "Vector3")
                        {
                            throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                        }
                        Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                        result = new QuaternionIntanceValue(Quaternion.Euler(v0_temp.getVector3()));
                    }
                    else
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_quaternion(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "FromToRotation")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    QuaternionIntanceValue result = new QuaternionIntanceValue(Quaternion.FromToRotation(v0_temp.getVector3(), v1_temp.getVector3()));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_quaternion(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "Inverse")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Quaternion value", node.getLine());
                    }
                    QuaternionIntanceValue v0_temp = (QuaternionIntanceValue)v0;
                    QuaternionIntanceValue result = new QuaternionIntanceValue(Quaternion.Inverse(v0_temp.getQuaternion()));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_quaternion(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "Lerp")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Quaternion value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Quaternion value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    QuaternionIntanceValue v0_temp = (QuaternionIntanceValue)v0;
                    QuaternionIntanceValue v1_temp = (QuaternionIntanceValue)v1;
                    setIndexFree(v2, node.getLine());
                    QuaternionIntanceValue result = new QuaternionIntanceValue(Quaternion.Lerp(v0_temp.getQuaternion(), v1_temp.getQuaternion(), pfv2));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_quaternion(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "LerpUnclamped")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Quaternion value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Quaternion value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    QuaternionIntanceValue v0_temp = (QuaternionIntanceValue)v0;
                    QuaternionIntanceValue v1_temp = (QuaternionIntanceValue)v1;
                    setIndexFree(v2, node.getLine());
                    QuaternionIntanceValue result = new QuaternionIntanceValue(Quaternion.LerpUnclamped(v0_temp.getQuaternion(), v1_temp.getQuaternion(), pfv2));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_quaternion(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "LookRotation")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    QuaternionIntanceValue result;
                    if (paramGivens.Count == 2)
                    {
                        Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                        if (v0.getType() != "Vector3")
                        {
                            throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                        }
                        Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                        if (v1.getType() != "Vector3")
                        {
                            throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                        }
                        Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                        Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                        result = new QuaternionIntanceValue(Quaternion.LookRotation(v0_temp.getVector3(), v1_temp.getVector3()));
                    }
                    else if (paramGivens.Count == 1)
                    {
                        Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                        if (v0.getType() != "Vector3")
                        {
                            throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                        }
                        Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                        result = new QuaternionIntanceValue(Quaternion.LookRotation(v0_temp.getVector3()));
                    }
                    else
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_quaternion(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "Normalize")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 1)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Quaternion value", node.getLine());
                    }
                    QuaternionIntanceValue v0_temp = (QuaternionIntanceValue)v0;
                    QuaternionIntanceValue result = new QuaternionIntanceValue(Quaternion.Normalize(v0_temp.getQuaternion()));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_quaternion(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "RotateTowards")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Quaternion value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Quaternion value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    QuaternionIntanceValue v0_temp = (QuaternionIntanceValue)v0;
                    QuaternionIntanceValue v1_temp = (QuaternionIntanceValue)v1;
                    setIndexFree(v2, node.getLine());
                    QuaternionIntanceValue result = new QuaternionIntanceValue(Quaternion.RotateTowards(v0_temp.getQuaternion(), v1_temp.getQuaternion(), pfv2));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_quaternion(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "Slerp")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Quaternion value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Quaternion value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    QuaternionIntanceValue v0_temp = (QuaternionIntanceValue)v0;
                    QuaternionIntanceValue v1_temp = (QuaternionIntanceValue)v1;
                    setIndexFree(v2, node.getLine());
                    QuaternionIntanceValue result = new QuaternionIntanceValue(Quaternion.Slerp(v0_temp.getQuaternion(), v1_temp.getQuaternion(), pfv2));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_quaternion(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else if (name == "SlerpUnclamped")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Quaternion value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Quaternion value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    QuaternionIntanceValue v0_temp = (QuaternionIntanceValue)v0;
                    QuaternionIntanceValue v1_temp = (QuaternionIntanceValue)v1;
                    setIndexFree(v2, node.getLine());
                    QuaternionIntanceValue result = new QuaternionIntanceValue(Quaternion.SlerpUnclamped(v0_temp.getQuaternion(), v1_temp.getQuaternion(), pfv2));
                    if (p_rightNode == null)
                    {
                        return result;
                    }
                    else
                    {
                        Value le;
                        string vn;
                        string tn;
                        return track_call_quaternion(p_rightNode, result, scope, environment, out le, out vn, out tn, false);
                    }
                }
                else
                {
                    throw new VarException("unsupported member " + name, node.getLine());
                }
            }
            else
            {
                AST_error(node);
                return null;
            }
        }

        private Value track_call_transform(ClassCall node, TransformInstanceValue t, string scope, ClassInstanceValue environment, out Value last_env, out string var_name, out string type_name, bool ifLeft)
        {
            AST leftNode = node.getLeft();
            if (leftNode.getAST_Type() == AST_Type.VARMETHODORCLASS)
            {
                VarMethodOrClass leftVar = (VarMethodOrClass)leftNode;
                string name = leftVar.getName();
                if (name == "position")
                {
                    ClassCall p_rightNode = node.getRight();
                    Vector3IntanceValue v = new Vector3IntanceValue(t.getTransform().position);
                    if (p_rightNode == null)
                    {
                        last_env = t;
                        var_name = name;
                        type_name = "Vector3";
                        return v;
                    }
                    else
                    {
                        if (ifLeft)
                        {
                            throw new VarException("Vector3 is data of value type, and it is meaningless to change it directly in transform instnace", node.getLine());
                        }
                        else
                        {
                            return track_call_vector3(p_rightNode, v, scope, environment, out last_env, out var_name, out type_name, ifLeft);
                        }
                    }
                }
                else if (name == "rotation")
                {
                    ClassCall p_rightNode = node.getRight();
                    QuaternionIntanceValue v = new QuaternionIntanceValue(t.getTransform().rotation);
                    if (p_rightNode == null)
                    {
                        last_env = t;
                        var_name = name;
                        type_name = "Quaternion";
                        return v;
                    }
                    else
                    {
                        if (ifLeft)
                        {
                            throw new VarException("Quaternion is data of value type, and it is meaningless to change it directly in transform instnace", node.getLine());
                        }
                        else
                        {
                            return track_call_quaternion(p_rightNode, v, scope, environment, out last_env, out var_name, out type_name, ifLeft);
                        }
                    }
                }
                else if (name == "eulerAngles")
                {
                    ClassCall p_rightNode = node.getRight();
                    Vector3IntanceValue v = new Vector3IntanceValue(t.getTransform().eulerAngles);
                    if (p_rightNode == null)
                    {
                        last_env = t;
                        var_name = name;
                        type_name = "Vector3";
                        return v;
                    }
                    else
                    {
                        if (ifLeft)
                        {
                            throw new VarException("Vector3 is data of value type, and it is meaningless to change it directly in transform instnace", node.getLine());
                        }
                        else
                        {
                            return track_call_vector3(p_rightNode, v, scope, environment, out last_env, out var_name, out type_name, ifLeft);
                        }
                    }
                }
                else if (name == "localScale")
                {
                    ClassCall p_rightNode = node.getRight();
                    Vector3IntanceValue v = new Vector3IntanceValue(t.getTransform().localScale);
                    if (p_rightNode == null)
                    {
                        last_env = t;
                        var_name = name;
                        type_name = "Vector3";
                        return v;
                    }
                    else
                    {
                        if (ifLeft)
                        {
                            throw new VarException("Vector3 is data of value type, and it is meaningless to change it directly in transform instnace", node.getLine());
                        }
                        else
                        {
                            return track_call_vector3(p_rightNode, v, scope, environment, out last_env, out var_name, out type_name, ifLeft);
                        }
                    }
                }
                else if (name == "forward")
                {
                    ClassCall p_rightNode = node.getRight();
                    Vector3IntanceValue v = new Vector3IntanceValue(t.getTransform().forward);
                    if (p_rightNode == null)
                    {
                        last_env = t;
                        var_name = name;
                        type_name = "Vector3";
                        return v;
                    }
                    else
                    {
                        if (ifLeft)
                        {
                            throw new VarException("Vector3 is data of value type, and it is meaningless to change it directly in transform instnace", node.getLine());
                        }
                        else
                        {
                            return track_call_vector3(p_rightNode, v, scope, environment, out last_env, out var_name, out type_name, ifLeft);
                        }
                    }
                }
                else if (name == "right")
                {
                    ClassCall p_rightNode = node.getRight();
                    Vector3IntanceValue v = new Vector3IntanceValue(t.getTransform().right);
                    if (p_rightNode == null)
                    {
                        last_env = t;
                        var_name = name;
                        type_name = "Vector3";
                        return v;
                    }
                    else
                    {
                        if (ifLeft)
                        {
                            throw new VarException("Vector3 is data of value type, and it is meaningless to change it directly in transform instnace", node.getLine());
                        }
                        else
                        {
                            return track_call_vector3(p_rightNode, v, scope, environment, out last_env, out var_name, out type_name, ifLeft);
                        }
                    }
                }
                else if (name == "up")
                {
                    ClassCall p_rightNode = node.getRight();
                    Vector3IntanceValue v = new Vector3IntanceValue(t.getTransform().up);
                    if (p_rightNode == null)
                    {
                        last_env = t;
                        var_name = name;
                        type_name = "Vector3";
                        return v;
                    }
                    else
                    {
                        if (ifLeft)
                        {
                            throw new VarException("Vector3 is data of value type, and it is meaningless to change it directly in transform instnace", node.getLine());
                        }
                        else
                        {
                            return track_call_vector3(p_rightNode, v, scope, environment, out last_env, out var_name, out type_name, ifLeft);
                        }
                    }
                }
                else
                {
                    throw new VarException("unsupported member " + name, node.getLine());
                }
            }
            else if (leftNode.getAST_Type() == AST_Type.METHOD_CALL)
            {
                if (ifLeft)
                {
                    throw new AssignException("Method cannot be placed to the left of assignment", node.getLine());
                }
                ClassCall p_rightNode = node.getRight();
                MethodCall theMethod = (MethodCall)leftNode;
                string name = theMethod.getMethodName();
                if (name == "LookAt")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count == 1)
                    {
                        Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                        if (v0.getType() != "Transform")
                        {
                            throw new MethodException("can't convert " + v0.getType() + " value to Transform value", node.getLine());
                        }
                        TransformInstanceValue v0_temp = (TransformInstanceValue)v0;
                        t.getTransform().LookAt(v0_temp.getTransform());
                    }
                    else if (paramGivens.Count == 2)
                    {
                        Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                        if (v0.getType() != "Transform")
                        {
                            throw new MethodException("can't convert " + v0.getType() + " value to Transform value", node.getLine());
                        }
                        Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                        if (v1.getType() != "Vector3")
                        {
                            throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                        }
                        TransformInstanceValue v0_temp = (TransformInstanceValue)v0;
                        Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                        t.getTransform().LookAt(v0_temp.getTransform(), v1_temp.getVector3());
                    }
                    else
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    if (p_rightNode == null)
                    {
                        last_env = t;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "Rotate")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count == 1)
                    {
                        Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                        if (v0.getType() != "Vector3")
                        {
                            throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                        }
                        Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                        t.getTransform().Rotate(v0_temp.getVector3());
                    }
                    else if (paramGivens.Count == 3)
                    {
                        Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                        float pfv0;
                        if (v0.getType() == "float")
                        {
                            pfv0 = floatValueList[v0.getIndex()];
                        }
                        else if (v0.getType() == "long")
                        {
                            pfv0 = longValueList[v0.getIndex()];
                        }
                        else if (v0.getType() == "int")
                        {
                            pfv0 = intValueList[v0.getIndex()];
                        }
                        else
                        {
                            pfv0 = 0;
                            throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                        }
                        Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                        float pfv1;
                        if (v1.getType() == "float")
                        {
                            pfv1 = floatValueList[v1.getIndex()];
                        }
                        else if (v1.getType() == "long")
                        {
                            pfv1 = longValueList[v1.getIndex()];
                        }
                        else if (v1.getType() == "int")
                        {
                            pfv1 = intValueList[v1.getIndex()];
                        }
                        else
                        {
                            pfv1 = 0;
                            throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                        }
                        Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                        float pfv2;
                        if (v2.getType() == "float")
                        {
                            pfv2 = floatValueList[v2.getIndex()];
                        }
                        else if (v2.getType() == "long")
                        {
                            pfv2 = longValueList[v2.getIndex()];
                        }
                        else if (v2.getType() == "int")
                        {
                            pfv2 = intValueList[v2.getIndex()];
                        }
                        else
                        {
                            pfv2 = 0;
                            throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                        }
                        setIndexFree(v0, node.getLine());
                        setIndexFree(v1, node.getLine());
                        setIndexFree(v2, node.getLine());
                        t.getTransform().Rotate(pfv0, pfv1, pfv2);
                    }
                    else if (paramGivens.Count == 2)
                    {
                        Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                        if (v0.getType() != "Vector3")
                        {
                            throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                        }
                        Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                        float pfv1;
                        if (v1.getType() == "float")
                        {
                            pfv1 = floatValueList[v1.getIndex()];
                        }
                        else if (v1.getType() == "long")
                        {
                            pfv1 = longValueList[v1.getIndex()];
                        }
                        else if (v1.getType() == "int")
                        {
                            pfv1 = intValueList[v1.getIndex()];
                        }
                        else
                        {
                            pfv1 = 0;
                            throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                        }
                        Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                        setIndexFree(v1, node.getLine());
                        t.getTransform().Rotate(v0_temp.getVector3(), pfv1);
                    }
                    else
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    if (p_rightNode == null)
                    {
                        last_env = t;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "RotateAround")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 3)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                    float pfv2;
                    if (v2.getType() == "float")
                    {
                        pfv2 = floatValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "long")
                    {
                        pfv2 = longValueList[v2.getIndex()];
                    }
                    else if (v2.getType() == "int")
                    {
                        pfv2 = intValueList[v2.getIndex()];
                    }
                    else
                    {
                        pfv2 = 0;
                        throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    setIndexFree(v2, node.getLine());
                    t.getTransform().RotateAround(v0_temp.getVector3(), v1_temp.getVector3(), pfv2);
                    if (p_rightNode == null)
                    {
                        last_env = t;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "SetPositionAndRotation")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count != 2)
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                    if (v0.getType() != "Vector3")
                    {
                        throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                    }
                    Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                    if (v1.getType() != "Quaternion")
                    {
                        throw new MethodException("can't convert " + v1.getType() + " value to Quaternion value", node.getLine());
                    }
                    Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                    QuaternionIntanceValue v1_temp = (QuaternionIntanceValue)v1;
                    t.getTransform().SetPositionAndRotation(v0_temp.getVector3(), v1_temp.getQuaternion());
                    if (p_rightNode == null)
                    {
                        last_env = t;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else if (name == "Translate")
                {
                    List<ParamGiven> paramGivens = theMethod.getParamsGiven();
                    if (paramGivens.Count == 1)
                    {
                        Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                        if (v0.getType() != "Vector3")
                        {
                            throw new MethodException("can't convert " + v0.getType() + " value to Vector3 value", node.getLine());
                        }
                        Vector3IntanceValue v0_temp = (Vector3IntanceValue)v0;
                        t.getTransform().Translate(v0_temp.getVector3());
                    }
                    else if (paramGivens.Count == 3)
                    {
                        Value v0 = trackForNumberOP(paramGivens[0].getNode(), scope, false, environment);
                        float pfv0;
                        if (v0.getType() == "float")
                        {
                            pfv0 = floatValueList[v0.getIndex()];
                        }
                        else if (v0.getType() == "long")
                        {
                            pfv0 = longValueList[v0.getIndex()];
                        }
                        else if (v0.getType() == "int")
                        {
                            pfv0 = intValueList[v0.getIndex()];
                        }
                        else
                        {
                            pfv0 = 0;
                            throw new MethodException("can't convert " + v0.getType() + " value to float value", node.getLine());
                        }
                        Value v1 = trackForNumberOP(paramGivens[1].getNode(), scope, false, environment);
                        float pfv1;
                        if (v1.getType() == "float")
                        {
                            pfv1 = floatValueList[v1.getIndex()];
                        }
                        else if (v1.getType() == "long")
                        {
                            pfv1 = longValueList[v1.getIndex()];
                        }
                        else if (v1.getType() == "int")
                        {
                            pfv1 = intValueList[v1.getIndex()];
                        }
                        else
                        {
                            pfv1 = 0;
                            throw new MethodException("can't convert " + v1.getType() + " value to float value", node.getLine());
                        }
                        Value v2 = trackForNumberOP(paramGivens[2].getNode(), scope, false, environment);
                        float pfv2;
                        if (v2.getType() == "float")
                        {
                            pfv2 = floatValueList[v2.getIndex()];
                        }
                        else if (v2.getType() == "long")
                        {
                            pfv2 = longValueList[v2.getIndex()];
                        }
                        else if (v2.getType() == "int")
                        {
                            pfv2 = intValueList[v2.getIndex()];
                        }
                        else
                        {
                            pfv2 = 0;
                            throw new MethodException("can't convert " + v2.getType() + " value to float value", node.getLine());
                        }
                        setIndexFree(v0, node.getLine());
                        setIndexFree(v1, node.getLine());
                        setIndexFree(v2, node.getLine());
                        t.getTransform().Translate(pfv0, pfv1, pfv2);
                    }
                    else
                    {
                        throw new MethodException("mismatched paramters number for method " + name, node.getLine());
                    }
                    if (p_rightNode == null)
                    {
                        last_env = t;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                    else
                    {
                        AST_error(p_rightNode);
                        last_env = null;
                        var_name = null;
                        type_name = null;
                        return null;
                    }
                }
                else
                {
                    throw new VarException("unsupported member " + name, node.getLine());
                }
            }
            else
            {
                AST_error(node);
                last_env = null;
                var_name = null;
                type_name = null;
                return null;
            }
        }

        private Value track_call_gameObject(ClassCall node, GameObjectInstanceValue go, string scope, ClassInstanceValue environment, out Value last_env, out string var_name, out string type_name, bool ifLeft)
        {
            AST leftNode = node.getLeft();
            if (leftNode.getAST_Type() == AST_Type.VARMETHODORCLASS)
            {
                VarMethodOrClass leftVar = (VarMethodOrClass)leftNode;
                string name = leftVar.getName();
                if (name == "transform")
                {
                    ClassCall t_rightNode = node.getRight();
                    TransformInstanceValue transform = new TransformInstanceValue(go.getGameObject().transform);
                    if (t_rightNode == null)
                    {
                        last_env = go;
                        var_name = name;
                        type_name = "Transform";
                        return transform;
                    }
                    else
                    {
                        return track_call_transform(t_rightNode, transform, scope, environment, out last_env, out var_name, out type_name, ifLeft);
                    }
                }
                else
                {
                    throw new VarException("unsupported member " + name, node.getLine());
                }
            }
            else
            {
                MethodCall methodCall = (MethodCall)leftNode;
                throw new MethodException("unsupported member " + methodCall.getMethodName(), node.getLine());
            }
        }

        private Value track_classCall(ClassCall node, string scope, ClassInstanceValue environment, ClassInstanceValue originEnv, out Value last_env, out string var_name, out string type_name, string orignScope, bool ifLeft)
        {
            AST leftNode = node.getLeft();
            Value theValue;
            var_name = null;
            type_name = null;
            last_env = environment;
            if (leftNode.getAST_Type() == AST_Type.VARMETHODORCLASS)
            {
                VarMethodOrClass leftVar = (VarMethodOrClass)leftNode;
                if (leftVar.ifGameObject() && scope == orignScope)
                {
                    ClassCall go_rightNode = node.getRight();
                    if (go_rightNode == null)
                    {
                        AST_error(go_rightNode);
                    }
                    else
                    {
                        string name = leftVar.getName();
                        if (name == "gameObject")
                        {
                            if (theGameObject == null)
                            {
                                throw new VarException("Unspecified GameObject", node.getLine());
                            }
                            if (theGameObject.ifNull())
                            {
                                throw new VarException("Unspecified GameObject", node.getLine());
                            }
                            return track_call_gameObject(go_rightNode, theGameObject, orignScope, originEnv, out last_env, out var_name, out type_name, ifLeft);
                        }
                        else
                        {
                            if (theGameObjects[name].ifNull())
                            {
                                throw new VarException("Unspecified GameObject", node.getLine());
                            }
                            return track_call_gameObject(go_rightNode, theGameObjects[name], orignScope, originEnv, out last_env, out var_name, out type_name, ifLeft);
                        }
                    }
                }
                else if (leftVar.ifTransform() && scope == orignScope)
                {
                    ClassCall tr_rightNode = node.getRight();
                    if (tr_rightNode == null)
                    {
                        AST_error(tr_rightNode);
                    }
                    else
                    {
                        string name = leftVar.getName();
                        if (name == "transform")
                        {
                            if (theTransform == null)
                            {
                                throw new VarException("Unspecified Transform", node.getLine());
                            }
                            if (theTransform.ifNull())
                            {
                                throw new VarException("Unspecified Transform", node.getLine());
                            }
                            return track_call_transform(tr_rightNode, theTransform, orignScope, originEnv, out last_env, out var_name, out type_name, ifLeft);
                        }
                        else
                        {
                            AST_error(node);
                        }
                    }
                }


                string type;
                bool flag;
                bool access;
                theValue = symbolTables[scope].getVarOrParamSymbleValueDefineFlagAndDefineType(leftVar.getName(), out type, out flag, out access, node.getLine(), environment);
                if (!access && scope != orignScope)
                {
                    throw new VarException("unaccessable member variable " + leftVar.getName(), node.getLine());
                }
                var_name = leftVar.getName();
                type_name = theValue.getType();
            }
            else if (leftNode.getAST_Type() == AST_Type.METHOD_CALL)
            {
                theValue = track_methodCall((MethodCall)leftNode, orignScope, environment, originEnv ,orignScope != scope);
            }
            else if (leftNode.getAST_Type() == AST_Type.INIT_METHOD_CALL)
            {
                theValue = track_initMethodCall((InitMethodCall)leftNode, orignScope, originEnv);
            }
            else if (leftNode.getAST_Type() == AST_Type.TYPE)
            {
                TYPE theType = (TYPE)leftNode;
                string typeName = theType.getName();
                if (typeName == "Vector3")
                {
                    if (ifLeft)
                    {
                        throw new AssignException(typeName + " cannot be placed to the left of assignment", node.getLine());
                    }
                    return track_call_staticVector3(node.getRight(), orignScope, originEnv);
                }
                else if (typeName == "Quaternion")
                {
                    if (ifLeft)
                    {
                        throw new AssignException(typeName + " cannot be placed to the left of assignment", node.getLine());
                    }
                    return track_call_staticQuaternion(node.getRight(), orignScope, originEnv);
                }
                else if (typeName == "Mathf")
                {
                    if (ifLeft)
                    {
                        throw new AssignException(typeName + " cannot be placed to the left of assignment", node.getLine());
                    }
                    return track_call_staticMathf(node.getRight(), orignScope, originEnv);
                }
                else if (typeName == "Time")
                {
                    if (ifLeft)
                    {
                        throw new AssignException(typeName + " cannot be placed to the left of assignment", node.getLine());
                    }
                    return track_call_staticTime(node.getRight(), orignScope, originEnv);
                }
                else
                {
                    throw new ClassException("unexpected static class name " + typeName, node.getLine());
                }
            }
            else
            {
                if (scope != orignScope)
                {
                    AST_error(node);
                    theValue = null;
                }
                theValue = trackForNumberOP(leftNode, scope, false, originEnv);
            }
            ClassCall rightNode = node.getRight();
            if (rightNode == null)
            {
                if (ifLeft && node.IfMethod())
                {
                    throw new AssignException("Method cannot be placed to the left of assignment", node.getLine());
                }
                else
                {
                    last_env = environment;
                    return theValue;
                }
            }
            if (theValue.getType() == "Vector3")
            {
                return track_call_vector3(rightNode, (Vector3IntanceValue)theValue, orignScope, originEnv, out last_env, out var_name, out type_name, ifLeft);
            }
            else if (theValue.getType() == "Quaternion")
            {
                return track_call_quaternion(rightNode, (QuaternionIntanceValue)theValue, orignScope, originEnv, out last_env, out var_name, out type_name, ifLeft);
            }
            else if (theValue.getType() == "Transform")
            {
                return track_call_transform(rightNode, (TransformInstanceValue)theValue, orignScope, originEnv, out last_env, out var_name, out type_name, ifLeft);
            }
            else if (theValue.getType() == "GameObject")
            {
                return track_call_gameObject(rightNode, (GameObjectInstanceValue)theValue, orignScope, originEnv, out last_env, out var_name, out type_name, ifLeft);
            }
            if (!symbolTables.ContainsKey(theValue.getType()))
            {
                throw new VarException("unexpected class name " + theValue.getType(), node.getLine());
            }
            else
            {
                if (theValue == null)
                {
                    throw new VarException("Object reference not set to an instance of an object", node.getLine());
                }
                return track_classCall(rightNode, theValue.getType(), (ClassInstanceValue)theValue, originEnv, out last_env, out var_name, out type_name, orignScope, ifLeft);
            }

        }

        private void track_forBlock(For_Block node, string scope, ClassInstanceValue environment)
        {
            string for_scope = node.getForScope();
            AST_Type s1type = node.getStatement1().getAST_Type();
            if (s1type != AST_Type.VARIABLE_DEC && s1type != AST_Type.ASSIGN && s1type != AST_Type.CLASS_CALL && s1type != AST_Type.METHOD_CALL && s1type != AST_Type.INIT_METHOD_CALL && s1type != AST_Type.UNARYOP)
            {
                throw new ASTException("Only assignment，declaration，call，decrement and increment can be used as a statement here", node.getLine());
            }
            track_in_method(node.getStatement1(), for_scope, environment, null);
            Value v = trackForNumberOP(node.getStatement2(), for_scope, false, environment);
            if (v.getType() != "bool")
            {
                throw new TypeException("unexpected type " + v.getType(), node.getLine());
            }
            bool bv = boolValueList[v.getIndex()];
            setIndexFree(v, node.getStatement2().getLine());
            while (bv)
            {
                track_in_method(node.getBlock(), for_scope, environment, for_scope);
                if (scope_break_flag.ContainsKey(for_scope))
                {
                    if (scope_break_flag[for_scope])
                    {
                        scope_break_flag[for_scope] = false;
                        break;
                    }
                }
                if (scope_continue_flag.ContainsKey(for_scope))
                {
                    if (scope_continue_flag[for_scope])
                    {
                        scope_continue_flag[for_scope] = false;
                    }
                }
                AST_Type s3type = node.getStatement3().getAST_Type();
                if (s1type != AST_Type.VARIABLE_DEC && s1type != AST_Type.ASSIGN && s1type != AST_Type.CLASS_CALL && s1type != AST_Type.METHOD_CALL && s1type != AST_Type.INIT_METHOD_CALL && s1type != AST_Type.UNARYOP)
                {
                    throw new ASTException("Only assignment，declaration，call，decrement and increment can be used as a statement here", node.getLine());
                }
                track_in_method(node.getStatement3(), for_scope, environment, null);
                v = trackForNumberOP(node.getStatement2(), for_scope, false, environment);
                if (v.getType() != "bool")
                {
                    throw new TypeException("unexpected type " + v.getType(), node.getLine());
                }
                bv = boolValueList[v.getIndex()];
                setIndexFree(v, node.getStatement2().getLine());
            }
        }

        private void track_whileBlock(While_Block node, string scope, ClassInstanceValue environment)
        {
            string while_scope = node.getWhileScope();
            Value v = trackForNumberOP(node.getExpr(), scope, false, environment);
            if (v.getType() != "bool")
            {
                throw new TypeException("unexpected type " + v.getType(), node.getLine());
            }
            bool bv = boolValueList[v.getIndex()];
            setIndexFree(v, node.getExpr().getLine());
            while (bv)
            {
                track_in_method(node.getBlock(), while_scope, environment, while_scope);
                if (scope_break_flag.ContainsKey(while_scope))
                {
                    if (scope_break_flag[while_scope])
                    {
                        scope_break_flag[while_scope] = false;
                        break;
                    }
                }
                if (scope_continue_flag.ContainsKey(while_scope))
                {
                    if (scope_continue_flag[while_scope])
                    {
                        scope_continue_flag[while_scope] = false;
                    }
                }
                v = trackForNumberOP(node.getExpr(), scope, false, environment);
                if (v.getType() != "bool")
                {
                    throw new TypeException("unexpected type " + v.getType(), node.getLine());
                }
                bv = boolValueList[v.getIndex()];
                setIndexFree(v, node.getExpr().getLine());
            }
        }

        private void track_ifBlock(If_Block node, string scope, ClassInstanceValue environment, string CirScope)
        {
            Value v = trackForNumberOP(node.getExpr(), scope, false, environment);
            if (v.getType() != "bool")
            {
                throw new TypeException("unexpected type " + v.getType(), node.getLine());
            }
            bool bv = boolValueList[v.getIndex()];
            setIndexFree(v, node.getLine());
            if (bv)
            {
                track_in_method(node.getBlock(), node.getIfScope(), environment, CirScope);
                return;
            }
            foreach (ElSE_IF_BLOCK elseIf in node.getElseIfBlocks())
            {
                Value elseIfV = trackForNumberOP(elseIf.getExpr(), scope, false, environment);
                if (elseIfV.getType() != "bool")
                {
                    throw new TypeException("unexpected type " + elseIfV.getType(), elseIf.getLine());
                }
                bool bElseIfV = boolValueList[elseIfV.getIndex()];
                setIndexFree(elseIfV, elseIf.getLine());
                if (bElseIfV)
                {
                    track_in_method(elseIf.getBlock(), elseIf.getScope(), environment, CirScope);
                    return;
                }
            }
            if (node.getElseBlock() != null)
            {
                track_in_method(node.getElseBlock(), node.getElseScope(), environment, CirScope);
                return;
            }
        }

        private void pass_parameters(SymbolTable methodSymbolTable, List<string> methodParams, List<ParamGiven> pGivenList, string scope, ClassInstanceValue environment)
        {
            for (int i = 0; i < methodParams.Count; i++)
            {
                string p = methodParams[i];
                ParamGiven pg = pGivenList[i];
                int line = pg.getNode().getLine();
                string vType = methodSymbolTable.getParamsType(p, line);
                Value pValue = trackForNumberOP(pg.getNode(), scope, false, environment);
                if (pValue.getIfClassType())
                {
                    if (pValue.getType() == vType)
                    {
                        methodSymbolTable.pushParamSymbolValue(pValue, p, line);
                        continue;
                    }
                    else
                    {
                        throw new MethodException("can't convert " + pValue.getType() + " value to " + vType + " value", line);
                    }
                }
                string pType = pValue.getType();
                int pIndex = pValue.getIndex();
                setIndexFree(pValue, line);
                int vIndex;
                Value v = init_a_new_value(out vIndex, vType, line);
                v.setCanNotFree();
                methodSymbolTable.pushParamSymbolValue(v, p, line);
                if (vType == "double")
                {
                    if (pType == "double")
                    {
                        double pV = doubleValueList[pIndex];
                        doubleValueList[vIndex] = pV;
                    }
                    else if (pType == "float")
                    {
                        float pV = floatValueList[pIndex];
                        doubleValueList[vIndex] = pV;
                    }
                    else if (pType == "int")
                    {
                        int pV = intValueList[pIndex];
                        doubleValueList[vIndex] = pV;
                    }
                    else if (pType == "long")
                    {
                        long pV = longValueList[pIndex];
                        doubleValueList[vIndex] = pV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + pType + " value to " + vType + " value", line);
                    }
                }
                else if (vType == "float")
                {
                    if (pType == "float")
                    {
                        float pV = floatValueList[pIndex];
                        floatValueList[vIndex] = pV;
                    }
                    else if (pType == "int")
                    {
                        int pV = intValueList[pIndex];
                        floatValueList[vIndex] = pV;
                    }
                    else if (pType == "long")
                    {
                        long pV = longValueList[pIndex];
                        floatValueList[vIndex] = pV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + pType + " value to " + vType + " value", line);
                    }
                }
                else if (vType == "int")
                {
                    if (pType == "int")
                    {
                        int pV = intValueList[pIndex];
                        intValueList[vIndex] = pV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + pType + " value to " + vType + " value", line);
                    }
                }
                else if (vType == "long")
                {
                    if (pType == "int")
                    {
                        int pV = intValueList[pIndex];
                        longValueList[vIndex] = pV;
                    }
                    else if (pType == "long")
                    {
                        long pV = longValueList[pIndex];
                        longValueList[vIndex] = pV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + pType + " value to " + vType + " value", line);
                    }
                }
                else if (vType == "bool")
                {
                    if (pType == "bool")
                    {
                        bool pV = boolValueList[pIndex];
                        boolValueList[vIndex] = pV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + pType + " value to " + vType + " value", line);
                    }
                }
                else if (vType == "decimal")
                {
                    if (pType == "decimal")
                    {
                        decimal pV = decimalValueList[pIndex];
                        decimalValueList[vIndex] = pV;
                    }
                    else if (pType == "int")
                    {
                        int pV = intValueList[pIndex];
                        decimalValueList[vIndex] = pV;
                    }
                    else if (pType == "long")
                    {
                        long pV = longValueList[pIndex];
                        decimalValueList[vIndex] = pV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + pType + " value to " + vType + " value", line);
                    }
                }
                else
                {
                    throw new TypeException("can't convert " + pType + " value to " + vType + " value", line);
                }
            }
        }

        private Value trackForNumberOP(AST node, string scope, bool ifMemberVarInit, ClassInstanceValue environment)
        {
            if (node.getAST_Type() == AST_Type.BINOP)
            {
                return track_op((BinOP)node, scope, ifMemberVarInit, environment);
            }
            else if (node.getAST_Type() == AST_Type.NUM)
            {
                return track_num((Num)node);
            }
            else if (node.getAST_Type() == AST_Type.UNARYOP)
            {
                return track_Uop((UnaryOP)node, scope, ifMemberVarInit, environment);
            }
            else if (node.getAST_Type() == AST_Type.TYPEALTER)
            {
                return track_typeAlterOp((TypeAlterOP)node, scope, ifMemberVarInit, environment);
            }
            else if (node.getAST_Type() == AST_Type.VARMETHODORCLASS)
            {
                if (ifMemberVarInit)
                {
                    throw new VarException("Field initializers cannot reference non static fields, methods, or properties", node.getLine());
                }
                return track_var((VarMethodOrClass)node, scope, environment);
            }
            else if (node.getAST_Type() == AST_Type.METHOD_CALL)
            {
                if (ifMemberVarInit)
                {
                    throw new VarException("Field initializers cannot reference non static fields, methods, or properties", node.getLine());
                }
                return track_methodCall((MethodCall)node, scope, environment, environment, false);
            }
            else if (node.getAST_Type() == AST_Type.CLASS_CALL)
            {
                if (ifMemberVarInit)
                {
                    throw new VarException("Field initializers cannot reference non static fields, methods, or properties", node.getLine());
                }
                Value a;
                string b;
                string c;
                return track_classCall((ClassCall)node, scope, environment, environment, out a, out b, out c, scope, false);
            }
            else if (node.getAST_Type() == AST_Type.INIT_METHOD_CALL)
            {
                return track_initMethodCall((InitMethodCall)node, scope, environment);
            }
            else if (node.getAST_Type() == AST_Type.BUILDIN_INIT_METHOD_CALL)
            {
                return track_buildInInitMethodCall((BuildInInitMethodCall)node, scope, environment);
            }
            else
            {
                AST_error(node);
                return null;
            }
        }

        private Value init_a_new_value(out int index, string type, int line)
        {
            index = getFreeIndex(type, line);
            if (index == -1)
            {
                if (type == "Vector3")
                {
                    Vector3IntanceValue v3 = new Vector3IntanceValue(new Vector3());
                    return v3;
                }
                else if (type == "Quaternion")
                {
                    QuaternionIntanceValue vq = new QuaternionIntanceValue(new Quaternion());
                    return vq;
                }
                else
                {
                    return null;
                }
            }
            Value newValue = new Value(type);
            newValue.setIndex(index);
            setIndexOccupied(newValue, line);
            return newValue;
        }

        private Value track_op(BinOP node, string scope, bool ifMemberVarInit, ClassInstanceValue environment)
        {
            int line = node.getLine();
            if (node.getOp().getType() == Basic_Type.PLUS)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "Vector3")
                {
                    if (typeV2 != "Vector3")
                    {
                        operator_error("+", typeV1, typeV2, line);
                        return null;
                    }
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    Vector3IntanceValue v2_temp = (Vector3IntanceValue)v2;
                    return new Vector3IntanceValue(v1_temp.getVector3() + v2_temp.getVector3());
                }
                if (typeV1 == "double")
                {
                    double dv1 = doubleValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        double result = dv1 + dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        double result = dv1 + fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        double result = dv1 + iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        double result = dv1 + lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("+", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "float")
                {
                    float fv1 = floatValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        double result = fv1 + dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        float result = fv1 + fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        float result = fv1 + iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        float result = fv1 + lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("+", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        double result = iv1 + dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        float result = iv1 + fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        int result = iv1 + iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "int", line);
                        intValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        long result = iv1 + lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        decimal result = iv1 + dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("+", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        double result = lv1 + dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        float result = lv1 + fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        long result = lv1 + iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        long result = lv1 + lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        decimal result = lv1 + dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("+", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "decimal")
                {
                    decimal dv1 = decimalValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        decimal result = dv1 + iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        decimal result = dv1 + lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        decimal result = dv1 + dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("+", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error("+", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.MINUS)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "Vector3")
                {
                    if (typeV2 != "Vector3")
                    {
                        operator_error("-", typeV1, typeV2, line);
                        return null;
                    }
                    Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                    Vector3IntanceValue v2_temp = (Vector3IntanceValue)v2;
                    return new Vector3IntanceValue(v1_temp.getVector3() - v2_temp.getVector3());
                }
                if (typeV1 == "double")
                {
                    double dv1 = doubleValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        double result = dv1 - dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        double result = dv1 - fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        double result = dv1 - iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        double result = dv1 - lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("-", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "float")
                {
                    float fv1 = floatValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        double result = fv1 - dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        float result = fv1 - fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        float result = fv1 - iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        float result = fv1 - lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("-", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        double result = iv1 - dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        float result = iv1 - fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        int result = iv1 - iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "int", line);
                        intValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        long result = iv1 - lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        decimal result = iv1 - dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("-", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        double result = lv1 - dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        float result = lv1 - fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        long result = lv1 - iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        long result = lv1 - lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        decimal result = lv1 - dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("-", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "decimal")
                {
                    decimal dv1 = decimalValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        decimal result = dv1 - iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        decimal result = dv1 - lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        decimal result = dv1 - dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("-", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error("-", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.MUL)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v2, line);
                setIndexFree(v1, line);
                if (typeV1 == "Vector3")
                {
                    if (typeV2 == "int")
                    {
                        Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                        return new Vector3IntanceValue(intValueList[indexV2] * v1_temp.getVector3());
                    }
                    else if (typeV2 == "long")
                    {
                        Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                        return new Vector3IntanceValue(longValueList[indexV2] * v1_temp.getVector3());
                    }
                    else
                    {
                        operator_error("*", typeV1, typeV2, line);
                        return null;
                    }
                }
                if (typeV1 == "Quaternion")
                {
                    if (typeV2 == "Quaternion")
                    {
                        QuaternionIntanceValue v1_temp = (QuaternionIntanceValue)v1;
                        QuaternionIntanceValue v2_temp = (QuaternionIntanceValue)v2;
                        return new QuaternionIntanceValue(v1_temp.getQuaternion() * v2_temp.getQuaternion());
                    }
                    else
                    {
                        operator_error("*", typeV1, typeV2, line);
                        return null;
                    }
                }
                if (typeV1 == "double")
                {
                    double dv1 = doubleValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        double result = dv1 * dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        double result = dv1 * fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        double result = dv1 * iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        double result = dv1 * lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("*", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "float")
                {
                    float fv1 = floatValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        double result = fv1 * dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        float result = fv1 * fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        float result = fv1 * iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        float result = fv1 * lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("*", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        double result = iv1 * dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        float result = iv1 * fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        int result = iv1 * iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "int", line);
                        intValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        long result = iv1 * lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        decimal result = iv1 * dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "Vector3")
                    {
                        Vector3IntanceValue v2_temp = (Vector3IntanceValue)v2;
                        return new Vector3IntanceValue(v2_temp.getVector3() * iv1);
                    }
                    else
                    {
                        operator_error("*", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        double result = lv1 * dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        float result = lv1 * fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        long result = lv1 * iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        long result = lv1 * lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        decimal result = lv1 * dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "Vector3")
                    {
                        Vector3IntanceValue v2_temp = (Vector3IntanceValue)v2;
                        return new Vector3IntanceValue(v2_temp.getVector3() * lv1);
                    }
                    else
                    {
                        operator_error("*", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "decimal")
                {
                    decimal dv1 = decimalValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        decimal result = dv1 * iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        decimal result = dv1 * lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        decimal result = dv1 * dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("*", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error("*", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.DIV)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "Vector3")
                {
                    if (typeV2 == "int")
                    {
                        Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                        return new Vector3IntanceValue(v1_temp.getVector3() / intValueList[indexV2]);
                    }
                    else if (typeV2 == "long")
                    {
                        Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                        return new Vector3IntanceValue(v1_temp.getVector3() / longValueList[indexV2]);
                    }
                    else
                    {
                        operator_error("/", typeV1, typeV2, line);
                        return null;
                    }
                }
                if (typeV1 == "double")
                {
                    double dv1 = doubleValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        if (dv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        double result = dv1 / dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        if (fv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        double result = dv1 / fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        if (iv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        double result = dv1 / iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        if (lv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        double result = dv1 / lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("/", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "float")
                {
                    float fv1 = floatValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        if (dv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        double result = fv1 / dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        if (fv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        float result = fv1 / fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        if (iv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        float result = fv1 / iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        if (lv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        float result = fv1 / lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("/", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        if (dv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        double result = iv1 / dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        if (fv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        float result = iv1 / fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        if (iv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        int result = iv1 / iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "int", line);
                        intValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        if (lv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        long result = iv1 / lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        if (dv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        decimal result = iv1 / dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("/", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        if (dv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        double result = lv1 / dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        if (fv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        float result = lv1 / fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        if (iv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        long result = lv1 / iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        if (lv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        long result = lv1 / lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        if (dv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        decimal result = lv1 / dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("/", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "decimal")
                {
                    decimal dv1 = decimalValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        if (iv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        decimal result = dv1 / iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        if (lv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        decimal result = dv1 / lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        if (dv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        decimal result = dv1 / dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("/", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error("/", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.REMAINDER)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "double")
                {
                    double dv1 = doubleValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        if (dv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        double result = dv1 % dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        if (fv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        double result = dv1 % fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        if (iv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        double result = dv1 % iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        if (lv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        double result = dv1 % lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("%", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "float")
                {
                    float fv1 = floatValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        if (dv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        double result = fv1 % dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        if (fv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        float result = fv1 % fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        if (iv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        float result = fv1 % iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        if (lv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        float result = fv1 % lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("%", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        if (dv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        double result = iv1 % dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        if (fv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        float result = iv1 % fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        if (iv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        int result = iv1 % iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "int", line);
                        intValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        if (lv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        long result = iv1 % lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        if (dv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        decimal result = iv1 % dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("%", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        if (dv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        double result = lv1 % dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "double", line);
                        doubleValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        if (fv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        float result = lv1 % fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "float", line);
                        floatValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        if (iv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        long result = lv1 % iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        if (lv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        long result = lv1 % lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        if (dv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        decimal result = lv1 % dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("%", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "decimal")
                {
                    decimal dv1 = decimalValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        if (iv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        decimal result = dv1 % iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        if (lv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        decimal result = dv1 % lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        if (dv2 == 0)
                        {
                            throw new VarException("divide by zero error", line);
                        }
                        decimal result = dv1 % dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "decimal", line);
                        decimalValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("%", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error("%", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.AND)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        int result = iv1 & iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "int", line);
                        intValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        long result = iv1 & lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("&", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        long result = lv1 & iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        long result = lv1 & lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("&", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error("&", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.OR)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        int result = iv1 | iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "int", line);
                        intValueList[rindex] = result;
                        return rV;
                    }
                    //else if (typeV2 == "long")
                    //{
                    //    long lv2 = longValueList[indexV2];
                    //    long result = iv1 | lv2;                                   //warning
                    //    int rindex;
                    //    Value rV = init_a_new_value(out rindex, "long", line);   
                    //    longValueList[rindex] = result;
                    //    return rV;
                    //}
                    else
                    {
                        operator_error("|", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    //if (typeV2 == "int")
                    //{
                    //    int iv2 = intValueList[indexV2];
                    //    long result = lv1 | iv2;                                      //warning
                    //    int rindex;
                    //    Value rV = init_a_new_value(out rindex, "long", line);
                    //    longValueList[rindex] = result;
                    //    return rV;
                    //}
                    if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        long result = lv1 | lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("|", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error("|", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.XOR)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        int result = iv1 ^ iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "int", line);
                        intValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        long result = iv1 ^ lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("^", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        long result = lv1 ^ iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        long result = lv1 ^ lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("^", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error("^", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.LEFT_SHIFT)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        int result = iv1 << iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "int", line);
                        intValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("<<", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        long result = lv1 << iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("<<", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error("<<", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.RIGHT_SHIFT)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        int result = iv1 >> iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "int", line);
                        intValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error(">>", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        long result = lv1 >> iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "long", line);
                        longValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error(">>", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error(">>", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.ANDAND)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "bool" && typeV2 == "bool")
                {
                    bool bv1 = boolValueList[indexV1];
                    bool bv2 = boolValueList[indexV2];
                    bool result = bv1 && bv2;
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "bool", line);
                    boolValueList[rindex] = result;
                    return rV;
                }
                else
                {
                    throw new TypeException("&& can only be used with boolen type value", line);
                }

            }
            else if (node.getOp().getType() == Basic_Type.OROR)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "bool" && typeV2 == "bool")
                {
                    bool bv1 = boolValueList[indexV1];
                    bool bv2 = boolValueList[indexV2];
                    bool result = bv1 || bv2;
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "bool", line);
                    boolValueList[rindex] = result;
                    return rV;
                }
                else
                {
                    throw new TypeException("&& can only be used with boolen type value", line);
                }
            }
            else if (node.getOp().getType() == Basic_Type.GREATER)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "double")
                {
                    double dv1 = doubleValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = dv1 > dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = dv1 > fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = dv1 > iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = dv1 > lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error(">", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "float")
                {
                    float fv1 = floatValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = fv1 > dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = fv1 > fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = fv1 > iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = fv1 > lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error(">", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = iv1 > dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = iv1 > fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = iv1 > iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = iv1 > lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = iv1 > dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error(">", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = lv1 > dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = lv1 > fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = lv1 > iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = lv1 > lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = lv1 > dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error(">", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "decimal")
                {
                    decimal dv1 = decimalValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = dv1 > iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = dv1 > lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = dv1 > dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error(">", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error(">", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.LESS)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "double")
                {
                    double dv1 = doubleValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = dv1 < dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = dv1 < fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = dv1 < iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = dv1 < lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("<", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "float")
                {
                    float fv1 = floatValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = fv1 < dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = fv1 < fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = fv1 < iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = fv1 < lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("<", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = iv1 < dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = iv1 < fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = iv1 < iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = iv1 < lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = iv1 < dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("<", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = lv1 < dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = lv1 < fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = lv1 < iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = lv1 < lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = lv1 < dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("<", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "decimal")
                {
                    decimal dv1 = decimalValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = dv1 < iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = dv1 < lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = dv1 < dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("<", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error("<", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.GEQUAL)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "double")
                {
                    double dv1 = doubleValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = dv1 >= dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = dv1 >= fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = dv1 >= iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = dv1 >= lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error(">=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "float")
                {
                    float fv1 = floatValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = fv1 >= dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = fv1 >= fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = fv1 >= iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = fv1 >= lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error(">=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = iv1 >= dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = iv1 >= fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = iv1 >= iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = iv1 >= lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = iv1 >= dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error(">=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = lv1 >= dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = lv1 >= fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = lv1 >= iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = lv1 >= lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = lv1 >= dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error(">=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "decimal")
                {
                    decimal dv1 = decimalValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = dv1 >= iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = dv1 >= lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = dv1 >= dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error(">=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error(">=", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.LEQUAL)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "double")
                {
                    double dv1 = doubleValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = dv1 <= dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = dv1 <= fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = dv1 <= iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = dv1 <= lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("<=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "float")
                {
                    float fv1 = floatValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = fv1 <= dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = fv1 <= fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = fv1 <= iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = fv1 <= lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("<=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = iv1 <= dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = iv1 <= fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = iv1 <= iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = iv1 <= lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = iv1 <= dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("<=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = lv1 <= dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = lv1 <= fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = lv1 <= iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = lv1 <= lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = lv1 <= dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("<=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "decimal")
                {
                    decimal dv1 = decimalValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = dv1 <= iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = dv1 <= lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = dv1 <= dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("<=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error("<=", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.EQUAL)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "Vector3")
                {
                    if (typeV2 == "Vector3")
                    {
                        Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                        Vector3IntanceValue v2_temp = (Vector3IntanceValue)v2;
                        bool result = v1_temp.getVector3() == v2_temp.getVector3();
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("==", typeV1, typeV2, line);
                        return null;
                    }
                }
                if (typeV1 == "Quaternion")
                {
                    if (typeV2 == "Quaternion")
                    {
                        QuaternionIntanceValue v1_temp = (QuaternionIntanceValue)v1;
                        QuaternionIntanceValue v2_temp = (QuaternionIntanceValue)v2;
                        bool result = v1_temp.getQuaternion() == v2_temp.getQuaternion();
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("==", typeV1, typeV2, line);
                        return null;
                    }
                }
                if (typeV1 == "double")
                {
                    double dv1 = doubleValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = dv1 == dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = dv1 == fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = dv1 == iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = dv1 == lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("==", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "float")
                {
                    float fv1 = floatValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = fv1 == dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = fv1 == fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = fv1 == iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = fv1 == lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("==", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = iv1 == dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = iv1 == fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = iv1 == iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = iv1 == lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = iv1 == dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("==", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = lv1 == dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = lv1 == fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = lv1 == iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = lv1 == lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = lv1 == dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("==", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "decimal")
                {
                    decimal dv1 = decimalValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = dv1 == iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = dv1 == lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = dv1 == dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("==", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "bool")
                {
                    bool bv1 = boolValueList[indexV1];
                    if (typeV2 == "bool")
                    {
                        bool bv2 = boolValueList[indexV2];
                        bool result = bv1 == bv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("==", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error("==", typeV1, typeV2, line);
                    return null;
                }
            }
            else if (node.getOp().getType() == Basic_Type.NEQUAL)
            {
                Value v1 = trackForNumberOP(node.getLeft(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                Value v2 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV2 = v2.getIndex();
                string typeV2 = v2.getType();
                setIndexFree(v1, line);
                setIndexFree(v2, line);
                if (typeV1 == "Vector3")
                {
                    if (typeV2 == "Vector3")
                    {
                        Vector3IntanceValue v1_temp = (Vector3IntanceValue)v1;
                        Vector3IntanceValue v2_temp = (Vector3IntanceValue)v2;
                        bool result = v1_temp.getVector3() != v2_temp.getVector3();
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("!=", typeV1, typeV2, line);
                        return null;
                    }
                }
                if (typeV1 == "Quaternion")
                {
                    if (typeV2 == "Quaternion")
                    {
                        QuaternionIntanceValue v1_temp = (QuaternionIntanceValue)v1;
                        QuaternionIntanceValue v2_temp = (QuaternionIntanceValue)v2;
                        bool result = v1_temp.getQuaternion() != v2_temp.getQuaternion();
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("!=", typeV1, typeV2, line);
                        return null;
                    }
                }
                if (typeV1 == "double")
                {
                    double dv1 = doubleValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = dv1 != dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = dv1 != fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = dv1 != iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = dv1 != lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("!=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "float")
                {
                    float fv1 = floatValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = fv1 != dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = fv1 != fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = fv1 != iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = fv1 != lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("!=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = iv1 != dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = iv1 != fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = iv1 != iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = iv1 != lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = iv1 != dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("!=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    if (typeV2 == "double")
                    {
                        double dv2 = doubleValueList[indexV2];
                        bool result = lv1 != dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "float")
                    {
                        float fv2 = floatValueList[indexV2];
                        bool result = lv1 != fv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = lv1 != iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = lv1 != lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = lv1 != dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("!=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "decimal")
                {
                    decimal dv1 = decimalValueList[indexV1];
                    if (typeV2 == "int")
                    {
                        int iv2 = intValueList[indexV2];
                        bool result = dv1 != iv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "long")
                    {
                        long lv2 = longValueList[indexV2];
                        bool result = dv1 != lv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else if (typeV2 == "decimal")
                    {
                        decimal dv2 = decimalValueList[indexV2];
                        bool result = dv1 != dv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("!=", typeV1, typeV2, line);
                        return null;
                    }
                }
                else if (typeV1 == "bool")
                {
                    bool bv1 = boolValueList[indexV1];
                    if (typeV2 == "bool")
                    {
                        bool bv2 = boolValueList[indexV2];
                        bool result = bv1 == bv2;
                        int rindex;
                        Value rV = init_a_new_value(out rindex, "bool", line);
                        boolValueList[rindex] = result;
                        return rV;
                    }
                    else
                    {
                        operator_error("==", typeV1, typeV2, line);
                        return null;
                    }
                }
                else
                {
                    operator_error("!=", typeV1, typeV2, line);
                    return null;
                }
            }
            else
            {
                AST_error(node);
                return null;
            }
        }

        private Value track_typeAlterOp(TypeAlterOP node, string scope, bool ifMemberVarInit, ClassInstanceValue environment)
        {
            int line = node.getLine();
            string type = node.getType().getName();
            Value v = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
            int vIndex = v.getIndex();
            string vType = v.getType();
            setIndexFree(v, line);
            if (type == "int")
            {
                int index;
                Value newValue = init_a_new_value(out index, "int", line);
                if (vType == "int")
                {
                    int iv = intValueList[vIndex];
                    intValueList[index] = iv;
                    return newValue;
                }
                else if (vType == "float")
                {
                    float fv = floatValueList[vIndex];
                    intValueList[index] = (int)fv;
                    return newValue;
                }
                else if (vType == "double")
                {
                    double dv = doubleValueList[vIndex];
                    intValueList[index] = (int)dv;
                    return newValue;
                }
                else if (vType == "long")
                {
                    long lv = longValueList[vIndex];
                    intValueList[index] = (int)lv;
                    return newValue;
                }
                else if (vType == "decimal")
                {
                    decimal dv = decimalValueList[vIndex];
                    intValueList[index] = (int)dv;
                    return newValue;
                }
                else
                {
                    throw new TypeException("cast from " + type + " to " + vType + " is not supported", line);
                }
            } 
            else if (type == "float")
            {
                int index;
                Value newValue = init_a_new_value(out index, "float", line);
                if (vType == "int")
                {
                    int iv = intValueList[vIndex];
                    floatValueList[index] = iv;
                    return newValue;
                }
                else if (vType == "float")
                {
                    float fv = floatValueList[vIndex];
                    floatValueList[index] = fv;
                    return newValue;
                }
                else if (vType == "double")
                {
                    double dv = doubleValueList[vIndex];
                    floatValueList[index] = (float)dv;
                    return newValue;
                }
                else if (vType == "long")
                {
                    long lv = longValueList[vIndex];
                    floatValueList[index] = lv;
                    return newValue;
                }
                else if (vType == "decimal")
                {
                    decimal dv = decimalValueList[vIndex];
                    floatValueList[index] = (float)dv;
                    return newValue;
                }
                else
                {
                    throw new TypeException("cast from " + type + " to " + vType + " is not supported", line);
                }
            }
            else if (type == "double")
            {
                int index;
                Value newValue = init_a_new_value(out index, "double", line);
                if (vType == "int")
                {
                    int iv = intValueList[vIndex];
                    doubleValueList[index] = iv;
                    return newValue;
                }
                else if (vType == "float")
                {
                    float fv = floatValueList[vIndex];
                    doubleValueList[index] = fv;
                    return newValue;
                }
                else if (vType == "double")
                {
                    double dv = doubleValueList[vIndex];
                    doubleValueList[index] = dv;
                    return newValue;
                }
                else if (vType == "long")
                {
                    long lv = longValueList[vIndex];
                    doubleValueList[index] = lv;
                    return newValue;
                }
                else if (vType == "decimal")
                {
                    decimal dv = decimalValueList[vIndex];
                    doubleValueList[index] = (double)dv;
                    return newValue;
                }
                else
                {
                    throw new TypeException("cast from " + type + " to " + vType + " is not supported", line);
                }
            }
            else if (type == "long")
            {
                int index;
                Value newValue = init_a_new_value(out index, "long", line);
                if (vType == "int")
                {
                    int iv = intValueList[vIndex];
                    longValueList[index] = iv;
                    return newValue;
                }
                else if (vType == "float")
                {
                    float fv = floatValueList[vIndex];
                    longValueList[index] = (long)fv;
                    return newValue;
                }
                else if (vType == "double")
                {
                    double dv = doubleValueList[vIndex];
                    longValueList[index] = (long)dv;
                    return newValue;
                }
                else if (vType == "long")
                {
                    long lv = longValueList[vIndex];
                    longValueList[index] = lv;
                    return newValue;
                }
                else if (vType == "decimal")
                {
                    decimal dv = decimalValueList[vIndex];
                    longValueList[index] = (long)dv;
                    return newValue;
                }
                else
                {
                    throw new TypeException("cast from " + type + " to " + vType + " is not supported", line);
                }
            }
            else if (type == "decimal")
            {
                int index;
                Value newValue = init_a_new_value(out index, "decimal", line);
                if (vType == "int")
                {
                    int iv = intValueList[vIndex];
                    decimalValueList[index] = iv;
                    return newValue;
                }
                else if (vType == "float")
                {
                    float fv = floatValueList[vIndex];
                    decimalValueList[index] = (decimal)fv;
                    return newValue;
                }
                else if (vType == "double")
                {
                    double dv = doubleValueList[vIndex];
                    decimalValueList[index] = (decimal)dv;
                    return newValue;
                }
                else if (vType == "long")
                {
                    long lv = longValueList[vIndex];
                    decimalValueList[index] = lv;
                    return newValue;
                }
                else if (vType == "decimal")
                {
                    decimal dv = decimalValueList[vIndex];
                    decimalValueList[index] = dv;
                    return newValue;
                }
                else
                {
                    throw new TypeException("cast from " + type + " to " + vType + " is not supported", line);
                }
            }
            else
            {
                throw new TypeException("cast from " + type + " to " + vType + " is not supported", line);
            }
        }

        private void track_UopWithAssign(UnaryOP node, string scope, ClassInstanceValue environment)
        {
            int line = node.getLine();
            if (node.getOP().getType() == Basic_Type.PLUSPLUS)
            {
                Value v = trackForNumberOP(node.getRight(), scope, false, environment);
                int index = v.getIndex();
                string type = v.getType();
                if (type == "double")
                {
                    double dv = doubleValueList[index];
                    doubleValueList[index] = dv+1;
                }
                else if (type == "float")
                {
                    float fv = floatValueList[index];
                    floatValueList[index] = fv+1;
                }
                else if (type == "int")
                {
                    int iv = intValueList[index];
                    intValueList[index] = iv+1;
                }
                else if (type == "long")
                {
                    long lv = longValueList[index];
                    longValueList[index] = lv+1;
                }
                else if (type == "decimal")
                {
                    decimal dv = decimalValueList[index];
                    decimalValueList[index] = dv+1;
                }
                else
                {
                    throw new TypeException("unexpected type " + type, line);
                }
            }
            else if (node.getOP().getType() == Basic_Type.MINUSMINUS)
            {
                Value v = trackForNumberOP(node.getRight(), scope, false, environment);
                int index = v.getIndex();
                string type = v.getType();
                if (type == "double")
                {
                    double dv = doubleValueList[index];
                    doubleValueList[index] = dv-1;
                }
                else if (type == "float")
                {
                    float fv = floatValueList[index];
                    floatValueList[index] = fv-1;
                }
                else if (type == "int")
                {
                    int iv = intValueList[index];
                    intValueList[index] = iv-1;
                }
                else if (type == "long")
                {
                    long lv = longValueList[index];
                    longValueList[index] = lv-1;
                }
                else if (type == "decimal")
                {
                    decimal dv = decimalValueList[index];
                    decimalValueList[index] = dv-1;
                }
                else
                {
                    throw new TypeException("unexpected type " + type, line);
                }
            }
            else
            {
                AST_error(node);
            }
        }

        private Value track_Uop(UnaryOP node, string scope, bool ifMemberVarInit, ClassInstanceValue environment)
        {
            int line = node.getLine();
            if (node.getOP().getType() == Basic_Type.PLUS)
            {
                return trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
            }
            else if (node.getOP().getType() == Basic_Type.MINUS)
            {
                Value v1 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                setIndexFree(v1, line);
                if (typeV1 == "double")
                {
                    double dv1 = doubleValueList[indexV1];
                    double result = -dv1;
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "double", line);
                    doubleValueList[rindex] = result;
                    return rV;
                }
                else if (typeV1 == "float")
                {
                    float fv1 = floatValueList[indexV1];
                    float result = -fv1;
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "float", line);
                    floatValueList[rindex] = result;
                    return rV;
                }
                else if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    int result = -iv1;
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "int", line);
                    intValueList[rindex] = result;
                    return rV;
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    long result = -lv1;
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "long", line);
                    longValueList[rindex] = result;
                    return rV;
                }
                else if (typeV1 == "decimal")
                {
                    decimal dv1 = decimalValueList[indexV1];
                    decimal result = -dv1;
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "decimal", line);
                    decimalValueList[rindex] = result;
                    return rV;
                }
                else
                {
                    throw new TypeException("unexpected type " + typeV1, line);
                }
            }
            else if (node.getOP().getType() == Basic_Type.NOT)
            {
                Value v1 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                setIndexFree(v1, line);
                if (typeV1 == "bool")
                {
                    bool bv1 = boolValueList[indexV1];
                    bool result = !bv1;
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "bool", line);
                    boolValueList[rindex] = result;
                    return rV;
                }
                else
                {
                    throw new TypeException("unexpected type " + typeV1, line);
                }
            }
            else if (node.getOP().getType() == Basic_Type.BITNOT)
            {
                Value v1 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                setIndexFree(v1, line);
                if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    int result = ~iv1;
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "int", line);
                    intValueList[rindex] = result;
                    return rV;
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    long result = ~lv1;
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "long", line);
                    longValueList[rindex] = result;
                    return rV;
                }
                else
                {
                    throw new TypeException("unexpected type " + typeV1, line);
                }
            }
            else if (node.getOP().getType() == Basic_Type.PLUSPLUS)
            {
                Value v1 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                if (v1.getIfCanFree())
                {
                    throw new OperatorException("++ can only be used with variables", line);
                }
                if (typeV1 == "double")
                {
                    double dv1 = doubleValueList[indexV1];
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "double", line);
                    if (node.getRightOrLeft())
                    {
                        doubleValueList[rindex] = dv1;
                    }
                    else
                    {
                        doubleValueList[rindex] = dv1 + 1;
                    }
                    doubleValueList[indexV1] = dv1 + 1;
                    return rV;
                }
                else if (typeV1 == "float")
                {
                    float fv1 = floatValueList[indexV1];
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "float", line);
                    if (node.getRightOrLeft())
                    {
                        floatValueList[rindex] = fv1;
                    }
                    else
                    {
                        floatValueList[rindex] = fv1 + 1;
                    }
                    floatValueList[indexV1] = fv1 + 1;
                    return rV;
                }
                else if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "int", line);
                    if (node.getRightOrLeft())
                    {
                        intValueList[rindex] = iv1;
                    }
                    else
                    {
                        intValueList[rindex] = iv1 + 1;
                    }
                    intValueList[indexV1] = iv1 + 1;
                    return rV;
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "long", line);
                    if (node.getRightOrLeft())
                    {
                        longValueList[rindex] = lv1;
                    }
                    else
                    {
                        longValueList[rindex] = lv1 + 1;
                    }
                    longValueList[indexV1] = lv1 + 1;
                    return rV;
                }
                else if (typeV1 == "decimal")
                {
                    decimal dv1 = decimalValueList[indexV1];
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "decimal", line);
                    if (node.getRightOrLeft())
                    {
                        decimalValueList[rindex] = dv1;
                    }
                    else
                    {
                        decimalValueList[rindex] = dv1 + 1;
                    }
                    decimalValueList[indexV1] = dv1 + 1;
                    return rV;
                }
                else
                {
                    throw new TypeException("unexpected type " + typeV1, line);
                }
            }
            else if (node.getOP().getType() == Basic_Type.MINUSMINUS)
            {
                Value v1 = trackForNumberOP(node.getRight(), scope, ifMemberVarInit, environment);
                int indexV1 = v1.getIndex();
                string typeV1 = v1.getType();
                if (v1.getIfCanFree())
                {
                    throw new OperatorException("-- can only be used with variables", line);
                }
                if (typeV1 == "double")
                {
                    double dv1 = doubleValueList[indexV1];
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "double", line);
                    if (node.getRightOrLeft())
                    {
                        doubleValueList[rindex] = dv1;
                    }
                    else
                    {
                        doubleValueList[rindex] = dv1 - 1;
                    }
                    doubleValueList[indexV1] = dv1 - 1;
                    return rV;
                }
                else if (typeV1 == "float")
                {
                    float fv1 = floatValueList[indexV1];
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "float", line);
                    if (node.getRightOrLeft())
                    {
                        floatValueList[rindex] = fv1;
                    }
                    else
                    {
                        floatValueList[rindex] = fv1 - 1;
                    }
                    floatValueList[indexV1] = fv1 - 1;
                    return rV;
                }
                else if (typeV1 == "int")
                {
                    int iv1 = intValueList[indexV1];
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "int", line);
                    if (node.getRightOrLeft())
                    {
                        intValueList[rindex] = iv1;
                    }
                    else
                    {
                        intValueList[rindex] = iv1 - 1;
                    }
                    intValueList[indexV1] = iv1 - 1;
                    return rV;
                }
                else if (typeV1 == "long")
                {
                    long lv1 = longValueList[indexV1];
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "long", line);
                    if (node.getRightOrLeft())
                    {
                        longValueList[rindex] = lv1;
                    }
                    else
                    {
                        longValueList[rindex] = lv1 - 1;
                    }
                    longValueList[indexV1] = lv1 - 1;
                    return rV;
                }
                else if (typeV1 == "decimal")
                {
                    decimal dv1 = decimalValueList[indexV1];
                    int rindex;
                    Value rV = init_a_new_value(out rindex, "decimal", line);
                    if (node.getRightOrLeft())
                    {
                        decimalValueList[rindex] = dv1;
                    }
                    else
                    {
                        decimalValueList[rindex] = dv1 - 1;
                    }
                    decimalValueList[indexV1] = dv1 - 1;
                    return rV;
                }
                else
                {
                    throw new TypeException("unexpected type " + typeV1, line);
                }
            }
            //else if (node.getOP().getType() == Basic_Type.SQRT)
            //{
            //    Type t = typeof(Math);
            //    MethodInfo sq = t.GetMethod("Sqrt");
            //    //return Math.Sqrt(double_trackForNumberOP(node.getRight(), scope));
            //    object[] par = new object[] { double_trackForNumberOP(node.getRight(), scope) };
            //    return (double)sq.Invoke(null, par);
            //}
            else
            {
                AST_error(node);
                return null;
            }
        }

        private Value track_num(Num node)
        {
            int line = node.getLine();
            if (node.getNumType() == Basic_Type.DOUBLE)
            {
                int rindex;
                Value rV = init_a_new_value(out rindex, "double", line);
                doubleValueList[rindex] = Convert.ToDouble(node.getValueStr());
                return rV;
            }
            else if (node.getNumType() == Basic_Type.FLOAT)
            {
                int rindex;
                Value rV = init_a_new_value(out rindex, "float", line);
                floatValueList[rindex] = Convert.ToSingle(node.getValueStr());
                return rV;
            }
            else if (node.getNumType() == Basic_Type.INT)
            {
                int rindex;
                Value rV = init_a_new_value(out rindex, "int", line);
                intValueList[rindex] = Convert.ToInt32(node.getValueStr());
                return rV;
            }
            else if (node.getNumType() == Basic_Type.BOOL)
            {
                int rindex;
                Value rV = init_a_new_value(out rindex, "bool", line);
                boolValueList[rindex] = Convert.ToBoolean(node.getValueStr());
                return rV;
            }
            else if (node.getNumType() == Basic_Type.DECIMAL)
            {
                int rindex;
                Value rV = init_a_new_value(out rindex, "decimal", line);
                decimalValueList[rindex] = Convert.ToDecimal(node.getValueStr());
                return rV;
            }
            else if (node.getNumType() == Basic_Type.LONG)
            {
                int rindex;
                Value rV = init_a_new_value(out rindex, "long", line);
                longValueList[rindex] = Convert.ToInt64(node.getValueStr());
                return rV;
            }
            else
            {
                Type_error(node.getNumType(), node.getLine());
                return null;
            }
        }

        private Value track_buildInInitMethodCall(BuildInInitMethodCall node, string callScope, ClassInstanceValue callEnv)
        {
            int retValueStack_count = retValueStack.Count;
            string className = node.getInitMethod().getName();
            if (className == "Vector3")
            {
                List<float> float_param = new List<float>();
                List<ParamGiven> paramGivens = node.getParamsGiven();
                foreach (ParamGiven p in paramGivens)
                {
                    Value pv = trackForNumberOP(p.getNode(), callScope, false, callEnv);
                    string pType = pv.getType();
                    int pIndex = pv.getIndex();
                    setIndexFree(pv, node.getLine());
                    if (pType == "float")
                    {
                        float pV = floatValueList[pIndex];
                        float_param.Add(pV);
                    }
                    else if (pType == "int")
                    {
                        int pV = intValueList[pIndex];
                        float_param.Add(pV);

                    }
                    else if (pType == "long")
                    {
                        long pV = longValueList[pIndex];
                        float_param.Add(pV);

                    }
                    else
                    {
                        throw new TypeException("unexpected type " + pType, node.getLine());
                    }
                }
                if (float_param.Count == 3)
                {
                    return new Vector3IntanceValue(new Vector3(float_param[0], float_param[1], float_param[2]));
                }
                else if (float_param.Count == 2)
                {
                    return new Vector3IntanceValue(new Vector3(float_param[0], float_param[1], 0));
                }
                else if (float_param.Count == 0)
                {
                    return new Vector3IntanceValue(new Vector3());
                }
                else
                {
                    throw new MethodException("mismatched paramters number for init method Vector3", node.getLine());
                }
            }
            else if (className == "Quaternion")
            {
                List<float> float_param = new List<float>();
                List<ParamGiven> paramGivens = node.getParamsGiven();
                foreach (ParamGiven p in paramGivens)
                {
                    Value pv = trackForNumberOP(p.getNode(), callScope, false, callEnv);
                    string pType = pv.getType();
                    int pIndex = pv.getIndex();
                    setIndexFree(pv, node.getLine());
                    if (pType == "float")
                    {
                        float pV = floatValueList[pIndex];
                        float_param.Add(pV);
                    }
                    else if (pType == "int")
                    {
                        int pV = intValueList[pIndex];
                        float_param.Add(pV);

                    }
                    else if (pType == "long")
                    {
                        long pV = longValueList[pIndex];
                        float_param.Add(pV);

                    }
                    else
                    {
                        throw new TypeException("unexpected type " + pType, node.getLine());
                    }
                }
                if (float_param.Count == 4)
                {
                    return new QuaternionIntanceValue(new Quaternion(float_param[0], float_param[1], float_param[2], float_param[3]));
                }
                else if (float_param.Count == 0)
                {
                    return new QuaternionIntanceValue(new Quaternion());
                }
                else
                {
                    throw new MethodException("mismatched paramters number for init method Quaternion", node.getLine());
                }
                    
            }
            else
            {
                throw new ClassException("unexpected class name " + className, node.getLine());
            }
        }

        private Value track_initMethodCall(InitMethodCall node, string callScope, ClassInstanceValue callEnv)
        {
            int retValueStack_count = retValueStack.Count;
            string className = node.getInitMethod().getName();
            string methodName = className + "*init";
            string retType;
            AST block;
            bool access;
            List<string> methodParams = symbolTables[className].getParamsRetTypeAndBlockNode(methodName, out retType, out block, out access, node.getLine());
            if (!access)
            {
                throw new MethodException("unaccessable member method " + methodName, node.getLine());
            }
            SymbolTable methodSymbolTable = symbolTables[methodName];
            List<ParamGiven> pGivenList = node.getParamsGiven();
            if (methodParams.Count != pGivenList.Count)
            {
                throw new MethodException("mismatched paramters number for init method " + methodName, node.getLine());
            }
            pass_parameters(methodSymbolTable, methodParams, pGivenList, callScope, callEnv);
            ClassInstanceValue newEnv = new ClassInstanceValue(className, symbolTables);
            List<MemberVariableDec> decNodeList = memberVarDic[className];
            foreach (MemberVariableDec the_node in decNodeList)
            {
                if (the_node.getAssign() != null)
                {
                    track_assign(the_node.getAssign(), className, newEnv, true);
                }
            }
            track_method_block((Compound)block, methodName, newEnv);
            foreach (string pName in methodParams)
            {
                Value pV = symbolTables[methodName].popParamSymbleValue(pName, node.getLine());
                pV.setCanFree();
                setIndexFree(pV, node.getLine());
            }
            List<Value> varList = symbolTables[methodName].popOwnVarSymbolValue();
            foreach (Value var in varList)
            {
                if (var != null)
                {
                    var.setCanFree();
                    setIndexFree(var, node.getLine());
                }
            }
            if (retValueStack.Count - retValueStack_count == 1)
            {
                throw new MethodException("unexpected return value for this method", node.getLine());
            }
            ifHaveReturn = false;
            return newEnv;
        }

        private Value track_methodCall(MethodCall node, string callScope, ClassInstanceValue environment, ClassInstanceValue callEnv, bool ifClassCall)
        {
            int retValueStack_count = retValueStack.Count;
            string methodName = environment.getType() + "*" + node.getMethodName();
            string retType;
            AST block;
            bool access;
            List<string> methodParams = symbolTables[environment.getType()].getParamsRetTypeAndBlockNode(methodName, out retType, out block, out access, node.getLine());
            if (ifClassCall && !access)
            {
                throw new MethodException("unaccessable member method " + methodName, node.getLine());
            }
            SymbolTable methodSymbolTable = symbolTables[methodName];
            List<ParamGiven> pGivenList = node.getParamsGiven();
            if (methodParams.Count != pGivenList.Count)
            {
                throw new MethodException("mismatched paramters number for method " + methodName, node.getLine());
            }
            pass_parameters(methodSymbolTable, methodParams, pGivenList, callScope, callEnv);
            symbolTables[methodName].pushOwnVarSymbolValue();
            track_method_block((Compound)block, methodName, environment);
            foreach (string pName in methodParams)
            {
                Value pV = symbolTables[methodName].popParamSymbleValue(pName, node.getLine());
                pV.setCanFree();
                setIndexFree(pV, node.getLine());
            }
            List<Value> varList = symbolTables[methodName].popOwnVarSymbolValue();
            foreach (Value var in varList)
            {
                if (var != null)
                {
                    var.setCanFree();
                    setIndexFree(var, node.getLine());
                }
            }
            if (retValueStack.Count - retValueStack_count == 1)
            {
                if (retType == "void")
                {
                    throw new MethodException("unexpected return value for this method", node.getLine());
                }
                ifHaveReturn = false;
                return retValueStack.Pop();
            }
            else if (retType != "void")
            {
                throw new MethodException("Return value required for this method", node.getLine());
            }
            else
            {
                ifHaveReturn = false;
                return null;
            }
        }

        private Value track_var(VarMethodOrClass node, string scope, ClassInstanceValue environment)
        {
            SymbolTable st = symbolTables[scope];
            string var_name = node.getName();
            if (var_name == "gameObject")
            {
                if (theGameObject == null)
                {
                    throw new VarException("Unspecified GameObject", node.getLine());
                }
                if (theGameObject.ifNull())
                {
                    throw new VarException("Unspecified GameObject", node.getLine());
                }
                return theGameObject;
            }
            else if (var_name == "transform")
            {
                if (theTransform == null)
                {
                    throw new VarException("Unspecified Transform", node.getLine());
                }
                if (theTransform.ifNull())
                {
                    throw new VarException("Unspecified Transform", node.getLine());
                }
                return theTransform;
            }
            else if (theGameObjects.ContainsKey(var_name))
            {
                if (theGameObjects[var_name].ifNull())
                {
                    throw new VarException("Unspecified GameObject", node.getLine());
                }
                return theGameObjects[var_name];
            }
            bool defineFlag;
            string type;
            bool access;
            Value value = st.getVarOrParamSymbleValueDefineFlagAndDefineType(var_name, out type, out defineFlag, out access, node.getLine(), environment);
            if (!defineFlag)
            {
                throw new VarException("undefined variable name " + var_name, node.getLine());
            }
            if (value == null)
            {
                throw new VarException("unassigned variable name " + var_name, node.getLine());
            }
            return value;
        }

        private void track_ret(RET node, string scope, ClassInstanceValue environment)
        {
            AST expr = node.getExpr();
            string retType = symbolTables[scope].getRetType(node.getMethodName(), node.getLine());
            Value result = trackForNumberOP(expr, scope, false, environment);
            retValueStack.Push(result);
            ifHaveReturn = true;
        }

        private void track_compound(Compound root, string scope, ClassInstanceValue environment, string CirScope)
        {
            if (CirScope != null)
            {
                foreach (AST node in root.getList())
                {
                    track_in_method(node, scope, environment, CirScope);
                    if (scope_break_flag.ContainsKey(CirScope))
                    {
                        if (scope_break_flag[CirScope])
                        {
                            return;
                        }
                    }
                    if (scope_continue_flag.ContainsKey(CirScope))
                    {
                        if (scope_continue_flag[CirScope])
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                foreach (AST node in root.getList())
                {
                    track_in_method(node, scope, environment, null);
                }
            }            
        }

        private void track_method_block(Compound root, string scope, ClassInstanceValue environment)
        {
            foreach (AST node in root.getList())
            {
                track_in_method(node, scope, environment, null);
                if (ifHaveReturn)
                {
                    break;
                }
            }
            ifHaveReturn = true;
        }

        private void track_assign(Assign node, string scope, ClassInstanceValue environment, bool ifMemberVar)
        {
            SymbolTable st = symbolTables[scope];
            if (node.getLeft().getAST_Type() == AST_Type.VARMETHODORCLASS)
            {
                VarMethodOrClass varNode = (VarMethodOrClass)node.getLeft();
                string var_name = varNode.getName();
                bool defineFlag;
                string type_define;
                bool access;
                Value v = st.getVarOrParamSymbleValueDefineFlagAndDefineType(var_name, out type_define, out defineFlag, out access, node.getLine(), environment);
                Value assignV = trackForNumberOP(node.getRight(), scope, ifMemberVar, environment);
                string aType = assignV.getType();
                int aIndex = assignV.getIndex();
                setIndexFree(assignV, node.getLine());
                if (v == null)
                {
                    int i_temp;
                    v = init_a_new_value(out i_temp, type_define, node.getLine());
                    if (v != null)
                    {
                        v.setCanNotFree();
                        st.setVarOrParamSymbolValue(v, var_name, node.getLine(), environment);
                    }
                }
                if (type_define != "int" && type_define != "long" && type_define != "float" && type_define != "double" && type_define != "decimal")
                {
                    if (type_define != assignV.getType())
                    {
                        throw new TypeException("unexpected type " + assignV.getType(), node.getLine());
                    }
                    if (type_define == "Vector3")
                    {
                        Vector3IntanceValue av3 = (Vector3IntanceValue)assignV;
                        Vector3IntanceValue v3 = new Vector3IntanceValue(av3.getVector3());
                        st.setVarOrParamSymbolValue(v3, var_name, node.getLine(), environment);
                        return;
                    }
                    else if (type_define == "Quaternion")
                    {
                        QuaternionIntanceValue avq = (QuaternionIntanceValue)assignV;
                        QuaternionIntanceValue vq = new QuaternionIntanceValue(avq.getQuaternion());
                        st.setVarOrParamSymbolValue(vq, var_name, node.getLine(), environment);
                        return;
                    }
                    else
                    {
                        st.setVarOrParamSymbolValue(assignV, var_name, node.getLine(), environment);
                        return;
                    }
                }
                string vType = v.getType();
                int vIndex = v.getIndex();
                if (!defineFlag)
                {
                    throw new VarException("undefined variable name " + var_name, node.getLine());
                }
                if (vType == "double")
                {
                    if (aType == "double")
                    {
                        double aV = doubleValueList[aIndex];
                        doubleValueList[vIndex] = aV;
                    }
                    else if (aType == "float")
                    {
                        float aV = floatValueList[aIndex];
                        doubleValueList[vIndex] = aV;
                    }
                    else if (aType == "int")
                    {
                        int aV = intValueList[aIndex];
                        doubleValueList[vIndex] = aV;
                    }
                    else if (aType == "long")
                    {
                        long aV = longValueList[aIndex];
                        doubleValueList[vIndex] = aV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + aType + " value to " + vType + " value", node.getLine());
                    }
                }
                else if (vType == "float")
                {
                    if (aType == "float")
                    {
                        float aV = floatValueList[aIndex];
                        floatValueList[vIndex] = aV;
                    }
                    else if (aType == "int")
                    {
                        int aV = intValueList[aIndex];
                        floatValueList[vIndex] = aV;
                    }
                    else if (aType == "long")
                    {
                        long aV = longValueList[aIndex];
                        floatValueList[vIndex] = aV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + aType + " value to " + vType + " value", node.getLine());
                    }
                }
                else if (vType == "int")
                {
                    if (aType == "int")
                    {
                        int aV = intValueList[aIndex];
                        intValueList[vIndex] = aV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + aType + " value to " + vType + " value", node.getLine());
                    }
                }
                else if (vType == "long")
                {
                    if (aType == "int")
                    {
                        int aV = intValueList[aIndex];
                        longValueList[vIndex] = aV;
                    }
                    else if (aType == "long")
                    {
                        long aV = longValueList[aIndex];
                        longValueList[vIndex] = aV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + aType + " value to " + vType + " value", node.getLine());
                    }
                }
                else if (vType == "bool")
                {
                    if (aType == "bool")
                    {
                        bool aV = boolValueList[aIndex];
                        boolValueList[vIndex] = aV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + aType + " value to " + vType + " value", node.getLine());
                    }
                }
                else if (vType == "decimal")
                {
                    if (aType == "decimal")
                    {
                        decimal aV = decimalValueList[aIndex];
                        decimalValueList[vIndex] = aV;
                    }
                    else if (aType == "int")
                    {
                        int aV = intValueList[aIndex];
                        decimalValueList[vIndex] = aV;
                    }
                    else if (aType == "long")
                    {
                        long aV = longValueList[aIndex];
                        decimalValueList[vIndex] = aV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + aType + " value to " + vType + " value", node.getLine());
                    }
                }
                else
                {
                    throw new TypeException("can't convert " + aType + " value to " + vType + " value", node.getLine());
                }
            }
            else if (node.getLeft().getAST_Type() == AST_Type.CLASS_CALL && !ifMemberVar)
            {
                ClassCall classCallNode = (ClassCall)node.getLeft();
                if (classCallNode.IfMethod())
                {
                    throw new AssignException("Method cannot be placed to the left of assignment", node.getLine());
                }
                Value last_env;
                string var_name;
                string type_name;
                Value v = track_classCall(classCallNode, scope, environment, environment, out last_env, out var_name, out type_name, scope, true);
                Value assignV = trackForNumberOP(node.getRight(), scope, ifMemberVar, environment);
                string aType = assignV.getType();
                int aIndex = assignV.getIndex();
                setIndexFree(assignV, node.getLine());
                if (last_env.getType() == "Transform")
                {
                    TransformInstanceValue v_temp = (TransformInstanceValue)last_env;
                    if (var_name == "position" && type_name == "Vector3" && assignV.getType() == "Vector3")
                    {
                        Vector3IntanceValue v3iv = (Vector3IntanceValue)assignV;
                        Vector3 v3 = v3iv.getVector3();
                        if (float.IsNaN(v3.x) || float.IsNaN(v3.y) || float.IsNaN(v3.z))
                        {
                            throw new VarException("Input " + var_name + " is " + v3, node.getLine());
                        }
                        v_temp.getTransform().position = v3;
                        return;
                    }
                    else if (var_name == "rotation" && type_name == "Quaternion" && assignV.getType() == "Quaternion")
                    {
                        QuaternionIntanceValue qv = (QuaternionIntanceValue)assignV;
                        Quaternion q = qv.getQuaternion();
                        if (float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w))
                        {
                            throw new VarException("Input " + var_name + " is " + q, node.getLine());
                        }
                        v_temp.getTransform().rotation = q;
                        return;
                    }
                    else if (var_name == "eulerAngles" && type_name == "Vector3" && assignV.getType() == "Vector3")
                    {
                        Vector3IntanceValue v3iv = (Vector3IntanceValue)assignV;
                        Vector3 v3 = v3iv.getVector3();
                        if (float.IsNaN(v3.x) || float.IsNaN(v3.y) || float.IsNaN(v3.z))
                        {
                            throw new VarException("Input " + var_name + " is " + v3, node.getLine());
                        }
                        v_temp.getTransform().eulerAngles = v3;
                    }
                    else if (var_name == "localScale" && type_name == "Vector3" && assignV.getType() == "Vector3")
                    {
                        Vector3IntanceValue v3iv = (Vector3IntanceValue)assignV;
                        Vector3 v3 = v3iv.getVector3();
                        if (float.IsNaN(v3.x) || float.IsNaN(v3.y) || float.IsNaN(v3.z))
                        {
                            throw new VarException("Input " + var_name + " is " + v3, node.getLine());
                        }
                        v_temp.getTransform().localScale = v3;
                    }
                    else if (var_name == "forward" && type_name == "Vector3" && assignV.getType() == "Vector3")
                    {
                        Vector3IntanceValue v3iv = (Vector3IntanceValue)assignV;
                        Vector3 v3 = v3iv.getVector3();
                        if (float.IsNaN(v3.x) || float.IsNaN(v3.y) || float.IsNaN(v3.z))
                        {
                            throw new VarException("Input " + var_name + " is " + v3, node.getLine());
                        }
                        v_temp.getTransform().forward = v3;
                    }
                    else if (var_name == "right" && type_name == "Vector3" && assignV.getType() == "Vector3")
                    {
                        Vector3IntanceValue v3iv = (Vector3IntanceValue)assignV;
                        Vector3 v3 = v3iv.getVector3();
                        if (float.IsNaN(v3.x) || float.IsNaN(v3.y) || float.IsNaN(v3.z))
                        {
                            throw new VarException("Input " + var_name + " is " + v3, node.getLine());
                        }
                        v_temp.getTransform().right = v3;
                    }
                    else if (var_name == "up" && type_name == "Vector3" && assignV.getType() == "Vector3")
                    {
                        Vector3IntanceValue v3iv = (Vector3IntanceValue)assignV;
                        Vector3 v3 = v3iv.getVector3();
                        if (float.IsNaN(v3.x) || float.IsNaN(v3.y) || float.IsNaN(v3.z))
                        {
                            throw new VarException("Input " + var_name + " is " + v3, node.getLine());
                        }
                        v_temp.getTransform().up = v3;
                    }
                    else
                    {
                        throw new AssignException("Invalid statement ", node.getLine());
                    }
                }
                else if (last_env.getType() == "Vector3")
                {
                    Vector3IntanceValue v_temp = (Vector3IntanceValue)last_env;
                    float aV;
                    Vector3 temp = v_temp.getVector3();
                    if (aType == "float")
                    {
                        aV = floatValueList[aIndex];
                    }
                    else if (aType == "int")
                    {
                        aV = intValueList[aIndex];
                    }
                    else if (aType == "long")
                    {
                        aV = longValueList[aIndex];
                    }
                    else
                    {
                        throw new TypeException("can't convert " + aType + "value to float value", node.getLine());
                    }
                    if (var_name == "x")
                    {
                        temp.Set(aV, temp.y, temp.z);
                        v_temp.setVector3(temp);
                    }
                    else if (var_name == "y")
                    {
                        temp.Set(temp.x, aV, temp.z);
                        v_temp.setVector3(temp);
                    }
                    else if (var_name == "z")
                    {
                        temp.Set(temp.x, temp.y, aV);
                        v_temp.setVector3(temp);
                    }
                    else
                    {
                        throw new VarException("unsupported member " + var_name, node.getLine());
                    }
                    return;
                }
                else if (last_env.getType() == "Quaternion")
                {
                    QuaternionIntanceValue v_temp = (QuaternionIntanceValue)last_env;
                    float aV;
                    Quaternion temp = v_temp.getQuaternion();
                    if (aType == "float")
                    {
                        aV = floatValueList[aIndex];
                    }
                    else if (aType == "int")
                    {
                        aV = intValueList[aIndex];
                    }
                    else if (aType == "long")
                    {
                        aV = longValueList[aIndex];
                    }
                    else
                    {
                        throw new TypeException("can't convert " + aType + "value to float value", node.getLine());
                    }
                    if (var_name == "x")
                    {
                        temp.Set(aV, temp.y, temp.z, temp.w);
                        v_temp.setQuaternion(temp);
                    }
                    else if (var_name == "y")
                    {
                        temp.Set(temp.x, aV, temp.z, temp.w);
                        v_temp.setQuaternion(temp);
                    }
                    else if (var_name == "z")
                    {
                        temp.Set(temp.x, temp.y, aV, temp.w);
                        v_temp.setQuaternion(temp);
                    }
                    else if (var_name == "w")
                    {
                        temp.Set(temp.x, temp.y, temp.z, aV);
                        v_temp.setQuaternion(temp);
                    }
                    else
                    {
                        throw new VarException("unsupported member " + var_name, node.getLine());
                    }
                    return;
                }
                ClassInstanceValue last_env_instanceValue = (ClassInstanceValue)last_env;
                if (v == null)
                {
                    int i_temp;
                    v = init_a_new_value(out i_temp, type_name, node.getLine());
                    if (v != null)
                    {
                        v.setCanNotFree();
                        last_env_instanceValue.setMemberValue(var_name, v, node.getLine());
                    }
                }
                if (type_name != "int" && type_name != "long" && type_name != "float" && type_name != "double" && type_name != "decimal")
                {
                    if (type_name != assignV.getType())
                    {
                        throw new TypeException("unexpected type " + assignV.getType(), node.getLine());
                    }
                    if (type_name == "Vector3")
                    {
                        Vector3IntanceValue av3 = (Vector3IntanceValue)assignV;
                        Vector3IntanceValue v3 = new Vector3IntanceValue(av3.getVector3());
                        last_env_instanceValue.setMemberValue(var_name, v3, node.getLine());
                        return;
                    }
                    else if (type_name == "Quaternion")
                    {
                        QuaternionIntanceValue avq = (QuaternionIntanceValue)assignV;
                        QuaternionIntanceValue vq = new QuaternionIntanceValue(avq.getQuaternion());
                        last_env_instanceValue.setMemberValue(var_name, vq, node.getLine());
                        return;
                    }
                    else
                    {
                        last_env_instanceValue.setMemberValue(var_name, assignV, node.getLine());
                        return;
                    }
                }
                string vType = v.getType();
                int vIndex = v.getIndex();
                if (vType == "double")
                {
                    if (aType == "double")
                    {
                        double aV = doubleValueList[aIndex];
                        doubleValueList[vIndex] = aV;
                    }
                    else if (aType == "float")
                    {
                        float aV = floatValueList[aIndex];
                        doubleValueList[vIndex] = aV;
                    }
                    else if (aType == "int")
                    {
                        int aV = intValueList[aIndex];
                        doubleValueList[vIndex] = aV;
                    }
                    else if (aType == "long")
                    {
                        long aV = longValueList[aIndex];
                        doubleValueList[vIndex] = aV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + aType + " value to " + vType + " value", node.getLine());
                    }
                }
                else if (vType == "float")
                {
                    if (aType == "float")
                    {
                        float aV = floatValueList[aIndex];
                        floatValueList[vIndex] = aV;
                    }
                    else if (aType == "int")
                    {
                        int aV = intValueList[aIndex];
                        floatValueList[vIndex] = aV;
                    }
                    else if (aType == "long")
                    {
                        long aV = longValueList[aIndex];
                        floatValueList[vIndex] = aV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + aType + " value to " + vType + " value", node.getLine());
                    }
                }
                else if (vType == "int")
                {
                    if (aType == "int")
                    {
                        int aV = intValueList[aIndex];
                        intValueList[vIndex] = aV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + aType + " value to " + vType + " value", node.getLine());
                    }
                }
                else if (vType == "long")
                {
                    if (aType == "int")
                    {
                        int aV = intValueList[aIndex];
                        longValueList[vIndex] = aV;
                    }
                    else if (aType == "long")
                    {
                        long aV = longValueList[aIndex];
                        longValueList[vIndex] = aV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + aType + " value to " + vType + " value", node.getLine());
                    }
                }
                else if (vType == "bool")
                {
                    if (aType == "bool")
                    {
                        bool aV = boolValueList[aIndex];
                        boolValueList[vIndex] = aV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + aType + " value to " + vType + " value", node.getLine());
                    }
                }
                else if (vType == "decimal")
                {
                    if (aType == "decimal")
                    {
                        decimal aV = decimalValueList[aIndex];
                        decimalValueList[vIndex] = aV;
                    }
                    else if (aType == "int")
                    {
                        int aV = intValueList[aIndex];
                        decimalValueList[vIndex] = aV;
                    }
                    else if (aType == "long")
                    {
                        long aV = longValueList[aIndex];
                        decimalValueList[vIndex] = aV;
                    }
                    else
                    {
                        throw new TypeException("can't convert " + aType + " value to " + vType + " value", node.getLine());
                    }
                }
                else
                {
                    throw new TypeException("can't convert " + aType + " value to " + vType + " value", node.getLine());
                }
            }
            else if (node.getLeft().getAST_Type() == AST_Type.CLASS_CALL && ifMemberVar)
            {
                AST_error(node);
            }
        }

        private void track_dec(VariableDec node, string scope, ClassInstanceValue environment)
        {
            string varName = node.getVar().getName();
            symbolTables[scope].setVarSymbolDefineFlag(varName, node.getLine());
            if (node.haveAssignment())
            {
                track_assign(node.getAssign(), scope, environment, false);
            }
        }

        private void track_empty()
        {

        }

        private void prepare()
        {
            SymbolTable st = symbolTables[global];
            List<AST> node_start_list = new List<AST>();
            List<AST> node_update_list = new List<AST>();

            string VarName = theCodeName + suffix_for_excute_instance;
            VarSymbol varSymbol = new VarSymbol(VarName, st.getTypeSymbol(theCodeName, 0));
            ClassInstanceValue value = new ClassInstanceValue(theCodeName, symbolTables);

            List<MemberVariableDec> decNodeList = memberVarDic[theCodeName];
            foreach (MemberVariableDec node in decNodeList)
            {
                if (node.getAssign() != null)
                {
                    track_assign(node.getAssign(), theCodeName, value, true);
                }
            }
            varSymbol.pushValue(value);
            monoEnv = value;
            st.define(varSymbol);

            VarMethodOrClass var = new VarMethodOrClass(new Token(Basic_Type.VARMETHODORCLASS, VarName, 0));
            VarMethodOrClass startMethod = new VarMethodOrClass(new Token(Basic_Type.VARMETHODORCLASS, "Start", 0));
            MethodCall startMethodCall = new MethodCall(startMethod, new List<ParamGiven>());
            ClassCall startClassCall = new ClassCall(startMethodCall, null, true);
            ClassCall startCall = new ClassCall(var, startClassCall, true);
            node_start_list.Add(startCall);

            VarMethodOrClass updateMethod = new VarMethodOrClass(new Token(Basic_Type.VARMETHODORCLASS, "FixedUpdate", 0));
            MethodCall updateMethodCall = new MethodCall(updateMethod, new List<ParamGiven>());
            ClassCall updateClassCall = new ClassCall(updateMethodCall, null, true);
            ClassCall updateCall = new ClassCall(var, updateClassCall, true);
            node_update_list.Add(updateCall);

            Excute_start_AST = new Compound(node_start_list);
            Excute_update_AST = new Compound(node_update_list);
        }

        public void excute_start()
        {
            prepare();
            track_in_method(Excute_start_AST, global, null, null);
        }

        public void excute_update()
        {
            track_in_method(Excute_update_AST, global, null, null);
        }

        public void monoEnv_debug()
        {
            Dictionary<string, Value> dec = monoEnv.forDebug();
            foreach (var v in dec)
            {
                if (v.Value == null)
                {
                    return;
                }
                else if (v.Value.getType() == "double")
                {
                    Debug.Log(v.Key + ":" + doubleValueList[v.Value.getIndex()]);
                }
                else if (v.Value.getType() == "float")
                {
                    Debug.Log(v.Key + ":" + floatValueList[v.Value.getIndex()]);
                }
                else if (v.Value.getType() == "long")
                {
                    Debug.Log(v.Key + ":" + longValueList[v.Value.getIndex()]);
                }
                else if (v.Value.getType() == "int")
                {
                    Debug.Log(v.Key + ":" + intValueList[v.Value.getIndex()]);
                }
                else if (v.Value.getType() == "decimal")
                {
                    Debug.Log(v.Key + ":" + decimalValueList[v.Value.getIndex()]);
                }
                else if (v.Value.getType() == "bool")
                {
                    Debug.Log(v.Key + ":" + boolValueList[v.Value.getIndex()]);
                }
                else
                {
                    Debug.Log(v.Key + ":" + v.Value.getType() + "_instance");
                }

            }
        }

        //public Dictionary<string, SymbolTable> getSymbolTables()
        //{
        //    return symbolTables;
        //}

        public string getMonoClassName()
        {
            return theCodeName;
        }
    }
}