// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class OutDeconstructVarPendingInference
    {
        public BoundDeconstructValuePlaceholder? Placeholder;

        public BoundDeconstructValuePlaceholder SetInferredTypeWithAnnotations(TypeWithAnnotations type, Binder binder, bool success)
        {

            // The val escape scope for this placeholder won't be used, so defaulting to narrowest scope
            Placeholder = new BoundDeconstructValuePlaceholder(this.Syntax, binder.LocalScopeDepth, type.Type, hasErrors: this.HasErrors || !success);
            return Placeholder;
        }

        public BoundDeconstructValuePlaceholder FailInference(Binder binder)
        {
            return SetInferredTypeWithAnnotations(TypeWithAnnotations.Create(binder.CreateErrorType()), binder, success: false);
        }
    }
}
