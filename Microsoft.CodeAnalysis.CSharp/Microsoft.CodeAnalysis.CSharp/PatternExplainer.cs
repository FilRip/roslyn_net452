using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class PatternExplainer
    {
        private static ImmutableArray<BoundDecisionDagNode> ShortestPathToNode(ImmutableArray<BoundDecisionDagNode> nodes, BoundDecisionDagNode node, bool nullPaths, out bool requiresFalseWhenClause)
        {
            PooledDictionary<BoundDecisionDagNode, (int distance, BoundDecisionDagNode next)> dist = PooledDictionary<BoundDecisionDagNode, (int, BoundDecisionDagNode)>.GetInstance();
            int length = nodes.Length;
            int infinity = 2 * length + 2;
            PooledDictionary<BoundDecisionDagNode, (int, BoundDecisionDagNode)> pooledDictionary;
            BoundDecisionDagNode key;
            (int, BoundDecisionDagNode) value;
            for (int num = length - 1; num >= 0; pooledDictionary.Add(key, value), num--)
            {
                BoundDecisionDagNode boundDecisionDagNode = nodes[num];
                pooledDictionary = dist;
                key = boundDecisionDagNode;
                BoundDecisionDagNode boundDecisionDagNode2 = boundDecisionDagNode;
                if (!(boundDecisionDagNode2 is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode))
                {
                    if (boundDecisionDagNode2 is BoundTestDecisionDagNode boundTestDecisionDagNode)
                    {
                        BoundDagTest test = boundTestDecisionDagNode.Test;
                        if (!(test is BoundDagNonNullTest))
                        {
                            if (test is BoundDagExplicitNullTest)
                            {
                                BoundTestDecisionDagNode boundTestDecisionDagNode2 = boundTestDecisionDagNode;
                                if (!nullPaths)
                                {
                                    value = (1 + distance(boundTestDecisionDagNode2.WhenFalse), boundTestDecisionDagNode2.WhenFalse);
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            BoundTestDecisionDagNode boundTestDecisionDagNode3 = boundTestDecisionDagNode;
                            if (!nullPaths)
                            {
                                value = (1 + distance(boundTestDecisionDagNode3.WhenTrue), boundTestDecisionDagNode3.WhenTrue);
                                continue;
                            }
                        }
                        BoundTestDecisionDagNode boundTestDecisionDagNode4 = boundTestDecisionDagNode;
                        int num2 = distance(boundTestDecisionDagNode4.WhenTrue);
                        int num3 = distance(boundTestDecisionDagNode4.WhenFalse);
                        value = ((num2 <= num3) ? (1 + num2, boundTestDecisionDagNode4.WhenTrue) : (1 + num3, boundTestDecisionDagNode4.WhenFalse));
                    }
                    else if (boundDecisionDagNode2 is BoundWhenDecisionDagNode boundWhenDecisionDagNode)
                    {
                        BoundWhenDecisionDagNode boundWhenDecisionDagNode2 = boundWhenDecisionDagNode;
                        int num4 = distance(boundWhenDecisionDagNode2.WhenTrue);
                        int num5 = distance(boundWhenDecisionDagNode2.WhenFalse);
                        value = ((num4 <= num5) ? (1 + num4, boundWhenDecisionDagNode2.WhenTrue) : (1 + ((num5 < length) ? length : 0) + num5, boundWhenDecisionDagNode2.WhenFalse));
                    }
                    else
                    {
                        value = ((boundDecisionDagNode == node) ? 1 : infinity, null);
                    }
                }
                else
                {
                    BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode2 = boundEvaluationDecisionDagNode;
                    value = (distance(boundEvaluationDecisionDagNode2.Next), boundEvaluationDecisionDagNode2.Next);
                }
            }
            int item = dist[nodes[0]].distance;
            requiresFalseWhenClause = item > length;
            ArrayBuilder<BoundDecisionDagNode> instance = ArrayBuilder<BoundDecisionDagNode>.GetInstance(item);
            BoundDecisionDagNode boundDecisionDagNode3 = nodes[0];
            while (boundDecisionDagNode3 != node)
            {
                instance.Add(boundDecisionDagNode3);
                if (!(boundDecisionDagNode3 is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode3))
                {
                    if (!(boundDecisionDagNode3 is BoundTestDecisionDagNode key2))
                    {
                        if (!(boundDecisionDagNode3 is BoundWhenDecisionDagNode boundWhenDecisionDagNode3))
                        {
                            throw ExceptionUtilities.Unreachable;
                        }
                        instance.RemoveLast();
                        boundDecisionDagNode3 = boundWhenDecisionDagNode3.WhenFalse;
                    }
                    else
                    {
                        boundDecisionDagNode3 = dist[key2].next;
                    }
                }
                else
                {
                    boundDecisionDagNode3 = boundEvaluationDecisionDagNode3.Next;
                }
            }
            dist.Free();
            return instance.ToImmutableAndFree();
            int distance(BoundDecisionDagNode x)
            {
                if (x == null)
                {
                    return infinity;
                }
                if (dist.TryGetValue(x, out var value2))
                {
                    return value2.Item1;
                }
                return infinity;
            }
        }

        internal static string SamplePatternForPathToDagNode(BoundDagTemp rootIdentifier, ImmutableArray<BoundDecisionDagNode> nodes, BoundDecisionDagNode targetNode, bool nullPaths, out bool requiresFalseWhenClause, out bool unnamedEnumValue)
        {
            unnamedEnumValue = false;
            ImmutableArray<BoundDecisionDagNode> immutableArray = ShortestPathToNode(nodes, targetNode, nullPaths, out requiresFalseWhenClause);
            Dictionary<BoundDagTemp, ArrayBuilder<(BoundDagTest, bool)>> dictionary = new Dictionary<BoundDagTemp, ArrayBuilder<(BoundDagTest, bool)>>();
            Dictionary<BoundDagTemp, ArrayBuilder<BoundDagEvaluation>> dictionary2 = new Dictionary<BoundDagTemp, ArrayBuilder<BoundDagEvaluation>>();
            int i = 0;
            for (int length = immutableArray.Length; i < length; i++)
            {
                BoundDecisionDagNode boundDecisionDagNode = immutableArray[i];
                if (!(boundDecisionDagNode is BoundTestDecisionDagNode boundTestDecisionDagNode))
                {
                    if (boundDecisionDagNode is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode)
                    {
                        BoundDagTemp input = boundEvaluationDecisionDagNode.Evaluation.Input;
                        if (!dictionary2.TryGetValue(input, out var value))
                        {
                            dictionary2.Add(input, value = new ArrayBuilder<BoundDagEvaluation>());
                        }
                        value.Add(boundEvaluationDecisionDagNode.Evaluation);
                    }
                    continue;
                }
                BoundDecisionDagNode boundDecisionDagNode2 = ((i < length - 1) ? immutableArray[i + 1] : targetNode);
                bool flag = boundTestDecisionDagNode.WhenTrue == boundDecisionDagNode2 || (boundTestDecisionDagNode.WhenFalse != boundDecisionDagNode2 && boundTestDecisionDagNode.WhenTrue is BoundWhenDecisionDagNode);
                BoundDagTest test = boundTestDecisionDagNode.Test;
                BoundDagTemp input2 = test.Input;
                if (!(test is BoundDagTypeTest) || flag)
                {
                    if (!dictionary.TryGetValue(input2, out var value2))
                    {
                        dictionary.Add(input2, value2 = new ArrayBuilder<(BoundDagTest, bool)>());
                    }
                    value2.Add((test, flag));
                }
            }
            return SamplePatternForTemp(rootIdentifier, dictionary, dictionary2, requireExactType: false, ref unnamedEnumValue);
        }

        private static string SamplePatternForTemp(BoundDagTemp input, Dictionary<BoundDagTemp, ArrayBuilder<(BoundDagTest test, bool sense)>> constraintMap, Dictionary<BoundDagTemp, ArrayBuilder<BoundDagEvaluation>> evaluationMap, bool requireExactType, ref bool unnamedEnumValue)
        {
            ImmutableArray<(BoundDagTest test, bool sense)> constraints = getArray<(BoundDagTest, bool)>(constraintMap, input);
            ImmutableArray<BoundDagEvaluation> evaluations = getArray<BoundDagEvaluation>(evaluationMap, input);
            return tryHandleSingleTest() ?? tryHandleTypeTestAndTypeEvaluation(ref unnamedEnumValue) ?? tryHandleUnboxNullableValueType(ref unnamedEnumValue) ?? tryHandleTuplePattern(ref unnamedEnumValue) ?? tryHandleNumericLimits(ref unnamedEnumValue) ?? tryHandleRecursivePattern(ref unnamedEnumValue) ?? produceFallbackPattern();
            static ImmutableArray<T> getArray<T>(Dictionary<BoundDagTemp, ArrayBuilder<T>> map, BoundDagTemp temp)
            {
                if (!map.TryGetValue(temp, out var value5))
                {
                    return ImmutableArray<T>.Empty;
                }
                return value5.ToImmutable();
            }
            static string makeConjunct(string oldPattern, string newPattern)
            {
                if (oldPattern == "_")
                {
                    return newPattern;
                }
                if (newPattern == "_")
                {
                    return oldPattern;
                }
                return oldPattern + " and " + newPattern;
            }
            string produceFallbackPattern()
            {
                if (!requireExactType)
                {
                    return "_";
                }
                return input.Type.ToDisplayString();
            }
            string tryHandleNumericLimits(ref bool unnamedEnumValue)
            {
                if (evaluations.IsEmpty && constraints.All(delegate ((BoundDagTest test, bool sense) t)
                {
                    var (boundDagTest3, _) = t;
                    if (boundDagTest3 is BoundDagValueTest)
                    {
                        return true;
                    }
                    if (boundDagTest3 is BoundDagRelationalTest)
                    {
                        return true;
                    }
                    if (boundDagTest3 is BoundDagExplicitNullTest)
                    {
                        if (!t.sense)
                        {
                            return true;
                        }
                    }
                    else if (boundDagTest3 is BoundDagNonNullTest && t.sense)
                    {
                        return true;
                    }
                    return false;
                }))
                {
                    IValueSetFactory fac = ValueSetFactory.ForType(input.Type);
                    if (fac != null)
                    {
                        IValueSet remainingValues = fac.AllValues;
                        ImmutableArray<(BoundDagTest, bool)>.Enumerator enumerator2 = constraints.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            var (boundDagTest2, sense) = enumerator2.Current;
                            if (!(boundDagTest2 is BoundDagValueTest boundDagValueTest))
                            {
                                if (boundDagTest2 is BoundDagRelationalTest boundDagRelationalTest)
                                {
                                    addRelation(boundDagRelationalTest.Relation, boundDagRelationalTest.Value);
                                }
                            }
                            else
                            {
                                addRelation(BinaryOperatorKind.Equal, boundDagValueTest.Value);
                            }
                            void addRelation(BinaryOperatorKind relation, ConstantValue value)
                            {
                                if (!value.IsBad)
                                {
                                    IValueSet valueSet = fac.Related(relation, value);
                                    if (!sense)
                                    {
                                        valueSet = valueSet.Complement();
                                    }
                                    remainingValues = remainingValues.Intersect(valueSet);
                                }
                            }
                        }
                        if (remainingValues.Complement().IsEmpty)
                        {
                            return "_";
                        }
                        return SampleValueString(remainingValues, input.Type, requireExactType, ref unnamedEnumValue);
                    }
                }
                return null;
            }
            string tryHandleRecursivePattern(ref bool unnamedEnumValue)
            {
                if (constraints.IsEmpty && evaluations.IsEmpty)
                {
                    return null;
                }
                if (!constraints.All(delegate ((BoundDagTest test, bool sense) c)
                {
                    var (boundDagTest, _) = c;
                    if (boundDagTest is BoundDagNonNullTest)
                    {
                        if (c.sense)
                        {
                            return true;
                        }
                    }
                    else if (boundDagTest is BoundDagExplicitNullTest && !c.sense)
                    {
                        return true;
                    }
                    return false;
                }))
                {
                    return null;
                }
                string text = null;
                Dictionary<Symbol, string> dictionary = new Dictionary<Symbol, string>();
                bool flag = false;
                ImmutableArray<BoundDagEvaluation>.Enumerator enumerator = evaluations.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BoundDagEvaluation current = enumerator.Current;
                    if (!(current is BoundDagDeconstructEvaluation boundDagDeconstructEvaluation))
                    {
                        if (!(current is BoundDagFieldEvaluation boundDagFieldEvaluation))
                        {
                            if (!(current is BoundDagPropertyEvaluation boundDagPropertyEvaluation))
                            {
                                return null;
                            }
                            string value2 = SamplePatternForTemp(new BoundDagTemp(boundDagPropertyEvaluation.Syntax, boundDagPropertyEvaluation.Property.Type, boundDagPropertyEvaluation), constraintMap, evaluationMap, requireExactType: false, ref unnamedEnumValue);
                            dictionary.Add(boundDagPropertyEvaluation.Property, value2);
                        }
                        else
                        {
                            string value3 = SamplePatternForTemp(new BoundDagTemp(boundDagFieldEvaluation.Syntax, boundDagFieldEvaluation.Field.Type, boundDagFieldEvaluation), constraintMap, evaluationMap, requireExactType: false, ref unnamedEnumValue);
                            dictionary.Add(boundDagFieldEvaluation.Field, value3);
                        }
                    }
                    else
                    {
                        MethodSymbol deconstructMethod = boundDagDeconstructEvaluation.DeconstructMethod;
                        int num = ((!deconstructMethod.RequiresInstanceReceiver) ? 1 : 0);
                        int num2 = deconstructMethod.Parameters.Length - num;
                        StringBuilder stringBuilder = new StringBuilder("(");
                        for (int i = 0; i < num2; i++)
                        {
                            string value4 = SamplePatternForTemp(new BoundDagTemp(boundDagDeconstructEvaluation.Syntax, deconstructMethod.Parameters[i + num].Type, boundDagDeconstructEvaluation, i), constraintMap, evaluationMap, requireExactType: false, ref unnamedEnumValue);
                            if (i != 0)
                            {
                                stringBuilder.Append(", ");
                            }
                            stringBuilder.Append(value4);
                        }
                        stringBuilder.Append(")");
                        string text2 = stringBuilder.ToString();
                        if (text != null && flag)
                        {
                            text += " { }";
                            flag = dictionary.Count != 0;
                        }
                        text = ((text == null) ? text2 : (text + " and " + text2));
                        flag = flag || num2 == 1;
                    }
                }
                string text3 = (requireExactType ? input.Type.ToDisplayString() : null);
                string text4 = ((flag | ((text == null && text3 == null) || dictionary.Count != 0)) ? (((text != null) ? " {" : "{") + string.Join(", ", dictionary.Select((KeyValuePair<Symbol, string> kvp) => " " + kvp.Key.Name + ": " + kvp.Value)) + " }") : null);
                return text3 + text + text4;
            }
            string tryHandleSingleTest()
            {
                if (evaluations.IsEmpty && constraints.Length == 1)
                {
                    (BoundDagTest, bool) tuple6 = constraints[0];
                    var (boundDagTest4, _) = tuple6;
                    if (boundDagTest4 is BoundDagNonNullTest)
                    {
                        if (tuple6.Item2)
                        {
                            if (!requireExactType)
                            {
                                return "not null";
                            }
                            return input.Type.ToDisplayString();
                        }
                        return "null";
                    }
                    if (boundDagTest4 is BoundDagExplicitNullTest)
                    {
                        if (!tuple6.Item2)
                        {
                            if (!requireExactType)
                            {
                                return "not null";
                            }
                            return input.Type.ToDisplayString();
                        }
                        return "null";
                    }
                    if (boundDagTest4 is BoundDagTypeTest boundDagTypeTest2)
                    {
                        TypeSymbol type4 = boundDagTypeTest2.Type;
                        bool item = tuple6.Item2;
                        return type4.ToDisplayString();
                    }
                }
                return null;
            }
            string tryHandleTuplePattern(ref bool unnamedEnumValue)
            {
                if (input.Type.IsTupleType && constraints.IsEmpty && evaluations.All(delegate (BoundDagEvaluation e)
                {
                    if (e is BoundDagFieldEvaluation boundDagFieldEvaluation3)
                    {
                        FieldSymbol field = boundDagFieldEvaluation3.Field;
                        return field.IsTupleElement();
                    }
                    return false;
                }))
                {
                    int length = input.Type.TupleElements.Length;
                    ArrayBuilder<string> arrayBuilder = new ArrayBuilder<string>(length);
                    arrayBuilder.AddMany("_", length);
                    ImmutableArray<BoundDagEvaluation>.Enumerator enumerator3 = evaluations.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        BoundDagFieldEvaluation boundDagFieldEvaluation2 = (BoundDagFieldEvaluation)enumerator3.Current;
                        BoundDagTemp input2 = new BoundDagTemp(boundDagFieldEvaluation2.Syntax, boundDagFieldEvaluation2.Field.Type, boundDagFieldEvaluation2);
                        int tupleElementIndex = boundDagFieldEvaluation2.Field.TupleElementIndex;
                        if (tupleElementIndex < 0 || tupleElementIndex >= length)
                        {
                            return null;
                        }
                        string oldPattern2 = arrayBuilder[tupleElementIndex];
                        string newPattern2 = SamplePatternForTemp(input2, constraintMap, evaluationMap, requireExactType: false, ref unnamedEnumValue);
                        arrayBuilder[tupleElementIndex] = makeConjunct(oldPattern2, newPattern2);
                    }
                    return "(" + string.Join(", ", arrayBuilder) + ")" + ((arrayBuilder.Count == 1) ? " { }" : null);
                }
                return null;
            }
            string tryHandleTypeTestAndTypeEvaluation(ref bool unnamedEnumValue)
            {
                if (evaluations.Length == 1 && constraints.Length == 1)
                {
                    (BoundDagTest, bool) tuple5 = constraints[0];
                    if (tuple5.Item1 is BoundDagTypeTest boundDagTypeTest)
                    {
                        TypeSymbol type2 = boundDagTypeTest.Type;
                        if (tuple5.Item2 && evaluations[0] is BoundDagTypeEvaluation boundDagTypeEvaluation2)
                        {
                            TypeSymbol type3 = boundDagTypeEvaluation2.Type;
                            if (type2.Equals(type3, TypeCompareKind.AllIgnoreOptions))
                            {
                                return SamplePatternForTemp(new BoundDagTemp(boundDagTypeEvaluation2.Syntax, boundDagTypeEvaluation2.Type, boundDagTypeEvaluation2), constraintMap, evaluationMap, requireExactType: true, ref unnamedEnumValue);
                            }
                        }
                    }
                }
                return null;
            }
            string tryHandleUnboxNullableValueType(ref bool unnamedEnumValue)
            {
                if (evaluations.Length == 1 && constraints.Length == 1)
                {
                    (BoundDagTest, bool) tuple4 = constraints[0];
                    if (tuple4.Item1 is BoundDagNonNullTest && tuple4.Item2 && evaluations[0] is BoundDagTypeEvaluation boundDagTypeEvaluation)
                    {
                        TypeSymbol type = boundDagTypeEvaluation.Type;
                        if (input.Type.IsNullableType() && input.Type.GetNullableUnderlyingType().Equals(type, TypeCompareKind.AllIgnoreOptions))
                        {
                            string text5 = SamplePatternForTemp(new BoundDagTemp(boundDagTypeEvaluation.Syntax, boundDagTypeEvaluation.Type, boundDagTypeEvaluation), constraintMap, evaluationMap, requireExactType: false, ref unnamedEnumValue);
                            if (!(text5 == "_"))
                            {
                                return text5;
                            }
                            return "not null";
                        }
                    }
                }
                return null;
            }
        }

        private static string SampleValueString(IValueSet remainingValues, TypeSymbol type, bool requireExactType, ref bool unnamedEnumValue)
        {
            if (type is NamedTypeSymbol namedTypeSymbol && type.TypeKind == TypeKind.Enum)
            {
                ImmutableArray<Symbol>.Enumerator enumerator = namedTypeSymbol.GetMembers().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    if (current is FieldSymbol fieldSymbol && fieldSymbol.IsConst && current.IsStatic && current.DeclaredAccessibility == Accessibility.Public)
                    {
                        ConstantValue constantValue = fieldSymbol.GetConstantValue(ConstantFieldsInProgress.Empty, earlyDecodingWellKnownAttributes: false);
                        if ((object)constantValue != null && remainingValues.Any(BinaryOperatorKind.Equal, constantValue))
                        {
                            return fieldSymbol.ToDisplayString();
                        }
                    }
                }
                unnamedEnumValue = true;
            }
            ConstantValue sample = remainingValues.Sample;
            if (sample != null)
            {
                return ValueString(sample, type, requireExactType);
            }
            TypeSymbol typeSymbol = type.EnumUnderlyingTypeOrSelf();
            if (typeSymbol.SpecialType == SpecialType.System_IntPtr)
            {
                if (remainingValues.Any(BinaryOperatorKind.GreaterThan, ConstantValue.Create(int.MaxValue)))
                {
                    return "> (" + type.ToDisplayString() + ")int.MaxValue";
                }
                if (remainingValues.Any(BinaryOperatorKind.LessThan, ConstantValue.Create(int.MinValue)))
                {
                    return "< (" + type.ToDisplayString() + ")int.MinValue";
                }
            }
            else if (typeSymbol.SpecialType == SpecialType.System_UIntPtr && remainingValues.Any(BinaryOperatorKind.GreaterThan, ConstantValue.Create(uint.MaxValue)))
            {
                return "> (" + type.ToDisplayString() + ")uint.MaxValue";
            }
            throw ExceptionUtilities.Unreachable;
        }

        private static string ValueString(ConstantValue value, TypeSymbol type, bool requireExactType)
        {
            bool num = (type.IsEnumType() || requireExactType || type.IsNativeIntegerType) && (!typeHasExactTypeLiteral(type) || value.IsNull);
            string text = PrimitiveValueString(value, type.EnumUnderlyingTypeOrSelf());
            if (!num)
            {
                return text;
            }
            return "(" + type.ToDisplayString() + ")" + text;
            static bool typeHasExactTypeLiteral(TypeSymbol type)
            {
                return type.SpecialType switch
                {
                    SpecialType.System_Int32 => true,
                    SpecialType.System_Int64 => true,
                    SpecialType.System_UInt32 => true,
                    SpecialType.System_UInt64 => true,
                    SpecialType.System_String => true,
                    SpecialType.System_Decimal => true,
                    SpecialType.System_Single => true,
                    SpecialType.System_Double => true,
                    SpecialType.System_Boolean => true,
                    SpecialType.System_Char => true,
                    _ => false,
                };
            }
        }

        private static string PrimitiveValueString(ConstantValue value, TypeSymbol type)
        {
            if (value.IsNull)
            {
                return "null";
            }
            switch (type.SpecialType)
            {
                case SpecialType.System_IntPtr:
                    if (!type.IsNativeIntegerType)
                    {
                        break;
                    }
                    goto case SpecialType.System_Boolean;
                case SpecialType.System_UIntPtr:
                    if (!type.IsNativeIntegerType)
                    {
                        break;
                    }
                    goto case SpecialType.System_Boolean;
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_String:
                    return ObjectDisplay.FormatPrimitive(value.Value, ObjectDisplayOptions.IncludeTypeSuffix | ObjectDisplayOptions.UseQuotes | ObjectDisplayOptions.EscapeNonPrintableCharacters);
                case SpecialType.System_Single:
                    {
                        float singleValue = value.SingleValue;
                        if (singleValue != float.NegativeInfinity)
                        {
                            if (singleValue != float.PositiveInfinity)
                            {
                                if (float.IsNaN(singleValue))
                                {
                                    return "float.NaN";
                                }
                                return ObjectDisplay.FormatPrimitive(singleValue, ObjectDisplayOptions.IncludeTypeSuffix);
                            }
                            return "float.PositiveInfinity";
                        }
                        return "float.NegativeInfinity";
                    }
                case SpecialType.System_Double:
                    {
                        double doubleValue = value.DoubleValue;
                        if (doubleValue != double.NegativeInfinity)
                        {
                            if (doubleValue != double.PositiveInfinity)
                            {
                                if (double.IsNaN(doubleValue))
                                {
                                    return "double.NaN";
                                }
                                return ObjectDisplay.FormatPrimitive(doubleValue, ObjectDisplayOptions.IncludeTypeSuffix);
                            }
                            return "double.PositiveInfinity";
                        }
                        return "double.NegativeInfinity";
                    }
            }
            return "_";
        }
    }
}
