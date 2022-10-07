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

            var local = variable as LocalSymbol;
            if ((object)local != null)
            {
                if (local.SynthesizedKind == SynthesizedLocalKind.LambdaDisplayClass)
                {
                    return GeneratedNames.MakeLambdaDisplayLocalName(uniqueId++);
                }

                if (local.SynthesizedKind == SynthesizedLocalKind.ExceptionFilterAwaitHoistedExceptionLocal)
                {
                    return GeneratedNames.MakeHoistedLocalFieldName(local.SynthesizedKind, uniqueId++);
                }

                if (local.SynthesizedKind == SynthesizedLocalKind.InstrumentationPayload)
                {
                    return GeneratedNames.MakeSynthesizedInstrumentationPayloadLocalFieldName(uniqueId++);
                }

                if (local.SynthesizedKind == SynthesizedLocalKind.UserDefined &&
                    (local.ScopeDesignatorOpt?.Kind() == SyntaxKind.SwitchSection ||
                     local.ScopeDesignatorOpt?.Kind() == SyntaxKind.SwitchExpressionArm))
                {
                    // The programmer can use the same identifier for pattern variables in different
                    // sections of a switch statement, but they are all hoisted into
                    // the same frame for the enclosing switch statement and must be given
                    // unique field names.
                    return GeneratedNames.MakeHoistedLocalFieldName(local.SynthesizedKind, uniqueId++, local.Name);
                }
            }

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
