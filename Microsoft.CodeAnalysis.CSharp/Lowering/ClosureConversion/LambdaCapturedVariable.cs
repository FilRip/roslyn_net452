// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    /// <summary>
    /// A field of a frame class that represents a variable that has been captured in a lambda.
    /// </summary>
    internal sealed class LambdaCapturedVariable : SynthesizedFieldSymbolBase
    {
        private readonly TypeWithAnnotations _type;
        private readonly bool _isThis;

        private LambdaCapturedVariable(SynthesizedContainer frame, TypeWithAnnotations type, string fieldName, bool isThisParameter)
            : base(frame,
                   fieldName,
                   isPublic: true,
                   isReadOnly: false,
                   isStatic: false)
        {

            // lifted fields do not need to have the CompilerGeneratedAttribute attached to it, the closure is already 
            // marked as being compiler generated.
            _type = type;
            _isThis = isThisParameter;
        }

        public static LambdaCapturedVariable Create(SynthesizedClosureEnvironment frame, Symbol captured, ref int uniqueId)
        {

            string fieldName = GetCapturedVariableFieldName(captured, ref uniqueId);
            TypeSymbol type = GetCapturedVariableFieldType(frame, captured);
            return new LambdaCapturedVariable(frame, TypeWithAnnotations.Create(type), fieldName, IsThis(captured));
        }

        private static bool IsThis(Symbol captured)
        {
            return captured is ParameterSymbol parameter && parameter.IsThis;
        }

        private static string GetCapturedVariableFieldName(Symbol variable, ref int uniqueId)
        {
            if (IsThis(variable))
            {
                return GeneratedNames.ThisProxyFieldName();
            }

            if (variable is LocalSymbol local)
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
            var local = variable as LocalSymbol;
            if (local is not null)
            {
                // if we're capturing a generic frame pointer, construct it with the new frame's type parameters
                if (local.Type.OriginalDefinition is SynthesizedClosureEnvironment lambdaFrame)
                {
                    // lambdaFrame may have less generic type parameters than frame, so trim them down (the first N will always match)
                    var typeArguments = frame.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
                    if (typeArguments.Length > lambdaFrame.Arity)
                    {
                        typeArguments = ImmutableArray.Create(typeArguments, 0, lambdaFrame.Arity);
                    }

                    return lambdaFrame.ConstructIfGeneric(typeArguments);
                }
            }

            return frame.TypeMap.SubstituteType((local is not null ? local.TypeWithAnnotations : ((ParameterSymbol)variable).TypeWithAnnotations).Type).Type;
        }

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            return _type;
        }

        internal override bool IsCapturedFrame
        {
            get
            {
                return _isThis;
            }
        }

        internal override bool SuppressDynamicAttribute
        {
            get
            {
                return false;
            }
        }
    }
}
