// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class DeconstructionVariablePendingInference
    {
        protected override ErrorCode InferenceFailedError => ErrorCode.ERR_TypeInferenceFailedForImplicitlyTypedDeconstructionVariable;
    }

    public partial class OutVariablePendingInference
    {
        protected override ErrorCode InferenceFailedError => ErrorCode.ERR_TypeInferenceFailedForImplicitlyTypedOutVariable;
    }

    public partial class VariablePendingInference : BoundExpression
    {
        internal BoundExpression SetInferredTypeWithAnnotations(TypeWithAnnotations type, BindingDiagnosticBag? diagnosticsOpt)
        {

            return SetInferredTypeWithAnnotations(type, null, diagnosticsOpt);
        }

        internal BoundExpression SetInferredTypeWithAnnotations(TypeWithAnnotations type, Binder? binderOpt, BindingDiagnosticBag? diagnosticsOpt)
        {
            bool inferenceFailed = !type.HasType;

            if (inferenceFailed)
            {
                type = TypeWithAnnotations.Create(binderOpt!.CreateErrorType("var"));
            }

            switch (this.VariableSymbol.Kind)
            {
                case SymbolKind.Local:
                    var localSymbol = (SourceLocalSymbol)this.VariableSymbol;

                    if (diagnosticsOpt?.DiagnosticBag != null)
                    {
                        if (inferenceFailed)
                        {
                            ReportInferenceFailure(diagnosticsOpt);
                        }
                        else
                        {
                            SyntaxNode typeOrDesignationSyntax = this.Syntax.Kind() == SyntaxKind.DeclarationExpression ?
                                ((DeclarationExpressionSyntax)this.Syntax).Type :
                                this.Syntax;

                            Binder.CheckRestrictedTypeInAsyncMethod(localSymbol.ContainingSymbol, type.Type, diagnosticsOpt, typeOrDesignationSyntax);
                        }
                    }

                    localSymbol.SetTypeWithAnnotations(type);
                    return new BoundLocal(this.Syntax, localSymbol, BoundLocalDeclarationKind.WithInferredType, constantValueOpt: null, isNullableUnknown: false, type: type.Type, hasErrors: this.HasErrors || inferenceFailed).WithWasConverted();

                case SymbolKind.Field:
                    var fieldSymbol = (GlobalExpressionVariable)this.VariableSymbol;
                    var inferenceDiagnostics = new BindingDiagnosticBag(DiagnosticBag.GetInstance()
#if DEBUG
                                                                        , PooledHashSet<AssemblySymbol>.GetInstance()
#endif
                                                                        );

                    if (inferenceFailed)
                    {
                        ReportInferenceFailure(inferenceDiagnostics);
                    }

                    type = fieldSymbol.SetTypeWithAnnotations(type, inferenceDiagnostics);
#if DEBUG
#endif
                    inferenceDiagnostics.Free();

                    return new BoundFieldAccess(this.Syntax,
                                                this.ReceiverOpt,
                                                fieldSymbol,
                                                null,
                                                LookupResultKind.Viable,
                                                isDeclaration: true,
                                                type: type.Type,
                                                hasErrors: this.HasErrors || inferenceFailed);

                default:
                    throw ExceptionUtilities.UnexpectedValue(this.VariableSymbol.Kind);
            }
        }

        internal BoundExpression FailInference(Binder binder, BindingDiagnosticBag? diagnosticsOpt)
        {
            return this.SetInferredTypeWithAnnotations(default, binder, diagnosticsOpt);
        }

        private void ReportInferenceFailure(BindingDiagnosticBag diagnostics)
        {
            SingleVariableDesignationSyntax designation = this.Syntax.Kind() switch
            {
                SyntaxKind.DeclarationExpression => (SingleVariableDesignationSyntax)((DeclarationExpressionSyntax)this.Syntax).Designation,
                SyntaxKind.SingleVariableDesignation => (SingleVariableDesignationSyntax)this.Syntax,
                _ => throw ExceptionUtilities.Unreachable,
            };
            Binder.Error(
                diagnostics, this.InferenceFailedError, designation.Identifier,
                designation.Identifier.ValueText);
        }

        protected abstract ErrorCode InferenceFailedError { get; }
    }
}
