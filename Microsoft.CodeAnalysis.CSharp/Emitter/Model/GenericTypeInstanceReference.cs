// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Emit
{
    /// <summary>
    /// Represents a reference to a generic type instantiation.
    /// Subclasses represent nested and namespace types.
    /// </summary>
    internal abstract class GenericTypeInstanceReference : NamedTypeReference, Cci.IGenericTypeInstanceReference
    {
        public GenericTypeInstanceReference(NamedTypeSymbol underlyingNamedType)
            : base(underlyingNamedType)
        {
            // Definition doesn't have custom modifiers on type arguments
        }

        public sealed override void Dispatch(Cci.MetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        ImmutableArray<Cci.ITypeReference> Cci.IGenericTypeInstanceReference.GetGenericArguments(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            var builder = ArrayBuilder<Cci.ITypeReference>.GetInstance();
            foreach (TypeWithAnnotations type in UnderlyingNamedType.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics)
            {
                builder.Add(moduleBeingBuilt.Translate(type.Type, syntaxNodeOpt: (CSharpSyntaxNode)context.SyntaxNode, diagnostics: context.Diagnostics));
            }

            return builder.ToImmutableAndFree();
        }

        Cci.INamedTypeReference Cci.IGenericTypeInstanceReference.GetGenericType(EmitContext context)
        {
            System.Diagnostics.Debug.Assert(UnderlyingNamedType.OriginalDefinition.IsDefinition);
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            return moduleBeingBuilt.Translate(UnderlyingNamedType.OriginalDefinition, syntaxNodeOpt: (CSharpSyntaxNode)context.SyntaxNode,
                                              diagnostics: context.Diagnostics, needDeclaration: true);
        }
    }
}
