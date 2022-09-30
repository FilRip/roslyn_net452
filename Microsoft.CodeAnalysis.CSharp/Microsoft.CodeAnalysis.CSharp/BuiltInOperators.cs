using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal class BuiltInOperators
    {
        private readonly CSharpCompilation _compilation;

        private ImmutableArray<UnaryOperatorSignature>[] _builtInUnaryOperators;

        private ImmutableArray<BinaryOperatorSignature>[][] _builtInOperators;

        internal BuiltInOperators(CSharpCompilation compilation)
        {
            _compilation = compilation;
        }

        private ImmutableArray<UnaryOperatorSignature> GetSignaturesFromUnaryOperatorKinds(int[] operatorKinds)
        {
            ArrayBuilder<UnaryOperatorSignature> instance = ArrayBuilder<UnaryOperatorSignature>.GetInstance();
            foreach (int kind in operatorKinds)
            {
                instance.Add(GetSignature((UnaryOperatorKind)kind));
            }
            return instance.ToImmutableAndFree();
        }

        internal void GetSimpleBuiltInOperators(UnaryOperatorKind kind, ArrayBuilder<UnaryOperatorSignature> operators, bool skipNativeIntegerOperators)
        {
            if (_builtInUnaryOperators == null)
            {
                ImmutableArray<UnaryOperatorSignature>[] value = new ImmutableArray<UnaryOperatorSignature>[10]
                {
                    GetSignaturesFromUnaryOperatorKinds(new int[28]
                    {
                        4097, 4098, 4099, 4100, 4101, 4102, 4103, 4104, 4105, 4106,
                        4107, 4108, 4109, 4110, 69633, 69634, 69635, 69636, 69637, 69638,
                        69639, 69640, 69641, 69642, 69643, 69644, 69645, 69646
                    }),
                    GetSignaturesFromUnaryOperatorKinds(new int[28]
                    {
                        4353, 4354, 4355, 4356, 4357, 4358, 4359, 4360, 4361, 4362,
                        4363, 4364, 4365, 4366, 69889, 69890, 69891, 69892, 69893, 69894,
                        69895, 69896, 69897, 69898, 69899, 69900, 69901, 69902
                    }),
                    GetSignaturesFromUnaryOperatorKinds(new int[28]
                    {
                        4609, 4610, 4611, 4612, 4613, 4614, 4615, 4616, 4617, 4618,
                        4619, 4620, 4621, 4622, 70145, 70146, 70147, 70148, 70149, 70150,
                        70151, 70152, 70153, 70154, 70155, 70156, 70157, 70158
                    }),
                    GetSignaturesFromUnaryOperatorKinds(new int[28]
                    {
                        4865, 4866, 4867, 4868, 4869, 4870, 4873, 4874, 4871, 4872,
                        4875, 4876, 4877, 4878, 70401, 70402, 70403, 70404, 70405, 70406,
                        70407, 70408, 70409, 70410, 70411, 70412, 70413, 70414
                    }),
                    GetSignaturesFromUnaryOperatorKinds(new int[18]
                    {
                        5125, 5126, 5127, 5128, 5129, 5130, 5132, 5133, 5134, 70661,
                        70662, 70663, 70664, 70665, 70666, 70668, 70669, 70670
                    }),
                    GetSignaturesFromUnaryOperatorKinds(new int[12]
                    {
                        5381, 5383, 5385, 5388, 5389, 5390, 70917, 70919, 70921, 70924,
                        70925, 70926
                    }),
                    GetSignaturesFromUnaryOperatorKinds(new int[2] { 5647, 71183 }),
                    GetSignaturesFromUnaryOperatorKinds(new int[12]
                    {
                        5893, 5894, 5895, 5896, 5897, 5898, 71429, 71430, 71431, 71432,
                        71433, 71434
                    }),
                    ImmutableArray<UnaryOperatorSignature>.Empty,
                    ImmutableArray<UnaryOperatorSignature>.Empty
                };
                Interlocked.CompareExchange(ref _builtInUnaryOperators, value, null);
            }
            ImmutableArray<UnaryOperatorSignature>.Enumerator enumerator = _builtInUnaryOperators[kind.OperatorIndex()].GetEnumerator();
            while (enumerator.MoveNext())
            {
                UnaryOperatorSignature current = enumerator.Current;
                if (skipNativeIntegerOperators)
                {
                    UnaryOperatorKind unaryOperatorKind = current.Kind.OperandTypes();
                    if ((uint)(unaryOperatorKind - 9) <= 1u)
                    {
                        continue;
                    }
                }
                operators.Add(current);
            }
        }

        internal UnaryOperatorSignature GetSignature(UnaryOperatorKind kind)
        {
            TypeSymbol typeSymbol = kind.OperandTypes() switch
            {
                UnaryOperatorKind.SByte => _compilation.GetSpecialType(SpecialType.System_SByte),
                UnaryOperatorKind.Byte => _compilation.GetSpecialType(SpecialType.System_Byte),
                UnaryOperatorKind.Short => _compilation.GetSpecialType(SpecialType.System_Int16),
                UnaryOperatorKind.UShort => _compilation.GetSpecialType(SpecialType.System_UInt16),
                UnaryOperatorKind.Int => _compilation.GetSpecialType(SpecialType.System_Int32),
                UnaryOperatorKind.UInt => _compilation.GetSpecialType(SpecialType.System_UInt32),
                UnaryOperatorKind.Long => _compilation.GetSpecialType(SpecialType.System_Int64),
                UnaryOperatorKind.ULong => _compilation.GetSpecialType(SpecialType.System_UInt64),
                UnaryOperatorKind.NInt => _compilation.CreateNativeIntegerTypeSymbol(signed: true),
                UnaryOperatorKind.NUInt => _compilation.CreateNativeIntegerTypeSymbol(signed: false),
                UnaryOperatorKind.Char => _compilation.GetSpecialType(SpecialType.System_Char),
                UnaryOperatorKind.Float => _compilation.GetSpecialType(SpecialType.System_Single),
                UnaryOperatorKind.Double => _compilation.GetSpecialType(SpecialType.System_Double),
                UnaryOperatorKind.Decimal => _compilation.GetSpecialType(SpecialType.System_Decimal),
                UnaryOperatorKind.Bool => _compilation.GetSpecialType(SpecialType.System_Boolean),
                _ => throw ExceptionUtilities.UnexpectedValue(kind.OperandTypes()),
            };
            if (kind.IsLifted())
            {
                typeSymbol = _compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(typeSymbol);
            }
            return new UnaryOperatorSignature(kind, typeSymbol, typeSymbol);
        }

        private ImmutableArray<BinaryOperatorSignature> GetSignaturesFromBinaryOperatorKinds(int[] operatorKinds)
        {
            ArrayBuilder<BinaryOperatorSignature> instance = ArrayBuilder<BinaryOperatorSignature>.GetInstance();
            foreach (int kind in operatorKinds)
            {
                instance.Add(GetSignature((BinaryOperatorKind)kind));
            }
            return instance.ToImmutableAndFree();
        }

        internal void GetSimpleBuiltInOperators(BinaryOperatorKind kind, ArrayBuilder<BinaryOperatorSignature> operators, bool skipNativeIntegerOperators)
        {
            if (_builtInOperators == null)
            {
                ImmutableArray<BinaryOperatorSignature>[] array = new ImmutableArray<BinaryOperatorSignature>[16]
                {
                    ImmutableArray<BinaryOperatorSignature>.Empty,
                    ImmutableArray<BinaryOperatorSignature>.Empty,
                    ImmutableArray<BinaryOperatorSignature>.Empty,
                    ImmutableArray<BinaryOperatorSignature>.Empty,
                    ImmutableArray<BinaryOperatorSignature>.Empty,
                    ImmutableArray<BinaryOperatorSignature>.Empty,
                    ImmutableArray<BinaryOperatorSignature>.Empty,
                    ImmutableArray<BinaryOperatorSignature>.Empty,
                    ImmutableArray<BinaryOperatorSignature>.Empty,
                    ImmutableArray<BinaryOperatorSignature>.Empty,
                    ImmutableArray<BinaryOperatorSignature>.Empty,
                    ImmutableArray<BinaryOperatorSignature>.Empty,
                    ImmutableArray<BinaryOperatorSignature>.Empty,
                    ImmutableArray.Create(GetSignature(BinaryOperatorKind.LogicalBoolAnd)),
                    ImmutableArray<BinaryOperatorSignature>.Empty,
                    ImmutableArray.Create(GetSignature(BinaryOperatorKind.LogicalBoolOr))
                };
                ImmutableArray<BinaryOperatorSignature>[] array2 = new ImmutableArray<BinaryOperatorSignature>[16]
                {
                    GetSignaturesFromBinaryOperatorKinds(new int[18]
                    {
                        4101, 4102, 4103, 4104, 4105, 4106, 4108, 4109, 4110, 69637,
                        69638, 69639, 69640, 69641, 69642, 69644, 69645, 69646
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[21]
                    {
                        4357, 4358, 4359, 4360, 4361, 4362, 4364, 4365, 4366, 69893,
                        69894, 69895, 69896, 69897, 69898, 69900, 69901, 69902, 4369, 4370,
                        4371
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[18]
                    {
                        4613, 4614, 4615, 4616, 4617, 4618, 4620, 4621, 4622, 70149,
                        70150, 70151, 70152, 70153, 70154, 70156, 70157, 70158
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[18]
                    {
                        4869, 4870, 4871, 4872, 4873, 4874, 4876, 4877, 4878, 70405,
                        70406, 70407, 70408, 70409, 70410, 70412, 70413, 70414
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[18]
                    {
                        5125, 5126, 5127, 5128, 5129, 5130, 5132, 5133, 5134, 70661,
                        70662, 70663, 70664, 70665, 70666, 70668, 70669, 70670
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[12]
                    {
                        5381, 5382, 5383, 5384, 5385, 5386, 70917, 70918, 70919, 70920,
                        70921, 70922
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[12]
                    {
                        5637, 5638, 5639, 5640, 5641, 5642, 71173, 71174, 71175, 71176,
                        71177, 71178
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[22]
                    {
                        5893, 5894, 5895, 5896, 5897, 5898, 5900, 5901, 5902, 5903,
                        71429, 71430, 71431, 71432, 71433, 71434, 71436, 71437, 71438, 71439,
                        5904, 5905
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[22]
                    {
                        6149, 6150, 6151, 6152, 6153, 6154, 6156, 6157, 6158, 6159,
                        71685, 71686, 71687, 71688, 71689, 71690, 71692, 71693, 71694, 71695,
                        6160, 6161
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[18]
                    {
                        6405, 6406, 6407, 6408, 6409, 6410, 6412, 6413, 6414, 71941,
                        71942, 71943, 71944, 71945, 71946, 71948, 71949, 71950
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[18]
                    {
                        6661, 6662, 6663, 6664, 6665, 6666, 6668, 6669, 6670, 72197,
                        72198, 72199, 72200, 72201, 72202, 72204, 72205, 72206
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[18]
                    {
                        6917, 6918, 6919, 6920, 6921, 6922, 6924, 6925, 6926, 72453,
                        72454, 72455, 72456, 72457, 72458, 72460, 72461, 72462
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[18]
                    {
                        7173, 7174, 7175, 7176, 7177, 7178, 7180, 7181, 7182, 72709,
                        72710, 72711, 72712, 72713, 72714, 72716, 72717, 72718
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[14]
                    {
                        7429, 7430, 7431, 7432, 7433, 7434, 7439, 72965, 72966, 72967,
                        72968, 72969, 72970, 72975
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[14]
                    {
                        7685, 7686, 7687, 7688, 7689, 7690, 7695, 73221, 73222, 73223,
                        73224, 73225, 73226, 73231
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new int[14]
                    {
                        7941, 7942, 7943, 7944, 7945, 7946, 7951, 73477, 73478, 73479,
                        73480, 73481, 73482, 73487
                    })
                };
                ImmutableArray<BinaryOperatorSignature>[][] value = new ImmutableArray<BinaryOperatorSignature>[2][] { array2, array };
                Interlocked.CompareExchange(ref _builtInOperators, value, null);
            }
            ImmutableArray<BinaryOperatorSignature>.Enumerator enumerator = _builtInOperators[kind.IsLogical() ? 1 : 0][kind.OperatorIndex()].GetEnumerator();
            while (enumerator.MoveNext())
            {
                BinaryOperatorSignature current = enumerator.Current;
                if (skipNativeIntegerOperators)
                {
                    BinaryOperatorKind binaryOperatorKind = current.Kind.OperandTypes();
                    if ((uint)(binaryOperatorKind - 9) <= 1u)
                    {
                        continue;
                    }
                }
                operators.Add(current);
            }
        }

        internal BinaryOperatorSignature GetSignature(BinaryOperatorKind kind)
        {
            TypeSymbol typeSymbol = LeftType(kind);
            switch (kind.Operator())
            {
                case BinaryOperatorKind.Multiplication:
                case BinaryOperatorKind.Subtraction:
                case BinaryOperatorKind.Division:
                case BinaryOperatorKind.Remainder:
                case BinaryOperatorKind.And:
                case BinaryOperatorKind.Xor:
                case BinaryOperatorKind.Or:
                    return new BinaryOperatorSignature(kind, typeSymbol, typeSymbol, typeSymbol);
                case BinaryOperatorKind.Addition:
                    return new BinaryOperatorSignature(kind, typeSymbol, RightType(kind), ReturnType(kind));
                case BinaryOperatorKind.LeftShift:
                case BinaryOperatorKind.RightShift:
                    {
                        TypeSymbol typeSymbol2 = _compilation.GetSpecialType(SpecialType.System_Int32);
                        if (kind.IsLifted())
                        {
                            typeSymbol2 = _compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(typeSymbol2);
                        }
                        return new BinaryOperatorSignature(kind, typeSymbol, typeSymbol2, typeSymbol);
                    }
                case BinaryOperatorKind.Equal:
                case BinaryOperatorKind.NotEqual:
                case BinaryOperatorKind.GreaterThan:
                case BinaryOperatorKind.LessThan:
                case BinaryOperatorKind.GreaterThanOrEqual:
                case BinaryOperatorKind.LessThanOrEqual:
                    return new BinaryOperatorSignature(kind, typeSymbol, typeSymbol, _compilation.GetSpecialType(SpecialType.System_Boolean));
                default:
                    return new BinaryOperatorSignature(kind, typeSymbol, RightType(kind), ReturnType(kind));
            }
        }

        private TypeSymbol LeftType(BinaryOperatorKind kind)
        {
            if (kind.IsLifted())
            {
                return LiftedType(kind);
            }
            switch (kind.OperandTypes())
            {
                case BinaryOperatorKind.Int:
                    return _compilation.GetSpecialType(SpecialType.System_Int32);
                case BinaryOperatorKind.UInt:
                    return _compilation.GetSpecialType(SpecialType.System_UInt32);
                case BinaryOperatorKind.Long:
                    return _compilation.GetSpecialType(SpecialType.System_Int64);
                case BinaryOperatorKind.ULong:
                    return _compilation.GetSpecialType(SpecialType.System_UInt64);
                case BinaryOperatorKind.NInt:
                    return _compilation.CreateNativeIntegerTypeSymbol(signed: true);
                case BinaryOperatorKind.NUInt:
                    return _compilation.CreateNativeIntegerTypeSymbol(signed: false);
                case BinaryOperatorKind.Float:
                    return _compilation.GetSpecialType(SpecialType.System_Single);
                case BinaryOperatorKind.Double:
                    return _compilation.GetSpecialType(SpecialType.System_Double);
                case BinaryOperatorKind.Decimal:
                    return _compilation.GetSpecialType(SpecialType.System_Decimal);
                case BinaryOperatorKind.Bool:
                    return _compilation.GetSpecialType(SpecialType.System_Boolean);
                case BinaryOperatorKind.Object:
                case BinaryOperatorKind.ObjectAndString:
                    return _compilation.GetSpecialType(SpecialType.System_Object);
                case BinaryOperatorKind.String:
                case BinaryOperatorKind.StringAndObject:
                    return _compilation.GetSpecialType(SpecialType.System_String);
                default:
                    return null;
            }
        }

        private TypeSymbol RightType(BinaryOperatorKind kind)
        {
            if (kind.IsLifted())
            {
                return LiftedType(kind);
            }
            switch (kind.OperandTypes())
            {
                case BinaryOperatorKind.Int:
                    return _compilation.GetSpecialType(SpecialType.System_Int32);
                case BinaryOperatorKind.UInt:
                    return _compilation.GetSpecialType(SpecialType.System_UInt32);
                case BinaryOperatorKind.Long:
                    return _compilation.GetSpecialType(SpecialType.System_Int64);
                case BinaryOperatorKind.ULong:
                    return _compilation.GetSpecialType(SpecialType.System_UInt64);
                case BinaryOperatorKind.NInt:
                    return _compilation.CreateNativeIntegerTypeSymbol(signed: true);
                case BinaryOperatorKind.NUInt:
                    return _compilation.CreateNativeIntegerTypeSymbol(signed: false);
                case BinaryOperatorKind.Float:
                    return _compilation.GetSpecialType(SpecialType.System_Single);
                case BinaryOperatorKind.Double:
                    return _compilation.GetSpecialType(SpecialType.System_Double);
                case BinaryOperatorKind.Decimal:
                    return _compilation.GetSpecialType(SpecialType.System_Decimal);
                case BinaryOperatorKind.Bool:
                    return _compilation.GetSpecialType(SpecialType.System_Boolean);
                case BinaryOperatorKind.String:
                case BinaryOperatorKind.ObjectAndString:
                    return _compilation.GetSpecialType(SpecialType.System_String);
                case BinaryOperatorKind.Object:
                case BinaryOperatorKind.StringAndObject:
                    return _compilation.GetSpecialType(SpecialType.System_Object);
                default:
                    return null;
            }
        }

        private TypeSymbol ReturnType(BinaryOperatorKind kind)
        {
            if (kind.IsLifted())
            {
                return LiftedType(kind);
            }
            switch (kind.OperandTypes())
            {
                case BinaryOperatorKind.Int:
                    return _compilation.GetSpecialType(SpecialType.System_Int32);
                case BinaryOperatorKind.UInt:
                    return _compilation.GetSpecialType(SpecialType.System_UInt32);
                case BinaryOperatorKind.Long:
                    return _compilation.GetSpecialType(SpecialType.System_Int64);
                case BinaryOperatorKind.ULong:
                    return _compilation.GetSpecialType(SpecialType.System_UInt64);
                case BinaryOperatorKind.NInt:
                    return _compilation.CreateNativeIntegerTypeSymbol(signed: true);
                case BinaryOperatorKind.NUInt:
                    return _compilation.CreateNativeIntegerTypeSymbol(signed: false);
                case BinaryOperatorKind.Float:
                    return _compilation.GetSpecialType(SpecialType.System_Single);
                case BinaryOperatorKind.Double:
                    return _compilation.GetSpecialType(SpecialType.System_Double);
                case BinaryOperatorKind.Decimal:
                    return _compilation.GetSpecialType(SpecialType.System_Decimal);
                case BinaryOperatorKind.Bool:
                    return _compilation.GetSpecialType(SpecialType.System_Boolean);
                case BinaryOperatorKind.Object:
                    return _compilation.GetSpecialType(SpecialType.System_Object);
                case BinaryOperatorKind.String:
                case BinaryOperatorKind.StringAndObject:
                case BinaryOperatorKind.ObjectAndString:
                    return _compilation.GetSpecialType(SpecialType.System_String);
                default:
                    return null;
            }
        }

        private TypeSymbol LiftedType(BinaryOperatorKind kind)
        {
            NamedTypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Nullable_T);
            return kind.OperandTypes() switch
            {
                BinaryOperatorKind.Int => specialType.Construct(_compilation.GetSpecialType(SpecialType.System_Int32)),
                BinaryOperatorKind.UInt => specialType.Construct(_compilation.GetSpecialType(SpecialType.System_UInt32)),
                BinaryOperatorKind.Long => specialType.Construct(_compilation.GetSpecialType(SpecialType.System_Int64)),
                BinaryOperatorKind.ULong => specialType.Construct(_compilation.GetSpecialType(SpecialType.System_UInt64)),
                BinaryOperatorKind.NInt => specialType.Construct(_compilation.CreateNativeIntegerTypeSymbol(signed: true)),
                BinaryOperatorKind.NUInt => specialType.Construct(_compilation.CreateNativeIntegerTypeSymbol(signed: false)),
                BinaryOperatorKind.Float => specialType.Construct(_compilation.GetSpecialType(SpecialType.System_Single)),
                BinaryOperatorKind.Double => specialType.Construct(_compilation.GetSpecialType(SpecialType.System_Double)),
                BinaryOperatorKind.Decimal => specialType.Construct(_compilation.GetSpecialType(SpecialType.System_Decimal)),
                BinaryOperatorKind.Bool => specialType.Construct(_compilation.GetSpecialType(SpecialType.System_Boolean)),
                _ => null,
            };
        }

        internal static bool IsValidObjectEquality(Conversions Conversions, TypeSymbol leftType, bool leftIsNull, bool leftIsDefault, TypeSymbol rightType, bool rightIsNull, bool rightIsDefault, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if ((object)leftType != null && leftType.IsTypeParameter())
            {
                if (leftType.IsValueType || (!leftType.IsReferenceType && !rightIsNull))
                {
                    return false;
                }
                leftType = ((TypeParameterSymbol)leftType).EffectiveBaseClass(ref useSiteInfo);
            }
            if ((object)rightType != null && rightType.IsTypeParameter())
            {
                if (rightType.IsValueType || (!rightType.IsReferenceType && !leftIsNull))
                {
                    return false;
                }
                rightType = ((TypeParameterSymbol)rightType).EffectiveBaseClass(ref useSiteInfo);
            }
            if (((object)leftType == null || !leftType.IsReferenceType) && !leftIsNull && !leftIsDefault)
            {
                return false;
            }
            if (((object)rightType == null || !rightType.IsReferenceType) && !rightIsNull && !rightIsDefault)
            {
                return false;
            }
            if (leftIsDefault && rightIsDefault)
            {
                return false;
            }
            if (leftIsDefault && rightIsNull)
            {
                return false;
            }
            if (leftIsNull && rightIsDefault)
            {
                return false;
            }
            if (leftIsNull || rightIsNull || leftIsDefault || rightIsDefault)
            {
                return true;
            }
            Conversion conversion = Conversions.ClassifyConversionFromType(leftType, rightType, ref useSiteInfo);
            if (conversion.IsIdentity || conversion.IsReference)
            {
                return true;
            }
            Conversion conversion2 = Conversions.ClassifyConversionFromType(rightType, leftType, ref useSiteInfo);
            if (conversion2.IsIdentity || conversion2.IsReference)
            {
                return true;
            }
            return false;
        }
    }
}
