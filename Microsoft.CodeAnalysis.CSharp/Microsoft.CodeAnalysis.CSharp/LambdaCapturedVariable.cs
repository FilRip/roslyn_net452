using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class LambdaCapturedVariable : SynthesizedFieldSymbolBase
    {
        private readonly TypeWithAnnotations _type;

        private readonly bool _isThis;

        internal override bool IsCapturedFrame => _isThis;

        internal override bool SuppressDynamicAttribute => false;

        private LambdaCapturedVariable(SynthesizedContainer frame, TypeWithAnnotations type, string fieldName, bool isThisParameter)
            : base(frame, fieldName, isPublic: true, isReadOnly: false, isStatic: false)
        {
            _type = type;
            _isThis = isThisParameter;
        }

        public static LambdaCapturedVariable Create(SynthesizedClosureEnvironment frame, Symbol captured, ref int uniqueId)
        {
            string capturedVariableFieldName = GetCapturedVariableFieldName(captured, ref uniqueId);
            TypeSymbol capturedVariableFieldType = GetCapturedVariableFieldType(frame, captured);
            return new LambdaCapturedVariable(frame, TypeWithAnnotations.Create(capturedVariableFieldType), capturedVariableFieldName, IsThis(captured));
        }

        private static bool IsThis(Symbol captured)
        {
            if (captured is ParameterSymbol parameterSymbol)
            {
                return parameterSymbol.IsThis;
            }
            return false;
        }

        private static string GetCapturedVariableFieldName(Symbol variable, ref int uniqueId)
        {
            if (IsThis(variable))
            {
                return GeneratedNames.ThisProxyFieldName();
            }
            if (variable is LocalSymbol localSymbol)
            {
                if (localSymbol.SynthesizedKind == SynthesizedLocalKind.LambdaDisplayClass)
                {
                    return GeneratedNames.MakeLambdaDisplayLocalName(uniqueId++);
                }
                if (localSymbol.SynthesizedKind == SynthesizedLocalKind.ExceptionFilterAwaitHoistedExceptionLocal)
                {
                    return GeneratedNames.MakeHoistedLocalFieldName(localSymbol.SynthesizedKind, uniqueId++);
                }
                if (localSymbol.SynthesizedKind == SynthesizedLocalKind.InstrumentationPayload)
                {
                    return GeneratedNames.MakeSynthesizedInstrumentationPayloadLocalFieldName(uniqueId++);
                }
                if (localSymbol.SynthesizedKind == SynthesizedLocalKind.UserDefined)
                {
                    SyntaxNode scopeDesignatorOpt = localSymbol.ScopeDesignatorOpt;
                    if (scopeDesignatorOpt == null || scopeDesignatorOpt.Kind() != SyntaxKind.SwitchSection)
                    {
                        SyntaxNode scopeDesignatorOpt2 = localSymbol.ScopeDesignatorOpt;
                        if (scopeDesignatorOpt2 == null || scopeDesignatorOpt2.Kind() != SyntaxKind.SwitchExpressionArm)
                        {
                            goto IL_00c6;
                        }
                    }
                    return GeneratedNames.MakeHoistedLocalFieldName(localSymbol.SynthesizedKind, uniqueId++, localSymbol.Name);
                }
            }
            goto IL_00c6;
        IL_00c6:
            return variable.Name;
        }

        private static TypeSymbol GetCapturedVariableFieldType(SynthesizedContainer frame, Symbol variable)
        {
            LocalSymbol localSymbol = variable as LocalSymbol;
            if ((object)localSymbol != null && localSymbol.Type.OriginalDefinition is SynthesizedClosureEnvironment synthesizedClosureEnvironment)
            {
                ImmutableArray<TypeWithAnnotations> immutableArray = frame.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
                if (immutableArray.Length > synthesizedClosureEnvironment.Arity)
                {
                    immutableArray = ImmutableArray.Create(immutableArray, 0, synthesizedClosureEnvironment.Arity);
                }
                return synthesizedClosureEnvironment.ConstructIfGeneric(immutableArray);
            }
            return frame.TypeMap.SubstituteType((localSymbol?.TypeWithAnnotations ?? ((ParameterSymbol)variable).TypeWithAnnotations).Type).Type;
        }

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            return _type;
        }
    }
}
