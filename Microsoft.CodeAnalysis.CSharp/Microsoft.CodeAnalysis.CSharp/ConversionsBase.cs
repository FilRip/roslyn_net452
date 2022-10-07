using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class ConversionsBase
    {
        private static class ConversionEasyOut
        {
            private static readonly byte[,] s_convkind;

            static ConversionEasyOut()
            {
                s_convkind = new byte[32, 32]
                {
                    {
                        2, 26, 27, 27, 27, 27, 27, 27, 27, 27,
                        27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
                        27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
                        27, 27
                    },
                    {
                        12, 2, 1, 1, 1, 1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                        1, 1
                    },
                    {
                        13, 1, 2, 1, 1, 1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1, 1, 1, 10, 1, 1,
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                        1, 1
                    },
                    {
                        13, 1, 1, 2, 23, 23, 3, 3, 23, 3,
                        3, 3, 3, 3, 3, 3, 3, 1, 10, 25,
                        25, 10, 10, 25, 10, 10, 10, 10, 10, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 23, 2, 3, 3, 3, 23, 23,
                        23, 23, 3, 23, 3, 3, 3, 1, 25, 10,
                        10, 10, 10, 25, 25, 25, 25, 10, 25, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 23, 23, 2, 3, 3, 23, 23,
                        23, 23, 3, 23, 3, 3, 3, 1, 25, 25,
                        10, 10, 10, 25, 25, 25, 25, 10, 25, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 23, 23, 23, 2, 3, 23, 23,
                        23, 23, 3, 23, 3, 3, 3, 1, 25, 25,
                        25, 10, 10, 25, 25, 25, 25, 10, 25, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 23, 23, 23, 23, 2, 23, 23,
                        23, 23, 23, 23, 3, 3, 3, 1, 25, 25,
                        25, 25, 10, 25, 25, 25, 25, 25, 25, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 23, 23, 3, 3, 3, 2, 3,
                        3, 3, 3, 3, 3, 3, 3, 1, 25, 25,
                        10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 23, 23, 23, 3, 3, 23, 2,
                        3, 3, 3, 3, 3, 3, 3, 1, 25, 25,
                        25, 10, 10, 25, 10, 10, 10, 10, 10, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 23, 23, 23, 23, 3, 23, 23,
                        2, 3, 23, 3, 3, 3, 3, 1, 25, 25,
                        25, 25, 10, 25, 25, 10, 10, 25, 10, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 23, 23, 23, 23, 23, 23, 23,
                        23, 2, 23, 23, 3, 3, 3, 1, 25, 25,
                        25, 25, 25, 25, 25, 25, 10, 25, 25, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 23, 23, 23, 23, 3, 23, 23,
                        23, 23, 2, 23, 3, 3, 3, 1, 25, 25,
                        25, 25, 10, 25, 25, 25, 25, 10, 25, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 23, 23, 23, 23, 23, 23, 23,
                        23, 3, 23, 2, 3, 3, 3, 1, 25, 25,
                        25, 25, 25, 25, 25, 25, 10, 25, 10, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 23, 23, 23, 23, 23, 23, 23,
                        23, 23, 23, 23, 2, 3, 23, 1, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 25, 25, 10,
                        10, 25
                    },
                    {
                        13, 1, 1, 23, 23, 23, 23, 23, 23, 23,
                        23, 23, 23, 23, 23, 2, 23, 1, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
                        10, 25
                    },
                    {
                        13, 1, 1, 23, 23, 23, 23, 23, 23, 23,
                        23, 23, 23, 23, 23, 23, 2, 1, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
                        25, 10
                    },
                    {
                        13, 1, 25, 1, 1, 1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1, 1, 1, 2, 1, 1,
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                        1, 1
                    },
                    {
                        13, 1, 1, 25, 25, 25, 25, 25, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 1, 2, 25,
                        25, 10, 10, 25, 10, 10, 10, 10, 10, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 25, 25, 25, 25, 25, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 1, 25, 2,
                        10, 10, 10, 25, 25, 25, 25, 10, 25, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 25, 25, 25, 25, 25, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 1, 25, 25,
                        2, 10, 10, 25, 25, 25, 25, 10, 25, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 25, 25, 25, 25, 25, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 1, 25, 25,
                        25, 2, 10, 25, 25, 25, 25, 10, 25, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 25, 25, 25, 25, 25, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 1, 25, 25,
                        25, 25, 2, 25, 25, 25, 25, 25, 25, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 25, 25, 25, 25, 25, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 1, 25, 25,
                        10, 10, 10, 2, 10, 10, 10, 10, 10, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 25, 25, 25, 25, 25, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 1, 25, 25,
                        25, 10, 10, 25, 2, 10, 10, 10, 10, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 25, 25, 25, 25, 25, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 1, 25, 25,
                        25, 25, 10, 25, 25, 2, 10, 25, 10, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 25, 25, 25, 25, 25, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 1, 25, 25,
                        25, 25, 25, 25, 25, 25, 2, 25, 25, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 25, 25, 25, 25, 25, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 1, 25, 25,
                        25, 25, 10, 25, 25, 25, 25, 2, 25, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 25, 25, 25, 25, 25, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 1, 25, 25,
                        25, 25, 25, 25, 25, 25, 10, 25, 2, 10,
                        10, 10
                    },
                    {
                        13, 1, 1, 25, 25, 25, 25, 25, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 1, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 25, 25, 2,
                        10, 25
                    },
                    {
                        13, 1, 1, 25, 25, 25, 25, 25, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 1, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
                        2, 25
                    },
                    {
                        13, 1, 1, 25, 25, 25, 25, 25, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 1, 25, 25,
                        25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
                        25, 2
                    }
                };
            }

            public static ConversionKind ClassifyConversion(TypeSymbol source, TypeSymbol target)
            {
                int num = source.TypeToIndex();
                if (num < 0)
                {
                    return ConversionKind.NoConversion;
                }
                int num2 = target.TypeToIndex();
                if (num2 < 0)
                {
                    return ConversionKind.NoConversion;
                }
                return (ConversionKind)s_convkind[num, num2];
            }
        }

        private delegate Conversion ClassifyConversionFromExpressionDelegate(ConversionsBase conversions, BoundExpression sourceExpression, TypeWithAnnotations destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool arg);

        private delegate Conversion ClassifyConversionFromTypeDelegate(ConversionsBase conversions, TypeWithAnnotations source, TypeWithAnnotations destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool arg);

        private const int MaximumRecursionDepth = 50;

        protected readonly AssemblySymbol corLibrary;

        protected readonly int currentRecursionDepth;

        internal readonly bool IncludeNullability;

        private ConversionsBase _lazyOtherNullability;

        private const bool F = false;

        private const bool T = true;

        private static readonly bool[,] s_implicitNumericConversions = new bool[12, 12]
        {
            {
                false, false, true, false, true, false, true, false, false, true,
                true, true
            },
            {
                false, false, true, true, true, true, true, true, false, true,
                true, true
            },
            {
                false, false, false, false, true, false, true, false, false, true,
                true, true
            },
            {
                false, false, false, false, true, true, true, true, false, true,
                true, true
            },
            {
                false, false, false, false, false, false, true, false, false, true,
                true, true
            },
            {
                false, false, false, false, false, false, true, true, false, true,
                true, true
            },
            {
                false, false, false, false, false, false, false, false, false, true,
                true, true
            },
            {
                false, false, false, false, false, false, false, false, false, true,
                true, true
            },
            {
                false, false, false, true, true, true, true, true, false, true,
                true, true
            },
            {
                false, false, false, false, false, false, false, false, false, false,
                true, false
            },
            {
                false, false, false, false, false, false, false, false, false, false,
                false, false
            },
            {
                false, false, false, false, false, false, false, false, false, false,
                false, false
            }
        };

        private static readonly bool[,] s_explicitNumericConversions = new bool[12, 12]
        {
            {
                false, true, false, true, false, true, false, true, true, false,
                false, false
            },
            {
                true, false, false, false, false, false, false, false, true, false,
                false, false
            },
            {
                true, true, false, true, false, true, false, true, true, false,
                false, false
            },
            {
                true, true, true, false, false, false, false, false, true, false,
                false, false
            },
            {
                true, true, true, true, false, true, false, true, true, false,
                false, false
            },
            {
                true, true, true, true, true, false, false, false, true, false,
                false, false
            },
            {
                true, true, true, true, true, true, false, true, true, false,
                false, false
            },
            {
                true, true, true, true, true, true, true, false, true, false,
                false, false
            },
            {
                true, true, true, false, false, false, false, false, false, false,
                false, false
            },
            {
                true, true, true, true, true, true, true, true, true, false,
                false, true
            },
            {
                true, true, true, true, true, true, true, true, true, true,
                false, true
            },
            {
                true, true, true, true, true, true, true, true, true, true,
                true, false
            }
        };

        internal AssemblySymbol CorLibrary => corLibrary;

        protected ConversionsBase(AssemblySymbol corLibrary, int currentRecursionDepth, bool includeNullability, ConversionsBase otherNullabilityOpt)
        {
            this.corLibrary = corLibrary;
            this.currentRecursionDepth = currentRecursionDepth;
            IncludeNullability = includeNullability;
            _lazyOtherNullability = otherNullabilityOpt;
        }

        internal ConversionsBase WithNullability(bool includeNullability)
        {
            if (IncludeNullability == includeNullability)
            {
                return this;
            }
            if (_lazyOtherNullability == null)
            {
                Interlocked.CompareExchange(ref _lazyOtherNullability, WithNullabilityCore(includeNullability), null);
            }
            return _lazyOtherNullability;
        }

        protected abstract ConversionsBase WithNullabilityCore(bool includeNullability);

        public abstract Conversion GetMethodGroupDelegateConversion(BoundMethodGroup source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo);

        public abstract Conversion GetMethodGroupFunctionPointerConversion(BoundMethodGroup source, FunctionPointerTypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo);

        public abstract Conversion GetStackAllocConversion(BoundStackAllocArrayCreation sourceExpression, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo);

        protected abstract ConversionsBase CreateInstance(int currentRecursionDepth);

        protected abstract Conversion GetInterpolatedStringConversion(BoundUnconvertedInterpolatedString source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo);

        public Conversion ClassifyImplicitConversionFromExpression(BoundExpression sourceExpression, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            TypeSymbol type = sourceExpression.Type;
            if ((object)type != null && HasIdentityConversionInternal(type, destination))
            {
                return Conversion.Identity;
            }
            Conversion result = ClassifyImplicitBuiltInConversionFromExpression(sourceExpression, type, destination, ref useSiteInfo);
            if (result.Exists)
            {
                return result;
            }
            if ((object)type != null)
            {
                Conversion result2 = FastClassifyConversion(type, destination);
                if (result2.Exists)
                {
                    if (result2.IsImplicit)
                    {
                        return result2;
                    }
                }
                else
                {
                    result = ClassifyImplicitBuiltInConversionSlow(type, destination, ref useSiteInfo);
                    if (result.Exists)
                    {
                        return result;
                    }
                }
            }
            result = GetImplicitUserDefinedConversion(sourceExpression, type, destination, ref useSiteInfo);
            if (result.Exists)
            {
                return result;
            }
            result = GetSwitchExpressionConversion(sourceExpression, destination, ref useSiteInfo);
            if (result.Exists)
            {
                return result;
            }
            return GetConditionalExpressionConversion(sourceExpression, destination, ref useSiteInfo);
        }

        public Conversion ClassifyImplicitConversionFromType(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (HasIdentityConversionInternal(source, destination))
            {
                return Conversion.Identity;
            }
            Conversion result = FastClassifyConversion(source, destination);
            if (result.Exists)
            {
                if (!result.IsImplicit)
                {
                    return Conversion.NoConversion;
                }
                return result;
            }
            Conversion result2 = ClassifyImplicitBuiltInConversionSlow(source, destination, ref useSiteInfo);
            if (result2.Exists)
            {
                return result2;
            }
            return GetImplicitUserDefinedConversion(null, source, destination, ref useSiteInfo);
        }

        public Conversion ClassifyConversionFromExpressionType(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (HasImplicitDynamicConversionFromExpression(source, destination))
            {
                return Conversion.ImplicitDynamic;
            }
            return ClassifyConversionFromType(source, destination, ref useSiteInfo);
        }

        private static bool TryGetVoidConversion(TypeSymbol source, TypeSymbol destination, out Conversion conversion)
        {
            bool flag = (object)source != null && source.SpecialType == SpecialType.System_Void;
            bool flag2 = destination.SpecialType == SpecialType.System_Void;
            if (flag && flag2)
            {
                conversion = Conversion.Identity;
                return true;
            }
            if (flag || flag2)
            {
                conversion = Conversion.NoConversion;
                return true;
            }
            conversion = default(Conversion);
            return false;
        }

        public Conversion ClassifyConversionFromExpression(BoundExpression sourceExpression, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool forCast = false)
        {
            if (TryGetVoidConversion(sourceExpression.Type, destination, out var conversion))
            {
                return conversion;
            }
            if (forCast)
            {
                return ClassifyConversionFromExpressionForCast(sourceExpression, destination, ref useSiteInfo);
            }
            Conversion result = ClassifyImplicitConversionFromExpression(sourceExpression, destination, ref useSiteInfo);
            if (result.Exists)
            {
                return result;
            }
            return ClassifyExplicitOnlyConversionFromExpression(sourceExpression, destination, ref useSiteInfo, forCast: false);
        }

        public Conversion ClassifyConversionFromType(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool forCast = false)
        {
            if (TryGetVoidConversion(source, destination, out var conversion))
            {
                return conversion;
            }
            if (forCast)
            {
                return ClassifyConversionFromTypeForCast(source, destination, ref useSiteInfo);
            }
            Conversion result = FastClassifyConversion(source, destination);
            if (result.Exists)
            {
                return result;
            }
            Conversion result2 = ClassifyImplicitBuiltInConversionSlow(source, destination, ref useSiteInfo);
            if (result2.Exists)
            {
                return result2;
            }
            Conversion implicitUserDefinedConversion = GetImplicitUserDefinedConversion(null, source, destination, ref useSiteInfo);
            if (implicitUserDefinedConversion.Exists)
            {
                return implicitUserDefinedConversion;
            }
            implicitUserDefinedConversion = ClassifyExplicitBuiltInOnlyConversion(source, destination, ref useSiteInfo, forCast: false);
            if (implicitUserDefinedConversion.Exists)
            {
                return implicitUserDefinedConversion;
            }
            return GetExplicitUserDefinedConversion(null, source, destination, ref useSiteInfo);
        }

        private Conversion ClassifyConversionFromExpressionForCast(BoundExpression source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            Conversion conversion = ClassifyImplicitConversionFromExpression(source, destination, ref useSiteInfo);
            if (conversion.Exists && !ExplicitConversionMayDifferFromImplicit(conversion))
            {
                return conversion;
            }
            Conversion result = ClassifyExplicitOnlyConversionFromExpression(source, destination, ref useSiteInfo, forCast: true);
            if (result.Exists)
            {
                return result;
            }
            return conversion;
        }

        private Conversion ClassifyConversionFromTypeForCast(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            Conversion result = FastClassifyConversion(source, destination);
            if (result.Exists)
            {
                return result;
            }
            Conversion conversion = ClassifyImplicitBuiltInConversionSlow(source, destination, ref useSiteInfo);
            if (conversion.Exists && !ExplicitConversionMayDifferFromImplicit(conversion))
            {
                return conversion;
            }
            Conversion result2 = ClassifyExplicitBuiltInOnlyConversion(source, destination, ref useSiteInfo, forCast: true);
            if (result2.Exists)
            {
                return result2;
            }
            if (conversion.Exists)
            {
                return conversion;
            }
            Conversion explicitUserDefinedConversion = GetExplicitUserDefinedConversion(null, source, destination, ref useSiteInfo);
            if (explicitUserDefinedConversion.Exists)
            {
                return explicitUserDefinedConversion;
            }
            return GetImplicitUserDefinedConversion(null, source, destination, ref useSiteInfo);
        }

        public static Conversion FastClassifyConversion(TypeSymbol source, TypeSymbol target)
        {
            ConversionKind conversionKind = ConversionEasyOut.ClassifyConversion(source, target);
            if (conversionKind != ConversionKind.ImplicitNullable && conversionKind != ConversionKind.ExplicitNullable)
            {
                return Conversion.GetTrivialConversion(conversionKind);
            }
            return Conversion.MakeNullableConversion(conversionKind, FastClassifyConversion(source.StrippedType(), target.StrippedType()));
        }

        public Conversion ClassifyBuiltInConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            Conversion result = FastClassifyConversion(source, destination);
            if (result.Exists)
            {
                return result;
            }
            Conversion result2 = ClassifyImplicitBuiltInConversionSlow(source, destination, ref useSiteInfo);
            if (result2.Exists)
            {
                return result2;
            }
            return ClassifyExplicitBuiltInOnlyConversion(source, destination, ref useSiteInfo, forCast: false);
        }

        public Conversion ClassifyStandardConversion(BoundExpression sourceExpression, TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            Conversion result = ClassifyStandardImplicitConversion(sourceExpression, source, destination, ref useSiteInfo);
            if (result.Exists)
            {
                return result;
            }
            if ((object)source != null)
            {
                return DeriveStandardExplicitFromOppositeStandardImplicitConversion(source, destination, ref useSiteInfo);
            }
            return Conversion.NoConversion;
        }

        private Conversion ClassifyStandardImplicitConversion(BoundExpression sourceExpression, TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            Conversion result = ClassifyImplicitBuiltInConversionFromExpression(sourceExpression, source, destination, ref useSiteInfo);
            if (result.Exists)
            {
                return result;
            }
            if ((object)source != null)
            {
                return ClassifyStandardImplicitConversion(source, destination, ref useSiteInfo);
            }
            return Conversion.NoConversion;
        }

        private Conversion ClassifyStandardImplicitConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (HasIdentityConversionInternal(source, destination))
            {
                return Conversion.Identity;
            }
            if (HasImplicitNumericConversion(source, destination))
            {
                return Conversion.ImplicitNumeric;
            }
            Conversion result = ClassifyImplicitNullableConversion(source, destination, ref useSiteInfo);
            if (result.Exists)
            {
                return result;
            }
            if (HasImplicitReferenceConversion(source, destination, ref useSiteInfo))
            {
                return Conversion.ImplicitReference;
            }
            if (HasBoxingConversion(source, destination, ref useSiteInfo))
            {
                return Conversion.Boxing;
            }
            if (HasImplicitPointerToVoidConversion(source, destination))
            {
                return Conversion.PointerToVoid;
            }
            if (HasImplicitPointerConversion(source, destination, ref useSiteInfo))
            {
                return Conversion.ImplicitPointer;
            }
            Conversion result2 = ClassifyImplicitTupleConversion(source, destination, ref useSiteInfo);
            if (result2.Exists)
            {
                return result2;
            }
            return Conversion.NoConversion;
        }

        private Conversion ClassifyImplicitBuiltInConversionSlow(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source.IsVoidType() || destination.IsVoidType())
            {
                return Conversion.NoConversion;
            }
            Conversion result = ClassifyStandardImplicitConversion(source, destination, ref useSiteInfo);
            if (result.Exists)
            {
                return result;
            }
            return Conversion.NoConversion;
        }

        private Conversion GetImplicitUserDefinedConversion(BoundExpression sourceExpression, TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return new Conversion(AnalyzeImplicitUserDefinedConversions(sourceExpression, source, destination, ref useSiteInfo), isImplicit: true);
        }

        private Conversion ClassifyExplicitBuiltInOnlyConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool forCast)
        {
            if (source.IsVoidType() || destination.IsVoidType())
            {
                return Conversion.NoConversion;
            }
            if (HasSpecialIntPtrConversion(source, destination))
            {
                return Conversion.IntPtr;
            }
            if (HasExplicitEnumerationConversion(source, destination))
            {
                return Conversion.ExplicitEnumeration;
            }
            Conversion result = ClassifyExplicitNullableConversion(source, destination, ref useSiteInfo, forCast);
            if (result.Exists)
            {
                return result;
            }
            if (HasExplicitReferenceConversion(source, destination, ref useSiteInfo))
            {
                if (source.Kind != SymbolKind.DynamicType)
                {
                    return Conversion.ExplicitReference;
                }
                return Conversion.ExplicitDynamic;
            }
            if (HasUnboxingConversion(source, destination, ref useSiteInfo))
            {
                return Conversion.Unboxing;
            }
            Conversion result2 = ClassifyExplicitTupleConversion(source, destination, ref useSiteInfo, forCast);
            if (result2.Exists)
            {
                return result2;
            }
            if (HasPointerToPointerConversion(source, destination))
            {
                return Conversion.PointerToPointer;
            }
            if (HasPointerToIntegerConversion(source, destination))
            {
                return Conversion.PointerToInteger;
            }
            if (HasIntegerToPointerConversion(source, destination))
            {
                return Conversion.IntegerToPointer;
            }
            if (HasExplicitDynamicConversion(source, destination))
            {
                return Conversion.ExplicitDynamic;
            }
            return Conversion.NoConversion;
        }

        private Conversion GetExplicitUserDefinedConversion(BoundExpression sourceExpression, TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return new Conversion(AnalyzeExplicitUserDefinedConversions(sourceExpression, source, destination, ref useSiteInfo), isImplicit: false);
        }

        private Conversion DeriveStandardExplicitFromOppositeStandardImplicitConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            Conversion conversion = ClassifyStandardImplicitConversion(destination, source, ref useSiteInfo);
            switch (conversion.Kind)
            {
                case ConversionKind.Identity:
                    return Conversion.Identity;
                case ConversionKind.ImplicitNumeric:
                    return Conversion.ExplicitNumeric;
                case ConversionKind.ImplicitReference:
                    return Conversion.ExplicitReference;
                case ConversionKind.Boxing:
                    return Conversion.Unboxing;
                case ConversionKind.NoConversion:
                    return Conversion.NoConversion;
                case ConversionKind.ImplicitPointerToVoid:
                    return Conversion.PointerToPointer;
                case ConversionKind.ImplicitTuple:
                    return Conversion.NoConversion;
                case ConversionKind.ImplicitNullable:
                    {
                        TypeSymbol source2 = source.StrippedType();
                        TypeSymbol destination2 = destination.StrippedType();
                        Conversion nestedConversion = DeriveStandardExplicitFromOppositeStandardImplicitConversion(source2, destination2, ref useSiteInfo);
                        return nestedConversion.Exists ? Conversion.MakeNullableConversion(ConversionKind.ExplicitNullable, nestedConversion) : Conversion.NoConversion;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(conversion.Kind);
            }
        }

        public bool IsBaseInterface(TypeSymbol baseType, TypeSymbol derivedType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!baseType.IsInterfaceType())
            {
                return false;
            }
            if (!(derivedType is NamedTypeSymbol namedTypeSymbol))
            {
                return false;
            }
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = namedTypeSymbol.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                if (HasIdentityConversionInternal(current, baseType))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsBaseClass(TypeSymbol derivedType, TypeSymbol baseType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!baseType.IsClassType())
            {
                return false;
            }
            TypeSymbol typeSymbol = derivedType.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            while ((object)typeSymbol != null)
            {
                if (HasIdentityConversionInternal(typeSymbol, baseType))
                {
                    return true;
                }
                typeSymbol = typeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }
            return false;
        }

        private static bool ExplicitConversionMayDifferFromImplicit(Conversion implicitConversion)
        {
            switch (implicitConversion.Kind)
            {
                case ConversionKind.ImplicitTupleLiteral:
                case ConversionKind.ImplicitTuple:
                case ConversionKind.ImplicitNullable:
                case ConversionKind.ImplicitDynamic:
                case ConversionKind.ImplicitUserDefined:
                case ConversionKind.ConditionalExpression:
                    return true;
                default:
                    return false;
            }
        }

        private Conversion ClassifyImplicitBuiltInConversionFromExpression(BoundExpression sourceExpression, TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (HasImplicitDynamicConversionFromExpression(source, destination))
            {
                return Conversion.ImplicitDynamic;
            }
            if (sourceExpression == null)
            {
                return Conversion.NoConversion;
            }
            if (HasImplicitEnumerationConversion(sourceExpression, destination))
            {
                return Conversion.ImplicitEnumeration;
            }
            Conversion result = ClassifyImplicitConstantExpressionConversion(sourceExpression, destination);
            if (result.Exists)
            {
                return result;
            }
            switch (sourceExpression.Kind)
            {
                case BoundKind.Literal:
                    {
                        Conversion result2 = ClassifyNullLiteralConversion(sourceExpression, destination);
                        if (result2.Exists)
                        {
                            return result2;
                        }
                        break;
                    }
                case BoundKind.DefaultLiteral:
                    return Conversion.DefaultLiteral;
                case BoundKind.ExpressionWithNullability:
                    {
                        BoundExpression expression = ((BoundExpressionWithNullability)sourceExpression).Expression;
                        Conversion result4 = ClassifyImplicitBuiltInConversionFromExpression(expression, expression.Type, destination, ref useSiteInfo);
                        if (result4.Exists)
                        {
                            return result4;
                        }
                        break;
                    }
                case BoundKind.TupleLiteral:
                    {
                        Conversion result3 = ClassifyImplicitTupleLiteralConversion((BoundTupleLiteral)sourceExpression, destination, ref useSiteInfo);
                        if (result3.Exists)
                        {
                            return result3;
                        }
                        break;
                    }
                case BoundKind.UnboundLambda:
                    if (HasAnonymousFunctionConversion(sourceExpression, destination))
                    {
                        return Conversion.AnonymousFunction;
                    }
                    break;
                case BoundKind.MethodGroup:
                    {
                        Conversion methodGroupDelegateConversion = GetMethodGroupDelegateConversion((BoundMethodGroup)sourceExpression, destination, ref useSiteInfo);
                        if (methodGroupDelegateConversion.Exists)
                        {
                            return methodGroupDelegateConversion;
                        }
                        break;
                    }
                case BoundKind.UnconvertedInterpolatedString:
                    {
                        Conversion interpolatedStringConversion = GetInterpolatedStringConversion((BoundUnconvertedInterpolatedString)sourceExpression, destination, ref useSiteInfo);
                        if (interpolatedStringConversion.Exists)
                        {
                            return interpolatedStringConversion;
                        }
                        break;
                    }
                case BoundKind.StackAllocArrayCreation:
                    {
                        Conversion stackAllocConversion = GetStackAllocConversion((BoundStackAllocArrayCreation)sourceExpression, destination, ref useSiteInfo);
                        if (stackAllocConversion.Exists)
                        {
                            return stackAllocConversion;
                        }
                        break;
                    }
                case BoundKind.UnconvertedAddressOfOperator:
                    if (destination is FunctionPointerTypeSymbol destination2)
                    {
                        Conversion methodGroupFunctionPointerConversion = GetMethodGroupFunctionPointerConversion(((BoundUnconvertedAddressOfOperator)sourceExpression).Operand, destination2, ref useSiteInfo);
                        if (methodGroupFunctionPointerConversion.Exists)
                        {
                            return methodGroupFunctionPointerConversion;
                        }
                    }
                    break;
                case BoundKind.ThrowExpression:
                    return Conversion.ImplicitThrow;
                case BoundKind.UnconvertedObjectCreationExpression:
                    return Conversion.ObjectCreation;
            }
            return Conversion.NoConversion;
        }

        private Conversion GetSwitchExpressionConversion(BoundExpression source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!(source is BoundConvertedSwitchExpression))
            {
                if (source is BoundUnconvertedSwitchExpression boundUnconvertedSwitchExpression)
                {
                    ArrayBuilder<Conversion> instance = ArrayBuilder<Conversion>.GetInstance(boundUnconvertedSwitchExpression.SwitchArms.Length);
                    ImmutableArray<BoundSwitchExpressionArm>.Enumerator enumerator = boundUnconvertedSwitchExpression.SwitchArms.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        BoundSwitchExpressionArm current = enumerator.Current;
                        Conversion item = ClassifyImplicitConversionFromExpression(current.Value, destination, ref useSiteInfo);
                        if (!item.Exists)
                        {
                            instance.Free();
                            return Conversion.NoConversion;
                        }
                        instance.Add(item);
                    }
                    return Conversion.MakeSwitchExpression(instance.ToImmutableAndFree());
                }
                return Conversion.NoConversion;
            }
            return Conversion.NoConversion;
        }

        private Conversion GetConditionalExpressionConversion(BoundExpression source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!(source is BoundUnconvertedConditionalOperator boundUnconvertedConditionalOperator))
            {
                return Conversion.NoConversion;
            }
            Conversion item = ClassifyImplicitConversionFromExpression(boundUnconvertedConditionalOperator.Consequence, destination, ref useSiteInfo);
            if (!item.Exists)
            {
                return Conversion.NoConversion;
            }
            Conversion item2 = ClassifyImplicitConversionFromExpression(boundUnconvertedConditionalOperator.Alternative, destination, ref useSiteInfo);
            if (!item2.Exists)
            {
                return Conversion.NoConversion;
            }
            return Conversion.MakeConditionalExpression(ImmutableArray.Create(item, item2));
        }

        private static Conversion ClassifyNullLiteralConversion(BoundExpression source, TypeSymbol destination)
        {
            if (!source.IsLiteralNull())
            {
                return Conversion.NoConversion;
            }
            if (destination.IsNullableType())
            {
                return Conversion.NullLiteral;
            }
            if (destination.IsReferenceType)
            {
                return Conversion.ImplicitReference;
            }
            if (destination.IsPointerOrFunctionPointer())
            {
                return Conversion.NullToPointer;
            }
            return Conversion.NoConversion;
        }

        private static Conversion ClassifyImplicitConstantExpressionConversion(BoundExpression source, TypeSymbol destination)
        {
            if (HasImplicitConstantExpressionConversion(source, destination))
            {
                return Conversion.ImplicitConstant;
            }
            if (destination.Kind == SymbolKind.NamedType)
            {
                NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)destination;
                if (namedTypeSymbol.OriginalDefinition.GetSpecialTypeSafe() == SpecialType.System_Nullable_T && HasImplicitConstantExpressionConversion(source, namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0].Type))
                {
                    return new Conversion(ConversionKind.ImplicitNullable, Conversion.ImplicitConstantUnderlying);
                }
            }
            return Conversion.NoConversion;
        }

        private Conversion ClassifyImplicitTupleLiteralConversion(BoundTupleLiteral source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            Conversion implicitTupleLiteralConversion = GetImplicitTupleLiteralConversion(source, destination, ref useSiteInfo);
            if (implicitTupleLiteralConversion.Exists)
            {
                return implicitTupleLiteralConversion;
            }
            if (destination.Kind == SymbolKind.NamedType)
            {
                NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)destination;
                if (namedTypeSymbol.OriginalDefinition.GetSpecialTypeSafe() == SpecialType.System_Nullable_T)
                {
                    Conversion implicitTupleLiteralConversion2 = GetImplicitTupleLiteralConversion(source, namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0].Type, ref useSiteInfo);
                    if (implicitTupleLiteralConversion2.Exists)
                    {
                        return new Conversion(ConversionKind.ImplicitNullable, ImmutableArray.Create(implicitTupleLiteralConversion2));
                    }
                }
            }
            return Conversion.NoConversion;
        }

        private Conversion ClassifyExplicitTupleLiteralConversion(BoundTupleLiteral source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool forCast)
        {
            Conversion explicitTupleLiteralConversion = GetExplicitTupleLiteralConversion(source, destination, ref useSiteInfo, forCast);
            if (explicitTupleLiteralConversion.Exists)
            {
                return explicitTupleLiteralConversion;
            }
            if (destination.Kind == SymbolKind.NamedType)
            {
                NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)destination;
                if (namedTypeSymbol.OriginalDefinition.GetSpecialTypeSafe() == SpecialType.System_Nullable_T)
                {
                    Conversion explicitTupleLiteralConversion2 = GetExplicitTupleLiteralConversion(source, namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0].Type, ref useSiteInfo, forCast);
                    if (explicitTupleLiteralConversion2.Exists)
                    {
                        return new Conversion(ConversionKind.ExplicitNullable, ImmutableArray.Create(explicitTupleLiteralConversion2));
                    }
                }
            }
            return Conversion.NoConversion;
        }

        internal static bool HasImplicitConstantExpressionConversion(BoundExpression source, TypeSymbol destination)
        {
            ConstantValue constantValue = source.ConstantValue;
            if (constantValue == null || (object)source.Type == null)
            {
                return false;
            }
            switch (source.Type.GetSpecialTypeSafe())
            {
                case SpecialType.System_Int32:
                    {
                        int num = ((!constantValue.IsBad) ? constantValue.Int32Value : 0);
                        switch (destination.GetSpecialTypeSafe())
                        {
                            case SpecialType.System_Byte:
                                if (0 <= num)
                                {
                                    return num <= 255;
                                }
                                return false;
                            case SpecialType.System_SByte:
                                if (-128 <= num)
                                {
                                    return num <= 127;
                                }
                                return false;
                            case SpecialType.System_Int16:
                                if (-32768 <= num)
                                {
                                    return num <= 32767;
                                }
                                return false;
                            case SpecialType.System_IntPtr:
                                if (destination.IsNativeIntegerType)
                                {
                                    return true;
                                }
                                break;
                            case SpecialType.System_UIntPtr:
                                if (!destination.IsNativeIntegerType)
                                {
                                    break;
                                }
                                goto case SpecialType.System_UInt32;
                            case SpecialType.System_UInt32:
                                return 0L <= num;
                            case SpecialType.System_UInt64:
                                return 0 <= num;
                            case SpecialType.System_UInt16:
                                if (0 <= num)
                                {
                                    return num <= 65535;
                                }
                                return false;
                        }
                        return false;
                    }
                case SpecialType.System_Int64:
                    if (destination.GetSpecialTypeSafe() == SpecialType.System_UInt64 && (constantValue.IsBad || 0 <= constantValue.Int64Value))
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        private Conversion ClassifyExplicitOnlyConversionFromExpression(BoundExpression sourceExpression, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool forCast)
        {
            TypeSymbol type = sourceExpression.Type;
            if (sourceExpression.Kind == BoundKind.TupleLiteral)
            {
                Conversion result = ClassifyExplicitTupleLiteralConversion((BoundTupleLiteral)sourceExpression, destination, ref useSiteInfo, forCast);
                if (result.Exists)
                {
                    return result;
                }
            }
            if ((object)type != null)
            {
                Conversion result2 = FastClassifyConversion(type, destination);
                if (result2.Exists)
                {
                    return result2;
                }
                Conversion result3 = ClassifyExplicitBuiltInOnlyConversion(type, destination, ref useSiteInfo, forCast);
                if (result3.Exists)
                {
                    return result3;
                }
            }
            return GetExplicitUserDefinedConversion(sourceExpression, type, destination, ref useSiteInfo);
        }

        private static bool HasImplicitEnumerationConversion(BoundExpression source, TypeSymbol destination)
        {
            if (!destination.IsEnumType() && (!destination.IsNullableType() || !destination.GetNullableUnderlyingType().IsEnumType()))
            {
                return false;
            }
            ConstantValue constantValue = source.ConstantValue;
            if (constantValue != null && (object)source.Type != null && IsNumericType(source.Type))
            {
                return IsConstantNumericZero(constantValue);
            }
            return false;
        }

        private static LambdaConversionResult IsAnonymousFunctionCompatibleWithDelegate(UnboundLambda anonymousFunction, TypeSymbol type)
        {
            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type;
            MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
            if ((object)delegateInvokeMethod == null || delegateInvokeMethod.HasUseSiteError)
            {
                return LambdaConversionResult.BadTargetType;
            }
            ImmutableArray<ParameterSymbol> parameters = delegateInvokeMethod.Parameters;
            if (anonymousFunction.HasSignature)
            {
                if (anonymousFunction.ParameterCount != delegateInvokeMethod.ParameterCount)
                {
                    return LambdaConversionResult.BadParameterCount;
                }
                if (anonymousFunction.HasExplicitlyTypedParameterList)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (parameters[i].RefKind != anonymousFunction.RefKind(i) || !parameters[i].Type.Equals(anonymousFunction.ParameterType(i), TypeCompareKind.AllIgnoreOptions))
                        {
                            return LambdaConversionResult.MismatchedParameterType;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < parameters.Length; j++)
                    {
                        if (parameters[j].RefKind != 0)
                        {
                            return LambdaConversionResult.RefInImplicitlyTypedLambda;
                        }
                    }
                    for (int k = 0; k < parameters.Length; k++)
                    {
                        if (parameters[k].TypeWithAnnotations.IsStatic)
                        {
                            return LambdaConversionResult.StaticTypeInImplicitlyTypedLambda;
                        }
                    }
                }
            }
            else
            {
                for (int l = 0; l < parameters.Length; l++)
                {
                    if (parameters[l].RefKind == RefKind.Out)
                    {
                        return LambdaConversionResult.MissingSignatureWithOutParameter;
                    }
                }
            }
            if (ErrorFacts.PreventsSuccessfulDelegateConversion(anonymousFunction.Bind(namedTypeSymbol).Diagnostics.Diagnostics))
            {
                return LambdaConversionResult.BindingFailed;
            }
            return LambdaConversionResult.Success;
        }

        private static LambdaConversionResult IsAnonymousFunctionCompatibleWithExpressionTree(UnboundLambda anonymousFunction, NamedTypeSymbol type)
        {
            TypeSymbol typeSymbol = ((type.Arity == 0) ? null : type.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0].Type);
            if ((object)typeSymbol != null && !typeSymbol.IsDelegateType())
            {
                return LambdaConversionResult.ExpressionTreeMustHaveDelegateTypeArgument;
            }
            if (anonymousFunction.Syntax.Kind() == SyntaxKind.AnonymousMethodExpression)
            {
                return LambdaConversionResult.ExpressionTreeFromAnonymousMethod;
            }
            if ((object)typeSymbol == null)
            {
                return LambdaConversionResult.Success;
            }
            return IsAnonymousFunctionCompatibleWithDelegate(anonymousFunction, typeSymbol);
        }

        public static LambdaConversionResult IsAnonymousFunctionCompatibleWithType(UnboundLambda anonymousFunction, TypeSymbol type)
        {
            if (type.SpecialType == SpecialType.System_Delegate)
            {
                if (IsFeatureInferredDelegateTypeEnabled(anonymousFunction))
                {
                    return LambdaConversionResult.Success;
                }
            }
            else
            {
                if (type.IsDelegateType())
                {
                    return IsAnonymousFunctionCompatibleWithDelegate(anonymousFunction, type);
                }
                if (type.IsGenericOrNonGenericExpressionType(out var isGenericType) && (isGenericType || IsFeatureInferredDelegateTypeEnabled(anonymousFunction)))
                {
                    return IsAnonymousFunctionCompatibleWithExpressionTree(anonymousFunction, (NamedTypeSymbol)type);
                }
            }
            return LambdaConversionResult.BadTargetType;
        }

        internal static bool IsFeatureInferredDelegateTypeEnabled(BoundExpression expr)
        {
            return expr.Syntax.IsFeatureEnabled(MessageID.IDS_FeatureInferredDelegateType);
        }

        private static bool HasAnonymousFunctionConversion(BoundExpression source, TypeSymbol destination)
        {
            if (source.Kind != BoundKind.UnboundLambda)
            {
                return false;
            }
            return IsAnonymousFunctionCompatibleWithType((UnboundLambda)source, destination) == LambdaConversionResult.Success;
        }

        internal Conversion ClassifyImplicitUserDefinedConversionForV6SwitchGoverningType(TypeSymbol sourceType, out TypeSymbol switchGoverningType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            UserDefinedConversionResult conversionResult = AnalyzeImplicitUserDefinedConversionForV6SwitchGoverningType(sourceType, ref useSiteInfo);
            if (conversionResult.Kind == UserDefinedConversionResultKind.Valid)
            {
                UserDefinedConversionAnalysis userDefinedConversionAnalysis = conversionResult.Results[conversionResult.Best];
                switchGoverningType = userDefinedConversionAnalysis.ToType;
            }
            else
            {
                switchGoverningType = null;
            }
            return new Conversion(conversionResult, isImplicit: true);
        }

        internal Conversion GetCallerLineNumberConversion(TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax syntax = new Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax(new Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LiteralExpressionSyntax(SyntaxKind.NumericLiteralExpression, new Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken(SyntaxKind.NumericLiteralToken)), null, 0);
            TypeSymbol specialType = corLibrary.GetSpecialType(SpecialType.System_Int32);
            BoundLiteral sourceExpression = new BoundLiteral(syntax, ConstantValue.Create(int.MaxValue), specialType);
            return ClassifyStandardImplicitConversion(sourceExpression, specialType, destination, ref useSiteInfo);
        }

        internal bool HasCallerLineNumberConversion(TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return GetCallerLineNumberConversion(destination, ref useSiteInfo).Exists;
        }

        internal bool HasCallerInfoStringConversion(TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            TypeSymbol specialType = corLibrary.GetSpecialType(SpecialType.System_String);
            return ClassifyStandardImplicitConversion(specialType, destination, ref useSiteInfo).Exists;
        }

        public static bool HasIdentityConversion(TypeSymbol type1, TypeSymbol type2)
        {
            return HasIdentityConversionInternal(type1, type2, includeNullability: false);
        }

        private static bool HasIdentityConversionInternal(TypeSymbol type1, TypeSymbol type2, bool includeNullability)
        {
            TypeCompareKind compareKind = (includeNullability ? (TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds | TypeCompareKind.ObliviousNullableModifierMatchesAny | TypeCompareKind.IgnoreNativeIntegers) : TypeCompareKind.AllIgnoreOptions);
            return type1.Equals(type2, compareKind);
        }

        private bool HasIdentityConversionInternal(TypeSymbol type1, TypeSymbol type2)
        {
            return HasIdentityConversionInternal(type1, type2, IncludeNullability);
        }

        internal bool HasTopLevelNullabilityIdentityConversion(TypeWithAnnotations source, TypeWithAnnotations destination)
        {
            if (!IncludeNullability)
            {
                return true;
            }
            if (source.NullableAnnotation.IsOblivious() || destination.NullableAnnotation.IsOblivious())
            {
                return true;
            }
            bool flag = IsPossiblyNullableTypeTypeParameter(in source);
            bool flag2 = IsPossiblyNullableTypeTypeParameter(in destination);
            if (flag && !flag2)
            {
                return destination.NullableAnnotation.IsAnnotated();
            }
            if (flag2 && !flag)
            {
                return source.NullableAnnotation.IsAnnotated();
            }
            return source.NullableAnnotation.IsAnnotated() == destination.NullableAnnotation.IsAnnotated();
        }

        internal bool HasTopLevelNullabilityImplicitConversion(TypeWithAnnotations source, TypeWithAnnotations destination)
        {
            if (!IncludeNullability)
            {
                return true;
            }
            if (source.NullableAnnotation.IsOblivious() || destination.NullableAnnotation.IsOblivious() || destination.NullableAnnotation.IsAnnotated())
            {
                return true;
            }
            if (IsPossiblyNullableTypeTypeParameter(in source) && !IsPossiblyNullableTypeTypeParameter(in destination))
            {
                return false;
            }
            return !source.NullableAnnotation.IsAnnotated();
        }

        private static bool IsPossiblyNullableTypeTypeParameter(in TypeWithAnnotations typeWithAnnotations)
        {
            TypeSymbol type = typeWithAnnotations.Type;
            if ((object)type != null)
            {
                if (!type.IsPossiblyNullableReferenceTypeTypeParameter())
                {
                    return type.IsNullableTypeOrTypeParameter();
                }
                return true;
            }
            return false;
        }

        public bool HasAnyNullabilityImplicitConversion(TypeWithAnnotations source, TypeWithAnnotations destination)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            if (HasTopLevelNullabilityImplicitConversion(source, destination))
            {
                return ClassifyImplicitConversionFromType(source.Type, destination.Type, ref useSiteInfo).Kind != ConversionKind.NoConversion;
            }
            return false;
        }

        public static bool HasIdentityConversionToAny<T>(T type, ArrayBuilder<T> targetTypes) where T : TypeSymbol
        {
            ArrayBuilder<T>.Enumerator enumerator = targetTypes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                if (HasIdentityConversionInternal(type, current, includeNullability: false))
                {
                    return true;
                }
            }
            return false;
        }

        public Conversion ConvertExtensionMethodThisArg(TypeSymbol parameterType, TypeSymbol thisType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            Conversion conversion = ClassifyImplicitExtensionMethodThisArgConversion(null, thisType, parameterType, ref useSiteInfo);
            if (!IsValidExtensionMethodThisArgConversion(conversion))
            {
                return Conversion.NoConversion;
            }
            return conversion;
        }

        public Conversion ClassifyImplicitExtensionMethodThisArgConversion(BoundExpression sourceExpressionOpt, TypeSymbol sourceType, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if ((object)sourceType != null)
            {
                if (HasIdentityConversionInternal(sourceType, destination))
                {
                    return Conversion.Identity;
                }
                if (HasBoxingConversion(sourceType, destination, ref useSiteInfo))
                {
                    return Conversion.Boxing;
                }
                if (HasImplicitReferenceConversion(sourceType, destination, ref useSiteInfo))
                {
                    return Conversion.ImplicitReference;
                }
            }
            if (sourceExpressionOpt != null && sourceExpressionOpt.Kind == BoundKind.TupleLiteral)
            {
                Conversion tupleLiteralConversion = GetTupleLiteralConversion((BoundTupleLiteral)sourceExpressionOpt, destination, ref useSiteInfo, ConversionKind.ImplicitTupleLiteral, delegate (ConversionsBase conversions, BoundExpression s, TypeWithAnnotations d, ref CompoundUseSiteInfo<AssemblySymbol> u, bool a)
                {
                    return conversions.ClassifyImplicitExtensionMethodThisArgConversion(s, s.Type, d.Type, ref u);
                }, arg: false);
                if (tupleLiteralConversion.Exists)
                {
                    return tupleLiteralConversion;
                }
            }
            if ((object)sourceType != null)
            {
                Conversion result = ClassifyTupleConversion(sourceType, destination, ref useSiteInfo, ConversionKind.ImplicitTuple, delegate (ConversionsBase conversions, TypeWithAnnotations s, TypeWithAnnotations d, ref CompoundUseSiteInfo<AssemblySymbol> u, bool a)
                {
                    return (!conversions.HasTopLevelNullabilityImplicitConversion(s, d)) ? Conversion.NoConversion : conversions.ClassifyImplicitExtensionMethodThisArgConversion(null, s.Type, d.Type, ref u);
                }, arg: false);
                if (result.Exists)
                {
                    return result;
                }
            }
            return Conversion.NoConversion;
        }

        public static bool IsValidExtensionMethodThisArgConversion(Conversion conversion)
        {
            switch (conversion.Kind)
            {
                case ConversionKind.Identity:
                case ConversionKind.ImplicitReference:
                case ConversionKind.Boxing:
                    return true;
                case ConversionKind.ImplicitTupleLiteral:
                case ConversionKind.ImplicitTuple:
                    {
                        ImmutableArray<Conversion>.Enumerator enumerator = conversion.UnderlyingConversions.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (!IsValidExtensionMethodThisArgConversion(enumerator.Current))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                default:
                    return false;
            }
        }

        private static int GetNumericTypeIndex(SpecialType specialType)
        {
            return specialType switch
            {
                SpecialType.System_SByte => 0,
                SpecialType.System_Byte => 1,
                SpecialType.System_Int16 => 2,
                SpecialType.System_UInt16 => 3,
                SpecialType.System_Int32 => 4,
                SpecialType.System_UInt32 => 5,
                SpecialType.System_Int64 => 6,
                SpecialType.System_UInt64 => 7,
                SpecialType.System_Char => 8,
                SpecialType.System_Single => 9,
                SpecialType.System_Double => 10,
                SpecialType.System_Decimal => 11,
                _ => -1,
            };
        }

        private static bool HasImplicitNumericConversion(TypeSymbol source, TypeSymbol destination)
        {
            int numericTypeIndex = GetNumericTypeIndex(source.SpecialType);
            if (numericTypeIndex < 0)
            {
                return false;
            }
            int numericTypeIndex2 = GetNumericTypeIndex(destination.SpecialType);
            if (numericTypeIndex2 < 0)
            {
                return false;
            }
            return s_implicitNumericConversions[numericTypeIndex, numericTypeIndex2];
        }

        private static bool HasExplicitNumericConversion(TypeSymbol source, TypeSymbol destination)
        {
            int numericTypeIndex = GetNumericTypeIndex(source.SpecialType);
            if (numericTypeIndex < 0)
            {
                return false;
            }
            int numericTypeIndex2 = GetNumericTypeIndex(destination.SpecialType);
            if (numericTypeIndex2 < 0)
            {
                return false;
            }
            return s_explicitNumericConversions[numericTypeIndex, numericTypeIndex2];
        }

        private static bool IsConstantNumericZero(ConstantValue value)
        {
            switch (value.Discriminator)
            {
                case ConstantValueTypeDiscriminator.SByte:
                    return value.SByteValue == 0;
                case ConstantValueTypeDiscriminator.Byte:
                    return value.ByteValue == 0;
                case ConstantValueTypeDiscriminator.Int16:
                    return value.Int16Value == 0;
                case ConstantValueTypeDiscriminator.Int32:
                case ConstantValueTypeDiscriminator.NInt:
                    return value.Int32Value == 0;
                case ConstantValueTypeDiscriminator.Int64:
                    return value.Int64Value == 0;
                case ConstantValueTypeDiscriminator.UInt16:
                    return value.UInt16Value == 0;
                case ConstantValueTypeDiscriminator.UInt32:
                case ConstantValueTypeDiscriminator.NUInt:
                    return value.UInt32Value == 0;
                case ConstantValueTypeDiscriminator.UInt64:
                    return value.UInt64Value == 0;
                case ConstantValueTypeDiscriminator.Single:
                case ConstantValueTypeDiscriminator.Double:
                    return value.DoubleValue == 0.0;
                case ConstantValueTypeDiscriminator.Decimal:
                    return value.DecimalValue == 0m;
                default:
                    return false;
            }
        }

        private static bool IsNumericType(TypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_IntPtr:
                    if (!type.IsNativeIntegerType)
                    {
                        break;
                    }
                    goto case SpecialType.System_Char;
                case SpecialType.System_UIntPtr:
                    if (!type.IsNativeIntegerType)
                    {
                        break;
                    }
                    goto case SpecialType.System_Char;
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
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                    return true;
            }
            return false;
        }

        private static bool HasSpecialIntPtrConversion(TypeSymbol source, TypeSymbol target)
        {
            TypeSymbol typeSymbol = source.StrippedType();
            TypeSymbol typeSymbol2 = target.StrippedType();
            TypeSymbol typeSymbol3;
            if (isIntPtrOrUIntPtr(typeSymbol))
            {
                typeSymbol3 = typeSymbol2;
            }
            else
            {
                if (!isIntPtrOrUIntPtr(typeSymbol2))
                {
                    return false;
                }
                typeSymbol3 = typeSymbol;
            }
            if (typeSymbol3.IsPointerOrFunctionPointer())
            {
                return true;
            }
            if (typeSymbol3.TypeKind == TypeKind.Enum)
            {
                return true;
            }
            SpecialType specialType = typeSymbol3.SpecialType;
            if ((uint)(specialType - 8) <= 11u)
            {
                return true;
            }
            return false;
            static bool isIntPtrOrUIntPtr(TypeSymbol type)
            {
                if (type.SpecialType == SpecialType.System_IntPtr || type.SpecialType == SpecialType.System_UIntPtr)
                {
                    return !type.IsNativeIntegerType;
                }
                return false;
            }
        }

        private static bool HasExplicitEnumerationConversion(TypeSymbol source, TypeSymbol destination)
        {
            if (IsNumericType(source) && destination.IsEnumType())
            {
                return true;
            }
            if (IsNumericType(destination) && source.IsEnumType())
            {
                return true;
            }
            if (source.IsEnumType() && destination.IsEnumType())
            {
                return true;
            }
            return false;
        }

        private Conversion ClassifyImplicitNullableConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!destination.IsNullableType())
            {
                return Conversion.NoConversion;
            }
            TypeSymbol nullableUnderlyingType = destination.GetNullableUnderlyingType();
            TypeSymbol typeSymbol = source.StrippedType();
            if (!typeSymbol.IsValueType)
            {
                return Conversion.NoConversion;
            }
            if (HasIdentityConversionInternal(typeSymbol, nullableUnderlyingType))
            {
                return new Conversion(ConversionKind.ImplicitNullable, Conversion.IdentityUnderlying);
            }
            if (HasImplicitNumericConversion(typeSymbol, nullableUnderlyingType))
            {
                return new Conversion(ConversionKind.ImplicitNullable, Conversion.ImplicitNumericUnderlying);
            }
            Conversion item = ClassifyImplicitTupleConversion(typeSymbol, nullableUnderlyingType, ref useSiteInfo);
            if (item.Exists)
            {
                return new Conversion(ConversionKind.ImplicitNullable, ImmutableArray.Create(item));
            }
            return Conversion.NoConversion;
        }

        private Conversion GetImplicitTupleLiteralConversion(BoundTupleLiteral source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return GetTupleLiteralConversion(source, destination, ref useSiteInfo, ConversionKind.ImplicitTupleLiteral, delegate (ConversionsBase conversions, BoundExpression s, TypeWithAnnotations d, ref CompoundUseSiteInfo<AssemblySymbol> u, bool a)
            {
                return conversions.ClassifyImplicitConversionFromExpression(s, d.Type, ref u);
            }, arg: false);
        }

        private Conversion GetExplicitTupleLiteralConversion(BoundTupleLiteral source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool forCast)
        {
            return GetTupleLiteralConversion(source, destination, ref useSiteInfo, ConversionKind.ExplicitTupleLiteral, delegate (ConversionsBase conversions, BoundExpression s, TypeWithAnnotations d, ref CompoundUseSiteInfo<AssemblySymbol> u, bool a)
            {
                return conversions.ClassifyConversionFromExpression(s, d.Type, ref u, a);
            }, forCast);
        }

        private Conversion GetTupleLiteralConversion(BoundTupleLiteral source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConversionKind kind, ClassifyConversionFromExpressionDelegate classifyConversion, bool arg)
        {
            ImmutableArray<BoundExpression> arguments = source.Arguments;
            if (!destination.IsTupleTypeOfCardinality(arguments.Length))
            {
                return Conversion.NoConversion;
            }
            ImmutableArray<TypeWithAnnotations> tupleElementTypesWithAnnotations = destination.TupleElementTypesWithAnnotations;
            ArrayBuilder<Conversion> instance = ArrayBuilder<Conversion>.GetInstance(arguments.Length);
            for (int i = 0; i < arguments.Length; i++)
            {
                BoundExpression sourceExpression = arguments[i];
                Conversion item = classifyConversion(this, sourceExpression, tupleElementTypesWithAnnotations[i], ref useSiteInfo, arg);
                if (!item.Exists)
                {
                    instance.Free();
                    return Conversion.NoConversion;
                }
                instance.Add(item);
            }
            return new Conversion(kind, instance.ToImmutableAndFree());
        }

        private Conversion ClassifyImplicitTupleConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return ClassifyTupleConversion(source, destination, ref useSiteInfo, ConversionKind.ImplicitTuple, delegate (ConversionsBase conversions, TypeWithAnnotations s, TypeWithAnnotations d, ref CompoundUseSiteInfo<AssemblySymbol> u, bool a)
            {
                return (!conversions.HasTopLevelNullabilityImplicitConversion(s, d)) ? Conversion.NoConversion : conversions.ClassifyImplicitConversionFromType(s.Type, d.Type, ref u);
            }, arg: false);
        }

        private Conversion ClassifyExplicitTupleConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool forCast)
        {
            return ClassifyTupleConversion(source, destination, ref useSiteInfo, ConversionKind.ExplicitTuple, delegate (ConversionsBase conversions, TypeWithAnnotations s, TypeWithAnnotations d, ref CompoundUseSiteInfo<AssemblySymbol> u, bool a)
            {
                return (!conversions.HasTopLevelNullabilityImplicitConversion(s, d)) ? Conversion.NoConversion : conversions.ClassifyConversionFromType(s.Type, d.Type, ref u, a);
            }, forCast);
        }

        private Conversion ClassifyTupleConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConversionKind kind, ClassifyConversionFromTypeDelegate classifyConversion, bool arg)
        {
            if (!source.TryGetElementTypesWithAnnotationsIfTupleType(out var elementTypes) || !destination.TryGetElementTypesWithAnnotationsIfTupleType(out var elementTypes2) || elementTypes.Length != elementTypes2.Length)
            {
                return Conversion.NoConversion;
            }
            ArrayBuilder<Conversion> instance = ArrayBuilder<Conversion>.GetInstance(elementTypes.Length);
            for (int i = 0; i < elementTypes.Length; i++)
            {
                Conversion item = classifyConversion(this, elementTypes[i], elementTypes2[i], ref useSiteInfo, arg);
                if (!item.Exists)
                {
                    instance.Free();
                    return Conversion.NoConversion;
                }
                instance.Add(item);
            }
            return new Conversion(kind, instance.ToImmutableAndFree());
        }

        private Conversion ClassifyExplicitNullableConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool forCast)
        {
            if (!source.IsNullableType() && !destination.IsNullableType())
            {
                return Conversion.NoConversion;
            }
            TypeSymbol typeSymbol = source.StrippedType();
            TypeSymbol typeSymbol2 = destination.StrippedType();
            if (HasIdentityConversionInternal(typeSymbol, typeSymbol2))
            {
                return new Conversion(ConversionKind.ExplicitNullable, Conversion.IdentityUnderlying);
            }
            if (HasImplicitNumericConversion(typeSymbol, typeSymbol2))
            {
                return new Conversion(ConversionKind.ExplicitNullable, Conversion.ImplicitNumericUnderlying);
            }
            if (HasExplicitNumericConversion(typeSymbol, typeSymbol2))
            {
                return new Conversion(ConversionKind.ExplicitNullable, Conversion.ExplicitNumericUnderlying);
            }
            Conversion item = ClassifyExplicitTupleConversion(typeSymbol, typeSymbol2, ref useSiteInfo, forCast);
            if (item.Exists)
            {
                return new Conversion(ConversionKind.ExplicitNullable, ImmutableArray.Create(item));
            }
            if (HasExplicitEnumerationConversion(typeSymbol, typeSymbol2))
            {
                return new Conversion(ConversionKind.ExplicitNullable, Conversion.ExplicitEnumerationUnderlying);
            }
            if (HasPointerToIntegerConversion(typeSymbol, typeSymbol2))
            {
                return new Conversion(ConversionKind.ExplicitNullable, Conversion.PointerToIntegerUnderlying);
            }
            return Conversion.NoConversion;
        }

        private bool HasCovariantArrayConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayTypeSymbol arrayTypeSymbol = source as ArrayTypeSymbol;
            ArrayTypeSymbol arrayTypeSymbol2 = destination as ArrayTypeSymbol;
            if ((object)arrayTypeSymbol == null || (object)arrayTypeSymbol2 == null)
            {
                return false;
            }
            if (!arrayTypeSymbol.HasSameShapeAs(arrayTypeSymbol2))
            {
                return false;
            }
            return HasImplicitReferenceConversion(arrayTypeSymbol.ElementTypeWithAnnotations, arrayTypeSymbol2.ElementTypeWithAnnotations, ref useSiteInfo);
        }

        public bool HasIdentityOrImplicitReferenceConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (HasIdentityConversionInternal(source, destination))
            {
                return true;
            }
            return HasImplicitReferenceConversion(source, destination, ref useSiteInfo);
        }

        private static bool HasImplicitDynamicConversionFromExpression(TypeSymbol expressionType, TypeSymbol destination)
        {
            if ((object)expressionType != null && expressionType.Kind == SymbolKind.DynamicType)
            {
                return !destination.IsPointerOrFunctionPointer();
            }
            return false;
        }

        private static bool HasExplicitDynamicConversion(TypeSymbol source, TypeSymbol destination)
        {
            if (source.Kind == SymbolKind.DynamicType)
            {
                return !destination.IsPointerOrFunctionPointer();
            }
            return false;
        }

        private bool HasArrayConversionToInterface(ArrayTypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!source.IsSZArray)
            {
                return false;
            }
            if (!destination.IsInterfaceType())
            {
                return false;
            }
            if (destination.SpecialType == SpecialType.System_Collections_IEnumerable)
            {
                return true;
            }
            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)destination;
            if (namedTypeSymbol.AllTypeArgumentCount() != 1)
            {
                return false;
            }
            if (!namedTypeSymbol.IsPossibleArrayGenericInterface())
            {
                return false;
            }
            TypeWithAnnotations elementTypeWithAnnotations = source.ElementTypeWithAnnotations;
            TypeWithAnnotations destination2 = namedTypeSymbol.TypeArgumentWithDefinitionUseSiteDiagnostics(0, ref useSiteInfo);
            if (IncludeNullability && !HasTopLevelNullabilityImplicitConversion(elementTypeWithAnnotations, destination2))
            {
                return false;
            }
            return HasIdentityOrImplicitReferenceConversion(elementTypeWithAnnotations.Type, destination2.Type, ref useSiteInfo);
        }

        private bool HasImplicitReferenceConversion(TypeWithAnnotations source, TypeWithAnnotations destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (IncludeNullability)
            {
                if (!HasTopLevelNullabilityImplicitConversion(source, destination))
                {
                    return false;
                }
                if (source.NullableAnnotation != destination.NullableAnnotation && HasIdentityConversionInternal(source.Type, destination.Type, includeNullability: true))
                {
                    return true;
                }
            }
            return HasImplicitReferenceConversion(source.Type, destination.Type, ref useSiteInfo);
        }

        internal bool HasImplicitReferenceConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source.IsErrorType())
            {
                return false;
            }
            if (!source.IsReferenceType)
            {
                return false;
            }
            if (destination.SpecialType == SpecialType.System_Object || destination.Kind == SymbolKind.DynamicType)
            {
                return true;
            }
            switch (source.TypeKind)
            {
                case TypeKind.Class:
                    if (destination.IsClassType() && IsBaseClass(source, destination, ref useSiteInfo))
                    {
                        return true;
                    }
                    return HasImplicitConversionToInterface(source, destination, ref useSiteInfo);
                case TypeKind.Interface:
                    return HasImplicitConversionToInterface(source, destination, ref useSiteInfo);
                case TypeKind.Delegate:
                    return HasImplicitConversionFromDelegate(source, destination, ref useSiteInfo);
                case TypeKind.TypeParameter:
                    return HasImplicitReferenceTypeParameterConversion((TypeParameterSymbol)source, destination, ref useSiteInfo);
                case TypeKind.Array:
                    return HasImplicitConversionFromArray(source, destination, ref useSiteInfo);
                default:
                    return false;
            }
        }

        private bool HasImplicitConversionToInterface(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!destination.IsInterfaceType())
            {
                return false;
            }
            if (source.IsClassType())
            {
                return HasAnyBaseInterfaceConversion(source, destination, ref useSiteInfo);
            }
            if (source.IsInterfaceType())
            {
                if (HasAnyBaseInterfaceConversion(source, destination, ref useSiteInfo))
                {
                    return true;
                }
                if (!HasIdentityConversionInternal(source, destination) && HasInterfaceVarianceConversion(source, destination, ref useSiteInfo))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasImplicitConversionFromArray(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!(source is ArrayTypeSymbol source2))
            {
                return false;
            }
            if (HasCovariantArrayConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            if (destination.GetSpecialTypeSafe() == SpecialType.System_Array)
            {
                return true;
            }
            if (IsBaseInterface(destination, corLibrary.GetDeclaredSpecialType(SpecialType.System_Array), ref useSiteInfo))
            {
                return true;
            }
            if (HasArrayConversionToInterface(source2, destination, ref useSiteInfo))
            {
                return true;
            }
            return false;
        }

        private bool HasImplicitConversionFromDelegate(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!source.IsDelegateType())
            {
                return false;
            }
            SpecialType specialTypeSafe = destination.GetSpecialTypeSafe();
            if (specialTypeSafe == SpecialType.System_MulticastDelegate || specialTypeSafe == SpecialType.System_Delegate || IsBaseInterface(destination, corLibrary.GetDeclaredSpecialType(SpecialType.System_MulticastDelegate), ref useSiteInfo))
            {
                return true;
            }
            if (HasDelegateVarianceConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            return false;
        }

        public bool HasImplicitTypeParameterConversion(TypeParameterSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (HasImplicitReferenceTypeParameterConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            if (HasImplicitBoxingTypeParameterConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            if (destination.TypeKind == TypeKind.TypeParameter && source.DependsOn((TypeParameterSymbol)destination))
            {
                return true;
            }
            return false;
        }

        private bool HasImplicitReferenceTypeParameterConversion(TypeParameterSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source.IsValueType)
            {
                return false;
            }
            if (HasImplicitEffectiveBaseConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            if (HasImplicitEffectiveInterfaceSetConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            if (destination.TypeKind == TypeKind.TypeParameter && source.DependsOn((TypeParameterSymbol)destination))
            {
                return true;
            }
            return false;
        }

        private bool HasImplicitEffectiveBaseConversion(TypeParameterSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            NamedTypeSymbol namedTypeSymbol = source.EffectiveBaseClass(ref useSiteInfo);
            if (HasIdentityConversionInternal(namedTypeSymbol, destination))
            {
                return true;
            }
            if (IsBaseClass(namedTypeSymbol, destination, ref useSiteInfo))
            {
                return true;
            }
            if (HasAnyBaseInterfaceConversion(namedTypeSymbol, destination, ref useSiteInfo))
            {
                return true;
            }
            return false;
        }

        private bool HasImplicitEffectiveInterfaceSetConversion(TypeParameterSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!destination.IsInterfaceType())
            {
                return false;
            }
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = source.AllEffectiveInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                if (HasInterfaceVarianceConversion(current, destination, ref useSiteInfo))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasAnyBaseInterfaceConversion(TypeSymbol derivedType, TypeSymbol baseType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!baseType.IsInterfaceType())
            {
                return false;
            }
            if (!(derivedType is NamedTypeSymbol namedTypeSymbol))
            {
                return false;
            }
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = namedTypeSymbol.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                if (HasInterfaceVarianceConversion(current, baseType, ref useSiteInfo))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasInterfaceVarianceConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            NamedTypeSymbol namedTypeSymbol = source as NamedTypeSymbol;
            NamedTypeSymbol namedTypeSymbol2 = destination as NamedTypeSymbol;
            if ((object)namedTypeSymbol == null || (object)namedTypeSymbol2 == null)
            {
                return false;
            }
            if (!namedTypeSymbol.IsInterfaceType() || !namedTypeSymbol2.IsInterfaceType())
            {
                return false;
            }
            return HasVariantConversion(namedTypeSymbol, namedTypeSymbol2, ref useSiteInfo);
        }

        private bool HasDelegateVarianceConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            NamedTypeSymbol namedTypeSymbol = source as NamedTypeSymbol;
            NamedTypeSymbol namedTypeSymbol2 = destination as NamedTypeSymbol;
            if ((object)namedTypeSymbol == null || (object)namedTypeSymbol2 == null)
            {
                return false;
            }
            if (!namedTypeSymbol.IsDelegateType() || !namedTypeSymbol2.IsDelegateType())
            {
                return false;
            }
            return HasVariantConversion(namedTypeSymbol, namedTypeSymbol2, ref useSiteInfo);
        }

        private bool HasVariantConversion(NamedTypeSymbol source, NamedTypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (currentRecursionDepth >= 50)
            {
                return false;
            }
            ThreeState value = HasVariantConversionQuick(source, destination);
            if (value.HasValue())
            {
                return value.Value();
            }
            return CreateInstance(currentRecursionDepth + 1).HasVariantConversionNoCycleCheck(source, destination, ref useSiteInfo);
        }

        private ThreeState HasVariantConversionQuick(NamedTypeSymbol source, NamedTypeSymbol destination)
        {
            if (HasIdentityConversionInternal(source, destination))
            {
                return ThreeState.True;
            }
            if (!TypeSymbol.Equals(source.OriginalDefinition, destination.OriginalDefinition, TypeCompareKind.ConsiderEverything))
            {
                return ThreeState.False;
            }
            return ThreeState.Unknown;
        }

        private bool HasVariantConversionNoCycleCheck(NamedTypeSymbol source, NamedTypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            ArrayBuilder<TypeWithAnnotations> instance2 = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            ArrayBuilder<TypeWithAnnotations> instance3 = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            try
            {
                source.OriginalDefinition.GetAllTypeArguments(instance, ref useSiteInfo);
                source.GetAllTypeArguments(instance2, ref useSiteInfo);
                destination.GetAllTypeArguments(instance3, ref useSiteInfo);
                for (int i = 0; i < instance.Count; i++)
                {
                    TypeWithAnnotations typeWithAnnotations = instance2[i];
                    TypeWithAnnotations typeWithAnnotations2 = instance3[i];
                    if (HasIdentityConversionInternal(typeWithAnnotations.Type, typeWithAnnotations2.Type) && HasTopLevelNullabilityIdentityConversion(typeWithAnnotations, typeWithAnnotations2))
                    {
                        continue;
                    }
                    TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)instance[i].Type;
                    switch (typeParameterSymbol.Variance)
                    {
                        case VarianceKind.None:
                            if (isTypeIEquatable(destination.OriginalDefinition) && TypeSymbol.Equals(typeWithAnnotations2.Type, typeWithAnnotations.Type, TypeCompareKind.AllNullableIgnoreOptions) && HasAnyNullabilityImplicitConversion(typeWithAnnotations2, typeWithAnnotations))
                            {
                                return true;
                            }
                            return false;
                        case VarianceKind.Out:
                            if (!HasImplicitReferenceConversion(typeWithAnnotations, typeWithAnnotations2, ref useSiteInfo))
                            {
                                return false;
                            }
                            break;
                        case VarianceKind.In:
                            if (!HasImplicitReferenceConversion(typeWithAnnotations2, typeWithAnnotations, ref useSiteInfo))
                            {
                                return false;
                            }
                            break;
                        default:
                            throw ExceptionUtilities.UnexpectedValue(typeParameterSymbol.Variance);
                    }
                }
            }
            finally
            {
                instance.Free();
                instance2.Free();
                instance3.Free();
            }
            return true;
            static bool isTypeIEquatable(NamedTypeSymbol type)
            {
                if ((object)type != null && type.IsInterface && type.Name == "IEquatable")
                {
                    NamespaceSymbol containingNamespace = type.ContainingNamespace;
                    if ((object)containingNamespace != null && containingNamespace.Name == "System")
                    {
                        NamespaceSymbol containingNamespace2 = containingNamespace.ContainingNamespace;
                        if ((object)containingNamespace2 != null && containingNamespace2.IsGlobalNamespace)
                        {
                            Symbol containingSymbol = type.ContainingSymbol;
                            if ((object)containingSymbol != null && containingSymbol.Kind == SymbolKind.Namespace)
                            {
                                return type.TypeParameters.Length == 1;
                            }
                        }
                    }
                }
                return false;
            }
        }

        private bool HasImplicitBoxingTypeParameterConversion(TypeParameterSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source.IsReferenceType)
            {
                return false;
            }
            if (HasImplicitEffectiveBaseConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            if (HasImplicitEffectiveInterfaceSetConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            if (destination.TypeKind == TypeKind.TypeParameter && source.DependsOn((TypeParameterSymbol)destination))
            {
                return true;
            }
            if (destination.Kind == SymbolKind.DynamicType)
            {
                return true;
            }
            return false;
        }

        public bool HasBoxingConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source.TypeKind == TypeKind.TypeParameter && HasImplicitBoxingTypeParameterConversion((TypeParameterSymbol)source, destination, ref useSiteInfo))
            {
                return true;
            }
            if (!source.IsValueType || !destination.IsReferenceType)
            {
                return false;
            }
            if (source.IsNullableType())
            {
                return HasBoxingConversion(source.GetNullableUnderlyingType(), destination, ref useSiteInfo);
            }
            if (source.IsRestrictedType())
            {
                return false;
            }
            if (destination.Kind == SymbolKind.DynamicType)
            {
                return !source.IsPointerOrFunctionPointer();
            }
            if (IsBaseClass(source, destination, ref useSiteInfo))
            {
                return true;
            }
            if (HasAnyBaseInterfaceConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            return false;
        }

        internal static bool HasImplicitPointerToVoidConversion(TypeSymbol source, TypeSymbol destination)
        {
            if (source.IsPointerOrFunctionPointer())
            {
                if (destination is PointerTypeSymbol pointerTypeSymbol)
                {
                    TypeSymbol pointedAtType = pointerTypeSymbol.PointedAtType;
                    if ((object)pointedAtType != null)
                    {
                        return pointedAtType.SpecialType == SpecialType.System_Void;
                    }
                }
                return false;
            }
            return false;
        }

        internal bool HasImplicitPointerConversion(TypeSymbol? source, TypeSymbol? destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source is FunctionPointerTypeSymbol functionPointerTypeSymbol)
            {
                FunctionPointerMethodSymbol signature = functionPointerTypeSymbol.Signature;
                if ((object)signature != null && destination is FunctionPointerTypeSymbol functionPointerTypeSymbol2)
                {
                    FunctionPointerMethodSymbol signature2 = functionPointerTypeSymbol2.Signature;
                    if ((object)signature2 != null)
                    {
                        if (signature.ParameterCount != signature2.ParameterCount || signature.CallingConvention != signature2.CallingConvention)
                        {
                            return false;
                        }
                        if (signature.CallingConvention == CallingConvention.Unmanaged && !signature.GetCallingConventionModifiers().SetEquals(signature2.GetCallingConventionModifiers()))
                        {
                            return false;
                        }
                        for (int i = 0; i < signature.ParameterCount; i++)
                        {
                            ParameterSymbol parameterSymbol = signature.Parameters[i];
                            ParameterSymbol parameterSymbol2 = signature2.Parameters[i];
                            if (parameterSymbol.RefKind != parameterSymbol2.RefKind)
                            {
                                return false;
                            }
                            if (!hasConversion(parameterSymbol.RefKind, signature2.Parameters[i].TypeWithAnnotations, signature.Parameters[i].TypeWithAnnotations, ref useSiteInfo))
                            {
                                return false;
                            }
                        }
                        if (signature.RefKind == signature2.RefKind)
                        {
                            return hasConversion(signature.RefKind, signature.ReturnTypeWithAnnotations, signature2.ReturnTypeWithAnnotations, ref useSiteInfo);
                        }
                        return false;
                    }
                }
            }
            return false;
            bool hasConversion(RefKind refKind, TypeWithAnnotations sourceType, TypeWithAnnotations destinationType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
            {
                if (refKind == RefKind.None)
                {
                    if (!IncludeNullability || HasTopLevelNullabilityImplicitConversion(sourceType, destinationType))
                    {
                        if (!HasIdentityOrImplicitReferenceConversion(sourceType.Type, destinationType.Type, ref useSiteInfo) && !HasImplicitPointerToVoidConversion(sourceType.Type, destinationType.Type))
                        {
                            return HasImplicitPointerConversion(sourceType.Type, destinationType.Type, ref useSiteInfo);
                        }
                        return true;
                    }
                    return false;
                }
                if (!IncludeNullability || HasTopLevelNullabilityIdentityConversion(sourceType, destinationType))
                {
                    return HasIdentityConversion(sourceType.Type, destinationType.Type);
                }
                return false;
            }
        }

        private bool HasIdentityOrReferenceConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (HasIdentityConversionInternal(source, destination))
            {
                return true;
            }
            if (HasImplicitReferenceConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            if (HasExplicitReferenceConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            return false;
        }

        private bool HasExplicitReferenceConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (source.SpecialType == SpecialType.System_Object)
            {
                if (destination.IsReferenceType)
                {
                    return true;
                }
            }
            else if (source.Kind == SymbolKind.DynamicType && destination.IsReferenceType)
            {
                return true;
            }
            if (destination.IsClassType() && IsBaseClass(destination, source, ref useSiteInfo))
            {
                return true;
            }
            if (source.IsClassType() && destination.IsInterfaceType() && !source.IsSealed && !HasAnyBaseInterfaceConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            if (source.IsInterfaceType() && destination.IsClassType() && (!destination.IsSealed || HasAnyBaseInterfaceConversion(destination, source, ref useSiteInfo)))
            {
                return true;
            }
            if (source.IsInterfaceType() && destination.IsInterfaceType() && !HasImplicitConversionToInterface(source, destination, ref useSiteInfo))
            {
                return true;
            }
            if (HasExplicitArrayConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            if (HasExplicitDelegateConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            if (HasExplicitReferenceTypeParameterConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            return false;
        }

        private bool HasExplicitReferenceTypeParameterConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            TypeParameterSymbol typeParameterSymbol = source as TypeParameterSymbol;
            TypeParameterSymbol typeParameterSymbol2 = destination as TypeParameterSymbol;
            if ((object)typeParameterSymbol2 != null && typeParameterSymbol2.IsReferenceType)
            {
                NamedTypeSymbol namedTypeSymbol = typeParameterSymbol2.EffectiveBaseClass(ref useSiteInfo);
                while ((object)namedTypeSymbol != null)
                {
                    if (HasIdentityConversionInternal(namedTypeSymbol, source))
                    {
                        return true;
                    }
                    namedTypeSymbol = namedTypeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
                }
            }
            if ((object)typeParameterSymbol2 != null && source.IsInterfaceType() && typeParameterSymbol2.IsReferenceType)
            {
                return true;
            }
            if ((object)typeParameterSymbol != null && typeParameterSymbol.IsReferenceType && destination.IsInterfaceType() && !HasImplicitReferenceTypeParameterConversion(typeParameterSymbol, destination, ref useSiteInfo))
            {
                return true;
            }
            if ((object)typeParameterSymbol != null && (object)typeParameterSymbol2 != null && typeParameterSymbol2.IsReferenceType && typeParameterSymbol2.DependsOn(typeParameterSymbol))
            {
                return true;
            }
            return false;
        }

        private bool HasUnboxingTypeParameterConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            TypeParameterSymbol typeParameterSymbol = source as TypeParameterSymbol;
            TypeParameterSymbol typeParameterSymbol2 = destination as TypeParameterSymbol;
            if ((object)typeParameterSymbol2 != null && !typeParameterSymbol2.IsReferenceType)
            {
                NamedTypeSymbol namedTypeSymbol = typeParameterSymbol2.EffectiveBaseClass(ref useSiteInfo);
                while ((object)namedTypeSymbol != null)
                {
                    if (TypeSymbol.Equals(namedTypeSymbol, source, TypeCompareKind.ConsiderEverything))
                    {
                        return true;
                    }
                    namedTypeSymbol = namedTypeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
                }
            }
            if (source.IsInterfaceType() && (object)typeParameterSymbol2 != null && !typeParameterSymbol2.IsReferenceType)
            {
                return true;
            }
            if ((object)typeParameterSymbol != null && !typeParameterSymbol.IsReferenceType && destination.IsInterfaceType() && !HasImplicitReferenceTypeParameterConversion(typeParameterSymbol, destination, ref useSiteInfo))
            {
                return true;
            }
            if ((object)typeParameterSymbol != null && (object)typeParameterSymbol2 != null && !typeParameterSymbol2.IsReferenceType && typeParameterSymbol2.DependsOn(typeParameterSymbol))
            {
                return true;
            }
            return false;
        }

        private bool HasExplicitDelegateConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (destination.IsDelegateType())
            {
                if (source.SpecialType == SpecialType.System_Delegate || source.SpecialType == SpecialType.System_MulticastDelegate)
                {
                    return true;
                }
                if (HasImplicitConversionToInterface(corLibrary.GetDeclaredSpecialType(SpecialType.System_Delegate), source, ref useSiteInfo))
                {
                    return true;
                }
            }
            if (!source.IsDelegateType() || !destination.IsDelegateType())
            {
                return false;
            }
            if (!TypeSymbol.Equals(source.OriginalDefinition, destination.OriginalDefinition, TypeCompareKind.ConsiderEverything))
            {
                return false;
            }
            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)source;
            NamedTypeSymbol namedTypeSymbol2 = (NamedTypeSymbol)destination;
            NamedTypeSymbol originalDefinition = namedTypeSymbol.OriginalDefinition;
            if (HasIdentityConversionInternal(source, destination))
            {
                return false;
            }
            if (HasDelegateVarianceConversion(source, destination, ref useSiteInfo))
            {
                return false;
            }
            ImmutableArray<TypeWithAnnotations> immutableArray = namedTypeSymbol.TypeArgumentsWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            ImmutableArray<TypeWithAnnotations> immutableArray2 = namedTypeSymbol2.TypeArgumentsWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            for (int i = 0; i < immutableArray.Length; i++)
            {
                TypeSymbol type = immutableArray[i].Type;
                TypeSymbol type2 = immutableArray2[i].Type;
                switch (originalDefinition.TypeParameters[i].Variance)
                {
                    case VarianceKind.None:
                        if (!HasIdentityConversionInternal(type, type2))
                        {
                            return false;
                        }
                        break;
                    case VarianceKind.Out:
                        if (!HasIdentityOrReferenceConversion(type, type2, ref useSiteInfo))
                        {
                            return false;
                        }
                        break;
                    case VarianceKind.In:
                        {
                            bool num = HasIdentityConversionInternal(type, type2);
                            bool flag = type.IsReferenceType && type2.IsReferenceType;
                            if (!(num || flag))
                            {
                                return false;
                            }
                            break;
                        }
                }
            }
            return true;
        }

        private bool HasExplicitArrayConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayTypeSymbol arrayTypeSymbol = source as ArrayTypeSymbol;
            ArrayTypeSymbol arrayTypeSymbol2 = destination as ArrayTypeSymbol;
            if ((object)arrayTypeSymbol != null && (object)arrayTypeSymbol2 != null)
            {
                if (arrayTypeSymbol.HasSameShapeAs(arrayTypeSymbol2))
                {
                    return HasExplicitReferenceConversion(arrayTypeSymbol.ElementType, arrayTypeSymbol2.ElementType, ref useSiteInfo);
                }
                return false;
            }
            if ((object)arrayTypeSymbol2 != null)
            {
                if (source.SpecialType == SpecialType.System_Array)
                {
                    return true;
                }
                ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = corLibrary.GetDeclaredSpecialType(SpecialType.System_Array).AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    NamedTypeSymbol current = enumerator.Current;
                    if (HasIdentityConversionInternal(current, source))
                    {
                        return true;
                    }
                }
            }
            if ((object)arrayTypeSymbol != null && arrayTypeSymbol.IsSZArray && destination.IsPossibleArrayGenericInterface() && HasExplicitReferenceConversion(arrayTypeSymbol.ElementType, ((NamedTypeSymbol)destination).TypeArgumentWithDefinitionUseSiteDiagnostics(0, ref useSiteInfo).Type, ref useSiteInfo))
            {
                return true;
            }
            if ((object)arrayTypeSymbol2 != null && arrayTypeSymbol2.IsSZArray)
            {
                SpecialType specialType = source.OriginalDefinition.SpecialType;
                if (specialType == SpecialType.System_Collections_Generic_IList_T || specialType == SpecialType.System_Collections_Generic_ICollection_T || specialType == SpecialType.System_Collections_Generic_IEnumerable_T || specialType == SpecialType.System_Collections_Generic_IReadOnlyList_T || specialType == SpecialType.System_Collections_Generic_IReadOnlyCollection_T)
                {
                    TypeSymbol type = ((NamedTypeSymbol)source).TypeArgumentWithDefinitionUseSiteDiagnostics(0, ref useSiteInfo).Type;
                    TypeSymbol elementType = arrayTypeSymbol2.ElementType;
                    if (HasIdentityConversionInternal(type, elementType))
                    {
                        return true;
                    }
                    if (HasImplicitReferenceConversion(type, elementType, ref useSiteInfo))
                    {
                        return true;
                    }
                    if (HasExplicitReferenceConversion(type, elementType, ref useSiteInfo))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool HasUnboxingConversion(TypeSymbol source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (destination.IsPointerOrFunctionPointer())
            {
                return false;
            }
            if (destination.IsRestrictedType())
            {
                return false;
            }
            SpecialType specialType = source.SpecialType;
            if ((specialType == SpecialType.System_Object || specialType == SpecialType.System_ValueType) && destination.IsValueType && !destination.IsNullableType())
            {
                return true;
            }
            if (source.IsInterfaceType() && destination.IsValueType && !destination.IsNullableType() && HasBoxingConversion(destination, source, ref useSiteInfo))
            {
                return true;
            }
            if (source.SpecialType == SpecialType.System_Enum && destination.IsEnumType())
            {
                return true;
            }
            if (source.IsReferenceType && destination.IsNullableType() && HasUnboxingConversion(source, destination.GetNullableUnderlyingType(), ref useSiteInfo))
            {
                return true;
            }
            if (HasUnboxingTypeParameterConversion(source, destination, ref useSiteInfo))
            {
                return true;
            }
            return false;
        }

        private static bool HasPointerToPointerConversion(TypeSymbol source, TypeSymbol destination)
        {
            if (source.IsPointerOrFunctionPointer())
            {
                return destination.IsPointerOrFunctionPointer();
            }
            return false;
        }

        private static bool HasPointerToIntegerConversion(TypeSymbol source, TypeSymbol destination)
        {
            if (!source.IsPointerOrFunctionPointer())
            {
                return false;
            }
            return IsIntegerTypeSupportingPointerConversions(destination.StrippedType());
        }

        private static bool HasIntegerToPointerConversion(TypeSymbol source, TypeSymbol destination)
        {
            if (!destination.IsPointerOrFunctionPointer())
            {
                return false;
            }
            return IsIntegerTypeSupportingPointerConversions(source);
        }

        private static bool IsIntegerTypeSupportingPointerConversions(TypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                    return true;
                case SpecialType.System_IntPtr:
                case SpecialType.System_UIntPtr:
                    return type.IsNativeIntegerType;
                default:
                    return false;
            }
        }

        private static TypeSymbol GetUnderlyingEffectiveType(TypeSymbol type, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if ((object)type != null)
            {
                type = type.StrippedType();
                if (type.IsTypeParameter())
                {
                    type = ((TypeParameterSymbol)type).EffectiveBaseClass(ref useSiteInfo);
                }
            }
            return type;
        }

        public static void AddTypesParticipatingInUserDefinedConversion(ArrayBuilder<NamedTypeSymbol> result, TypeSymbol type, bool includeBaseTypes, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if ((object)type == null)
            {
                return;
            }
            bool flag = result.Count > 0;
            if (type.IsClassType() || type.IsStructType())
            {
                NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type;
                if (!flag || !HasIdentityConversionToAny(namedTypeSymbol, result))
                {
                    result.Add(namedTypeSymbol);
                }
            }
            if (!includeBaseTypes)
            {
                return;
            }
            NamedTypeSymbol namedTypeSymbol2 = type.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            while ((object)namedTypeSymbol2 != null)
            {
                if (!flag || !HasIdentityConversionToAny(namedTypeSymbol2, result))
                {
                    result.Add(namedTypeSymbol2);
                }
                namedTypeSymbol2 = namedTypeSymbol2.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }
        }

        private UserDefinedConversionResult AnalyzeExplicitUserDefinedConversions(BoundExpression sourceExpression, TypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            ComputeUserDefinedExplicitConversionTypeSet(source, target, instance, ref useSiteInfo);
            ArrayBuilder<UserDefinedConversionAnalysis> instance2 = ArrayBuilder<UserDefinedConversionAnalysis>.GetInstance();
            ComputeApplicableUserDefinedExplicitConversionSet(sourceExpression, source, target, instance, instance2, ref useSiteInfo);
            instance.Free();
            ImmutableArray<UserDefinedConversionAnalysis> immutableArray = instance2.ToImmutableAndFree();
            if (immutableArray.Length == 0)
            {
                return UserDefinedConversionResult.NoApplicableOperators(immutableArray);
            }
            TypeSymbol typeSymbol = MostSpecificSourceTypeForExplicitUserDefinedConversion(immutableArray, sourceExpression, source, ref useSiteInfo);
            if ((object)typeSymbol == null)
            {
                return UserDefinedConversionResult.NoBestSourceType(immutableArray);
            }
            TypeSymbol typeSymbol2 = MostSpecificTargetTypeForExplicitUserDefinedConversion(immutableArray, target, ref useSiteInfo);
            if ((object)typeSymbol2 == null)
            {
                return UserDefinedConversionResult.NoBestTargetType(immutableArray);
            }
            int? num = MostSpecificConversionOperator(typeSymbol, typeSymbol2, immutableArray);
            if (!num.HasValue)
            {
                return UserDefinedConversionResult.Ambiguous(immutableArray);
            }
            return UserDefinedConversionResult.Valid(immutableArray, num.Value);
        }

        private static void ComputeUserDefinedExplicitConversionTypeSet(TypeSymbol source, TypeSymbol target, ArrayBuilder<NamedTypeSymbol> d, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            TypeSymbol underlyingEffectiveType = GetUnderlyingEffectiveType(source, ref useSiteInfo);
            TypeSymbol underlyingEffectiveType2 = GetUnderlyingEffectiveType(target, ref useSiteInfo);
            AddTypesParticipatingInUserDefinedConversion(d, underlyingEffectiveType, includeBaseTypes: true, ref useSiteInfo);
            AddTypesParticipatingInUserDefinedConversion(d, underlyingEffectiveType2, includeBaseTypes: true, ref useSiteInfo);
        }

        private void ComputeApplicableUserDefinedExplicitConversionSet(BoundExpression sourceExpression, TypeSymbol source, TypeSymbol target, ArrayBuilder<NamedTypeSymbol> d, ArrayBuilder<UserDefinedConversionAnalysis> u, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayBuilder<NamedTypeSymbol>.Enumerator enumerator = d.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                AddUserDefinedConversionsToExplicitCandidateSet(sourceExpression, source, target, u, current, "op_Explicit", ref useSiteInfo);
                AddUserDefinedConversionsToExplicitCandidateSet(sourceExpression, source, target, u, current, "op_Implicit", ref useSiteInfo);
            }
        }

        private void AddUserDefinedConversionsToExplicitCandidateSet(BoundExpression sourceExpression, TypeSymbol source, TypeSymbol target, ArrayBuilder<UserDefinedConversionAnalysis> u, NamedTypeSymbol declaringType, string operatorName, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (((object)source != null && source.IsInterfaceType()) || target.IsInterfaceType())
            {
                return;
            }
            ImmutableArray<MethodSymbol>.Enumerator enumerator = declaringType.GetOperators(operatorName).GetEnumerator();
            while (enumerator.MoveNext())
            {
                MethodSymbol current = enumerator.Current;
                if (current.ReturnsVoid || current.ParameterCount != 1 || current.ReturnType.TypeKind == TypeKind.Error)
                {
                    continue;
                }
                TypeSymbol typeSymbol = current.GetParameterType(0);
                TypeSymbol typeSymbol2 = current.ReturnType;
                Conversion sourceConversion = EncompassingExplicitConversion(sourceExpression, source, typeSymbol, ref useSiteInfo);
                Conversion targetConversion = EncompassingExplicitConversion(null, typeSymbol2, target, ref useSiteInfo);
                if (!sourceConversion.Exists && (object)source != null && source.IsNullableType() && EncompassingExplicitConversion(null, source.GetNullableUnderlyingType(), typeSymbol, ref useSiteInfo).Exists)
                {
                    sourceConversion = ClassifyBuiltInConversion(source, typeSymbol, ref useSiteInfo);
                }
                if (!targetConversion.Exists && (object)target != null && target.IsNullableType() && EncompassingExplicitConversion(null, typeSymbol2, target.GetNullableUnderlyingType(), ref useSiteInfo).Exists)
                {
                    targetConversion = ClassifyBuiltInConversion(typeSymbol2, target, ref useSiteInfo);
                }
                if (!sourceConversion.Exists || !targetConversion.Exists)
                {
                    continue;
                }
                if ((object)source != null && source.IsNullableType() && typeSymbol.IsNonNullableValueType() && target.CanBeAssignedNull())
                {
                    TypeSymbol typeSymbol3 = MakeNullableType(typeSymbol);
                    TypeSymbol typeSymbol4 = (typeSymbol2.IsNonNullableValueType() ? MakeNullableType(typeSymbol2) : typeSymbol2);
                    Conversion sourceConversion2 = EncompassingExplicitConversion(sourceExpression, source, typeSymbol3, ref useSiteInfo);
                    Conversion targetConversion2 = EncompassingExplicitConversion(null, typeSymbol4, target, ref useSiteInfo);
                    u.Add(UserDefinedConversionAnalysis.Lifted(current, sourceConversion2, targetConversion2, typeSymbol3, typeSymbol4));
                    continue;
                }
                if (target.IsNullableType() && typeSymbol2.IsNonNullableValueType())
                {
                    typeSymbol2 = MakeNullableType(typeSymbol2);
                    targetConversion = EncompassingExplicitConversion(null, typeSymbol2, target, ref useSiteInfo);
                }
                if ((object)source != null && source.IsNullableType() && typeSymbol.IsNonNullableValueType())
                {
                    typeSymbol = MakeNullableType(typeSymbol);
                    sourceConversion = EncompassingExplicitConversion(null, typeSymbol, source, ref useSiteInfo);
                }
                u.Add(UserDefinedConversionAnalysis.Normal(current, sourceConversion, targetConversion, typeSymbol, typeSymbol2));
            }
        }

        private TypeSymbol MostSpecificSourceTypeForExplicitUserDefinedConversion(ImmutableArray<UserDefinedConversionAnalysis> u, BoundExpression sourceExpression, TypeSymbol source, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if ((object)source != null)
            {
                if (u.Any((UserDefinedConversionAnalysis conv) => TypeSymbol.Equals(conv.FromType, source, TypeCompareKind.ConsiderEverything)))
                {
                    return source;
                }
                CompoundUseSiteInfo<AssemblySymbol> inLambdaUseSiteInfo = useSiteInfo;
                bool func(UserDefinedConversionAnalysis conv) => IsEncompassedBy(sourceExpression, source, conv.FromType, ref inLambdaUseSiteInfo);
                if (u.Any(func))
                {
                    TypeSymbol result = MostEncompassedType(u, func, (UserDefinedConversionAnalysis conv) => conv.FromType, ref inLambdaUseSiteInfo);
                    useSiteInfo = inLambdaUseSiteInfo;
                    return result;
                }
                useSiteInfo = inLambdaUseSiteInfo;
            }
            return MostEncompassingType(u, (UserDefinedConversionAnalysis conv) => conv.FromType, ref useSiteInfo);
        }

        private TypeSymbol MostSpecificTargetTypeForExplicitUserDefinedConversion(ImmutableArray<UserDefinedConversionAnalysis> u, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (u.Any((UserDefinedConversionAnalysis conv) => TypeSymbol.Equals(conv.ToType, target, TypeCompareKind.ConsiderEverything)))
            {
                return target;
            }
            CompoundUseSiteInfo<AssemblySymbol> inLambdaUseSiteInfo = useSiteInfo;
            bool func(UserDefinedConversionAnalysis conv) => IsEncompassedBy(null, conv.ToType, target, ref inLambdaUseSiteInfo);
            if (u.Any(func))
            {
                TypeSymbol result = MostEncompassingType(u, func, (UserDefinedConversionAnalysis conv) => conv.ToType, ref inLambdaUseSiteInfo);
                useSiteInfo = inLambdaUseSiteInfo;
                return result;
            }
            useSiteInfo = inLambdaUseSiteInfo;
            return MostEncompassedType(u, (UserDefinedConversionAnalysis conv) => conv.ToType, ref useSiteInfo);
        }

        private Conversion EncompassingExplicitConversion(BoundExpression expr, TypeSymbol a, TypeSymbol b, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            Conversion result = ClassifyStandardConversion(expr, a, b, ref useSiteInfo);
            if (!result.IsEnumeration)
            {
                return result;
            }
            return Conversion.NoConversion;
        }

        private UserDefinedConversionResult AnalyzeImplicitUserDefinedConversions(BoundExpression sourceExpression, TypeSymbol source, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            ComputeUserDefinedImplicitConversionTypeSet(source, target, instance, ref useSiteInfo);
            ArrayBuilder<UserDefinedConversionAnalysis> instance2 = ArrayBuilder<UserDefinedConversionAnalysis>.GetInstance();
            ComputeApplicableUserDefinedImplicitConversionSet(sourceExpression, source, target, instance, instance2, ref useSiteInfo);
            instance.Free();
            ImmutableArray<UserDefinedConversionAnalysis> immutableArray = instance2.ToImmutableAndFree();
            if (immutableArray.Length == 0)
            {
                return UserDefinedConversionResult.NoApplicableOperators(immutableArray);
            }
            TypeSymbol typeSymbol = MostSpecificSourceTypeForImplicitUserDefinedConversion(immutableArray, source, ref useSiteInfo);
            if ((object)typeSymbol == null)
            {
                return UserDefinedConversionResult.NoBestSourceType(immutableArray);
            }
            TypeSymbol typeSymbol2 = MostSpecificTargetTypeForImplicitUserDefinedConversion(immutableArray, target, ref useSiteInfo);
            if ((object)typeSymbol2 == null)
            {
                return UserDefinedConversionResult.NoBestTargetType(immutableArray);
            }
            int? num = MostSpecificConversionOperator(typeSymbol, typeSymbol2, immutableArray);
            if (!num.HasValue)
            {
                return UserDefinedConversionResult.Ambiguous(immutableArray);
            }
            return UserDefinedConversionResult.Valid(immutableArray, num.Value);
        }

        private static void ComputeUserDefinedImplicitConversionTypeSet(TypeSymbol s, TypeSymbol t, ArrayBuilder<NamedTypeSymbol> d, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            TypeSymbol underlyingEffectiveType = GetUnderlyingEffectiveType(s, ref useSiteInfo);
            TypeSymbol underlyingEffectiveType2 = GetUnderlyingEffectiveType(t, ref useSiteInfo);
            AddTypesParticipatingInUserDefinedConversion(d, underlyingEffectiveType, includeBaseTypes: true, ref useSiteInfo);
            AddTypesParticipatingInUserDefinedConversion(d, underlyingEffectiveType2, includeBaseTypes: false, ref useSiteInfo);
        }

        private void ComputeApplicableUserDefinedImplicitConversionSet(BoundExpression sourceExpression, TypeSymbol source, TypeSymbol target, ArrayBuilder<NamedTypeSymbol> d, ArrayBuilder<UserDefinedConversionAnalysis> u, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool allowAnyTarget = false)
        {
            if (((object)source != null && source.IsInterfaceType()) || ((object)target != null && target.IsInterfaceType()))
            {
                return;
            }
            ArrayBuilder<NamedTypeSymbol>.Enumerator enumerator = d.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ImmutableArray<MethodSymbol>.Enumerator enumerator2 = enumerator.Current.GetOperators("op_Implicit").GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    MethodSymbol current = enumerator2.Current;
                    if (current.ReturnsVoid || current.ParameterCount != 1)
                    {
                        continue;
                    }
                    TypeSymbol parameterType = current.GetParameterType(0);
                    TypeSymbol typeSymbol = current.ReturnType;
                    Conversion sourceConversion = EncompassingImplicitConversion(sourceExpression, source, parameterType, ref useSiteInfo);
                    Conversion targetConversion = (allowAnyTarget ? Conversion.Identity : EncompassingImplicitConversion(null, typeSymbol, target, ref useSiteInfo));
                    if (sourceConversion.Exists && targetConversion.Exists)
                    {
                        if ((object)target != null && target.IsNullableType() && typeSymbol.IsNonNullableValueType())
                        {
                            typeSymbol = MakeNullableType(typeSymbol);
                            targetConversion = (allowAnyTarget ? Conversion.Identity : EncompassingImplicitConversion(null, typeSymbol, target, ref useSiteInfo));
                        }
                        u.Add(UserDefinedConversionAnalysis.Normal(current, sourceConversion, targetConversion, parameterType, typeSymbol));
                    }
                    else if ((object)source != null && source.IsNullableType() && parameterType.IsNonNullableValueType() && (allowAnyTarget || target.CanBeAssignedNull()))
                    {
                        TypeSymbol typeSymbol2 = MakeNullableType(parameterType);
                        TypeSymbol typeSymbol3 = (typeSymbol.IsNonNullableValueType() ? MakeNullableType(typeSymbol) : typeSymbol);
                        Conversion sourceConversion2 = EncompassingImplicitConversion(sourceExpression, source, typeSymbol2, ref useSiteInfo);
                        Conversion targetConversion2 = ((!allowAnyTarget) ? EncompassingImplicitConversion(null, typeSymbol3, target, ref useSiteInfo) : Conversion.Identity);
                        if (sourceConversion2.Exists && targetConversion2.Exists)
                        {
                            u.Add(UserDefinedConversionAnalysis.Lifted(current, sourceConversion2, targetConversion2, typeSymbol2, typeSymbol3));
                        }
                    }
                }
            }
        }

        private TypeSymbol MostSpecificSourceTypeForImplicitUserDefinedConversion(ImmutableArray<UserDefinedConversionAnalysis> u, TypeSymbol source, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if ((object)source != null && u.Any((UserDefinedConversionAnalysis conv) => TypeSymbol.Equals(conv.FromType, source, TypeCompareKind.ConsiderEverything)))
            {
                return source;
            }
            return MostEncompassedType(u, (UserDefinedConversionAnalysis conv) => conv.FromType, ref useSiteInfo);
        }

        private TypeSymbol MostSpecificTargetTypeForImplicitUserDefinedConversion(ImmutableArray<UserDefinedConversionAnalysis> u, TypeSymbol target, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (u.Any((UserDefinedConversionAnalysis conv) => TypeSymbol.Equals(conv.ToType, target, TypeCompareKind.ConsiderEverything)))
            {
                return target;
            }
            return MostEncompassingType(u, (UserDefinedConversionAnalysis conv) => conv.ToType, ref useSiteInfo);
        }

        private static int LiftingCount(UserDefinedConversionAnalysis conv)
        {
            int num = 0;
            if (!TypeSymbol.Equals(conv.FromType, conv.Operator.GetParameterType(0), TypeCompareKind.ConsiderEverything))
            {
                num++;
            }
            if (!TypeSymbol.Equals(conv.ToType, conv.Operator.ReturnType, TypeCompareKind.ConsiderEverything))
            {
                num++;
            }
            return num;
        }

        private static int? MostSpecificConversionOperator(TypeSymbol sx, TypeSymbol tx, ImmutableArray<UserDefinedConversionAnalysis> u)
        {
            return MostSpecificConversionOperator((UserDefinedConversionAnalysis conv) => TypeSymbol.Equals(conv.FromType, sx, TypeCompareKind.ConsiderEverything) && TypeSymbol.Equals(conv.ToType, tx, TypeCompareKind.ConsiderEverything), u);
        }

        private static int? MostSpecificConversionOperator(Func<UserDefinedConversionAnalysis, bool> constraint, ImmutableArray<UserDefinedConversionAnalysis> u)
        {
            BestIndex bestIndex = UniqueIndex(u, (UserDefinedConversionAnalysis conv) => constraint(conv) && LiftingCount(conv) == 0);
            if (bestIndex.Kind == BestIndexKind.Best)
            {
                return bestIndex.Best;
            }
            if (bestIndex.Kind == BestIndexKind.Ambiguous)
            {
                return null;
            }
            BestIndex bestIndex2 = UniqueIndex(u, (UserDefinedConversionAnalysis conv) => constraint(conv) && LiftingCount(conv) == 1);
            if (bestIndex2.Kind == BestIndexKind.Best)
            {
                return bestIndex2.Best;
            }
            if (bestIndex2.Kind == BestIndexKind.Ambiguous)
            {
                return null;
            }
            BestIndex bestIndex3 = UniqueIndex(u, (UserDefinedConversionAnalysis conv) => constraint(conv) && LiftingCount(conv) == 2);
            if (bestIndex3.Kind == BestIndexKind.Best)
            {
                return bestIndex3.Best;
            }
            _ = bestIndex3.Kind;
            _ = 2;
            return null;
        }

        private static BestIndex UniqueIndex<T>(ImmutableArray<T> items, Func<T, bool> predicate)
        {
            if (items.IsEmpty)
            {
                return BestIndex.None();
            }
            int? num = null;
            for (int i = 0; i < items.Length; i++)
            {
                if (predicate(items[i]))
                {
                    if (num.HasValue)
                    {
                        return BestIndex.IsAmbiguous(num.Value, i);
                    }
                    num = i;
                }
            }
            if (num.HasValue)
            {
                return BestIndex.HasBest(num.Value);
            }
            return BestIndex.None();
        }

        private bool IsEncompassedBy(BoundExpression aExpr, TypeSymbol a, TypeSymbol b, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return EncompassingImplicitConversion(aExpr, a, b, ref useSiteInfo).Exists;
        }

        private Conversion EncompassingImplicitConversion(BoundExpression aExpr, TypeSymbol a, TypeSymbol b, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            Conversion result = ClassifyStandardImplicitConversion(aExpr, a, b, ref useSiteInfo);
            if (!IsEncompassingImplicitConversionKind(result.Kind))
            {
                return Conversion.NoConversion;
            }
            return result;
        }

        private static bool IsEncompassingImplicitConversionKind(ConversionKind kind)
        {
            switch (kind)
            {
                case ConversionKind.NoConversion:
                case ConversionKind.ImplicitEnumeration:
                case ConversionKind.ExplicitTupleLiteral:
                case ConversionKind.ExplicitTuple:
                case ConversionKind.ImplicitDynamic:
                case ConversionKind.ExplicitDynamic:
                case ConversionKind.ImplicitUserDefined:
                case ConversionKind.AnonymousFunction:
                case ConversionKind.MethodGroup:
                case ConversionKind.ExplicitNumeric:
                case ConversionKind.ExplicitEnumeration:
                case ConversionKind.ExplicitNullable:
                case ConversionKind.ExplicitReference:
                case ConversionKind.Unboxing:
                case ConversionKind.ExplicitUserDefined:
                case ConversionKind.ExplicitPointerToPointer:
                case ConversionKind.ExplicitIntegerToPointer:
                case ConversionKind.ExplicitPointerToInteger:
                case ConversionKind.IntPtr:
                case ConversionKind.InterpolatedString:
                case ConversionKind.SwitchExpression:
                case ConversionKind.ConditionalExpression:
                case ConversionKind.StackAllocToPointerType:
                case ConversionKind.StackAllocToSpanType:
                    return false;
                case ConversionKind.Identity:
                case ConversionKind.ImplicitNumeric:
                case ConversionKind.ImplicitThrow:
                case ConversionKind.ImplicitTupleLiteral:
                case ConversionKind.ImplicitTuple:
                case ConversionKind.ImplicitNullable:
                case ConversionKind.NullLiteral:
                case ConversionKind.ImplicitReference:
                case ConversionKind.Boxing:
                case ConversionKind.ImplicitPointerToVoid:
                case ConversionKind.ImplicitNullToPointer:
                case ConversionKind.ImplicitConstant:
                case ConversionKind.DefaultLiteral:
                    return true;
                default:
                    throw ExceptionUtilities.UnexpectedValue(kind);
            }
        }

        private TypeSymbol MostEncompassedType<T>(ImmutableArray<T> items, Func<T, TypeSymbol> extract, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return MostEncompassedType(items, (T x) => true, extract, ref useSiteInfo);
        }

        private TypeSymbol MostEncompassedType<T>(ImmutableArray<T> items, Func<T, bool> valid, Func<T, TypeSymbol> extract, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            CompoundUseSiteInfo<AssemblySymbol> inLambdaUseSiteInfo = useSiteInfo;
            int? num = UniqueBestValidIndex(items, valid, delegate (T left, T right)
            {
                TypeSymbol typeSymbol = extract(left);
                TypeSymbol typeSymbol2 = extract(right);
                if (TypeSymbol.Equals(typeSymbol, typeSymbol2, TypeCompareKind.ConsiderEverything))
                {
                    return BetterResult.Equal;
                }
                bool flag = IsEncompassedBy(null, typeSymbol, typeSymbol2, ref inLambdaUseSiteInfo);
                bool flag2 = IsEncompassedBy(null, typeSymbol2, typeSymbol, ref inLambdaUseSiteInfo);
                if (flag == flag2)
                {
                    return BetterResult.Neither;
                }
                return (!flag) ? BetterResult.Right : BetterResult.Left;
            });
            useSiteInfo = inLambdaUseSiteInfo;
            if (num.HasValue)
            {
                return extract(items[num.Value]);
            }
            return null;
        }

        private TypeSymbol MostEncompassingType<T>(ImmutableArray<T> items, Func<T, TypeSymbol> extract, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return MostEncompassingType(items, (T x) => true, extract, ref useSiteInfo);
        }

        private TypeSymbol MostEncompassingType<T>(ImmutableArray<T> items, Func<T, bool> valid, Func<T, TypeSymbol> extract, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            CompoundUseSiteInfo<AssemblySymbol> inLambdaUseSiteInfo = useSiteInfo;
            int? num = UniqueBestValidIndex(items, valid, delegate (T left, T right)
            {
                TypeSymbol typeSymbol = extract(left);
                TypeSymbol typeSymbol2 = extract(right);
                if (TypeSymbol.Equals(typeSymbol, typeSymbol2, TypeCompareKind.ConsiderEverything))
                {
                    return BetterResult.Equal;
                }
                bool flag = IsEncompassedBy(null, typeSymbol2, typeSymbol, ref inLambdaUseSiteInfo);
                bool flag2 = IsEncompassedBy(null, typeSymbol, typeSymbol2, ref inLambdaUseSiteInfo);
                if (flag == flag2)
                {
                    return BetterResult.Neither;
                }
                return (!flag) ? BetterResult.Right : BetterResult.Left;
            });
            useSiteInfo = inLambdaUseSiteInfo;
            if (num.HasValue)
            {
                return extract(items[num.Value]);
            }
            return null;
        }

        private static int? UniqueBestValidIndex<T>(ImmutableArray<T> items, Func<T, bool> valid, Func<T, T, BetterResult> better)
        {
            if (items.IsEmpty)
            {
                return null;
            }
            int? result = null;
            T arg = default(T);
            for (int i = 0; i < items.Length; i++)
            {
                T val = items[i];
                if (!valid(val))
                {
                    continue;
                }
                if (!result.HasValue)
                {
                    result = i;
                    arg = val;
                    continue;
                }
                switch (better(arg, val))
                {
                    case BetterResult.Neither:
                        result = null;
                        arg = default(T);
                        break;
                    case BetterResult.Right:
                        result = i;
                        arg = val;
                        break;
                }
            }
            if (!result.HasValue)
            {
                return null;
            }
            for (int j = 0; j < result.Value; j++)
            {
                T val2 = items[j];
                if (valid(val2))
                {
                    BetterResult betterResult = better(arg, val2);
                    if (betterResult != 0 && betterResult != BetterResult.Equal)
                    {
                        return null;
                    }
                }
            }
            return result;
        }

        private NamedTypeSymbol MakeNullableType(TypeSymbol type)
        {
            return corLibrary.GetDeclaredSpecialType(SpecialType.System_Nullable_T).Construct(type);
        }

        protected UserDefinedConversionResult AnalyzeImplicitUserDefinedConversionForV6SwitchGoverningType(TypeSymbol source, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            ComputeUserDefinedImplicitConversionTypeSet(source, null, instance, ref useSiteInfo);
            ArrayBuilder<UserDefinedConversionAnalysis> instance2 = ArrayBuilder<UserDefinedConversionAnalysis>.GetInstance();
            ComputeApplicableUserDefinedImplicitConversionSet(null, source, null, instance, instance2, ref useSiteInfo, allowAnyTarget: true);
            instance.Free();
            ImmutableArray<UserDefinedConversionAnalysis> immutableArray = instance2.ToImmutableAndFree();
            int? num = MostSpecificConversionOperator((UserDefinedConversionAnalysis conv) => conv.ToType.IsValidV6SwitchGoverningType(isTargetTypeOfUserDefinedOp: true), immutableArray);
            if (num.HasValue)
            {
                return UserDefinedConversionResult.Valid(immutableArray, num.Value);
            }
            return UserDefinedConversionResult.NoApplicableOperators(immutableArray);
        }
    }
}
