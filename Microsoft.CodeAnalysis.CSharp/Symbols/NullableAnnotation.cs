// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace Microsoft.CodeAnalysis.CSharp
{
    /// <summary>
    /// The nullable annotations that can apply in source.
    /// </summary>
    /// <remarks>
    /// The order of values here is used in the computation of <see cref="NullableAnnotationExtensions.Meet(NullableAnnotation, NullableAnnotation)"/>,
    /// <see cref="NullableAnnotationExtensions.Join(NullableAnnotation, NullableAnnotation)"/>, and
    /// <see cref="NullableAnnotationExtensions.EnsureCompatible(NullableAnnotation, NullableAnnotation)"/>.  If the order here is changed
    /// then those implementations may have to be revised (or simplified).
    /// </remarks>
    public enum NullableAnnotation : byte
    {
        /// <summary>
        /// Type is not annotated - string, int, T (including the case when T is unconstrained).
        /// </summary>
        NotAnnotated,

        /// <summary>
        /// The type is not annotated in a context where the nullable feature is not enabled.
        /// Used for interoperation with existing pre-nullable code.
        /// </summary>
        Oblivious,

        /// <summary>
        /// Type is annotated with '?' - string?, T?.
        /// </summary>
        Annotated,

        /// <summary>
        /// Used for indexed type parameters and used locally in override/implementation checks.
        /// When substituting a type parameter with Ignored annotation into some original type parameter
        /// with some other annotation, the result is the annotation from the original symbol.
        ///
        /// T annotated + (T -> U ignored) = U annotated
        /// T oblivious + (T -> U ignored) = U oblivious
        /// T not-annotated + (T -> U ignored) = U not-annotated
        /// </summary>
        Ignored,
    }
}
