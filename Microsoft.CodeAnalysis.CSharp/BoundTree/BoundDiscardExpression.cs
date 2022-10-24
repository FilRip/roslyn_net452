// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class BoundDiscardExpression
    {
        public BoundExpression SetInferredTypeWithAnnotations(TypeWithAnnotations type)
        {
            return this.Update(type.Type);
        }

        public BoundDiscardExpression FailInference(Binder binder, BindingDiagnosticBag? diagnosticsOpt)
        {
            if (diagnosticsOpt?.DiagnosticBag != null)
            {
                Binder.Error(diagnosticsOpt, ErrorCode.ERR_DiscardTypeInferenceFailed, this.Syntax);
            }
            return this.Update(binder.CreateErrorType("var"));
        }

        public override Symbol ExpressionSymbol
        {
            get
            {
                return new DiscardSymbol(TypeWithAnnotations.Create(this.Type, this.TopLevelNullability.Annotation.ToInternalAnnotation()));
            }
        }
    }
}
